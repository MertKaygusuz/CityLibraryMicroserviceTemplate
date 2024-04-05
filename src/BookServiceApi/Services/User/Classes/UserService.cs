using BookServiceApi.Repositories.User;
using BookServiceApi.Services.User.Interfaces;

namespace BookServiceApi.Services.User.Classes;

public class UserService(IUsersRepo usersRepo) : IUserService
{
    private readonly IUsersRepo _usersRepo = usersRepo;

    public async Task<bool> CheckIfUserExistsAsync(string userId)
    {
        return await _usersRepo.DoesEntityExistAsync(userId);
    }
}