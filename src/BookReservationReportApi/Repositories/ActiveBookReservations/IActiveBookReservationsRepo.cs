using CityLibrary.Shared.DbBase.Mongo;
using CityLibrary.Shared.SharedModels;

namespace BookReservationReportApi.Repositories.ActiveBookReservations
{
    public interface IActiveBookReservationsRepo : IBaseRepo<Entities.ActiveBookReservations>
    {
        Task UpdateUserPartAsync(UserModel model);

        Task UpdateBookPartAsync(BookModel model);
    }
}