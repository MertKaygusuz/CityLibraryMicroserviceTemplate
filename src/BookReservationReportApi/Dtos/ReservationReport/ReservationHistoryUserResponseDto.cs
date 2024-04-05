namespace BookReservationReportApi.Dtos.ReservationReport
{
    public class ReservationHistoryUserResponseDto
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string BookTitle { get; set; }

        public DateTime FirstPublishDate { get; set; }

        public short EditionNumber { get; set; }

        public DateTime EditionDate { get; set; }

        public DateTime DeliveryDateToUser { get; set; }

        public DateTime RecievedDate { get; set; }
    }
}