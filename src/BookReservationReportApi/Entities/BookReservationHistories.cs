using System.ComponentModel.DataAnnotations.Schema;
using CityLibrary.Shared.DbBase.Mongo;
using CityLibrary.Shared.SharedModels;
using MongoDB.Bson.Serialization.Attributes;

namespace BookReservationReportApi.Entities
{
    [Table("BookReservationHistories")]
    public class BookReservationHistories : TableBase
    {
        [BsonRequired]
        public DateTime DeliveryDateToUser { get; set; }

        [BsonRequired]
        public DateTime RecievedDate { get; set; }

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