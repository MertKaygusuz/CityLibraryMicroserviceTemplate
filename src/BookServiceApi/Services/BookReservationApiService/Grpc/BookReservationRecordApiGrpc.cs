using BookReservationReturn;
using BookServiceApi.Interceptors;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using BookServiceApi.AppSettings;

namespace BookServiceApi.Services.BookReservationApiService.Grpc
{
    public class BookReservationRecordApiGrpc(IOptions<AppSetting> options, IHttpContextAccessor httpContextAccessor) : IBookReservationRecordApiGrpc
    {
        // private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        // private readonly IOptions<AppSetting> _options = options;

        public async Task CallReturnBookAsync(Entities.User UserRecord, Entities.Book BookRecord)
        {
            using var channel = GrpcChannel.ForAddress(options.Value.BookReservationGrpcEndPoint);
            var invoker = channel.Intercept(new GrpcHeadersInterceptor(httpContextAccessor));
            var client = new GrpcBookReservation.GrpcBookReservationClient(invoker);
            var request = new GrpcActiveBookReservationModel
            {
                UserId = UserRecord.UserId,
                User = new GrpcUserModel
                {
                    UserId = UserRecord.UserId,
                    UserName = UserRecord.UserName,
                    FullName = UserRecord.FullName,
                    BirthDate = Timestamp.FromDateTime(UserRecord.BirthDate.ToUniversalTime()),
                    Address = UserRecord.Address
                },
                BookId = BookRecord.BookId,
                Book = new GrpcBookModel 
                {
                    BookId = BookRecord.BookId,
                    Author = BookRecord.Author,
                    BookTitle = BookRecord.BookTitle,
                    FirstPublishDate = Timestamp.FromDateTime(BookRecord.FirstPublishDate.ToUniversalTime()),
                    EditionNumber = BookRecord.EditionNumber,
                    EditionDate = Timestamp.FromDateTime(BookRecord.EditionDate.ToUniversalTime()),
                    TitleType = (int) BookRecord.TitleType,
                    CoverType = (int) BookRecord.CoverType
                }
            };

            // grpc call here
            await client.ReturnBookAsync(request);
        }
    }
}