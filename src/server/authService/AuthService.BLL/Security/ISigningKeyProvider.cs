using Microsoft.IdentityModel.Tokens;

namespace AuthService.BLL.Security;

/// <summary>
/// Internal to the BLL on purpose: it deals in Microsoft.IdentityModel types, which should not
/// leak into the abstractions the API layer consumes. The API sees <see cref="Abstractions.Services.IJwksService"/> instead.
/// </summary>
public interface ISigningKeyProvider
{
    /// <summary>Key id of the active signing key. Derived from the key itself, so it is stable across restarts.</summary>
    string Kid { get; }

    /// <summary>Private key material - used only to sign, never exposed over HTTP.</summary>
    SigningCredentials SigningCredentials { get; }

    /// <summary>
    /// Public halves of the active key plus every retired key. These are re-imported from
    /// public-only parameters, so they carry no private material to leak.
    /// </summary>
    IReadOnlyList<RsaSecurityKey> PublicKeys { get; }
}
