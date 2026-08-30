namespace AuthService.Models.Auth;

/// <summary>
/// A single JWKS entry. Only the public RSA parameters exist on this type at all - there is no
/// field a private component could be written into.
/// </summary>
public class JsonWebKeyModel
{
    public string KeyType { get; set; } = null!;

    public string Use { get; set; } = null!;

    public string Algorithm { get; set; } = null!;

    public string KeyId { get; set; } = null!;

    /// <summary>RSA modulus, base64url.</summary>
    public string Modulus { get; set; } = null!;

    /// <summary>RSA public exponent, base64url.</summary>
    public string Exponent { get; set; } = null!;
}
