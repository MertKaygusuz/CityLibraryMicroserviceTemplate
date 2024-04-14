using BookReservationReportApi.Entities;
using BookReservationReportApi.Repositories.ActiveBookReservations;
using BookReservationReportApi.Repositories.BookReservationHistories;
using BookReservationReportApi.Resources;
using BookReservationReturn;
using CityLibrary.Shared.SharedEnums;
using CityLibrary.Shared.SharedModels;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using MongoDB.Driver.Linq;

namespace BookReservationReportApi.GrpcServices.ReservationReport
{
    [Authorize(Roles = "Admin")]
    public class GrpcReservationRecordService(IActiveBookReservationsRepo activeBookReservationsRepo, 
                                              IBookReservationHistoriesRepo bookReservationHistoriesRepo,
                                              IStringLocalizer<ExceptionsResource> localizer) : GrpcBookReservation.GrpcBookReservationBase
    {
        private readonly IActiveBookReservationsRepo _activeBookReservationsRepo = activeBookReservationsRepo;
        private readonly IBookReservationHistoriesRepo _bookReservationHistoriesRepo = bookReservationHistoriesRepo;
        private readonly IStringLocalizer<ExceptionsResource> _localizer = localizer;
        public override async Task<Empty> ReturnBook(GrpcActiveBookReservationModel request, ServerCallContext context)
        {
            var activeReservation = await _activeBookReservationsRepo.GetData(x => x.BookId == request.BookId && x.UserId == request.UserId)
                                                                        .OrderBy(x => x.DeliveryDateToUser)
                                                                        .FirstOrDefaultAsync();
            if (activeReservation is null)
                throw new RpcException(new Status(StatusCode.NotFound, _localizer["Active_Book_Reservation_Not_Found"]));

            var historyRecord = new BookReservationHistory
            {
                BookId = request.BookId,
                UserId = request.UserId,
                DeliveryDateToUser = activeReservation.DeliveryDateToUser,
                RecievedDate = DateTime.UtcNow,
                User = new UserModel()
                {
                    UserId = request.UserId,
                    UserName = request.User.UserName,
                    FullName = request.User.FullName,
                    BirthDate = request.User.BirthDate.ToDateTime(),
                    Address = request.User.Address
                },
                Book = new BookModel()
                {
                    BookId = request.BookId,
                    Author = request.Book.Author,
                    BookTitle = request.Book.BookTitle,
                    FirstPublishDate = request.Book.FirstPublishDate.ToDateTime(),
                    EditionNumber = (short) request.Book.EditionNumber,
                    EditionDate = request.Book.EditionDate.ToDateTime(),
                    TitleType = (BookTitleTypes) request.Book.TitleType,
                    CoverType = (BookCoverTypes) request.Book.CoverType
                }
            };
            await _bookReservationHistoriesRepo.AddAsync(historyRecord);
            await _activeBookReservationsRepo.DeleteSoftlyAsync(activeReservation);  

            return new Empty();
            // return Task.FromResult(new Empty());
        }
    }
}