using BookServiceApi.Entities;
using CityLibrary.Shared.DbBase.SQL;

namespace BookServiceApi.Repositories.Book
{
    public interface IBooksRepo : IBaseRepo<Entities.Book, int>
    {
    }
}