using BookReservationReportApi.Entities;
using BookReservationReportApi.Repositories.ActiveBookReservations;
using BookReservationReportApi.Repositories.BookReservationHistories;
using BookReservationReportApi.Resources;
using BookReservationReportApi.Services.ReservationReport.Interfaces;
using CityLibrary.Shared.ExceptionHandling;
using CityLibrary.Shared.SharedModels;
using Microsoft.Extensions.Localization;
using MongoDB.Driver.Linq;

namespace BookReservationReportApi.Services.ReservationReport.Classes
{
    public class ReservationRecordService(IActiveBookReservationsRepo activeBookReservationsRepo, IBookReservationHistoriesRepo bookReservationHistoriesRepo, IStringLocalizer<ExceptionsResource> localizer) : IReservationRecordService
    {
        private readonly IActiveBookReservationsRepo _activeBookReservationsRepo = activeBookReservationsRepo;
        private readonly IBookReservationHistoriesRepo _bookReservationHistoriesRepo = bookReservationHistoriesRepo;
        private readonly IStringLocalizer<ExceptionsResource> _localizer = localizer;

        public async Task BookReservationReturnAsync(ActiveBookReservationModel model)
        {
            var activeReservation = await _activeBookReservationsRepo.GetData(x => x.BookId == model.BookId && x.UserId == model.UserId)
                                                                     .OrderBy(x => x.DeliveryDateToUser)
                                                                     .FirstOrDefaultAsync();

            if (activeReservation is null)
                throw new CustomBusinessException(_localizer["Active_Book_Reservation_Not_Found"]);

            var historyRecord = new BookReservationHistory()
            {
                BookId = model.BookId,
                UserId = model.UserId,
                User = model.User,
                Book = model.Book,
                DeliveryDateToUser = activeReservation.DeliveryDateToUser,
                RecievedDate = DateTime.UtcNow
            };
            await _bookReservationHistoriesRepo.AddAsync(historyRecord);
            await _activeBookReservationsRepo.DeleteSoftlyAsync(activeReservation);                     
        }

        public async Task ReservationCreateAsync(ActiveBookReservationModel model)
        {
            var activeBookReservation = new ActiveBookReservation
            {
                DeliveryDateToUser = model.DeliveryDateToUser,
                UserId = model.UserId,
                BookId = model.BookId,
                User = model.User,
                Book = model.Book
            };

            await _activeBookReservationsRepo.AddAsync(activeBookReservation);
        }
    }
}