---
name: create-migration
description: Create a new EF Core migration for PostgreSQL database
user_invocable: true
arguments:
  - name: name
    description: Migration name (e.g., AddUserTable, RenameColumn). The name should be in PascalCase and describe the change being made.
    required: true
  - name: service
    description: "Service name (e.g., newsService). If omitted, auto-detect all services with EF Core DAL projects."
    required: false
---

# Create PostgreSQL Migration

Create a new EF Core migration for any service that uses EF Core + PostgreSQL.

## Steps

1. **Determine the target service(s):**
   - If `$service` is provided, use it directly.
   - If `$service` is NOT provided, search for DAL projects that reference `Microsoft.EntityFrameworkCore` under `src/server/*/` and ask the user which service to target (or use the only one if there's just one match).

2. **Resolve project paths** using the convention:
   - `--project src/server/$service/*.DAL.PostgreSql/*.DAL.PostgreSql.csproj` (the DAL project containing DbContext and migrations)
   - `--startup-project src/server/$service/*.API/*.API.csproj` (the API project used for configuration/DI)
   - Use glob to confirm these paths exist before running the command.

3. **Run the EF Core migration command:**

```bash
dotnet ef migrations add $name \
  --project <resolved DAL project path> \
  --startup-project <resolved API project path> \
  --output-dir Migrations
```

4. If the command succeeds, read the generated migration file (the `*.cs` file without `.Designer.cs`) and show the user the `Up` and `Down` methods so they can verify the migration is correct.

5. If the command fails, analyze the error and help the user fix it. Common issues:
   - Database connection not available — suggest running the PostgreSQL docker container
   - Model errors — check the entity configurations
   - Missing `dotnet-ef` tool — suggest `dotnet tool install --global dotnet-ef`
