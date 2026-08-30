using AuthService.BLL.Abstractions.Settings;
using AuthService.BLL.Security;
using AuthService.BLL.Services;
using AuthService.Models.Auth;
using AuthService.Models.Users;
using Common.Auth.Constants;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.BLL.Tests.Services;

public class TokenServiceTests : IDisposable
{
    private readonly string _keysPath =
        Path.Combine(Path.GetTempPath(), $"authservice-tokens-{Guid.NewGuid():N}");

    private readonly JwtSettings _settings;
    private readonly SigningKeyProvider _signingKeyProvider;
    private readonly TokenService _sut;

    public TokenServiceTests()
    {
        _settings = new JwtSettings
        {
            Issuer = "https://localhost:7330",
            Audience = "news-aggregator",
            AccessTokenMinutes = 15,
            RefreshTokenDays = 14,
            KeysPath = _keysPath,
        };

        _signingKeyProvider = new SigningKeyProvider(_settings, NullLogger<SigningKeyProvider>.Instance);
        _sut = new TokenService(_settings, _signingKeyProvider);
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
    public void CreateAccessToken_User_ContainsSubEmailRolesAndPermissionClaims()
    {
        // Arrange
        UserModel user = CreateUser();

        // Act
        AccessTokenModel result = _sut.CreateAccessToken(user);

        // Assert
        JsonWebToken token = new JsonWebTokenHandler().ReadJsonWebToken(result.Token);

        token.Issuer.Should().Be(_settings.Issuer);
        token.Audiences.Should().ContainSingle().Which.Should().Be(_settings.Audience);
        token.GetClaim(AuthClaimTypes.Subject).Value.Should().Be(user.Id.ToString());
        token.GetClaim(AuthClaimTypes.Email).Value.Should().Be(user.Email);
        token.GetClaim(AuthClaimTypes.Name).Value.Should().Be(user.DisplayName);
        token.GetClaim(JwtRegisteredClaimNames.Jti).Value.Should().NotBeNullOrWhiteSpace();

        ValuesOf(token, AuthClaimTypes.Role).Should().BeEquivalentTo(user.Roles);
        ValuesOf(token, AuthClaimTypes.Permission).Should().BeEquivalentTo(user.Permissions);
    }

    [Fact]
    public void CreateAccessToken_User_UsesRs256AndCarriesTheProvidersKid()
    {
        // Act
        AccessTokenModel result = _sut.CreateAccessToken(CreateUser());

        // Assert
        JsonWebToken token = new JsonWebTokenHandler().ReadJsonWebToken(result.Token);

        token.Alg.Should().Be(SecurityAlgorithms.RsaSha256);
        token.Kid.Should().Be(_signingKeyProvider.Kid);
    }

    [Fact]
    public async Task CreateAccessToken_User_VerifiesAgainstThePublishedPublicKey()
    {
        // The whole point of RS256 + JWKS: other services validate with the public half only.

        // Arrange
        AccessTokenModel result = _sut.CreateAccessToken(CreateUser());

        var validationParameters = new TokenValidationParameters
        {
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            IssuerSigningKey = _signingKeyProvider.PublicKeys[0],
            ValidateIssuerSigningKey = true,
        };

        // Act
        TokenValidationResult validation =
            await new JsonWebTokenHandler().ValidateTokenAsync(result.Token, validationParameters);

        // Assert
        validation.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateAccessToken_User_ExpiresAfterTheConfiguredLifetime()
    {
        // Act
        AccessTokenModel result = _sut.CreateAccessToken(CreateUser());

        // Assert
        result.ExpiresInSeconds.Should().Be(15 * 60);
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(15), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CreateAccessToken_UserWithoutDisplayName_FallsBackToUserName()
    {
        // Arrange
        UserModel user = CreateUser();
        user.DisplayName = null;

        // Act
        AccessTokenModel result = _sut.CreateAccessToken(user);

        // Assert
        JsonWebToken token = new JsonWebTokenHandler().ReadJsonWebToken(result.Token);
        token.GetClaim(AuthClaimTypes.Name).Value.Should().Be(user.UserName);
    }

    [Fact]
    public void CreateRefreshToken_Always_ReturnsRawTokenWithMatchingHash()
    {
        // Act
        RawRefreshTokenModel result = _sut.CreateRefreshToken();

        // Assert
        result.RawToken.Should().NotBeNullOrWhiteSpace();
        result.TokenHash.Should().Be(_sut.HashRefreshToken(result.RawToken));
        result.TokenHash.Should().HaveLength(64, "SHA-256 rendered as lowercase hex is 64 chars");
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(14), TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CreateRefreshToken_CalledTwice_ReturnsDifferentTokens()
    {
        // Act
        RawRefreshTokenModel first = _sut.CreateRefreshToken();
        RawRefreshTokenModel second = _sut.CreateRefreshToken();

        // Assert
        second.RawToken.Should().NotBe(first.RawToken);
        second.TokenHash.Should().NotBe(first.TokenHash);
    }

    [Fact]
    public void HashRefreshToken_SameInput_IsDeterministic()
    {
        // Act & Assert - lookup by hash depends on this
        _sut.HashRefreshToken("some-token").Should().Be(_sut.HashRefreshToken("some-token"));
        _sut.HashRefreshToken("some-token").Should().NotBe(_sut.HashRefreshToken("other-token"));
    }

    private static UserModel CreateUser() => new()
    {
        Id = Guid.NewGuid(),
        Email = "tester@example.com",
        UserName = "tester",
        DisplayName = "Tester",
        Roles = [Roles.User],
        Permissions = [Permissions.CommentWrite, Permissions.NewsVote],
    };

    private static string[] ValuesOf(JsonWebToken token, string claimType) =>
        token.Claims.Where(_ => _.Type == claimType).Select(_ => _.Value).ToArray();
}
