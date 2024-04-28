using System.Security.Claims;
using CityLibrary.Shared.Statics.Methods;
using Microsoft.Extensions.Options;
using UserServiceApi.AppSettings;
using UserServiceApi.Dtos.Token;

namespace UserServiceApi.IntegrationTests.Helpers
{
    public class AuthHelpers
    {
        // public static Dictionary<string, object> GetBearerForUser(bool isAdmin = true, string userId = "d964dfdf-7cdc-4a7a-a951-04b540bac28d", string userName = "Admin", string fullName = "fakeFullName")
        // {
        //     return new Dictionary<string, object> 
        //     { 
        //         { ClaimTypes.Sid, userId },
        //         { ClaimTypes.NameIdentifier, userName },
        //         { ClaimTypes.Name, fullName },
        //         { ClaimTypes.Role, isAdmin ? ["Admin", "User"] : new List<string>() {"User"} }
        //     };
        // }

        public static string GetAdminToken(IOptions<AppSetting> options) 
        {
            var tokenOptions = options.Value.TokenOptions;
            return TokenRelated.GetAdminTokenForIntegrationTests(tokenOptions.SecurityKey, tokenOptions.Audience, tokenOptions.Issuer);
        }

        public static string GetUserToken(IOptions<AppSetting> options) 
        {
            var tokenOptions = options.Value.TokenOptions;
            return TokenRelated.GetDefaultUserTokenForIntegrationTests(tokenOptions.SecurityKey, tokenOptions.Audience, tokenOptions.Issuer);
        }
    }
}