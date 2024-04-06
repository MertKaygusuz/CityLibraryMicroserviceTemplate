using BookReservationReportApi.ContextRelated;
using BookReservationReportApi.Repositories.Base;
using CityLibrary.Shared.Extensions;
using CityLibrary.Shared.SharedModels;
using MongoDB.Driver;

namespace BookReservationReportApi.Repositories.BookReservationHistories
{
    public class BookReservationHistoriesRepo(AppDbContext context, IMongoClient client) : BaseRepo<Entities.BookReservationHistories>(context, client), IBookReservationHistoriesRepo
    {
        public async Task UpdateBookPartAsync(BookModel model)
        {
            var historyReservationFilter = Builders<Entities.BookReservationHistories>.Filter.Where(x => x.IsDeleted != true && x.BookId == model.BookId);
            await UpdateManyAsync(historyReservationFilter, Builders<Entities.BookReservationHistories>.Update.Set(x => x.Book, model).OnUpdate());
        }

        public async Task UpdateUserPartAsync(UserModel model)
        {
            var historyReservationFilter = Builders<Entities.BookReservationHistories>.Filter.Where(x => x.IsDeleted != true && x.UserId == model.UserId);
            await UpdateManyAsync(historyReservationFilter, Builders<Entities.BookReservationHistories>.Update.Set(x => x.User, model).OnUpdate());
        }
    }
}