---
name: up-to-date-tests
description: Verify test coverage is up-to-date for a given service project. Checks that all BLL services, validators, extensions, mappers, DAL stores, and API controllers have corresponding test files and test methods.
user_invocable: true
arguments:
  - name: project
    description: "Service name to check (e.g., newsService, crawlerService, notificationService)"
    required: true
---

# Up-to-date Tests

Verify that tests exist and are current for all testable source files in the given service project.

## Step 1: Unit test coverage for Extensions, Mappers, BLL Services, and Validators

### 1.1 Collect source files

Gather all source files from the service that should have unit tests:

- `src/server/$project/*.BLL/Services/*.cs` — BLL services
- `src/server/$project/*.BLL/Validators/*.cs` — validators
- `src/server/$project/*.API/Extensions/**/*.cs` — API request/response extensions (ToModel, ToRequest, etc.)
- `src/server/$project/*.DAL*/Mappers/*.cs` — DAL mappers (ToEntity, ToDomain, etc.)
- `src/server/$project/*.DAL*/Extensions/*.cs` — DAL extensions

Exclude interfaces (`I*.cs`), DI registrations (`ServiceCollectionExtensions.cs`, `*Registration*.cs`), and `Program.cs`.

### 1.2 Check file-level coverage

For each source file `{Name}.cs`, look for a matching test file `{Name}Tests.cs` in the corresponding test project:

- BLL services/validators tests: `tests/$project/*.BLL.Tests/`
- Extensions/mappers tests: `tests/$project/*.BLL.Tests/` (or relevant test project)

Search recursively — test files may be in subdirectories (e.g., `Services/`, `Validators/`, `Extensions/`).

### 1.3 Handle missing test files

If a test file is missing entirely, use the `create-unit-test` skill to create it:
```
/create-unit-test {sourceFilePath}
```

### 1.4 Check method-level coverage

If the test file exists, verify method-level coverage:

1. Read the source file — collect all public/internal methods. For extensions, this includes static methods like `ToModel()`, `ToRequest()`, `ToEntity()`, etc. Pay attention to **overloads** — a method `ToModel(this NewsRequest request)` and `ToModel(this CommentRequest request)` are different and both need tests.
2. Read the test file — collect all test method names.
3. Compare: for each source method, there should be at least one test method that references it by name (following the `{MethodName}_{Condition}_{ExpectedResult}` convention). Consider the **return type or input type** in the test name to distinguish overloads (e.g., `ToModel_NewsRequest_ReturnsNewsModel` vs `ToModel_CommentRequest_ReturnsCommentModel`).
4. Report any source methods that have no corresponding test methods. For each missing test method, add it to the existing test file following the project's test conventions (xUnit + AutoFixture + FluentAssertions, `_sut` pattern).

## Step 2: Integration test coverage for DAL Stores and API Controllers

### 2.1 Collect source files

- `src/server/$project/*.DAL*/Stores/*.cs` — DAL store implementations (exclude interfaces `I*.cs`)
- `src/server/$project/*.API/Controllers/*.cs` — API controllers

### 2.2 Check file-level coverage

For each source file, look for a matching test file:

- Store `{Name}Store.cs` → `tests/$project/*.DAL.IntegrationTests/Stores/{Name}StoreTests.cs`
- Controller `{Name}Controller.cs` → `tests/$project/*.API.Tests/Controllers/{Name}ControllerTests.cs`

### 2.3 Handle missing test files

If a test file is missing, use the `create-integration-test` skill:
```
/create-integration-test {sourceFilePath}
```

### 2.4 Check method-level coverage

Same approach as Step 1.4 — compare public methods in the source file against test methods in the test file. For stores, check CRUD operations (`InsertAsync`, `GetAsync`, `UpdateAsync`, `DeleteAsync`, etc.). For controllers, check all action methods. Add missing test methods to the existing test file.

## Step 3: Run all tests

Run the tests for the service:

```bash
dotnet test tests/$project/ --verbosity normal
```

## Step 4: Fix failing tests

If any tests fail:

1. Analyze the error message and stack trace.
2. Fix the failing test or the source code as appropriate.
3. Re-run the tests.
4. **If the same error persists after 3 attempts** — stop, report the error message to the user, and wait for instructions before continuing.
