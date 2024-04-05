using UserServiceApi.Dtos.Authentication;
using UserServiceApi.Dtos.Token.Records;

namespace UserServiceApi.Services.Token.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ReturnTokenRecord> LoginAsync(LoginDto loginDto);

        Task<ReturnTokenRecord> RefreshLoginTokenAsync(string refreshTokenKey);

        Task LogoutAsync(string tokenKey);
    }
}
