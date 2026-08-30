using AutoFixture;

using AuthService.API.Extensions.Auth;
using AuthService.API.Models.Auth;
using AuthService.Models.Auth;
using AuthService.Models.Users;
using FluentAssertions;

namespace AuthService.BLL.Tests.Extensions;

public class AuthRequestExtensionsTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToModel_RegisterRequest_MapsEveryField()
    {
        // Arrange
        var request = _fixture.Create<RegisterRequest>();

        // Act
        RegisterModel result = request.ToModel();

        // Assert
        result.Should().BeEquivalentTo(request);
    }

    [Fact]
    public void ToModel_LoginRequest_MapsEveryField()
    {
        // Arrange
        var request = _fixture.Create<LoginRequest>();

        // Act
        LoginModel result = request.ToModel();

        // Assert
        result.Should().BeEquivalentTo(request);
    }

    [Fact]
    public void ToResponse_UserModel_MapsThePublicProfileOnly()
    {
        // Arrange
        var model = _fixture.Create<UserModel>();

        // Act
        UserResponse result = model.ToResponse();

        // Assert
        result.Should().BeEquivalentTo(model, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public void ToMeResponse_UserModel_AlsoCarriesTheTimestamps()
    {
        // Arrange
        var model = _fixture.Create<UserModel>();

        // Act
        MeResponse result = model.ToMeResponse();

        // Assert
        result.Should().BeEquivalentTo(model);
    }

    [Fact]
    public void ToResponse_AuthResult_CopiesTheAccessTokenAndUserButNotTheRefreshToken()
    {
        // Arrange
        AuthResultModel result = AuthResultModel.Success(
            new AccessTokenModel { Token = "access", ExpiresInSeconds = 900 },
            new RawRefreshTokenModel { RawToken = "refresh" },
            _fixture.Create<UserModel>());

        // Act
        AuthResponse response = result.ToResponse();

        // Assert
        response.AccessToken.Should().Be("access");
        response.TokenType.Should().Be("Bearer");
        response.ExpiresIn.Should().Be(900);
        response.User.Id.Should().Be(result.User!.Id);
    }
}
