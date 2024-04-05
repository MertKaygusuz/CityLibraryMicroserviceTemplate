using BookReservationReportApi.ContextRelated;
using BookReservationReportApi.Repositories.Base;
using MongoDB.Driver;

namespace BookReservationReportApi.Repositories.BookReservationHistories
{
    public class BookReservationHistoriesRepo(AppDbContext context, IMongoClient client) : BaseRepo<Entities.BookReservationHistories>(context, client), IBookReservationHistoriesRepo
    {
        
    }
}