using UserServiceApi.ContextRelated;
using UserServiceApi.Entities;
using UserServiceApi.Repositories.Base;

namespace UserServiceApi.Repositories
{
    public class UserRolesRepo(AppDbContext dbContext) : BaseRepo<UserRoles, int>(dbContext), IUserRolesRepo
    {
    }
}
