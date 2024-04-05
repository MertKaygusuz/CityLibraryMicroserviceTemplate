using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserServiceApi.Entities;
using CityLibrary.Shared.DbBase.SQL;

namespace UserServiceApi.Repositories
{
    public interface IRolesRepo : IBaseRepo<Roles, int>
    {
        LocalView<Roles> GetLocalViewWithLinqExp(Expression<Func<Roles, bool>> whereClause);
    }
}
