using AuthService.BLL.Validators;
using AuthService.Models.Auth;
using FluentAssertions;

namespace AuthService.BLL.Tests.Validators;

public class AuthValidatorTests
{
    private readonly AuthValidator _sut = new();

    [Fact]
    public void Validate_ValidRegisterModel_DoesNotThrow()
    {
        // Act
        Action act = () => _sut.Validate(ValidRegisterModel());

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("no-at-sign")]
    [InlineData("missing@domain")]
    [InlineData("spaces in@email.com")]
    public void Validate_RegisterModelWithInvalidEmail_ThrowsArgumentException(string email)
    {
        // Arrange
        RegisterModel model = ValidRegisterModel();
        model.Email = email;

        // Act
        Action act = () => _sut.Validate(model);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_RegisterModelWithEmptyUserName_ThrowsArgumentException()
    {
        // Arrange
        RegisterModel model = ValidRegisterModel();
        model.UserName = "  ";

        // Act
        Action act = () => _sut.Validate(model);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("User name is required.*");
    }

    [Fact]
    public void Validate_RegisterModelWithEmptyPassword_ThrowsArgumentException()
    {
        // Arrange
        RegisterModel model = ValidRegisterModel();
        model.Password = string.Empty;

        // Act
        Action act = () => _sut.Validate(model);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Password is required.*");
    }

    [Fact]
    public void Validate_RegisterModelWithShortPassword_DoesNotThrow()
    {
        // Length and complexity are Identity's PasswordOptions, not a shape problem - the
        // validator must not duplicate (and drift from) those rules.

        // Arrange
        RegisterModel model = ValidRegisterModel();
        model.Password = "abc";

        // Act
        Action act = () => _sut.Validate(model);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_RegisterModelWithOverlongEmail_ThrowsArgumentException()
    {
        // Arrange
        RegisterModel model = ValidRegisterModel();
        model.Email = $"{new string('a', 250)}@example.com";

        // Act
        Action act = () => _sut.Validate(model);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("*at most 256 characters*");
    }

    [Fact]
    public void Validate_ValidLoginModel_DoesNotThrow()
    {
        // Act
        Action act = () => _sut.Validate(new LoginModel { Email = "a@b.co", Password = "Passw0rd!" });

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Validate_LoginModelWithEmptyPassword_ThrowsArgumentException()
    {
        // Act
        Action act = () => _sut.Validate(new LoginModel { Email = "a@b.co", Password = string.Empty });

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_NullModel_ThrowsArgumentNullException()
    {
        // Act
        Action registerAct = () => _sut.Validate((RegisterModel)null!);
        Action loginAct = () => _sut.Validate((LoginModel)null!);

        // Assert
        registerAct.Should().Throw<ArgumentNullException>();
        loginAct.Should().Throw<ArgumentNullException>();
    }

    private static RegisterModel ValidRegisterModel() => new()
    {
        Email = "tester@example.com",
        UserName = "tester",
        Password = "Passw0rd!",
        DisplayName = "Tester",
    };
}
