using System.Security.Claims;

using Common.Auth.Constants;
using Common.Auth.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Common.Auth.Tests.Services;

public class CurrentUserTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly CurrentUser _sut;

    public CurrentUserTests()
    {
        _sut = new CurrentUser(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void Properties_AuthenticatedUser_AreReadFromClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        SetUser(
            new Claim(AuthClaimTypes.Subject, userId.ToString()),
            new Claim(AuthClaimTypes.Email, "a@b.c"),
            new Claim(AuthClaimTypes.Name, "tester"),
            new Claim(AuthClaimTypes.Role, Roles.User),
            new Claim(AuthClaimTypes.Permission, Permissions.CommentWrite),
            new Claim(AuthClaimTypes.Permission, Permissions.NewsVote));

        // Assert
        _sut.IsAuthenticated.Should().BeTrue();
        _sut.UserId.Should().Be(userId);
        _sut.Email.Should().Be("a@b.c");
        _sut.DisplayName.Should().Be("tester");
        _sut.Roles.Should().BeEquivalentTo([Roles.User]);
        _sut.Permissions.Should().BeEquivalentTo([Permissions.CommentWrite, Permissions.NewsVote]);
    }

    [Fact]
    public void HasPermission_GrantedPermission_ReturnsTrue()
    {
        // Arrange
        SetUser(new Claim(AuthClaimTypes.Permission, Permissions.CommentWrite));

        // Assert
        _sut.HasPermission(Permissions.CommentWrite).Should().BeTrue();
        _sut.HasPermission(Permissions.CommentModerate).Should().BeFalse();
    }

    [Fact]
    public void Properties_NoHttpContext_ReturnDefaults()
    {
        // Arrange
        _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns((HttpContext?)null);

        // Assert
        _sut.IsAuthenticated.Should().BeFalse();
        _sut.UserId.Should().BeNull();
        _sut.Email.Should().BeNull();
        _sut.Permissions.Should().BeEmpty();
        _sut.HasPermission(Permissions.NewsVote).Should().BeFalse();
    }

    [Fact]
    public void UserId_AnonymousUser_ReturnsNull()
    {
        // Arrange - an unauthenticated identity has no authentication type and no claims
        _httpContextAccessorMock
            .Setup(_ => _.HttpContext)
            .Returns(new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) });

        // Assert
        _sut.IsAuthenticated.Should().BeFalse();
        _sut.UserId.Should().BeNull();
    }

    [Fact]
    public void UserId_MalformedSubjectClaim_ReturnsNull()
    {
        // Arrange
        SetUser(new Claim(AuthClaimTypes.Subject, "not-a-guid"));

        // Assert
        _sut.UserId.Should().BeNull();
    }

    private void SetUser(params Claim[] claims)
    {
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var context = new DefaultHttpContext { User = new ClaimsPrincipal(identity) };

        _httpContextAccessorMock.Setup(_ => _.HttpContext).Returns(context);
    }
}
