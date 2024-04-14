using CityLibrary.Shared.ExceptionHandling;
using Microsoft.Extensions.Localization;
using UserServiceApi.Dtos.Authentication;
using UserServiceApi.Dtos.Token;
using UserServiceApi.Dtos.Token.Records;
using UserServiceApi.Entities.Cache;
using UserServiceApi.Extensions;
using UserServiceApi.Helpers;
using UserServiceApi.Resources;
using UserServiceApi.Services.Token.Interfaces;
using UserServiceApi.Services.User.Interfaces;

namespace UserServiceApi.Services.Token.Classes
{
    public class AuthenticationService(
        ILogger<AuthenticationService> logger,
        IStringLocalizer<ExceptionsResource> localizer,
        IUserService userService,
        IRefreshTokenService refreshTokenService,
        IAccessTokenService accessTokenService,
        IVerifyPasswords verifyPasswords) : IAuthenticationService
    {
        private readonly ILogger _logger = logger;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
        private readonly IAccessTokenService _accessTokenService = accessTokenService;
        private readonly IUserService _userService = userService;
        private readonly IStringLocalizer<ExceptionsResource> _localizer = localizer;
        private readonly IVerifyPasswords _verifyPasswords = verifyPasswords;

        public async Task<ReturnTokenRecord> LoginAsync(LoginDto loginDto)
        {
            var user = await _userService.GetUserByUserNameAsync(loginDto.UserName);
            if (user is null || _verifyPasswords.VerifyPasswordHash(loginDto.Password, user.Password, _localizer) is false)
                throw new CustomBusinessException(_localizer["Login_Failed"]);

            CreateTokenDto createTokenDto = new()
            {
                FullName = user.FullName,
                UserName = user.UserName,
                UserId = user.UserId,
                UserRoleNames = user.Roles.Select(x => x.RoleName)
            };

            var token = _accessTokenService.CreateToken(createTokenDto);

            var newRefreshToken = new RefreshTokens()
            {
                ClientAgent = token.ClientAgent,
                ClientIp = token.ClientIp,
                TokenKey = token.RefreshTokenKey,
                FullName = user.FullName,
                UserName = user.UserName,
                UserId = user.UserId,
                UserRoleNames = createTokenDto.UserRoleNames
            };

            await _refreshTokenService.CreateOrUpdateAsync(newRefreshToken);

            _logger.LogInformation($"Refresh login was executed successfully. UserName: {user.UserName}, UserId: {user.UserId}," +
                                    $" IP: {token.ClientIp}, Agent: {token.ClientAgent}");

            return new ReturnTokenRecord(token.AccessToken, token.RefreshTokenKey);
        }

        public async Task LogoutAsync(string tokenKey)
        {
            await _refreshTokenService.DeleteAsync(tokenKey);
        }

        public async Task<ReturnTokenRecord> RefreshLoginTokenAsync(string refreshTokenKey)
        {
            RefreshTokens oldToken = await _refreshTokenService.GetByKeyAsync(refreshTokenKey);

            if (oldToken is null)
                throw new CustomBusinessException("Refresh token could not be found!");

            if (DateTime.Compare(DateTime.Now, oldToken.DueTime) > 0)
                throw new CustomStatusException(_localizer["Session_Timeout"], 401);
            
            CreateTokenDto createTokenDto = new()
            {
                UserId = oldToken.UserId,
                UserName = oldToken.UserName,
                FullName = oldToken.FullName,
                UserRoleNames = oldToken.UserRoleNames
            };
            CreateTokenResultDto newToken = _accessTokenService.CreateToken(createTokenDto);

            var newRefreshToken = new RefreshTokens()
            {
                ClientAgent = newToken.ClientAgent,
                ClientIp = newToken.ClientIp,
                TokenKey = newToken.RefreshTokenKey,
                FullName = oldToken.FullName, // get some old values from cache
                UserName = oldToken.UserName,
                UserId = oldToken.UserId,
                UserRoleNames = oldToken.UserRoleNames
            };

            await _refreshTokenService.CreateOrUpdateAsync(newRefreshToken);
            await _refreshTokenService.DeleteAsync(refreshTokenKey);

            _logger.LogInformation($"Refresh login was executed successfully. UserName: {oldToken.UserId}, UserName: {oldToken.UserId}," +
                                    $" IP: {newToken.ClientIp}, Agent: {newToken.ClientAgent}");

            return new ReturnTokenRecord(newToken.AccessToken, newToken.RefreshTokenKey);
        }
    }
}
