using UserServiceApi.ContextRelated;
using UserServiceApi.Entities;
using UserServiceApi.Repositories.Base;

namespace UserServiceApi.Repositories
{
    public class UsersRepo(AppDbContext dbContext) : BaseRepo<User, string>(dbContext), IUsersRepo
    {
    }
}
