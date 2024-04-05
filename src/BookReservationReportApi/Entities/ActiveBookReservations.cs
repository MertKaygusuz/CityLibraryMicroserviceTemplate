using System.ComponentModel.DataAnnotations.Schema;
using CityLibrary.Shared.DbBase.Mongo;
using CityLibrary.Shared.SharedModels;
using MongoDB.Bson.Serialization.Attributes;

namespace BookReservationReportApi.Entities
{
    [Table("ActiveBookReservations")]
    public class ActiveBookReservations : TableBase
    {
        [BsonRequired]
        public DateTime DeliveryDateToUser { get; set; }

        //Default return period 7 days
        // [BsonIgnore]
        // public DateTime AvailableAt => DeliveryDateToUser.AddDays(7);

        [BsonRequired]
        public string UserId { get; set; }

        [BsonRequired]
        public UserModel User { get; set; }

        [BsonRequired]
        public int BookId { get; set; }

        [BsonRequired]
        public BookModel Book { get; set; }
    }
}