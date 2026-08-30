using System.Security.Cryptography;

using AuthService.BLL.Abstractions.Services;
using AuthService.BLL.Security;
using AuthService.Models.Auth;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.BLL.Services;

public class JwksService(ISigningKeyProvider signingKeyProvider) : IJwksService
{
    public IReadOnlyCollection<JsonWebKeyModel> GetPublicKeys() =>
        signingKeyProvider.PublicKeys.Select(ToModel).ToArray();

    /// <summary>
    /// Exports the public parameters explicitly rather than using JsonWebKeyConverter, which
    /// happily includes d/p/q/dp/dq/qi when handed a key that has them. Passing
    /// includePrivateParameters: false makes leaking private material impossible here.
    /// </summary>
    private static JsonWebKeyModel ToModel(RsaSecurityKey key)
    {
        RSAParameters parameters = key.Rsa.ExportParameters(includePrivateParameters: false);

        return new JsonWebKeyModel
        {
            KeyType = JsonWebAlgorithmsKeyTypes.RSA,
            Use = "sig",
            Algorithm = SecurityAlgorithms.RsaSha256,
            KeyId = key.KeyId,
            Modulus = Base64UrlEncoder.Encode(parameters.Modulus),
            Exponent = Base64UrlEncoder.Encode(parameters.Exponent),
        };
    }
}
