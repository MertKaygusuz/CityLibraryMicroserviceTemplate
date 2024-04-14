using CityLibrary.Shared.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using UserServiceApi.AppSettings;
using UserServiceApi.Entities.Cache;
using UserServiceApi.Services.Token.Interfaces;


namespace UserServiceApi.Services.Token.Classes
{
    public class RefreshTokenService(IOptions<AppSetting> options, IDistributedCache cache) : IRefreshTokenService
    {
        private readonly TokenOptions _tokenOptions = options.Value.TokenOptions;
        private readonly IDistributedCache _cache = cache;

        public async Task DeleteAsync(string key)
        {
            await _cache.RemoveRecordAsync(key);
        }

        public async Task<bool> DoesEntityExistAsync(IConvertible id)
        {
            return (await GetByKeyAsync(id as string)) is not null;
        }

        public async Task<RefreshToken> GetByKeyAsync(string key)
        {
            return await _cache.GetRecordAsync<RefreshToken>(key);
        }

        public async Task CreateOrUpdateAsync(RefreshToken token, bool autoExpiration = true)
        {
            if (autoExpiration)
                token.DueTime = DateTime.Now.AddHours(_tokenOptions.RefreshTokenExpiration);
            await _cache.SaveRecordAsync(token.TokenKey, token, TimeSpan.FromHours(_tokenOptions.RefreshTokenExpiration));
        }
    }
}
