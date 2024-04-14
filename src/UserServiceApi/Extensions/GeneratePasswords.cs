using BCrypt.Net;
using CityLibrary.Shared.ExceptionHandling;
using Microsoft.Extensions.Localization;
using UserServiceApi.Resources;

namespace UserServiceApi.Extensions
{
    public static class GeneratePasswords
    {
        private const byte MinimumPasswordLength = 8;

        public static IStringLocalizer<ExceptionsResource> Localizer;
        public static void CreatePasswordHash(this string password, out string passwordHash)
        {
            if (password == null || string.IsNullOrWhiteSpace(password))
                throw new CustomBusinessException(Localizer["Password_Null"]);
            if (password.Contains(' '))
                throw new CustomBusinessException(Localizer["Password_Space"]);
            if (password.Length < MinimumPasswordLength)
                throw new CustomBusinessException(string.Format(Localizer["Password_Min_Length"], MinimumPasswordLength));

            passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, hashType: HashType.SHA384);
        }
    }
}
