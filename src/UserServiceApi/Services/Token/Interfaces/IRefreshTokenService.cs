using CityLibrary.Shared.BaseCheckService;
using UserServiceApi.Entities.Cache;

namespace UserServiceApi.Services.Token.Interfaces
{
    public interface IRefreshTokenService : IBaseCheckService
    {
        Task CreateOrUpdateAsync(RefreshToken token, bool autoExpiration = true);

        Task DeleteAsync(string key);

        Task<RefreshToken> GetByKeyAsync(string key);
    }
}
