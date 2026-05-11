FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY JobApplicationTracker.slnx .
COPY src/JobApplicationTracker.Domain/JobApplicationTracker.Domain.csproj src/JobApplicationTracker.Domain/
COPY src/JobApplicationTracker.Application/JobApplicationTracker.Application.csproj src/JobApplicationTracker.Application/
COPY src/JobApplicationTracker.Infrastructure/JobApplicationTracker.Infrastructure.csproj src/JobApplicationTracker.Infrastructure/
COPY src/JobApplicationTracker.Api/JobApplicationTracker.Api.csproj src/JobApplicationTracker.Api/
COPY tests/JobApplicationTracker.UnitTests/JobApplicationTracker.UnitTests.csproj tests/JobApplicationTracker.UnitTests/
COPY tests/JobApplicationTracker.IntegrationTests/JobApplicationTracker.IntegrationTests.csproj tests/JobApplicationTracker.IntegrationTests/

RUN dotnet restore src/JobApplicationTracker.Api/JobApplicationTracker.Api.csproj

COPY src/ src/

RUN dotnet publish src/JobApplicationTracker.Api/JobApplicationTracker.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "JobApplicationTracker.Api.dll"]
