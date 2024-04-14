using BookServiceApi.Entities;
using CityLibrary.Shared.DbBase.SQL;

namespace BookServiceApi.Repositories.User;

public interface IUsersRepo : IBaseRepo<Entities.User, string>
{
    
}