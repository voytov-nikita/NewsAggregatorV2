---
name: create-unit-test
description: Create a unit test file or add a test method for a given source file
user_invocable: true
arguments:
  - name: file
    description: Path to the source file for which to create tests (e.g., src/server/newsService/NewsService.BLL/Services/CommentsService.cs)
    required: true
  - name: method
    description: "Name of a specific method to create a test for. If omitted, create tests for all public/internal methods in the file."
    required: false
---

# Create Unit Test

Create or update a unit test file for the given source file, following the project's testing conventions.

## Step 1: Locate or create the test file

1. Determine the **service name** from the source file path (e.g., `src/server/newsService/...` → `newsService`).
2. Determine which **layer** the source file belongs to:
   - `*.BLL/Services/` or `*.BLL/Validators/` → test project: `tests/$service/*.BLL.Tests/`
   - `*.API/Extensions/` → test project: `tests/$service/*.BLL.Tests/Extensions/` (add API project reference if missing)
   - `*.DAL*/Extensions/` or `*.DAL*/Mappers/` → test project: `tests/$service/*.BLL.Tests/Extensions/` (add DAL project reference if missing)
3. Build the test file path: mirror the source file's subfolder structure, appending `Tests` to the class name.
   - Source: `Services/CommentsService.cs` → Test: `Services/CommentsServiceTests.cs`
   - Source: `Validators/NewsValidator.cs` → Test: `Validators/NewsValidatorTests.cs`
   - Source: `Extensions/News/NewsRequestExtensions.cs` → Test: `Extensions/NewsRequestExtensionsTests.cs`
4. If the **test file already exists**, read it and proceed to Step 3 (adding methods).
5. If the **test file does not exist**, proceed to Step 2.

## Step 2: Ensure the test project exists

1. Check if the test project directory and `.csproj` file exist at the expected path (e.g., `tests/$service/*.BLL.Tests/`).
2. If the test project **exists**:
   - Verify it has the required `ProjectReference` to the source file's project. If missing, add it to the `.csproj`.
3. If the test project **does not exist**, create it:
   - Create directory `tests/$service/{ServiceName}.BLL.Tests/`
   - Create the `.csproj` file with:
     - `<TargetFramework>` matching the source project (check the source `.csproj`)
     - `<LangVersion>latest</LangVersion>`
     - `<ImplicitUsings>enable</ImplicitUsings>`
     - `<Nullable>enable</Nullable>`
     - `<IsPackable>false</IsPackable>`
     - Required NuGet packages:
       - `Microsoft.NET.Test.Sdk`
       - `AutoFixture`
       - `AutoFixture.AutoMoq`
       - `FluentAssertions` (6.x)
       - `Moq`
       - `xunit`
       - `xunit.categories`
       - `Xunit.Extensions.AssemblyFixture`
       - `xunit.runner.visualstudio` (with `<PrivateAssets>all</PrivateAssets>`)
     - `<Using Include="Xunit"/>` in an `<ItemGroup>`
     - `ProjectReference` entries to the source project and its abstractions/models projects
   - Create a `MoqExtensions.cs` helper (look for an existing one in other test projects in the repo and copy it).
   - Add the new test project to the relevant `.sln` file using `dotnet sln add`.
   - If creation fails for any reason, report the error to the user and stop.

## Step 3: Create test methods

Read the source file and identify the target methods:
- If `$method` is provided, find only that method.
- If `$method` is omitted, find all `public` and `internal` methods (skip constructors, private methods, and DI registrations).

For each method, check if a test already exists in the test file (match by `{MethodName}_` prefix in test method names). Skip methods that already have tests.

### Test method naming convention

Each test method name consists of 3 parts separated by underscores:

```
{MethodName}_{InputDescription}_{ExpectedResult}
```

- **MethodName** — exact name of the method under test
- **InputDescription** — describes the input scenario:
  - For a happy-path test: `ValidData`, `ValidModel`, `ValidFilter`, etc.
  - For specific invalid input: describe concretely what's wrong, e.g., `ModelWithEmptyTitle`, `NullContent`, `ContentTooLong`
  - For `Theory` tests with multiple inputs: keep generic, describe the parameter category
