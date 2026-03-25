# MinimalAPIMoviez - Docker deployment kit

## What this kit assumes
- Your repo root contains `MinimalAPIMoviez.sln` and the `MinimalAPIMoviez/` project folder.
- You want to deploy on a Linux host using Docker Compose.
- The API will run on port `8080`.
- SQL Server and Redis will run as containers next to the API.
- File uploads are stored locally in the Docker volume mounted at `/app/wwwroot`.
- The API itself handles DB retry and migration startup, so compose does not rely on a fragile SQL healthcheck.

## Important code changes you should apply first
Your current codebase has 4 deployment blockers:
1. **JWT signing key** is expected from configuration, but production config is missing.
2. **CORS** currently uses `WithOrigins("*")`, which is not valid for wildcard usage.
3. **Azure blob storage** is hardcoded; for a simple Linux deployment, local storage is easier.
4. **Database migrations** are not applied automatically and startup can race SQL readiness.

Use the replacement files in `patches/`.

## Files included
- `Dockerfile`
- `.dockerignore`
- `docker-compose.yml`
- `.env.example`
- `patches/MinimalAPIMoviez/Program.cs`
- `patches/MinimalAPIMoviez/Services/LocalFileStorage.cs`

## Deployment steps
1. Copy these files into the **root of your repository**.
2. Replace your project files with the versions under `patches/`.
3. Create a real env file:
   ```bash
   cp .env.example .env
   ```
4. Generate a JWT key:
   ```bash
   openssl rand -base64 64
   ```
   Put that value into `JWT_SIGNING_KEY_BASE64`.
5. Build and run:
   ```bash
   docker compose up -d --build
   ```
6. Check containers:
   ```bash
   docker compose ps
   docker compose logs -f api
   ```

## API URLs
- Swagger UI: `http://YOUR_SERVER_IP:8080/swagger`
- GraphQL: `http://YOUR_SERVER_IP:8080/graphql`

## Notes
- If you have a real frontend domain, set `ALLOWED_ORIGIN=https://your-frontend-domain.com`.
- If you prefer Azure Blob Storage, keep your old storage class and set `Storage__Provider=Azure` plus `ConnectionStrings__AzureConnection` in `.env`.
- Your repository currently contains secrets inside appsettings. Rotate those secrets and remove them from source control.
