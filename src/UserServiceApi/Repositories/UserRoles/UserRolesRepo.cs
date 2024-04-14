using UserServiceApi.ContextRelated;
using UserServiceApi.Entities;
using UserServiceApi.Repositories.Base;

namespace UserServiceApi.Repositories
{
    public class UserRolesRepo(AppDbContext dbContext) : BaseRepo<UserRole, int>(dbContext), IUserRolesRepo
    {
    }
}
