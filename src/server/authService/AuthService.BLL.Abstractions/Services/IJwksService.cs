using AuthService.Models.Auth;

namespace AuthService.BLL.Abstractions.Services;

public interface IJwksService
{
    /// <summary>Public signing keys to publish at /.well-known/jwks.json.</summary>
    IReadOnlyCollection<JsonWebKeyModel> GetPublicKeys();
}
