namespace BookReservationReportApi.Dtos.ReservationReport
{
    public class NumberOfBooksReservedByUsersResponseDto
    {
        public string UserName { get; set; }

        public string UserFullName { get; set; }

        public int ActiveBookReservationsCount { get; set; }
    }
}