- **ExpectedResult** — what should happen: `Success`, `ReturnsExpectedResult`, `ThrowsArgumentException`, `ReturnsEmpty`, etc.

### Test structure rules

Follow **AAA (Arrange, Act, Assert)** structure in every test:

```csharp
[Fact]
public async Task MethodName_InputDescription_ExpectedResult()
{
    // Arrange
    var model = _fixture.Create<SomeModel>();

    // Act
    Func<Task> act = () => _sut.MethodAsync(model);

    // Assert
    await act.Should().NotThrowAsync<Exception>();
    _dependencyMock.Verify(x => x.Method(It.Is(model.IsEqualTo())), Times.Once);
}
```

### Testing conventions

1. **AutoFixture** — use `Fixture` (or `IFixture`) to generate test data:
   - `_fixture.Create<T>()` for single objects
   - `_fixture.CreateMany<T>(count)` for collections
   - `_fixture.Build<T>().With(x => x.Prop, value).Create()` for customized objects
   - `_fixture.Customize<T>(c => c.With(...))` for type-level customization

2. **xUnit attributes**:
   - `[Fact]` for single-case tests
   - `[Theory]` with `[InlineData(...)]` for parameterized tests (e.g., multiple invalid inputs, boundary values)
   - Do NOT use `[MemberData]` or `[ClassData]` unless the data set is large or complex

3. **FluentAssertions** for all assertions:
   - `.Should().Be()`, `.Should().BeEquivalentTo()`, `.Should().NotThrow()`, `.Should().Throw<T>()`
   - For async: `.Should().NotThrowAsync()`, `.Should().ThrowAsync<T>()`
   - Use `.WithMessage("...")` when the exception message is important

4. **Moq** for dependencies:
   - Store mocks as `private Mock<IInterface> _mockName;` fields
   - Use `_mockName.Verify(...)` to check interactions
   - Use `It.Is(expected.IsEqualTo())` with MoqExtensions for argument matching
   - Use `_mockName.Setup(...).ReturnsAsync(...)` when the method needs a return value

5. **SUT (System Under Test)**:
   - Store as `private ClassName _sut;`
   - Initialize in the test class constructor with mocked dependencies

6. **Test class structure for services/validators** (classes with dependencies):
   ```csharp
   public class SomeServiceTests
   {
       private Mock<IDependency> _dependencyMock;
       private SomeService _sut;
       private Fixture _fixture = new Fixture();

       public SomeServiceTests()
       {
           _dependencyMock = new Mock<IDependency>();
           _sut = new SomeService(_dependencyMock.Object);
       }
   }
   ```

7. **Test class structure for extensions/mappers** (static methods, no dependencies):
   ```csharp
   public class SomeExtensionsTests
   {
       private readonly Fixture _fixture = new Fixture();

       [Fact]
       public void ToModel_SomeRequest_ReturnsSomeModel()
       {
           // Arrange
           var request = _fixture.Create<SomeRequest>();

           // Act
           var result = request.ToModel();

           // Assert
           result.PropertyA.Should().Be(request.PropertyA);
       }
   }
   ```

### What tests to generate per method

- **Always create a happy-path test** (`ValidData_Success` or `ValidInput_ReturnsExpectedResult`).
- **For void/Task pass-through methods** (service calls store): verify the dependency was called with correct arguments using `Verify(..., Times.Once)`.
- **For mapping/extension methods**: assert each mapped property matches the source.
- **For validator methods**: create a happy-path test AND one negative test per validation rule (e.g., `NullTitle_ThrowsArgumentException`, `ContentTooLong_ThrowsArgumentException`).
- **For methods with branching logic**: create a test per branch.

## Step 4: Verify

After creating or updating test files, build the test project:

```bash
dotnet build <test-project.csproj>
```

If the build fails, fix the issues (missing usings, wrong references, etc.) and retry. Do not run the tests — that is handled separately.
