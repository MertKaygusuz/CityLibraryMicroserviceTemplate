using BookServiceApi.Entities;
using BookServiceApi.Services.BookReservationApiService.Grpc;

namespace BookServiceApi.IntegrationTests.Mocks;

public class BookReservationRecordGrpcMock : IBookReservationRecordApiGrpc
{
    public Task CallReturnBookAsync(User UserRecord, Book BookRecord)
    {
        return Task.CompletedTask;
    }
}
