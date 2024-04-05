﻿using BCrypt.Net;
using CityLibrary.Shared.ExceptionHandling;
using Microsoft.Extensions.Localization;
using UserServiceApi.Resources;

namespace UserServiceApi.Extensions
{
    public static class GenerateAndVerifyPasswords
    {
        private const byte MinimumPasswordLength = 8;

        public static IStringLocalizer<ExceptionsResource> Localizer;
        public static void CreatePasswordHash(this string password, out string passwordHash)
        {
            if (password == null)
                throw new CustomBusinessException(Localizer["Password_Null"]);
            if (string.IsNullOrWhiteSpace(password))
                throw new CustomBusinessException(Localizer["Password_Space"]);
            if (password.Length < MinimumPasswordLength)
                throw new CustomBusinessException( string.Format(Localizer["Password_Min_Length"], MinimumPasswordLength));

            passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, hashType: HashType.SHA384);
        }

        public static bool VerifyPasswordHash(this string password, string passwordHash)
        {
            if (password == null)
                throw new CustomBusinessException(Localizer["Password_Null"]);
            if (string.IsNullOrWhiteSpace(password))
                throw new CustomBusinessException(Localizer["Password_Space"]);

            return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash, hashType: HashType.SHA384);
        }
    }
}
