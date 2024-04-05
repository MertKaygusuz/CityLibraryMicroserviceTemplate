using CityLibrary.Shared.BaseCheckService;
using UserServiceApi.Entities.Cache;

namespace UserServiceApi.Services.Token.Interfaces
{
    public interface IRefreshTokenService : IBaseCheckService
    {
        Task CreateOrUpdateAsync(RefreshTokens token, bool autoExpiration = true);

        Task DeleteAsync(string key);

        Task<RefreshTokens> GetByKeyAsync(string key);
    }
}
