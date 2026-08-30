using System.Text.Json;

using AuthService.API.Controllers;
using AuthService.API.Models.Discovery;
using AuthService.BLL.Abstractions.Services;
using AuthService.BLL.Abstractions.Settings;
using AuthService.Models.Auth;
using FluentAssertions;
using Moq;

namespace AuthService.BLL.Tests.Controllers;

public class DiscoveryControllerTests
{
    private readonly Mock<IJwksService> _jwksServiceMock = new();

    private readonly JwtSettings _jwtSettings = new()
    {
        Issuer = "https://localhost:7330",
        Audience = "news-aggregator",
    };

    private readonly DiscoveryController _sut;

    public DiscoveryControllerTests()
    {
        _sut = new DiscoveryController(_jwksServiceMock.Object, _jwtSettings);
    }

    [Fact]
    public void GetOpenIdConfiguration_Always_PointsAtThisServicesIssuerAndJwks()
    {
        // Act
        OpenIdConfigurationResponse result = _sut.GetOpenIdConfiguration();

        // Assert - the issuer must match every consumer's Authority byte for byte
        result.Issuer.Should().Be(_jwtSettings.Issuer);
        result.JwksUri.Should().Be("https://localhost:7330/.well-known/jwks.json");
        result.SigningAlgorithms.Should().BeEquivalentTo(["RS256"]);
        result.Claims.Should().Contain(["sub", "role", "permission"]);
    }

    [Theory]
    [InlineData("https://localhost:7330")]
    [InlineData("https://localhost:7330/")]
    public void GetOpenIdConfiguration_IssuerWithOrWithoutTrailingSlash_BuildsASingleSlashJwksUri(string issuer)
    {
        // Arrange
        _jwtSettings.Issuer = issuer;

        // Act
        OpenIdConfigurationResponse result = _sut.GetOpenIdConfiguration();

        // Assert
        result.JwksUri.Should().Be("https://localhost:7330/.well-known/jwks.json");
    }

    [Fact]
    public void GetJwks_Always_MapsEveryPublishedKey()
    {
        // Arrange
        var key = new JsonWebKeyModel
        {
            KeyType = "RSA",
            Use = "sig",
            Algorithm = "RS256",
            KeyId = "abc123",
            Modulus = "modulus",
            Exponent = "AQAB",
        };

        _jwksServiceMock.Setup(_ => _.GetPublicKeys()).Returns([key]);

        // Act
        JwksResponse result = _sut.GetJwks();

        // Assert
        JsonWebKeyResponse mapped = result.Keys.Should().ContainSingle().Subject;
        mapped.KeyId.Should().Be(key.KeyId);
        mapped.Modulus.Should().Be(key.Modulus);
        mapped.Exponent.Should().Be(key.Exponent);
    }

    [Fact]
    public void GetJwks_Always_SerializesWithRfc7517FieldNamesAndNoPrivateComponents()
    {
        // Arrange
        _jwksServiceMock.Setup(_ => _.GetPublicKeys()).Returns([new JsonWebKeyModel
        {
            KeyType = "RSA", Use = "sig", Algorithm = "RS256",
            KeyId = "abc123", Modulus = "modulus", Exponent = "AQAB",
        }]);

        // Act
        string json = JsonSerializer.Serialize(_sut.GetJwks());

        // Assert
        json.Should().Contain("\"kty\"").And.Contain("\"kid\"").And.Contain("\"n\"").And.Contain("\"e\"");

        foreach (string privateComponent in new[] { "\"d\":", "\"p\":", "\"q\":", "\"dp\":", "\"dq\":", "\"qi\":" })
        {
            json.Should().NotContain(privateComponent);
        }
    }
}
