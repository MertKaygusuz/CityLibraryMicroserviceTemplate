using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserServiceApi.ContextRelated;
using UserServiceApi.Entities;
using UserServiceApi.Repositories.Base;

namespace UserServiceApi.Repositories
{
    public class RolesRepo(AppDbContext dbContext) : BaseRepo<Role, int>(dbContext), IRolesRepo
    {
        public void SetUserRolesWithLinqExp(User user, Expression<Func<Role, bool>> whereClause)
        {
            var roles = GetDataWithLinqExp(whereClause);
            _dbcontext.Roles.AttachRange(roles);
            var localRoles = _dbcontext.Roles.Local;
            user.Roles = [.. localRoles];
        }
    }
}
