# AI-Assisted Development Decisions

## 1. What AI generated, and what I changed

AI was used throughout the implementation as a coding assistant. It generated initial versions of the repetitive application code, including the ASP.NET Core controllers, DTOs, service and repository interfaces, Entity Framework persistence setup, JWT wiring, exception handling, and Angular client scaffolding.

I made the main architectural and operational decisions myself. In particular, I organized the solution using Clean Architecture, added the Docker and Docker Compose setup, implemented the generic API response format, configured CORS, and added IP-based rate limiting. I also connected the layers, adjusted configuration and routing, added validation, database seeding and migrations, and updated the README with local and Docker run instructions. I reviewed and modified the AI-generated code to fit the track-management requirements and remain responsible for checking the final output rather than treating it as trusted.

## 2. Security issues found and how they were handled

The main issues found were:

- Development secrets are stored in repository files. The SQL Server SA password, demo-user password, and JWT signing key appear in `docker-compose.yml`, `README.md`, and application configuration. These values are suitable only for a local throwaway environment. They must be replaced with environment variables, user secrets, or a secret manager before deployment, and any exposed real secrets must be rotated.
- The login implementation compares one configured username and password directly. This is a deliberately minimal demo authentication mechanism, but it is not a production identity system and does not hash or manage multiple user passwords. I kept it for the assignment's local demo and would replace it with a proper identity provider or hashed credential store in production.
- Docker runs the API in the `Development` environment and exposes Swagger over HTTP. Development Swagger and plain HTTP should not be enabled for a public deployment; production should use HTTPS and restrict or disable Swagger.
- SQL Server uses `TrustServerCertificate=True`. That is convenient for local development but weakens certificate validation and should be replaced with properly trusted certificates in production.
- The API has JWT issuer, audience, lifetime, and signing-key validation, restricted development CORS origins, generic responses for unhandled exceptions, and a rate limiter. These controls reduce common risks, but they do not make the demo authentication and committed secrets production-ready.

I documented the local credentials because they are needed to run the sample, but they should be treated as intentionally public development values, never reused, and removed from source control for a real deployment.

## 3. One thing AI got wrong

AI initially treated the hard-coded demo login as if it were an acceptable authentication design rather than clearly labeling it as a development-only shortcut. That was wrong because the password is stored in plaintext configuration and the endpoint validates it with a direct string comparison. Anyone who obtains the configuration can log in, and the design does not provide password hashing, account management, lockout, or password rotation.

I kept the mechanism only because this project needs a simple seeded demo user for local evaluation, and I documented the limitation. A production version should use ASP.NET Core Identity or an external identity provider, store only password hashes, keep secrets outside the repository, and use HTTPS.
