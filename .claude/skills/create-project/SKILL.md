---
name: create-project
description: Scaffold a new three-layer backend microservice (API / BLL / DAL) following the repository conventions
user_invocable: true
arguments:
  - name: name
    description: "Service name in PascalCase, without the `Service` suffix (e.g. `Auth` produces `AuthService`). Passing `AuthService` works too — the suffix is not duplicated."
    required: true
  - name: db
    description: "Data store: `postgres` (default, EF Core + PostgreSQL like NewsService) or `mongo` (like CrawlerService/NotificationService)."
    required: false
---

# Create a three-layer service

Scaffold a new backend microservice with the same layout, wiring and conventions as the existing services, plus
its solution file and BLL test project. Templates live in `templates/` next to this file.

The generated code compiles and runs but contains no domain logic — it is a skeleton with `Controllers/`,
`Services/`, `Stores/` and friends left empty for the feature work that follows.

## Step 1: Resolve names and check for collisions

From `$name` derive:

| Placeholder | Meaning | Example (`$name = Auth`) |
|---|---|---|
| `__SERVICE_FULL__` | PascalCase name with the `Service` suffix | `AuthService` |
| `__SERVICE_CAMEL__` | camelCase folder name and DB schema | `authService` |
| `__PORT__` | HTTPS port (Step 2) | `7330` |
| `__DAL_PROJECT__` | DAL project name | `AuthService.DAL.PostgreSql` |
| `__DAL_NAMESPACE__` | namespace of `AddDataAccessLayer` | `AuthService.DAL.PostgreSql` |

If `$name` already ends with `Service`, do not append it again.

Stop with a clear message — do not overwrite anything — if any of these already exist:

- `src/server/__SERVICE_CAMEL__/`
- `__SERVICE_FULL__.sln` in the repository root
- `tests/__SERVICE_CAMEL__/`

## Step 2: Pick a free port

Read every `src/server/*/*.API/Properties/launchSettings.json`, collect the ports used in `applicationUrl`, and
choose the next free one in the `73x0` series (existing: 7300 NewsService, 7310 CrawlerService, 7320
NotificationService). Report the chosen port to the user.

## Step 3: Resolve the `db` variant

Default to `postgres` when `$db` is omitted.

| | `postgres` | `mongo` |
|---|---|---|
| DAL project name | `__SERVICE_FULL__.DAL.PostgreSql` | `__SERVICE_FULL__.DAL` |
| DAL templates | `templates/dal-postgres/` | `templates/dal-mongo/` |
| `__DAL_NAMESPACE__` | `__SERVICE_FULL__.DAL.PostgreSql` | `__SERVICE_FULL__.DAL` |
| `ConnectionStringSettings` | `templates/api/ConnectionStringSettings.postgres.cs.template` | `...mongo.cs.template` |
| `appsettings.json` | `templates/api/appsettings.postgres.json.template` | `...mongo.json.template` |

`__API_DB_PACKAGES__` in the API csproj:

- **postgres** — the EF Core design-time package, so the API can serve as `--startup-project` for `dotnet ef`:
  ```xml
        <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.3">
          <PrivateAssets>all</PrivateAssets>
          <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
        </PackageReference>
  ```
- **mongo** — empty string.

`__DAL_REGISTRATION__` in `Program.cs` (keep the 8-space indentation so it lines up inside `Main`):

- **postgres**
  ```csharp
        builder.Services.AddDataAccessLayer(globalSettings.ConnectionStrings.PostgreSql);
  ```
- **mongo**
  ```csharp
        builder.Services.AddDataAccessLayer(new DatabaseSettings
        {
            ConnectionString = globalSettings.ConnectionStrings.Mongo,
            DatabaseName = globalSettings.ConnectionStrings.DatabaseName,
        });
  ```

## Step 4: Create the projects

Create `src/server/__SERVICE_CAMEL__/` with six projects. Copy each template, replacing every placeholder.

```
__SERVICE_FULL__.API/                 <- templates/api/Api.csproj.template, Program.cs.template
    Controllers/                      (empty)
    Models/                           (empty)
    Extensions/                       (empty)
    Settings/GlobalSettings.cs        <- templates/api/GlobalSettings.cs.template
    Settings/ConnectionStringSettings.cs
    Properties/launchSettings.json
    appsettings.json
    appsettings.Development.json
__SERVICE_FULL__.BLL/                 <- templates/bll/Bll.csproj.template
    Services/                         (empty)
    Validators/                       (empty)
    ServiceCollectionsExtension.cs    <- templates/bll/ServiceCollectionsExtension.cs.template
__SERVICE_FULL__.BLL.Abstractions/    <- templates/bll/BllAbstractions.csproj.template
    Services/                         (empty)
__SERVICE_FULL__.DAL.Abstractions/    <- templates/dal/DalAbstractions.csproj.template
    Stores/                           (empty)
__DAL_PROJECT__/                      <- templates/dal-postgres/ or templates/dal-mongo/
    Entities/  Stores/                (empty)
    postgres only: EntityConfigurations/, Migrations/, Filters/MigrationsFilter.cs,
                   __SERVICE_FULL__DbContext.cs
    mongo only:    Mappers/
__SERVICE_FULL__.Models/              <- templates/models/Models.csproj.template
```

