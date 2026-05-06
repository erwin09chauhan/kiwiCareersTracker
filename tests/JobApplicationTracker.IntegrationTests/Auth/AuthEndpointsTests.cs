using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JobApplicationTracker.Application.Auth.Dtos;
using JobApplicationTracker.IntegrationTests.Common;

namespace JobApplicationTracker.IntegrationTests.Auth;

public class AuthEndpointsTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ThenLogin_ReturnsTokens()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        var password = "Password123";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new { email, password });
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthResultDto>();
        registerResult.Should().NotBeNull();
        registerResult!.AccessToken.Should().NotBeNullOrEmpty();

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", new { email, password });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResultDto>();
        loginResult.Should().NotBeNull();
        loginResult!.AccessToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetApplications_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/applications");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateApplication_ThenGetApplications_ReturnsCreatedApplication()
    {
        var email = $"{Guid.NewGuid()}@example.com";
        var password = "Password123";

        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", new { email, password });
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthResultDto>();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", registerResult!.AccessToken);

        var createResponse = await _client.PostAsJsonAsync("/api/v1/applications", new
        {
            company = "Acme",
            role = "Backend Engineer",
            appliedDate = DateTime.UtcNow
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var listResponse = await _client.GetAsync("/api/v1/applications");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await listResponse.Content.ReadAsStringAsync();
        content.Should().Contain("Acme");
    }
}
