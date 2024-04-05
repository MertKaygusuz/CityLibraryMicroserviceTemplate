namespace CityLibrary.Shared.BaseCheckService;

public interface IBaseCheckService
{
    Task<bool> DoesEntityExistAsync(IConvertible id);
}