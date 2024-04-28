using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CityLibrary.Shared.Statics.Methods
{
    public static class TokenRelated
    {
        public static SecurityKey GetSymmetricSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }

        public static string GetDefaultUserTokenForIntegrationTests(string securityKey, string audience, string issuer) 
        {
            JwtSecurityToken jwtSecurityToken = new(
                issuer: issuer,
                expires: DateTime.Now.AddHours(100),
                notBefore: DateTime.Now,
                claims: GetClaimsDefault(audience),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature));

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        public static string GetAdminTokenForIntegrationTests(string securityKey, string audience, string issuer) 
        {
            JwtSecurityToken jwtSecurityToken = new(
                issuer: issuer,
                expires: DateTime.Now.AddHours(100),
                notBefore: DateTime.Now,
                claims: GetClaimsAdmin(audience),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature));

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        #region private methods
        private static IEnumerable<Claim> GetClaimsDefault(string audience)
        {
            var userRoleNames = new List<string>() {"User"};
            var userClaims = new List<Claim>
            {
                new(ClaimTypes.Sid, "75a4749d-1090-4ade-894e-2612adcd0c1c"),
                new(ClaimTypes.NameIdentifier, "User1"),
                new(ClaimTypes.Name, "User1"),
                new(JwtRegisteredClaimNames.Aud, audience)
            };

            userClaims.AddRange(userRoleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

            return userClaims;
        }

        private static IEnumerable<Claim> GetClaimsAdmin(string audience)
        {
            var userRoleNames = new List<string>() {"Admin", "User"};

            var userClaims = new List<Claim>
            {
                new(ClaimTypes.Sid, "d964dfdf-7cdc-4a7a-a951-04b540bac28d"),
                new(ClaimTypes.NameIdentifier, "Admin"),
                new(ClaimTypes.Name, "Admin"),
                new(JwtRegisteredClaimNames.Aud, audience)
            };

            userClaims.AddRange(userRoleNames.Select(roleName => new Claim(ClaimTypes.Role, roleName)));

            return userClaims;
        }
        #endregion
    }
}
