---
name: create-integration-test
description: Create an integration test file for a DAL store or API controller (not yet implemented)
user_invocable: true
arguments:
  - name: file
    description: Path to the source file (DAL store or API controller) for which to create integration tests
    required: true
  - name: method
    description: "Name of a specific method to create a test for. If omitted, applies to all public methods."
    required: false
---

# Create Integration Test

> **Status: NOT IMPLEMENTED**
>
> This skill is a placeholder. Integration test creation is not yet supported.
> When invoked, inform the user that integration test generation is not yet implemented and skip test creation.
> Log the source file path and method (if provided) so the user knows what was skipped.

## Expected future behavior

Once implemented, this skill should:

1. Determine the test project based on the source file layer:
   - `*.DAL*/Stores/*.cs` → `tests/$service/*.DAL.IntegrationTests/Stores/`
   - `*.API/Controllers/*.cs` → `tests/$service/*.API.Tests/Controllers/`
2. Create the test project if missing (with Testcontainers for the relevant database).
3. Create test files following the `BaseTest` / `EnvironmentFixture` pattern.
4. Generate integration test methods using real database operations (insert → query → assert).
