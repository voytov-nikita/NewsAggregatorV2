using System.Text.Json.Serialization;

namespace AuthService.API.Models.Discovery;

/// <summary>
/// JWKS entry, RFC 7517 field names. Only public RSA parameters are represented.
/// </summary>
public class JsonWebKeyResponse
{
    [JsonPropertyName("kty")]
    public string KeyType { get; set; } = null!;

    [JsonPropertyName("use")]
    public string Use { get; set; } = null!;

    [JsonPropertyName("alg")]
    public string Algorithm { get; set; } = null!;

    [JsonPropertyName("kid")]
    public string KeyId { get; set; } = null!;

    [JsonPropertyName("n")]
    public string Modulus { get; set; } = null!;

    [JsonPropertyName("e")]
    public string Exponent { get; set; } = null!;
}
