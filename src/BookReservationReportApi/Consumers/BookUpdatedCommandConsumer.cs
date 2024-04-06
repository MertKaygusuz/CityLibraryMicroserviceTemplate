using BookReservationReportApi.Repositories.ActiveBookReservations;
using BookReservationReportApi.Repositories.BookReservationHistories;
using CityLibrary.Shared.SharedModels.QueueModels;
using MassTransit;

namespace BookReservationReportApi.Consumers
{
    public class BookUpdatedCommandConsumer(IActiveBookReservationsRepo activeBookReservationsRepo, IBookReservationHistoriesRepo bookReservationHistoriesRepo) : IConsumer<BookUpdated>
    {
        private readonly IActiveBookReservationsRepo _activeBookReservationsRepo = activeBookReservationsRepo;
        private readonly IBookReservationHistoriesRepo _bookReservationHistoriesRepo = bookReservationHistoriesRepo;
        public async Task Consume(ConsumeContext<BookUpdated> context)
        {
            var updateTaskForActiveReservations = _activeBookReservationsRepo.UpdateBookPartAsync(context.Message);
            var updateTaskForHistoryReservations = _bookReservationHistoriesRepo.UpdateBookPartAsync(context.Message);

            await Task.WhenAll(updateTaskForActiveReservations, updateTaskForHistoryReservations);
        }
    }
}