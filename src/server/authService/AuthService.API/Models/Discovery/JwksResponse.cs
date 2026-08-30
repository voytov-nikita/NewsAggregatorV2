using System.Text.Json.Serialization;

namespace AuthService.API.Models.Discovery;

public class JwksResponse
{
    [JsonPropertyName("keys")]
    public IReadOnlyCollection<JsonWebKeyResponse> Keys { get; set; } = [];
}
