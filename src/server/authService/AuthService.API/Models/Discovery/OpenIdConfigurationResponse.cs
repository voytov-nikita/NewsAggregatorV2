using System.Text.Json.Serialization;

namespace AuthService.API.Models.Discovery;

/// <summary>
/// Minimal OIDC discovery document. Enough for AddJwtBearer(o =&gt; o.Authority = ...) to find the
/// signing keys; this service is not a full OAuth2 authorization server.
/// </summary>
public class OpenIdConfigurationResponse
{
    [JsonPropertyName("issuer")]
    public string Issuer { get; set; } = null!;

    [JsonPropertyName("jwks_uri")]
    public string JwksUri { get; set; } = null!;

    [JsonPropertyName("token_endpoint")]
    public string TokenEndpoint { get; set; } = null!;

    [JsonPropertyName("id_token_signing_alg_values_supported")]
    public IReadOnlyCollection<string> SigningAlgorithms { get; set; } = [];

    [JsonPropertyName("grant_types_supported")]
    public IReadOnlyCollection<string> GrantTypes { get; set; } = [];

    [JsonPropertyName("subject_types_supported")]
    public IReadOnlyCollection<string> SubjectTypes { get; set; } = [];

    [JsonPropertyName("claims_supported")]
    public IReadOnlyCollection<string> Claims { get; set; } = [];
}
