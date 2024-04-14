using BCrypt.Net;
using CityLibrary.Shared.ExceptionHandling;
using Microsoft.Extensions.Localization;
using UserServiceApi.Resources;

namespace UserServiceApi.Helpers
{
    public class VerifyPasswords : IVerifyPasswords
    {
        public bool VerifyPasswordHash(string password, string passwordHash, IStringLocalizer<ExceptionsResource> localizer)
        {
            if (password == null || string.IsNullOrWhiteSpace(password))
                throw new CustomBusinessException(localizer["Password_Null"]);
            if (password.Contains(' '))
                throw new CustomBusinessException(localizer["Password_Space"]);

            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash, hashType: HashType.SHA384);
        }
    }
}