using JobApplicationTracker.Application.Common.Interfaces;
using JobApplicationTracker.Infrastructure.BackgroundJobs;
using JobApplicationTracker.Infrastructure.Persistence;
using JobApplicationTracker.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using HealthChecks.RabbitMQ;

namespace JobApplicationTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(provider =>
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("Redis") ?? "localhost:6379"));

        services.AddScoped<ICacheService, RedisCacheService>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IPublishEndpointService, PublishEndpointService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddHostedService<ReminderNotificationService>();

        services.AddMassTransit(busConfig =>
        {
            busConfig.SetKebabCaseEndpointNameFormatter();

            busConfig.AddConsumer<Messaging.Consumers.AuditLogConsumer>();
            busConfig.AddConsumer<Messaging.Consumers.StatusChangeNotificationConsumer>();

            busConfig.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqHost = configuration["RabbitMq:Host"] ?? "localhost";
                var rabbitMqUser = configuration["RabbitMq:Username"] ?? "guest";
                var rabbitMqPass = configuration["RabbitMq:Password"] ?? "guest";

                cfg.Host(rabbitMqHost, "/", h =>
                {
                    h.Username(rabbitMqUser);
                    h.Password(rabbitMqPass);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddHealthChecks()
                    .AddNpgSql(configuration.GetConnectionString("Postgres")!, name: "postgres")
                    .AddRedis(configuration.GetConnectionString("Redis")!, name: "redis")
                    .AddRabbitMQ(sp =>
                    {
                        var factory = new RabbitMQ.Client.ConnectionFactory
                        {
                            Uri = new Uri($"amqp://{configuration["RabbitMq:Username"]}:{configuration["RabbitMq:Password"]}@{configuration["RabbitMq:Host"]}:5672")
                        };
                        return factory.CreateConnectionAsync();
                    }, name: "rabbitmq");
        return services;
    }
}
