namespace CityLibrary.Shared.SharedModels
{
    public class ActiveBookReservationModel
    {
        public DateTime DeliveryDateToUser { get; set; }

        public string UserId { get; set; }

        public UserModel User { get; set; }

        public int BookId { get; set; }

        public BookModel Book { get; set; }
    }
}