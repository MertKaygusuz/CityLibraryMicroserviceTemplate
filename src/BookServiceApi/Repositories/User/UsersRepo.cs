using BookServiceApi.ContextRelated;
using BookServiceApi.Entities;
using BookServiceApi.Repositories.Base;

namespace BookServiceApi.Repositories.User;

public class UsersRepo(AppDbContext dbContext) : BaseRepo<Users, string>(dbContext), IUsersRepo
{
}