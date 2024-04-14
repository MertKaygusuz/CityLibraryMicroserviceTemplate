using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserServiceApi.Entities;
using CityLibrary.Shared.DbBase.SQL;

namespace UserServiceApi.Repositories
{
    public interface IRolesRepo : IBaseRepo<Role, int>
    {
        void SetUserRolesWithLinqExp(User user, Expression<Func<Role, bool>> whereClause);
    }
}
