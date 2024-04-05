using CityLibrary.Shared.SharedModels;

namespace BookReservationReportApi.Services.ReservationReport.Interfaces
{
    public interface IReservationRecordService
    {
        Task ReservationCreateAsync(ActiveBookReservationModel model);

        Task BookReservationReturnAsync(ActiveBookReservationModel model);
    }
}