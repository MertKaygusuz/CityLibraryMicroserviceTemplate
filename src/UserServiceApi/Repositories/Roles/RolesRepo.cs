using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserServiceApi.ContextRelated;
using UserServiceApi.Entities;
using UserServiceApi.Repositories.Base;

namespace UserServiceApi.Repositories
{
    public class RolesRepo(AppDbContext dbContext) : BaseRepo<Roles, int>(dbContext), IRolesRepo
    {
        public void SetUserRolesWithLinqExp(Users user, Expression<Func<Roles, bool>> whereClause)
        {
            var roles = GetDataWithLinqExp(whereClause);
            _dbcontext.Roles.AttachRange(roles);
            var localRoles = _dbcontext.Roles.Local;
            user.Roles = [.. localRoles];
        }
    }
}