Empty folders do not survive in git, so create them only where the layer will obviously need them. The postgres
`Migrations` folder is carried by the `<Folder Include="Migrations\" />` entry in the csproj, exactly as
NewsService does it.

The API csproj template intentionally does **not** reference `MessageQueue` or `Common.Auth` — add those by hand
when the service actually produces or consumes messages, or needs JWT validation.

Three known defects in the existing `Program.cs` files that these templates deliberately do **not** reproduce:
`UseCors` without a matching `AddCors` (NotificationService); a duplicated `AddCustomControllers()` call
(`ConfigureCommonApiSettings()` already calls it); and a bare `if (app.Environment.IsDevelopment())` guard around
Swagger, which is false under the `local` profile because it sets `ASPNETCORE_ENVIRONMENT=Local`.

## Step 5: Create the test project

`tests/__SERVICE_CAMEL__/__SERVICE_FULL__.BLL.Tests/` from `templates/tests/BllTests.csproj.template`, plus a
verbatim copy of `templates/tests/MoqExtensions.cs`. Create empty `Services/` and `Controllers/` folders to
mirror the source tree, per the convention in the existing test projects.

## Step 6: Build the solution file

Do **not** hand-write the `.sln` — generate it, so GUIDs and the configuration matrix are valid. Run from the
repository root.

Two flags are non-negotiable, because the SDK's defaults produce a layout that does **not** match the other
solutions in this repository:

- `--format sln` — the installed SDK (10.0.301) defaults to the newer `.slnx` XML format; the rest of the
  repository uses classic `.sln`.
- `--include-references false` — this defaults to **true**, so the very first `add` silently drags in every
  transitively referenced project (Common, Common.API, Logger, API.Logger…) and files each one into an
  auto-generated solution-folder chain mirroring its path on disk (`src` → `server` → `common`). A later
  explicit `--solution-folder common` is then a no-op on an already-added project and just leaves an empty
  folder behind.

The repository convention, as seen in `NewsService.sln` / `CrawlerService.sln` / `NotificationService.sln`:
**the service's own projects sit at the solution root**, common projects go in a single flat `common` folder,
and test projects in a flat `tests` folder. `--in-root` is what keeps the service projects out of a folder.

```bash
dotnet new sln -n __SERVICE_FULL__ -o . --format sln
dotnet sln __SERVICE_FULL__.sln add --in-root --include-references false <the six service projects>
dotnet sln __SERVICE_FULL__.sln add --solution-folder common --include-references false src/server/common/Common/Common.csproj src/server/common/Common.API/Common.API.csproj src/server/common/Common.Auth/Common.Auth.csproj src/server/common/Logger/Logger.csproj src/server/common/API.Logger/API.Logger.csproj
dotnet sln __SERVICE_FULL__.sln add --solution-folder tests --include-references false tests/__SERVICE_CAMEL__/__SERVICE_FULL__.BLL.Tests/__SERVICE_FULL__.BLL.Tests.csproj
```

Add `src/server/common/MessageQueue/MessageQueue.csproj` only if the service produces or consumes messages.

**Any test project that is not the service's own** — e.g. `tests/common/Common.Auth.Tests` — goes into a
solution folder mirroring its path under `tests/`, so shared-library tests stay visually separate from the
service's tests. `--solution-folder` accepts a nested path and creates the intermediate folder:

```bash
dotnet sln __SERVICE_FULL__.sln add --solution-folder tests/common --include-references false tests/common/Common.Auth.Tests/Common.Auth.Tests.csproj
```

Afterwards, verify the layout — it is easy to get wrong and easy to check:

```bash
grep -E '^Project\("\{2150E333' __SERVICE_FULL__.sln    # expect: common, tests (+ tests/common if applicable)
sed -n '/NestedProjects/,/EndGlobalSection/p' __SERVICE_FULL__.sln
```

## Step 7: Verify

```bash
dotnet build __SERVICE_FULL__.sln
```

Fix any errors and rebuild until green. Then report to the user: the chosen port and `db` variant, the created
project paths, and the fact that the service is empty by design plus what to add next.

Optionally confirm it boots — `dotnet run --project src/server/__SERVICE_CAMEL__/__SERVICE_FULL__.API` and open
`https://localhost:__PORT__/swagger`.

## Step 8: Update the docs

Per the repository's standing instruction, update the root `CLAUDE.md` without waiting to be asked:

- add the service to **Project Overview** and to **Repository Layout**,
- note its solution file and HTTPS port,
- add a line to **Available Skills** if this skill is not listed there yet.

Update `readme.md` only if the service introduced new infrastructure (a new container, a new database). A
postgres service reusing the existing `local-news` database with its own schema does not.
