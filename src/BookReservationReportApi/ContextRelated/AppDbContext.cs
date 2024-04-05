using MongoDB.Driver;

namespace BookReservationReportApi.ContextRelated
{
    public class AppDbContext(IMongoClient client, string dbName)
    {
        public IMongoDatabase Database { get; set; } = client.GetDatabase(dbName);
    }
}