# TrackManagement

TrackManagement is an ASP.NET Core 9 API with an Angular 19 client and SQL Server persistence.

## Prerequisites

- .NET 9 SDK
- Node.js and npm
- SQL Server, or Docker Desktop
- Entity Framework Core CLI tools

Install the EF Core CLI tool once if it is not already available:

```bash
dotnet tool install --global dotnet-ef
```

## Run Locally

1. Start SQL Server and make sure the connection string in `TrackManagement.API/appsettings.json` points to it. The default is a local SQL Server instance using Windows authentication:

	```text
	Server=.;Database=TrackManagementDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True;
	```

2. Restore and build the solution:

	```bash
	dotnet restore TrackManagement.sln
	dotnet build TrackManagement.sln
	```

3. Apply the database migrations from the repository root:

	```bash
	dotnet ef database update --project TrackManagement.Persistence --startup-project TrackManagement.API
	```

4. Start the API:

	```bash
	dotnet run --project TrackManagement.API --launch-profile http
	```

	The API is available at `http://localhost:5262`. In Development, Swagger is available at `http://localhost:5262/swagger`.

5. In a second terminal, install and start the Angular client:

	```bash
	cd TrackManagement.Client
	npm install
	npm start
	```

	Open `http://localhost:4200`.

## Run With Docker Compose

Start SQL Server and the application containers:

```bash
docker compose up --build -d
```

The services use these URLs:

- Client: `http://localhost:4200`
- API: `http://localhost:44334`
- Swagger: `http://localhost:44334/swagger`

The API container does not automatically apply migrations. After SQL Server is healthy, apply them from the repository root:

```bash
dotnet ef database update --project TrackManagement.Persistence --startup-project TrackManagement.API --connection "Server=localhost,1433;Database=TrackManagementDb;User Id=sa;Password=Abram123@#;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

Stop the containers with:

```bash
docker compose down
```

Add `-v` to the command when the SQL Server volume should also be removed.

## Authentication

The database seeder creates these accounts when they do not already exist:

```text
Admin account:
Username: admin
Password: Password123@#
Role: Admin

Operator account:
Username: operator
Password: Password123@#
Role: Operator
```

Only users with the `Admin` role can open the **Add user** page and create accounts. Available roles are `Admin`, `Operator`, `Editor`, and `Viewer`.

Login security is configured in `TrackManagement.API/appsettings.json`:

```json
"LoginSecurity": {
	"MaxFailedAttempts": 5,
	"LockoutMinutes": 10
}
```

After five invalid password attempts, the account is locked for the configured duration. Refresh tokens reuse one database row per username. Logout sets `RevokedAtUtc` on that row instead of inserting another row.

Request a token from the API:

```bash
curl -X POST http://localhost:5262/api/auth/login ^
  -H "Content-Type: application/json" ^
  -d "{\"username\":\"admin\",\"password\":\"Password123@#\"}"
```

For Docker, replace `5262` with `44334`. The response contains `token` and `expiresAtUtc`:

```json
{
  "token": "<jwt>",
  "expiresAtUtc": "<utc timestamp>"
}
```

Send the token to protected endpoints using the `Authorization` header:

```text
Authorization: Bearer <jwt>
```

The same token can be configured in Swagger by selecting **Authorize** and entering `Bearer <jwt>`.