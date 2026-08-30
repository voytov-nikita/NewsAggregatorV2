using AuthService.BLL.Abstractions.Services;
using AuthService.BLL.Abstractions.Validators;
using AuthService.BLL.Services;
using AuthService.DAL.Abstractions.Stores;
using AuthService.Models.Auth;
using AuthService.Models.Users;
using Common.Auth.Constants;
using FluentAssertions;
using Moq;

namespace AuthService.BLL.Tests.Services;

public class AuthenticationServiceTests
{
    private readonly Mock<IUsersStore> _usersStoreMock = new();
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock = new();
    private readonly Mock<IAuthValidator> _validatorMock = new();
    private readonly RequestContextModel _context = new();
    private readonly AuthenticationService _sut;

    public AuthenticationServiceTests()
    {
        _sut = new AuthenticationService(
            _usersStoreMock.Object,
            _tokenServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _validatorMock.Object);

        _tokenServiceMock
            .Setup(_ => _.CreateAccessToken(It.IsAny<UserModel>()))
            .Returns(new AccessTokenModel { Token = "access", ExpiresInSeconds = 900 });

        _refreshTokenServiceMock
            .Setup(_ => _.IssueAsync(It.IsAny<Guid>(), It.IsAny<RequestContextModel>()))
            .ReturnsAsync(new RawRefreshTokenModel { RawToken = "refresh", TokenHash = "hash" });
    }

