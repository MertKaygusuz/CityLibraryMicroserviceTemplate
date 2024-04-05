using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserServiceApi.ContextRelated;
using UserServiceApi.Entities;
using UserServiceApi.Repositories.Base;

namespace UserServiceApi.Repositories
{
    public class RolesRepo(AppDbContext dbContext) : BaseRepo<Roles, int>(dbContext), IRolesRepo
    {
        public LocalView<Roles> GetLocalViewWithLinqExp(Expression<Func<Roles, bool>> whereClause)
        {
            var roles = GetDataWithLinqExp(whereClause);
            _dbcontext.Roles.AttachRange(roles);
            return _dbcontext.Roles.Local;
        }
    }
}
