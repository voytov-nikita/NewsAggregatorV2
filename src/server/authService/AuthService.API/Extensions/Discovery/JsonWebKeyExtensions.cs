using AuthService.API.Models.Discovery;
using AuthService.Models.Auth;

namespace AuthService.API.Extensions.Discovery;

public static class JsonWebKeyExtensions
{
    public static JsonWebKeyResponse ToResponse(this JsonWebKeyModel model) => new()
    {
        KeyType = model.KeyType,
        Use = model.Use,
        Algorithm = model.Algorithm,
        KeyId = model.KeyId,
        Modulus = model.Modulus,
        Exponent = model.Exponent,
    };
}
