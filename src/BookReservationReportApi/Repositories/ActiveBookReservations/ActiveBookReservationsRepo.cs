using BookReservationReportApi.ContextRelated;
using BookReservationReportApi.Repositories.Base;
using CityLibrary.Shared.Extensions;
using CityLibrary.Shared.SharedModels;
using MongoDB.Driver;

namespace BookReservationReportApi.Repositories.ActiveBookReservations
{
    public class ActiveBookReservationsRepo(AppDbContext context, IMongoClient client) : BaseRepo<Entities.ActiveBookReservations>(context, client), IActiveBookReservationsRepo
    {
        public async Task UpdateBookPartAsync(BookModel model)
        {
            var activeReservationFilter = Builders<Entities.ActiveBookReservations>.Filter.Where(x => x.IsDeleted != true && x.BookId == model.BookId);
            await UpdateManyAsync(activeReservationFilter, Builders<Entities.ActiveBookReservations>.Update.Set(x => x.Book, model).OnUpdate());
        }

        public async Task UpdateUserPartAsync(UserModel model)
        {
            var activeReservationFilter = Builders<Entities.ActiveBookReservations>.Filter.Where(x => x.IsDeleted != true && x.UserId == model.UserId);
            await UpdateManyAsync(activeReservationFilter, Builders<Entities.ActiveBookReservations>.Update.Set(x => x.User, model).OnUpdate());
        }
    }
}