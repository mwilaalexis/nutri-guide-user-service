# NutriGuide User Service

Authentication and user profile microservice for the [NutriGuide](https://github.com/mwilaalexis/nutri-guide-web) platform.

## Responsibilities

- User registration and login
- JWT access tokens and refresh tokens
- User profile management
- Weight tracking with historical entries for charting

## Tech stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- AutoMapper
- Docker (optional SQL Server via Docker Compose)

## Architecture

```
Controllers  ->  Services  ->  Repositories  ->  EF Core / SQL Server
```

Dependency injection is centralized in extension methods under `Extensions/`.

## Key endpoints

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/auth/register` | Create account |
| POST | `/api/auth/login` | Obtain JWT |
| POST | `/api/auth/refresh` | Refresh token |
| GET/PUT | `/api/profile` | Profile (authenticated) |
| POST | `/api/profile/weight-entries` | Log weight entry |
| GET | `/api/profile/weight-entries` | Weight history series |

## Run locally

```bash
cd UserProfileService
docker compose -f Docker-compose.yml up -d   # optional SQL Server
dotnet run --project UserProfileServiceProject
```

Default HTTP port: `5185` (see `launchSettings.json`).

## Related repositories

| Service | Repository |
|---------|------------|
| Gateway | [nutri-guide-gateway](https://github.com/mwilaalexis/nutri-guide-gateway) |
| Food catalog | [nutri-guide-food-service](https://github.com/mwilaalexis/nutri-guide-food-service) |
| Meal plans | [nutri-guide-plan-service](https://github.com/mwilaalexis/nutri-guide-plan-service) |
| Frontend | [nutri-guide-web](https://github.com/mwilaalexis/nutri-guide-web) |
