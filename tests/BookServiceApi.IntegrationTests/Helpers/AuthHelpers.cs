using CityLibrary.Shared.Statics.Methods;
using Microsoft.Extensions.Options;
using BookServiceApi.AppSettings;

namespace BookServiceApi.IntegrationTests.Helpers
{
    public class AuthHelpers
    {
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