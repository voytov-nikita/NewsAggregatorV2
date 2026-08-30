using AuthService.API.Extensions.Auth;
using AuthService.API.Models.Auth;
using AuthService.API.Settings;
using AuthService.BLL.Abstractions.Services;
using AuthService.Models.Auth;
using AuthService.Models.Users;
using Common.Auth.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly CookieSettings _cookieSettings;

    public AuthController(IAuthenticationService authenticationService, CookieSettings cookieSettings)
    {
        _authenticationService = authenticationService;
        _cookieSettings = cookieSettings;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        AuthResultModel result = await _authenticationService.RegisterAsync(
            request.ToModel(),
            Request.ToRequestContext());

        return HandleAuthResult(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        AuthResultModel result = await _authenticationService.LoginAsync(
            request.ToModel(),
            Request.ToRequestContext());

        return HandleAuthResult(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        string? refreshToken = Request.GetRefreshToken(_cookieSettings);

        AuthResultModel result = await _authenticationService.RefreshAsync(
            refreshToken ?? string.Empty,
            Request.ToRequestContext());

        if (!result.Succeeded)
        {
            // A rejected refresh means the cookie is worthless - drop it so the client stops retrying.
            Response.ClearRefreshTokenCookie(_cookieSettings);
        }

        return HandleAuthResult(result);
    }

    /// <summary>
    /// Revokes the refresh token. The access token stays valid until it expires - that is inherent
    /// to stateless JWT, and the 15-minute lifetime is the mitigation. Instant revocation would
    /// need a jti denylist checked on every request.
    /// </summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromQuery] bool allDevices = false)
    {
        await _authenticationService.LogoutAsync(
            Request.GetRefreshToken(_cookieSettings),
            User.GetUserId(),
            allDevices);

        Response.ClearRefreshTokenCookie(_cookieSettings);

        // Idempotent and non-informative: an unknown token gets the same 204.
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<MeResponse>> Me()
    {
        Guid? userId = User.GetUserId();

        if (userId is null)
        {
            return Unauthorized();
        }

        UserModel? user = await _authenticationService.GetByIdAsync(userId.Value);

        return user is null ? Unauthorized() : user.ToMeResponse();
    }

    private ActionResult<AuthResponse> HandleAuthResult(AuthResultModel result)
    {
        if (!result.Succeeded)
        {
            return ToErrorResult(result);
        }

        Response.SetRefreshTokenCookie(result.RefreshToken!, _cookieSettings);

        return Ok(result.ToResponse());
    }

    private ActionResult<AuthResponse> ToErrorResult(AuthResultModel result) => result.Error switch
    {
        AuthErrorCode.InvalidCredentials => Unauthorized(Problem(result, StatusCodes.Status401Unauthorized)),
        AuthErrorCode.InvalidRefreshToken => Unauthorized(Problem(result, StatusCodes.Status401Unauthorized)),
        AuthErrorCode.LockedOut => StatusCode(
            StatusCodes.Status403Forbidden,
            Problem(result, StatusCodes.Status403Forbidden)),
        _ => BadRequest(Problem(result, StatusCodes.Status400BadRequest)),
    };

    private static ProblemDetails Problem(AuthResultModel result, int statusCode)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = result.Error.ToString(),
            Detail = result.Message,
        };

        if (result.ValidationErrors.Count > 0)
        {
            problem.Extensions["errors"] = result.ValidationErrors;
        }

        return problem;
    }
}
