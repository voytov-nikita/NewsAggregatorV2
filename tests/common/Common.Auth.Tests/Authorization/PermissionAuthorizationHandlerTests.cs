using System.Security.Claims;

using Common.Auth.Authorization;
using Common.Auth.Constants;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;

namespace Common.Auth.Tests.Authorization;

public class PermissionAuthorizationHandlerTests
{
    private readonly PermissionAuthorizationHandler _sut = new();

    [Fact]
    public async Task HandleAsync_UserWithMatchingPermissionClaim_Succeeds()
    {
        // Arrange
        var requirement = new PermissionRequirement(Permissions.CommentWrite);
        AuthorizationHandlerContext context = CreateContext(requirement, Permissions.CommentWrite);

        // Act
        await _sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_UserWithDifferentPermissionClaim_DoesNotSucceed()
    {
        // Arrange
        var requirement = new PermissionRequirement(Permissions.CommentModerate);
        AuthorizationHandlerContext context = CreateContext(requirement, Permissions.CommentWrite);

        // Act
        await _sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_UserWithoutAnyPermissionClaim_DoesNotSucceed()
    {
        // Arrange
        var requirement = new PermissionRequirement(Permissions.NewsVote);
        AuthorizationHandlerContext context = CreateContext(requirement);

        // Act
        await _sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_AdminRoleWithoutThePermissionClaim_DoesNotSucceed()
    {
        // Admin is seeded with every permission, so the handler must not carry a superuser bypass.

        // Arrange
        var requirement = new PermissionRequirement(Permissions.SourcesManage);
        var identity = new ClaimsIdentity([new Claim(AuthClaimTypes.Role, Roles.Admin)], "TestAuth");
        var context = new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(identity), null);

        // Act
        await _sut.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    private static AuthorizationHandlerContext CreateContext(
        PermissionRequirement requirement,
        params string[] permissions)
    {
        IEnumerable<Claim> claims = permissions.Select(_ => new Claim(AuthClaimTypes.Permission, _));
        var identity = new ClaimsIdentity(claims, "TestAuth");

        return new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(identity), null);
    }
}
