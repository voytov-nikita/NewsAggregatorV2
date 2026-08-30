using Common.Auth.Authorization;
using Common.Auth.Constants;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;

namespace Common.Auth.Tests.Authorization;

public class PermissionPolicyProviderTests
{
    private readonly PermissionPolicyProvider _sut =
        new(Options.Create(new AuthorizationOptions()));

    [Fact]
    public async Task GetPolicyAsync_PermissionPolicyName_ReturnsPolicyWithPermissionRequirement()
    {
        // Act
        AuthorizationPolicy? policy = await _sut.GetPolicyAsync(
            PermissionPolicy.NameFor(Permissions.NewsVote));

        // Assert
        policy.Should().NotBeNull();
        policy!.Requirements.Should().ContainSingle(_ =>
            _ is PermissionRequirement && ((PermissionRequirement)_).Permission == Permissions.NewsVote);
        policy.Requirements.Should().Contain(_ => _ is DenyAnonymousAuthorizationRequirement);
    }

    [Fact]
    public async Task GetPolicyAsync_SamePolicyNameTwice_ReturnsCachedInstance()
    {
        // Arrange
        string name = PermissionPolicy.NameFor(Permissions.StatsView);

        // Act
        AuthorizationPolicy? first = await _sut.GetPolicyAsync(name);
        AuthorizationPolicy? second = await _sut.GetPolicyAsync(name);

        // Assert
        second.Should().BeSameAs(first);
    }

    [Fact]
    public async Task GetPolicyAsync_UnknownPolicyName_FallsBackToDefaultProvider()
    {
        // Act
        AuthorizationPolicy? policy = await _sut.GetPolicyAsync("SomeUnrelatedPolicy");

        // Assert
        policy.Should().BeNull();
    }

    [Theory]
    [InlineData("perm:", false)]
    [InlineData("permission:news.vote", false)]
    [InlineData("perm:news.vote", true)]
    public void TryGetPermission_PolicyName_DetectsPermissionPolicies(string policyName, bool expected)
    {
        // Act
        bool result = PermissionPolicy.TryGetPermission(policyName, out _);

        // Assert
        result.Should().Be(expected);
    }
}