    [Fact]
    public async Task RegisterAsync_NewEmail_CreatesUserInTheDefaultRoleAndIssuesTokens()
    {
        // Arrange
        UserModel user = CreateUser();
        _usersStoreMock.Setup(_ => _.FindByEmailAsync(user.Email)).ReturnsAsync((UserModel?)null);
        _usersStoreMock
            .Setup(_ => _.CreateAsync(It.IsAny<CreateUserModel>(), Roles.User))
            .ReturnsAsync(new CreateUserResultModel { Succeeded = true, User = user });

        // Act
        AuthResultModel result = await _sut.RegisterAsync(NewRegisterModel(user.Email), _context);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.AccessToken!.Token.Should().Be("access");
        result.RefreshToken!.RawToken.Should().Be("refresh");
        result.User.Should().Be(user);

        _usersStoreMock.Verify(_ => _.CreateAsync(It.IsAny<CreateUserModel>(), Roles.User), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_FailsWithoutCreatingAUser()
    {
        // Arrange
        _usersStoreMock.Setup(_ => _.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(CreateUser());

        // Act
        AuthResultModel result = await _sut.RegisterAsync(NewRegisterModel(), _context);

        // Assert
        result.Error.Should().Be(AuthErrorCode.EmailAlreadyTaken);
        _usersStoreMock.Verify(_ => _.CreateAsync(It.IsAny<CreateUserModel>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_IdentityRejectsThePassword_SurfacesItsMessages()
    {
        // Arrange
        _usersStoreMock.Setup(_ => _.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((UserModel?)null);
        _usersStoreMock
            .Setup(_ => _.CreateAsync(It.IsAny<CreateUserModel>(), It.IsAny<string>()))
            .ReturnsAsync(new CreateUserResultModel
            {
                Succeeded = false,
                Errors = ["Passwords must be at least 8 characters."],
            });

        // Act
        AuthResultModel result = await _sut.RegisterAsync(NewRegisterModel(), _context);

        // Assert
        result.Error.Should().Be(AuthErrorCode.ValidationFailed);
        result.ValidationErrors.Should().ContainSingle().Which.Should().Contain("8 characters");
    }

    [Fact]
    public async Task RegisterAsync_MalformedModel_LetsTheValidatorThrow()
    {
        // Arrange
        _validatorMock
            .Setup(_ => _.Validate(It.IsAny<RegisterModel>()))
            .Throws(new ArgumentException("Email is required."));

        // Act
        Func<Task> act = () => _sut.RegisterAsync(NewRegisterModel(), _context);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ResetsFailuresRecordsLoginAndIssuesTokens()
    {
        // Arrange
        UserModel user = CreateUser();
        SetupExistingUser(user, passwordValid: true);

        // Act
        AuthResultModel result = await _sut.LoginAsync(NewLoginModel(user.Email), _context);

        // Assert
        result.Succeeded.Should().BeTrue();
        _usersStoreMock.Verify(_ => _.ResetFailedAttemptsAsync(user.Id), Times.Once);
        _usersStoreMock.Verify(_ => _.SetLastLoginAsync(user.Id, It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_FailsAndCountsTheAttempt()
    {
        // Arrange
        UserModel user = CreateUser();
        SetupExistingUser(user, passwordValid: false);

        // Act
        AuthResultModel result = await _sut.LoginAsync(NewLoginModel(user.Email), _context);

        // Assert
        result.Error.Should().Be(AuthErrorCode.InvalidCredentials);
        _usersStoreMock.Verify(_ => _.RegisterFailedAttemptAsync(user.Id), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ReturnsTheSameFailureAsAWrongPassword()
    {
        // The two must be indistinguishable, otherwise the endpoint enumerates registered emails.

        // Arrange
        UserModel user = CreateUser();
        SetupExistingUser(user, passwordValid: false);
        AuthResultModel wrongPassword = await _sut.LoginAsync(NewLoginModel(user.Email), _context);

        _usersStoreMock.Setup(_ => _.FindByEmailAsync("nobody@example.com")).ReturnsAsync((UserModel?)null);

        // Act
        AuthResultModel unknownEmail = await _sut.LoginAsync(NewLoginModel("nobody@example.com"), _context);

        // Assert
        unknownEmail.Error.Should().Be(wrongPassword.Error);
        unknownEmail.Message.Should().Be(wrongPassword.Message);
    }

    [Fact]
    public async Task LoginAsync_LockedOutUser_FailsWithoutCheckingThePassword()
    {
        // Arrange
        UserModel user = CreateUser();
        _usersStoreMock.Setup(_ => _.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _usersStoreMock.Setup(_ => _.IsLockedOutAsync(user.Id)).ReturnsAsync(true);

        // Act
        AuthResultModel result = await _sut.LoginAsync(NewLoginModel(user.Email), _context);

        // Assert
        result.Error.Should().Be(AuthErrorCode.LockedOut);
        _usersStoreMock.Verify(_ => _.VerifyPasswordAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_ValidToken_ReturnsFreshTokensForTheOwner()
    {
        // Arrange
        UserModel user = CreateUser();
        SetupRotation(user.Id, succeeded: true);
        _usersStoreMock.Setup(_ => _.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _usersStoreMock.Setup(_ => _.IsLockedOutAsync(user.Id)).ReturnsAsync(false);

        // Act
        AuthResultModel result = await _sut.RefreshAsync("raw", _context);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.RefreshToken!.RawToken.Should().Be("rotated");
        result.User.Should().Be(user);
    }

    [Fact]
    public async Task RefreshAsync_EmptyToken_FailsWithoutTouchingTheStore()
    {
        // Act
        AuthResultModel result = await _sut.RefreshAsync(string.Empty, _context);

        // Assert
        result.Error.Should().Be(AuthErrorCode.InvalidRefreshToken);
        _refreshTokenServiceMock.Verify(
            _ => _.RotateAsync(It.IsAny<string>(), It.IsAny<RequestContextModel>()),
            Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_RotationRejected_FailsWithInvalidRefreshToken()
    {
        // Arrange
        SetupRotation(Guid.NewGuid(), succeeded: false);

        // Act
        AuthResultModel result = await _sut.RefreshAsync("raw", _context);

        // Assert
        result.Error.Should().Be(AuthErrorCode.InvalidRefreshToken);
    }

    [Fact]
    public async Task RefreshAsync_UserLockedOutSinceIssue_RevokesEveryTokenTheyHold()
    {
        // Arrange
        UserModel user = CreateUser();
        SetupRotation(user.Id, succeeded: true);
        _usersStoreMock.Setup(_ => _.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _usersStoreMock.Setup(_ => _.IsLockedOutAsync(user.Id)).ReturnsAsync(true);

        // Act
        AuthResultModel result = await _sut.RefreshAsync("raw", _context);

        // Assert
        result.Error.Should().Be(AuthErrorCode.InvalidRefreshToken);
        _refreshTokenServiceMock.Verify(_ => _.RevokeAllForUserAsync(user.Id), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_AllDevices_RevokesEveryTokenInsteadOfJustTheCurrentOne()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        await _sut.LogoutAsync("raw", userId, allDevices: true);

        // Assert
        _refreshTokenServiceMock.Verify(_ => _.RevokeAllForUserAsync(userId), Times.Once);
        _refreshTokenServiceMock.Verify(_ => _.RevokeAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LogoutAsync_NoToken_DoesNothingAndDoesNotThrow()
    {
        // Act
        Func<Task> act = () => _sut.LogoutAsync(null, null, allDevices: false);

        // Assert
        await act.Should().NotThrowAsync();
        _refreshTokenServiceMock.Verify(_ => _.RevokeAsync(It.IsAny<string>()), Times.Never);
    }

    private static UserModel CreateUser() => new()
    {
        Id = Guid.NewGuid(),
        Email = "tester@example.com",
        UserName = "tester",
        Roles = [Roles.User],
        Permissions = [Permissions.CommentWrite],
    };

    private static RegisterModel NewRegisterModel(string email = "tester@example.com") => new()
    {
        Email = email,
        UserName = "tester",
        Password = "Passw0rd!",
    };

    private static LoginModel NewLoginModel(string email = "tester@example.com") => new()
    {
        Email = email,
        Password = "Passw0rd!",
    };

    private void SetupExistingUser(UserModel user, bool passwordValid)
    {
        _usersStoreMock.Setup(_ => _.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _usersStoreMock.Setup(_ => _.IsLockedOutAsync(user.Id)).ReturnsAsync(false);
        _usersStoreMock.Setup(_ => _.VerifyPasswordAsync(user.Id, It.IsAny<string>())).ReturnsAsync(passwordValid);
    }

    private void SetupRotation(Guid userId, bool succeeded) =>
        _refreshTokenServiceMock
            .Setup(_ => _.RotateAsync(It.IsAny<string>(), It.IsAny<RequestContextModel>()))
            .ReturnsAsync(succeeded
                ? RefreshRotationResultModel.Success(userId, new RawRefreshTokenModel { RawToken = "rotated" })
                : RefreshRotationResultModel.Failure(RefreshErrorCode.Reused));
}
