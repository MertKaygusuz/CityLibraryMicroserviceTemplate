namespace BookServiceApi.Services.BookReservationApiService.Grpc
{
    public interface IBookReservationRecordApiGrpc
    {
        Task CallReturnBookAsync(Entities.User UserRecord, Entities.Book BookRecord);
    }
}