using CityLibrary.Shared.DbBase.Mongo;
using CityLibrary.Shared.SharedModels;

namespace BookReservationReportApi.Repositories.BookReservationHistories
{
    public interface IBookReservationHistoriesRepo : IBaseRepo<Entities.BookReservationHistory>
    {
        Task UpdateUserPartAsync(UserModel model);

        Task UpdateBookPartAsync(BookModel model);
    }
}