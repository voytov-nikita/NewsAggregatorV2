using AuthService.BLL.Abstractions.Settings;
using AuthService.BLL.Security;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace AuthService.BLL.Tests.Security;

public class SigningKeyProviderTests : IDisposable
{
    private readonly string _keysPath =
        Path.Combine(Path.GetTempPath(), $"authservice-keys-{Guid.NewGuid():N}");

    public void Dispose()
    {
        if (Directory.Exists(_keysPath))
        {
            Directory.Delete(_keysPath, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Constructor_NoKeyFile_GeneratesAndPersistsKey()
    {
        // Act
        using var sut = CreateSut();

        // Assert
        File.Exists(Path.Combine(_keysPath, "signing-key.pem")).Should().BeTrue();
        sut.Kid.Should().NotBeNullOrWhiteSpace();
        sut.SigningCredentials.Algorithm.Should().Be("RS256");
    }

    [Fact]
    public void Constructor_ExistingKeyFile_ReusesItAndKeepsTheSameKid()
    {
        // A random kid per startup would be a subtle bug: tokens issued before a restart would
        // carry a kid no longer present in JWKS and strict validators would reject them.

        // Arrange
        using var first = CreateSut();

        // Act
        using var second = CreateSut();

        // Assert
        second.Kid.Should().Be(first.Kid);
    }

    [Fact]
    public void PublicKeys_Always_ContainNoPrivateParameters()
    {
        // Act
        using var sut = CreateSut();

        // Assert
        sut.PublicKeys.Should().ContainSingle();
        sut.PublicKeys[0].KeyId.Should().Be(sut.Kid);

        Action exportPrivate = () => sut.PublicKeys[0].Rsa.ExportParameters(includePrivateParameters: true);
        exportPrivate.Should().Throw<Exception>("the published key must not hold private material");
    }

    [Fact]
    public void PublicKeys_RetiredKeyPresent_PublishesItAlongsideTheActiveKey()
    {
        // Retired keys stay in JWKS so tokens signed before a rotation keep validating.

        // Arrange
        using (var original = CreateSut())
        {
            Directory.CreateDirectory(Path.Combine(_keysPath, "retired"));
            File.Move(
                Path.Combine(_keysPath, "signing-key.pem"),
                Path.Combine(_keysPath, "retired", $"{original.Kid}.pem"));
        }

        // Act - a fresh provider generates a new active key and keeps publishing the retired one
        using var rotated = CreateSut();

        // Assert
        rotated.PublicKeys.Should().HaveCount(2);
        rotated.PublicKeys.Select(_ => _.KeyId).Should().Contain(rotated.Kid);
        rotated.PublicKeys.Select(_ => _.KeyId).Should().OnlyHaveUniqueItems();
    }

    private SigningKeyProvider CreateSut() =>
        new(new JwtSettings { KeysPath = _keysPath }, NullLogger<SigningKeyProvider>.Instance);
}
