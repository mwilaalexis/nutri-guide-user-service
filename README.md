# NutriGuide - User and Authentication Service

Backend API for user accounts, login, profiles, and weight tracking.

## What this project does

Users can register, log in, receive a **JWT token**, and manage their profile. The service also stores weight entries over time for charts in the app.

## Main features

- Register and login
- JWT access and refresh tokens
- User profile (read and update)
- Weight history entries

## Technologies

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- AutoMapper

## Where this fits in NutriGuide

```
Client -> API Gateway -> User Service (this repo) -> SQL Server
```

Other services use the same login token to know which user is calling the API.

## Prerequisites

- .NET 8 SDK
- SQL Server (local or Docker)
- (Optional) SQL Server Management Studio

## Run locally

1. Clone the repository:
   ```bash
   git clone https://github.com/mwilaalexis/nutri-guide-user-service.git
   cd nutri-guide-user-service
   ```
2. Update the connection string in `appsettings.json` (or `appsettings.Development.json`).
3. Optional - start SQL Server with Docker:
   ```bash
   docker compose -f Docker-compose.yml up -d
   ```
4. Apply database migrations (if not applied on startup):
   ```bash
   dotnet ef database update --project UserProfileServiceProject
   ```
5. Run the API:
   ```bash
   dotnet run --project UserProfileServiceProject
   ```
6. Open Swagger: `https://localhost:7004/swagger` or `http://localhost:5185/swagger` (see `launchSettings.json`).

## API overview

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/auth/register` | Create account | No |
| POST | `/api/auth/login` | Get JWT | No |
| POST | `/api/auth/refresh` | Refresh token | No |
| GET | `/api/profile` | Get profile | Yes |
| PUT | `/api/profile` | Update profile | Yes |
| POST | `/api/profile/weight-entries` | Add weight | Yes |
| GET | `/api/profile/weight-entries` | List weights | Yes |

## Suggested folder structure

```
nutri-guide-user-service/
├── UserProfileServiceProject/   # API (Controllers, Program.cs)
├── UserProject.core/             # Entities, interfaces
├── UserProject.DataAccess/       # DbContext, repositories
├── Docker-compose.yml
└── README.md
```

## Skills demonstrated

- REST API design
- Authentication with JWT
- Entity Framework Core and SQL Server
- Repository-style data access
- Docker basics for local database

## Ideas to improve for recruiters

- [ ] Add 2-3 unit tests for auth or profile logic
- [ ] Add example `.http` file for testing endpoints
- [ ] Document sample JSON for register/login in README
- [ ] Add GitHub Action for `dotnet build` on push

## Related repositories

- [nutri-guide-gateway](https://github.com/mwilaalexis/nutri-guide-gateway)
- [nutri-guide-food-service](https://github.com/mwilaalexis/nutri-guide-food-service)
- [nutri-guide-plan-service](https://github.com/mwilaalexis/nutri-guide-plan-service)
- [nutri-guide-web](https://github.com/mwilaalexis/nutri-guide-web)

## Author

**Alex Mwila** - [@mwilaalexis](https://github.com/mwilaalexis)
