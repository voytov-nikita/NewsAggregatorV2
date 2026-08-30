using AuthService.BLL.Abstractions.Services;
using AuthService.BLL.Services;
using AuthService.DAL.Abstractions.Stores;
using AuthService.Models.Auth;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AuthService.BLL.Tests.Services;

public class RefreshTokenServiceTests
{
    private readonly Mock<ITokenService> _tokenServiceMock = new();
    private readonly Mock<IRefreshTokensStore> _refreshTokensStoreMock = new();
    private readonly RequestContextModel _context = new() { IpAddress = "127.0.0.1", UserAgent = "tests" };
    private readonly RefreshTokenService _sut;

    public RefreshTokenServiceTests()
    {
        _sut = new RefreshTokenService(
            _tokenServiceMock.Object,
            _refreshTokensStoreMock.Object,
            NullLogger<RefreshTokenService>.Instance);

        _tokenServiceMock.Setup(_ => _.HashRefreshToken(It.IsAny<string>())).Returns((string raw) => $"hash:{raw}");
    }

    [Fact]
    public async Task IssueAsync_ValidUser_StoresHashAndStartsANewFamily()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetupCreatedToken("fresh");

        // Act
        RawRefreshTokenModel result = await _sut.IssueAsync(userId, _context);

        // Assert
        result.RawToken.Should().Be("fresh");

        _refreshTokensStoreMock.Verify(
            _ => _.CreateAsync(It.Is<CreateRefreshTokenModel>(model =>
                model.UserId == userId
                && model.TokenHash == "hash:fresh"
                && model.FamilyId == null
                && model.CreatedByIp == "127.0.0.1")),
            Times.Once);
    }

    [Fact]
    public async Task RotateAsync_ValidToken_IssuesAReplacementInTheSameFamilyAndMarksTheOldOneRotated()
    {
        // Arrange
        RefreshTokenModel stored = StoredToken();
        var replacementId = Guid.NewGuid();

        _refreshTokensStoreMock.Setup(_ => _.FindByHashAsync("hash:old")).ReturnsAsync(stored);
        SetupCreatedToken("new", replacementId, stored.FamilyId);

        // Act
        RefreshRotationResultModel result = await _sut.RotateAsync("old", _context);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.UserId.Should().Be(stored.UserId);
        result.Token!.RawToken.Should().Be("new");

        _refreshTokensStoreMock.Verify(
            _ => _.CreateAsync(It.Is<CreateRefreshTokenModel>(model => model.FamilyId == stored.FamilyId)),
            Times.Once);
        _refreshTokensStoreMock.Verify(_ => _.MarkRotatedAsync(stored.Id, replacementId), Times.Once);
    }

    [Fact]
    public async Task RotateAsync_UnknownToken_FailsWithNotFound()
    {
        // Arrange
        _refreshTokensStoreMock.Setup(_ => _.FindByHashAsync(It.IsAny<string>())).ReturnsAsync((RefreshTokenModel?)null);

        // Act
        RefreshRotationResultModel result = await _sut.RotateAsync("whatever", _context);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Error.Should().Be(RefreshErrorCode.NotFound);
    }

    [Fact]
    public async Task RotateAsync_ExpiredToken_FailsAndDoesNotIssueAReplacement()
    {
        // Arrange
        RefreshTokenModel stored = StoredToken();
        stored.ExpiresAt = DateTime.UtcNow.AddMinutes(-1);

        _refreshTokensStoreMock.Setup(_ => _.FindByHashAsync("hash:old")).ReturnsAsync(stored);

        // Act
        RefreshRotationResultModel result = await _sut.RotateAsync("old", _context);

        // Assert
        result.Error.Should().Be(RefreshErrorCode.Expired);
        _refreshTokensStoreMock.Verify(_ => _.CreateAsync(It.IsAny<CreateRefreshTokenModel>()), Times.Never);
    }

    [Fact]
    public async Task RotateAsync_ReusedToken_RevokesTheWholeFamily()
    {
        // Replaying an already-revoked token means it leaked, so every live token in the chain
        // must die - including the newest one, whose holder is no longer trustworthy.

        // Arrange
        RefreshTokenModel stored = StoredToken();
        stored.RevokedAt = DateTime.UtcNow.AddMinutes(-5);

        _refreshTokensStoreMock.Setup(_ => _.FindByHashAsync("hash:old")).ReturnsAsync(stored);

        // Act
        RefreshRotationResultModel result = await _sut.RotateAsync("old", _context);

        // Assert
        result.Error.Should().Be(RefreshErrorCode.Reused);

        _refreshTokensStoreMock.Verify(_ => _.RevokeFamilyAsync(stored.FamilyId, true), Times.Once);
        _refreshTokensStoreMock.Verify(_ => _.CreateAsync(It.IsAny<CreateRefreshTokenModel>()), Times.Never);
    }

    [Fact]
    public async Task RevokeAsync_LiveToken_RevokesIt()
    {
        // Arrange
        RefreshTokenModel stored = StoredToken();
        _refreshTokensStoreMock.Setup(_ => _.FindByHashAsync("hash:old")).ReturnsAsync(stored);

        // Act
        await _sut.RevokeAsync("old");

        // Assert
        _refreshTokensStoreMock.Verify(_ => _.RevokeAsync(stored.Id), Times.Once);
    }

    [Fact]
    public async Task RevokeAsync_AlreadyRevokedToken_DoesNothing()
    {
        // Arrange
        RefreshTokenModel stored = StoredToken();
        stored.RevokedAt = DateTime.UtcNow;

        _refreshTokensStoreMock.Setup(_ => _.FindByHashAsync("hash:old")).ReturnsAsync(stored);

        // Act
        await _sut.RevokeAsync("old");

        // Assert
        _refreshTokensStoreMock.Verify(_ => _.RevokeAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task RevokeAsync_UnknownToken_DoesNotThrow()
    {
        // Arrange
        _refreshTokensStoreMock.Setup(_ => _.FindByHashAsync(It.IsAny<string>())).ReturnsAsync((RefreshTokenModel?)null);

        // Act
        Func<Task> act = () => _sut.RevokeAsync("whatever");

        // Assert
        await act.Should().NotThrowAsync();
    }

    private static RefreshTokenModel StoredToken() => new()
    {
        Id = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        FamilyId = Guid.NewGuid(),
        ExpiresAt = DateTime.UtcNow.AddDays(14),
    };

    private void SetupCreatedToken(string rawToken, Guid? createdId = null, Guid? familyId = null)
    {
        _tokenServiceMock.Setup(_ => _.CreateRefreshToken()).Returns(new RawRefreshTokenModel
        {
            RawToken = rawToken,
            TokenHash = $"hash:{rawToken}",
            ExpiresAt = DateTime.UtcNow.AddDays(14),
        });

        _refreshTokensStoreMock
            .Setup(_ => _.CreateAsync(It.IsAny<CreateRefreshTokenModel>()))
            .ReturnsAsync(new RefreshTokenModel
            {
                Id = createdId ?? Guid.NewGuid(),
                FamilyId = familyId ?? Guid.NewGuid(),
            });
    }
}
