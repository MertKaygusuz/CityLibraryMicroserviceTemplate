using BookReservationReportApi.Repositories.ActiveBookReservations;
using BookReservationReportApi.Repositories.BookReservationHistories;
using CityLibrary.Shared.SharedModels.QueueModels;
using MassTransit;

namespace BookReservationReportApi.Consumers
{
    public class UserUpdatedEventConsumer(IActiveBookReservationsRepo activeBookReservationsRepo, IBookReservationHistoriesRepo bookReservationHistoriesRepo) : IConsumer<UserUpdated>
    {
        private readonly IActiveBookReservationsRepo _activeBookReservationsRepo = activeBookReservationsRepo;
        private readonly IBookReservationHistoriesRepo _bookReservationHistoriesRepo = bookReservationHistoriesRepo;
        public async Task Consume(ConsumeContext<UserUpdated> context)
        {
            var updateTaskForActiveReservations = _activeBookReservationsRepo.UpdateUserPartAsync(context.Message);
            var updateTaskForHistoryReservations = _bookReservationHistoriesRepo.UpdateUserPartAsync(context.Message);

            await Task.WhenAll(updateTaskForActiveReservations, updateTaskForHistoryReservations);
        }
    }
}