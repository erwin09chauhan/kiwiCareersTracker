using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace JobApplicationTracker.Api.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConnectionMultiplexer _redis;
    private readonly int _limit;
    private readonly int _windowSeconds;

    private static readonly LuaScript _rateLimitScript = LuaScript.Prepare(@"
        local current = redis.call('INCR', @key)
        if current == 1 then
            redis.call('EXPIRE', @key, @window)
        end
        return current;
    ");

    public RateLimitingMiddleware(RequestDelegate next, IConnectionMultiplexer redis, IConfiguration configuration)
    {
        _next = next;
        _redis = redis;
        _limit = configuration.GetValue<int?>("RateLimiting:Limit") ?? 100;
        _windowSeconds = configuration.GetValue<int?>("RateLimiting:WindowSeconds") ?? 60;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientId = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"ratelimit:{clientId}:{context.Request.Path}";

        var db = _redis.GetDatabase();

        var count = (long)await db.ScriptEvaluateAsync(_rateLimitScript, new { key = (RedisKey)key, window = _windowSeconds });

        if (count > _limit)
        {
            var ttl = await db.KeyTimeToLiveAsync(key);
            var retryAfter = (int)(ttl?.TotalSeconds ?? _windowSeconds);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.Headers.RetryAfter = retryAfter.ToString();
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc6585#section-4",
                Title = "Too Many Requests",
                Status = StatusCodes.Status429TooManyRequests,
                Detail = $"Rate limit of {_limit} requests per {_windowSeconds} seconds exceeded. Please try again in {retryAfter} seconds."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
            return;
        }

        context.Response.Headers["X-RateLimit-Limit"] = _limit.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, _limit - count).ToString();

        await _next(context);
    }
}