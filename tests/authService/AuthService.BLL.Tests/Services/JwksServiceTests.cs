using AuthService.BLL.Abstractions.Settings;
using AuthService.BLL.Security;
using AuthService.BLL.Services;
using AuthService.Models.Auth;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace AuthService.BLL.Tests.Services;

public class JwksServiceTests : IDisposable
{
    private readonly string _keysPath =
        Path.Combine(Path.GetTempPath(), $"authservice-jwks-{Guid.NewGuid():N}");

    private readonly SigningKeyProvider _signingKeyProvider;
    private readonly JwksService _sut;

    public JwksServiceTests()
    {
        _signingKeyProvider = new SigningKeyProvider(
            new JwtSettings { KeysPath = _keysPath },
            NullLogger<SigningKeyProvider>.Instance);

        _sut = new JwksService(_signingKeyProvider);
    }

    public void Dispose()
    {
        _signingKeyProvider.Dispose();

        if (Directory.Exists(_keysPath))
        {
            Directory.Delete(_keysPath, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void GetPublicKeys_Always_ReturnsTheActiveKeyInJwksShape()
    {
        // Act
        IReadOnlyCollection<JsonWebKeyModel> result = _sut.GetPublicKeys();

        // Assert
        JsonWebKeyModel key = result.Should().ContainSingle().Subject;

        key.KeyType.Should().Be("RSA");
        key.Use.Should().Be("sig");
        key.Algorithm.Should().Be("RS256");
        key.KeyId.Should().Be(_signingKeyProvider.Kid);
        key.Modulus.Should().NotBeNullOrWhiteSpace();
        key.Exponent.Should().Be("AQAB", "65537 is the standard RSA public exponent");
    }

    [Fact]
    public void GetPublicKeys_Always_ExposesNoPrivateKeyMaterial()
    {
        // JsonWebKeyConverter would happily include d/p/q/dp/dq/qi when handed a private key;
        // the model has no field for them and the export is public-only. This test guards both.

        // Act
        IReadOnlyCollection<JsonWebKeyModel> result = _sut.GetPublicKeys();

        // Assert
        string serialized = System.Text.Json.JsonSerializer.Serialize(result);

        typeof(JsonWebKeyModel).GetProperties().Select(_ => _.Name)
            .Should().BeEquivalentTo("KeyType", "Use", "Algorithm", "KeyId", "Modulus", "Exponent");

        serialized.Should().NotContain("\"D\"").And.NotContain("\"P\"").And.NotContain("\"Q\"");
    }
}
