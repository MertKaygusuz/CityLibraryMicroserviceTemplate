using BookReservationReportApi.ContextRelated;
using BookReservationReportApi.Repositories.Base;
using MongoDB.Driver;

namespace BookReservationReportApi.Repositories.ActiveBookReservations
{
    public class ActiveBookReservationsRepo(AppDbContext context, IMongoClient client) : BaseRepo<Entities.ActiveBookReservations>(context, client), IActiveBookReservationsRepo
    {
    }
}