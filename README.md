# Job Application Tracker API

A .NET 10 / ASP.NET Core REST API for tracking job applications, built with Clean Architecture and CQRS (MediatR).

## Stack

- ASP.NET Core 10, EF Core 10 + PostgreSQL
- MediatR (CQRS), FluentValidation, AutoMapper
- MassTransit + RabbitMQ (async domain events)
- Redis (caching + rate limiting)
- Serilog + Seq (logging)
- OpenTelemetry (tracing)
- JWT authentication
- xUnit, NSubstitute, Testcontainers

## Prerequisites

- .NET 10 SDK
- Docker Desktop

## Local Setup

1. Start infrastructure containers:

   ```powershell
   docker compose up -d
   ```

2. Apply database migrations:

   ```powershell
   dotnet ef database update --project src/JobApplicationTracker.Infrastructure --startup-project src/JobApplicationTracker.Api
   ```

3. Run the API:

   ```powershell
   dotnet run --project src/JobApplicationTracker.Api
   ```

   API available at http://localhost:5101. OpenAPI doc at /openapi/v1.json.

4. Seq UI: http://localhost:5341
   RabbitMQ management UI: http://localhost:15672 (jobtracker / jobtracker)

## Authentication

1. Register: `POST /api/v1/auth/register` with `{ "email", "password" }`
2. Use the returned `accessToken` as `Authorization: Bearer <token>` on all other endpoints.
3. Refresh: `POST /api/v1/auth/refresh` with `{ "refreshToken" }`

## Tests

```powershell
dotnet test tests/JobApplicationTracker.UnitTests
dotnet test tests/JobApplicationTracker.IntegrationTests
```

Integration tests require Docker Desktop running (Testcontainers spins up a Postgres instance).

## Configuration

Update `src/JobApplicationTracker.Api/appsettings.json`:

- `ConnectionStrings:Postgres` / `ConnectionStrings:Redis`
- `RabbitMq:Host` / `Username` / `Password`
- `Jwt:Secret` — change this for any non-local deployment
- `Seq:ServerUrl`, `OpenTelemetry:OtlpEndpoint`
