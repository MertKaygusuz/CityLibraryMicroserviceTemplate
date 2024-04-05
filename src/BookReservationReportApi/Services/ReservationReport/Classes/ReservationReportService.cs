using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookReservationReportApi.Dtos.ReservationReport;
using BookReservationReportApi.Entities;
using BookReservationReportApi.Repositories.ActiveBookReservations;
using BookReservationReportApi.Repositories.BookReservationHistories;
using BookReservationReportApi.Services.ReservationReport.Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BookReservationReportApi.Services.ReservationReport.Classes
{
    public class ReservationReportService(IActiveBookReservationsRepo activeBookReservationsRepo, IBookReservationHistoriesRepo bookReservationHistoriesRepo) : IReservationReportService
    {
        private readonly IActiveBookReservationsRepo _activeBookReservationsRepo = activeBookReservationsRepo;
        private readonly IBookReservationHistoriesRepo _bookReservationHistoriesRepo = bookReservationHistoriesRepo;

        public async Task<IEnumerable<ActiveBookReservationsResponseDto>> GetAllActiveBookReservationsAsync(ActiveBookReservationsFilterDto filter)
        {
            var baseData = _activeBookReservationsRepo.GetData();
            
            if(filter is not null)
            {
                if (!string.IsNullOrEmpty(filter.BookTitle))
                    baseData = baseData.Where(x => x.Book.BookTitle == filter.BookTitle);

                if (!string.IsNullOrEmpty(filter.UserName))
                    baseData = baseData.Where(x => x.User.UserName == filter.UserName);
            }

            var result = await baseData.Select(x => new ActiveBookReservationsResponseDto
            {
                DeliveryDateToUser = x.DeliveryDateToUser,
                // AvailableAt = x.AvailableAt,
                UserName = x.User.UserName,
                UserFullName = x.User.FullName,
                BookTitle = x.Book.BookTitle,
                EditionNumber = x.Book.EditionNumber,
                CoverType = x.Book.CoverType
            }).OrderByDescending(x => x.DeliveryDateToUser)
              .ToListAsync();

            return result;
        }

        public async Task<IEnumerable<NumberOfBooksReservedByUsersResponseDto>> GetNumberOfBooksReservedPerUsersAsync()
        {
            var result = await _activeBookReservationsRepo.GetData()
                                                        .Select(x => new
                                                        {
                                                            x.User.FullName,
                                                            x.User.UserName
                                                        })
                                                        .GroupBy(x => new { x.UserName, x.FullName })
                                                        .Select(x => new NumberOfBooksReservedByUsersResponseDto
                                                        {
                                                            UserName = x.Key.UserName,
                                                            UserFullName = x.Key.FullName,
                                                            ActiveBookReservationsCount = x.Count()
                                                        })
                                                        .OrderBy((x) => x.UserName)
                                                        .ToListAsync();


            return result;
        }

        public async Task<IEnumerable<ReservationHistoryBookResponseDto>> GetReservationHistoryByBookAsync(ReservationHistoryBookDto dto)
        {
            var result = await _bookReservationHistoriesRepo.GetData()
                                                           .Where(x => x.BookId == dto.BookId)
                                                           .Select(x => new ReservationHistoryBookResponseDto
                                                           {
                                                               BookTitle = x.Book.BookTitle,
                                                               FirstPublishDate = x.Book.FirstPublishDate,
                                                               EditionNumber = x.Book.EditionNumber,
                                                               EditionDate = x.Book.EditionDate,
                                                               DeliveryDateToUser = x.DeliveryDateToUser,
                                                               RecievedDate = x.RecievedDate,
                                                               UserName = x.User.UserName,
                                                               FullName = x.User.FullName
                                                           })
                                                           .OrderBy((x) => x.BookTitle)
                                                           .ToListAsync();
            
            return result;
        }

        public async Task<IEnumerable<ReservationHistoryUserResponseDto>> GetReservationHistoryByUserAsync(ReservationHistoryUserDto dto)
        {
            var result = await _bookReservationHistoriesRepo.GetData()
                                                           .Where(x => x.UserId == dto.UserId)
                                                           .Select(x => new ReservationHistoryUserResponseDto
                                                           {
                                                               BookTitle = x.Book.BookTitle,
                                                               FirstPublishDate = x.Book.FirstPublishDate,
                                                               EditionNumber = x.Book.EditionNumber,
                                                               EditionDate = x.Book.EditionDate,
                                                               DeliveryDateToUser = x.DeliveryDateToUser,
                                                               RecievedDate = x.RecievedDate,
                                                               UserName = x.User.UserName,
                                                               FullName = x.User.FullName
                                                           })
                                                           .OrderBy((x) => x.UserName)
                                                           .ToListAsync();
            
            return result;
        }

        public async Task<IEnumerable<ReservationHistoryBookResponseDto>> GetReservationHistoryPerBookAsync()
        {
            var result = await _bookReservationHistoriesRepo.GetData()
                                                            .Select(x => new ReservationHistoryBookResponseDto
                                                            {
                                                                BookTitle = x.Book.BookTitle,
                                                                FirstPublishDate = x.Book.FirstPublishDate,
                                                                EditionNumber = x.Book.EditionNumber,
                                                                EditionDate = x.Book.EditionDate,
                                                                DeliveryDateToUser = x.DeliveryDateToUser,
                                                                RecievedDate = x.RecievedDate,
                                                                UserName = x.User.UserName,
                                                                FullName = x.User.FullName
                                                            })
                                                            .OrderBy((x) => x.BookTitle)
                                                            .ToListAsync();
            
            return result;
        }

        public async Task<IEnumerable<ReservationHistoryUserResponseDto>> GetReservationHistoryPerUserAsync()
        {
            var result = await _bookReservationHistoriesRepo.GetData()
                                                           .Select(x => new ReservationHistoryUserResponseDto
                                                           {
                                                               BookTitle = x.Book.BookTitle,
                                                               FirstPublishDate = x.Book.FirstPublishDate,
                                                               EditionNumber = x.Book.EditionNumber,
                                                               EditionDate = x.Book.EditionDate,
                                                               DeliveryDateToUser = x.DeliveryDateToUser,
                                                               RecievedDate = x.RecievedDate,
                                                               UserName = x.User.UserName,
                                                               FullName = x.User.FullName
                                                           })
                                                           .OrderBy((x) => x.UserName)
                                                           .ToListAsync();
            
            return result;
        }
    }
}