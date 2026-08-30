using AuthService.Models.Auth;
using AuthService.Models.Users;

namespace AuthService.BLL.Abstractions.Services;

public interface ITokenService
{
    AccessTokenModel CreateAccessToken(UserModel user);

    RawRefreshTokenModel CreateRefreshToken();

    /// <summary>Hashes a raw refresh token the same way <see cref="CreateRefreshToken"/> does, for lookup.</summary>
    string HashRefreshToken(string rawToken);
}
