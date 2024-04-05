namespace BookServiceApi.Services.User.Interfaces;

public interface IUserService
{
    Task<bool> CheckIfUserExistsAsync(string userId);   
}