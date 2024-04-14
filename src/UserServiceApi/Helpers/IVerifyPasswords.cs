using Microsoft.Extensions.Localization;
using UserServiceApi.Resources;

namespace UserServiceApi.Helpers
{
    public interface IVerifyPasswords
    {
        bool VerifyPasswordHash(string password, string passwordHash, IStringLocalizer<ExceptionsResource> localizer);
    }
}