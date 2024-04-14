using AutoFixture;
using CityLibrary.Shared.ExceptionHandling;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using UserServiceApi.Dtos.Authentication;
using UserServiceApi.Dtos.Token;
using UserServiceApi.Dtos.Token.Records;
using UserServiceApi.Entities;
using UserServiceApi.Entities.Cache;
using UserServiceApi.Helpers;
using UserServiceApi.Resources;
using UserServiceApi.Services.Token.Classes;
using UserServiceApi.Services.Token.Interfaces;
using UserServiceApi.Services.User.Interfaces;

namespace UserServiceApi.UnitTests;

public class AuthenticationServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<ILogger<AuthenticationService>> _loggerMock;
    private readonly Mock<IStringLocalizer<ExceptionsResource>> _localizerMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock;
    private readonly Mock<IAccessTokenService> _accessTokenServiceMock;
    private readonly Mock<IVerifyPasswords> _verifyPasswordsMock;
    private readonly AuthenticationService _authenticationService;

    public AuthenticationServiceTests()
    {
        _loggerMock = new Mock<ILogger<AuthenticationService>>();
        _localizerMock = new Mock<IStringLocalizer<ExceptionsResource>>();
        _userServiceMock = new Mock<IUserService>();
        _refreshTokenServiceMock = new Mock<IRefreshTokenService>();
        _accessTokenServiceMock = new Mock<IAccessTokenService>();
        _verifyPasswordsMock = new Mock<IVerifyPasswords>();

        _authenticationService = new AuthenticationService(
            _loggerMock.Object,
            _localizerMock.Object,
            _userServiceMock.Object,
            _refreshTokenServiceMock.Object,
            _accessTokenServiceMock.Object,
            _verifyPasswordsMock.Object);

        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokenRecord()
    {
        // Arrange
        var loginDto = _fixture.Create<LoginDto>();
        var user = _fixture.Create<Users>();
        var token = _fixture.Create<CreateTokenResultDto>();
        var newRefreshToken = _fixture.Create<RefreshTokens>();

        _userServiceMock.Setup(x => x.GetUserByUserNameAsync(loginDto.UserName)).ReturnsAsync(user);
        _verifyPasswordsMock.Setup(x => x.VerifyPasswordHash(loginDto.Password, user.Password, _localizerMock.Object)).Returns(true);
        _accessTokenServiceMock.Setup(x => x.CreateToken(It.IsAny<CreateTokenDto>())).Returns(token);
        _refreshTokenServiceMock.Setup(x => x.CreateOrUpdateAsync(It.IsAny<RefreshTokens>(), true)).Returns(Task.CompletedTask);

        // Act
        var result = await _authenticationService.LoginAsync(loginDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(token.AccessToken, result.AccessToken);
        Assert.Equal(token.RefreshTokenKey, result.RefreshToken);
        Assert.IsType<ReturnTokenRecord>(result);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsCustomBusinessException()
    {
        // Arrange
        var loginDto = _fixture.Create<LoginDto>();
        var user = _fixture.Create<Users>();

        _userServiceMock.Setup(x => x.GetUserByUserNameAsync(loginDto.UserName)).ReturnsAsync(user);
        _verifyPasswordsMock.Setup(x => x.VerifyPasswordHash(loginDto.Password, user.Password, _localizerMock.Object)).Returns(false);


        // Act & Assert
        await Assert.ThrowsAsync<CustomBusinessException>(() => _authenticationService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LoginAsync_WithNotExistingUser_ThrowsCustomBusinessException()
    {
        // Arrange
        var loginDto = _fixture.Create<LoginDto>();
        Users user = null;

        _userServiceMock.Setup(x => x.GetUserByUserNameAsync(loginDto.UserName)).ReturnsAsync(user);
        _verifyPasswordsMock.Setup(x => x.VerifyPasswordHash(loginDto.Password, "fake_password", _localizerMock.Object)).Returns(true);


        // Act & Assert
        await Assert.ThrowsAsync<CustomBusinessException>(() => _authenticationService.LoginAsync(loginDto));
    }

    [Fact]
    public async Task LogoutAsync_ValidTokenKey_CallsDeleteAsync()
    {
        // Arrange
        var tokenKey = _fixture.Create<string>();

        // Act
        await _authenticationService.LogoutAsync(tokenKey);

        // Assert
        _refreshTokenServiceMock.Verify(x => x.DeleteAsync(tokenKey), Times.Once);
    }

    [Fact]
    public async Task RefreshLoginTokenAsync_ValidRefreshTokenKey_ReturnsTokenRecord()
    {
        // Arrange
        var refreshTokenKey = _fixture.Create<string>();
        var oldToken = _fixture.Create<RefreshTokens>();
        oldToken.DueTime = DateTime.Now.AddHours(2);
        var newToken = _fixture.Create<CreateTokenResultDto>();
        var newRefreshToken = _fixture.Create<RefreshTokens>();

        _refreshTokenServiceMock.Setup(x => x.GetByKeyAsync(refreshTokenKey)).ReturnsAsync(oldToken);
        _accessTokenServiceMock.Setup(x => x.CreateToken(It.IsAny<CreateTokenDto>())).Returns(newToken);
        _refreshTokenServiceMock.Setup(x => x.CreateOrUpdateAsync(It.IsAny<RefreshTokens>(), true)).Returns(Task.CompletedTask);
        _refreshTokenServiceMock.Setup(x => x.DeleteAsync(refreshTokenKey)).Returns(Task.CompletedTask);

        // Act
        var result = await _authenticationService.RefreshLoginTokenAsync(refreshTokenKey);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newToken.AccessToken, result.AccessToken);
        Assert.Equal(newToken.RefreshTokenKey, result.RefreshToken);
        Assert.IsType<ReturnTokenRecord>(result);
    }

    [Fact]
    public async Task RefreshLoginTokenAsync_InValidRefreshTokenKey_ReturnsTokenRecord()
    {
        // Arrange
        var refreshTokenKey = _fixture.Create<string>();
        RefreshTokens oldToken = null;

        _refreshTokenServiceMock.Setup(x => x.GetByKeyAsync(refreshTokenKey)).ReturnsAsync(oldToken);

         // Act & Assert
        await Assert.ThrowsAsync<CustomBusinessException>(() => _authenticationService.RefreshLoginTokenAsync(refreshTokenKey));
    }

    [Fact]
    public async Task RefreshLoginTokenAsync_ExperiedRefreshTokenKey_ReturnsTokenRecord()
    {
        // Arrange
        var refreshTokenKey = _fixture.Create<string>();
        var oldToken = _fixture.Create<RefreshTokens>();
        oldToken.DueTime = DateTime.Now.AddHours(-2);

        _refreshTokenServiceMock.Setup(x => x.GetByKeyAsync(refreshTokenKey)).ReturnsAsync(oldToken);

         // Act & Assert
        await Assert.ThrowsAsync<CustomStatusException>(() => _authenticationService.RefreshLoginTokenAsync(refreshTokenKey));
    }
}