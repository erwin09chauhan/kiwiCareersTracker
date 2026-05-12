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

## Architecture

The solution follows Clean Architecture with four projects:

- **Domain** — entities, enums, domain events. No external dependencies.
- **Application** — CQRS commands/queries (MediatR), validators, DTOs, AutoMapper profiles, interfaces (`IApplicationDbContext`, `ICurrentUserService`, etc.)
- **Infrastructure** — EF Core `ApplicationDbContext`, repositories, MassTransit consumers, JWT/password hashing implementations
- **Api** — controllers, middleware (exception handling, rate limiting), OpenAPI config, DI wiring

Cross-cutting concerns:

- **Per-user data isolation** is enforced via EF Core global query filters (`HasQueryFilter`) on all user-owned entities, scoped to `ICurrentUserService.UserId`.
- **Errors** are returned as RFC 7807 `application/problem+json` via `ExceptionHandlingMiddleware`.
- **Pagination** uses Base64-encoded cursors (`CreatedAtUtc|Id` or `Id`) for stable ordering across requests.
- **Domain events** (e.g. `ApplicationStatusChangedEvent`) are published via MassTransit/RabbitMQ and consumed asynchronously to write audit log entries and notifications.

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
4. Current user: `GET /api/v1/auth/me`
5. Logout: `POST /api/v1/auth/logout`

## API Overview

| Area | Base route | Notes |
|---|---|---|
| Auth | `/api/v1/auth` | register, login, refresh, logout, me |
| Applications | `/api/v1/applications` | CRUD, status updates, cursor-paginated list with filter/search/sort |
| Contacts | `/api/v1/applications/{id}/contacts` | per-application CRUD |
| Notes | `/api/v1/applications/{id}/notes` | per-application CRUD |
| Reminders | `/api/v1/applications/{id}/reminders`, `/api/v1/reminders/upcoming` | CRUD + complete + cross-application upcoming view |
| Audit Log | `/api/v1/applications/{id}/audit`, `/api/v1/audit` | per-application and global, cursor-paginated |
| Notifications | `/api/v1/notifications` | list, mark read/read-all, delete, cursor-paginated |
| Dashboard | `/api/v1/dashboard/summary`, `/api/v1/dashboard/activity` | stats + combined activity feed |

Full schema and try-it-out available via `/openapi/v1.json` (includes JWT Bearer security scheme).

## Tests

```powershell
dotnet test tests/JobApplicationTracker.UnitTests
dotnet test tests/JobApplicationTracker.IntegrationTests
```

- Unit tests use EF Core InMemory + NSubstitute + the MassTransit in-memory test harness.
- Integration tests use `WebApplicationFactory` + Testcontainers (require Docker Desktop running).

## Configuration

Update `src/JobApplicationTracker.Api/appsettings.json`:

- `ConnectionStrings:Postgres` / `ConnectionStrings:Redis`
- `RabbitMq:Host` / `Username` / `Password`
- `Jwt:Secret` — change this for any non-local deployment
- `Jwt:Issuer` / `Audience` / `AccessTokenExpiryMinutes`
- `Cors:AllowedOrigins` — array of allowed frontend origins
- `ApplyMigrationsOnStartup` — set to `true` to run `dotnet ef database update` automatically on app startup (useful for hosted environments like Render/Neon where you can't run migration commands directly)
- `Seq:ServerUrl`, `OpenTelemetry:OtlpEndpoint`

## Deployment

A multi-stage `Dockerfile` builds and publishes the API into an `mcr.microsoft.com/dotnet/aspnet:10.0` runtime image, listening on port 8080.

```powershell
docker build -t jobapplicationtracker-api .
docker run -p 8080:8080 jobapplicationtracker-api
```

`render.yaml` provides a ready-to-use [Render](https://render.com) web service definition (Docker runtime) with environment variables for connecting to a managed Postgres (e.g. [Neon](https://neon.tech)), Redis, RabbitMQ, Seq, and an OTLP collector. Set `ApplyMigrationsOnStartup=true` in the deployed environment so the database schema is created automatically on first boot.

Health checks are exposed at `/health` (liveness) and `/health/detail` (per-dependency status: Postgres, Redis, RabbitMQ, MassTransit bus).
