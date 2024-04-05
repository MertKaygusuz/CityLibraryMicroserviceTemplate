using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using UserServiceApi.AppSettings;
using UserServiceApi.Dtos.Token;
using UserServiceApi.Services.Token.Interfaces;
using static CityLibrary.Shared.Statics.Methods.TokenRelated;
using static CityLibrary.Shared.Statics.Methods.RandomGenerators;

namespace UserServiceApi.Services.Token.Classes
{
    public class AccessTokenService(IOptions<AppSetting> options, IHttpContextAccessor httpContextAccessor) : IAccessTokenService
    {
        private readonly TokenOptions _tokenOptions = options.Value.TokenOptions;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public CreateTokenResultDto CreateToken(CreateTokenDto dto)
        {
            var accessTokenExpiration = DateTime.Now.AddHours(_tokenOptions.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddHours(_tokenOptions.RefreshTokenExpiration);
            var securityKey = GetSymmetricSecurityKey(_tokenOptions.SecurityKey);

            JwtSecurityToken jwtSecurityToken = new(
                issuer: _tokenOptions.Issuer,
                expires: accessTokenExpiration,
                 notBefore: DateTime.Now,
                 claims: GetClaims(dto),
                 signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature));

            return new CreateTokenResultDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshTokenKey = CreateRefreshTokenKey(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration,
                ClientIp = Convert.ToString(_httpContextAccessor.HttpContext!.Connection.RemoteIpAddress),
                ClientAgent = _httpContextAccessor.HttpContext.Request.Headers.UserAgent,
                UserName = dto.UserName
            };
        }

        #region private methods
        private static string CreateRefreshTokenKey()
        {
            return RandomStringFromBytes();
        }

        private IEnumerable<Claim> GetClaims(CreateTokenDto dto)
        {
            var userClaims = new List<Claim>
            {
                new(ClaimTypes.Sid, dto.UserId),
                new(ClaimTypes.NameIdentifier, dto.UserName),
                new(ClaimTypes.Name,dto.FullName),
                new(JwtRegisteredClaimNames.Aud, _tokenOptions.Audience)
            };

            userClaims.AddRange(dto.UserRoleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

            return userClaims;
        }
        #endregion
    }
}
