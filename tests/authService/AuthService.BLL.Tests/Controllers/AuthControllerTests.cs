using System.Security.Claims;
using System.Text.Json;

using AuthService.API.Controllers;
using AuthService.API.Models.Auth;
using AuthService.API.Settings;
using AuthService.BLL.Abstractions.Services;
using AuthService.Models.Auth;
using AuthService.Models.Users;
using Common.Auth.Constants;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuthService.BLL.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthenticationService> _authenticationServiceMock = new();
    private readonly CookieSettings _cookieSettings = new();
    private readonly DefaultHttpContext _httpContext = new();
    private readonly AuthController _sut;

    public AuthControllerTests()
    {
        _sut = new AuthController(_authenticationServiceMock.Object, _cookieSettings)
        {
            ControllerContext = new ControllerContext { HttpContext = _httpContext },
        };
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsTokenAndSetsTheRefreshCookie()
    {
        // Arrange
        _authenticationServiceMock
            .Setup(_ => _.RegisterAsync(It.IsAny<RegisterModel>(), It.IsAny<RequestContextModel>()))
            .ReturnsAsync(SuccessResult());

        // Act
        ActionResult<AuthResponse> result = await _sut.Register(new RegisterRequest());

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<AuthResponse>()
            .Which.AccessToken.Should().Be("access-token");

        SetCookieHeader().Should().Contain("na_rt=refresh-token")
            .And.Contain("httponly")
            .And.Contain("path=/api/v1/auth");
    }

    [Fact]
    public async Task Login_ValidCredentials_NeverPutsTheRefreshTokenInTheResponseBody()
    {
        // The refresh token must only ever travel in the httpOnly cookie: a XSS can read a
        // response body, but not that cookie.

        // Arrange
        _authenticationServiceMock
            .Setup(_ => _.LoginAsync(It.IsAny<LoginModel>(), It.IsAny<RequestContextModel>()))
            .ReturnsAsync(SuccessResult());

        // Act
        ActionResult<AuthResponse> result = await _sut.Login(new LoginRequest());

        // Assert
        var response = (AuthResponse)((OkObjectResult)result.Result!).Value!;
        string serialized = JsonSerializer.Serialize(response);

        serialized.Should().NotContain("refresh-token");
        typeof(AuthResponse).GetProperties().Select(_ => _.Name)
            .Should().NotContain(name => name.Contains("Refresh", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Login_InvalidCredentials_Returns401()
    {
        // Arrange
        SetupLoginFailure(AuthErrorCode.InvalidCredentials);

        // Act
        ActionResult<AuthResponse> result = await _sut.Login(new LoginRequest());

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        SetCookieHeader().Should().BeEmpty();
    }

    [Fact]
    public async Task Login_LockedOutUser_Returns403()
    {
        // Arrange
        SetupLoginFailure(AuthErrorCode.LockedOut);

        // Act
        ActionResult<AuthResponse> result = await _sut.Login(new LoginRequest());

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
    }

    [Theory]
    [InlineData(AuthErrorCode.EmailAlreadyTaken)]
    [InlineData(AuthErrorCode.ValidationFailed)]
    public async Task Register_RejectedByBusinessRules_Returns400(AuthErrorCode error)
    {
        // Arrange
        _authenticationServiceMock
            .Setup(_ => _.RegisterAsync(It.IsAny<RegisterModel>(), It.IsAny<RequestContextModel>()))
            .ReturnsAsync(AuthResultModel.Failure(error, "nope"));

        // Act
        ActionResult<AuthResponse> result = await _sut.Register(new RegisterRequest());

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Refresh_RejectedToken_Returns401AndClearsTheCookie()
    {
        // Arrange
        _httpContext.Request.Headers.Cookie = "na_rt=stale";
        _authenticationServiceMock
            .Setup(_ => _.RefreshAsync(It.IsAny<string>(), It.IsAny<RequestContextModel>()))
            .ReturnsAsync(AuthResultModel.Failure(AuthErrorCode.InvalidRefreshToken));

        // Act
        ActionResult<AuthResponse> result = await _sut.Refresh();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        SetCookieHeader().Should().Contain("na_rt=;", "the worthless cookie is dropped");
    }

    [Fact]
    public async Task Refresh_ValidCookie_PassesTheCookieValueToTheService()
    {
        // Arrange
        _httpContext.Request.Headers.Cookie = "na_rt=the-token";
        _authenticationServiceMock
            .Setup(_ => _.RefreshAsync(It.IsAny<string>(), It.IsAny<RequestContextModel>()))
            .ReturnsAsync(SuccessResult());

        // Act
        await _sut.Refresh();

        // Assert
        _authenticationServiceMock.Verify(
            _ => _.RefreshAsync("the-token", It.IsAny<RequestContextModel>()),
            Times.Once);
    }

    [Fact]
    public async Task Logout_NoCookie_Returns204Anyway()
    {
        // Idempotent and non-informative: an unknown token must not be distinguishable.

        // Act
        IActionResult result = await _sut.Logout();

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Me_AuthenticatedUser_ReturnsTheProfile()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetUser(userId);

        _authenticationServiceMock
            .Setup(_ => _.GetByIdAsync(userId))
            .ReturnsAsync(new UserModel
            {
                Id = userId,
                Email = "a@b.co",
                UserName = "tester",
                Roles = [Roles.User],
                Permissions = [Permissions.CommentWrite],
            });

        // Act
        ActionResult<MeResponse> result = await _sut.Me();

        // Assert
        result.Value!.Id.Should().Be(userId);
        result.Value.Permissions.Should().BeEquivalentTo([Permissions.CommentWrite]);
    }

    [Fact]
    public async Task Me_UserDeletedSinceTokenWasIssued_Returns401()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetUser(userId);
        _authenticationServiceMock.Setup(_ => _.GetByIdAsync(userId)).ReturnsAsync((UserModel?)null);

        // Act
        ActionResult<MeResponse> result = await _sut.Me();

        // Assert
        result.Result.Should().BeOfType<UnauthorizedResult>();
    }

    private static AuthResultModel SuccessResult() => AuthResultModel.Success(
        new AccessTokenModel { Token = "access-token", ExpiresInSeconds = 900 },
        new RawRefreshTokenModel { RawToken = "refresh-token", ExpiresAt = DateTime.UtcNow.AddDays(14) },
        new UserModel { Id = Guid.NewGuid(), Email = "a@b.co", UserName = "tester" });

    private void SetupLoginFailure(AuthErrorCode error) =>
        _authenticationServiceMock
            .Setup(_ => _.LoginAsync(It.IsAny<LoginModel>(), It.IsAny<RequestContextModel>()))
            .ReturnsAsync(AuthResultModel.Failure(error, "nope"));

    private void SetUser(Guid userId) =>
        _httpContext.User = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(AuthClaimTypes.Subject, userId.ToString())], "TestAuth"));

    private string SetCookieHeader() =>
        string.Join(";", _httpContext.Response.Headers.SetCookie.ToArray()).ToLowerInvariant();
}
