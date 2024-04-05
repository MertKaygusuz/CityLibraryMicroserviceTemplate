using BookReservationReportApi.Dtos.ReservationReport;

namespace BookReservationReportApi.Services.ReservationReport.Interfaces
{
    public interface IReservationReportService
    {
        Task<IEnumerable<NumberOfBooksReservedByUsersResponseDto>> GetNumberOfBooksReservedPerUsersAsync();

        Task<IEnumerable<ReservationHistoryUserResponseDto>> GetReservationHistoryPerUserAsync();

        Task<IEnumerable<ReservationHistoryBookResponseDto>> GetReservationHistoryPerBookAsync();

        Task<IEnumerable<ActiveBookReservationsResponseDto>> GetAllActiveBookReservationsAsync(ActiveBookReservationsFilterDto filter);

        Task<IEnumerable<ReservationHistoryUserResponseDto>> GetReservationHistoryByUserAsync(ReservationHistoryUserDto dto);

        Task<IEnumerable<ReservationHistoryBookResponseDto>> GetReservationHistoryByBookAsync(ReservationHistoryBookDto dto);
    }
}