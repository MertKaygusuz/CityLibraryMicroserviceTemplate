using UserServiceApi.Entities;
using CityLibrary.Shared.DbBase.SQL;

namespace UserServiceApi.Repositories
{
    public interface IUsersRepo : IBaseRepo<Users, string>
    {
    }
}
