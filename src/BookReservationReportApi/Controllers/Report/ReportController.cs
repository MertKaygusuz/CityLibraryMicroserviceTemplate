using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookReservationReportApi.Dtos.ReservationReport;
using BookReservationReportApi.Services.ReservationReport.Interfaces;

namespace BookReservationReportApi.Controllers.Report
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ReportController(IReservationReportService reservationReportService) : ControllerBase
    {
        private readonly IReservationReportService _reservationReportService = reservationReportService;

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<ActiveBookReservationsResponseDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<ActiveBookReservationsResponseDto>> GetActiveBookReservations(ActiveBookReservationsFilterDto dto)
        {
            return await _reservationReportService.GetAllActiveBookReservationsAsync(dto);
        }
        

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NumberOfBooksReservedByUsersResponseDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<NumberOfBooksReservedByUsersResponseDto>> GetNumberOfBooksReservedPerUsers()
        {
            return await _reservationReportService.GetNumberOfBooksReservedPerUsersAsync();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReservationHistoryBookResponseDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<ReservationHistoryBookResponseDto>> GetReservationHistoryPerBook()
        {
            return await _reservationReportService.GetReservationHistoryPerBookAsync();
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReservationHistoryUserResponseDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<ReservationHistoryUserResponseDto>> GetReservationHistoryPerUser()
        {
            return await _reservationReportService.GetReservationHistoryPerUserAsync();
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<ReservationHistoryBookResponseDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<ReservationHistoryBookResponseDto>> GetReservationHistoryByBook(ReservationHistoryBookDto dto)
        {
            return await _reservationReportService.GetReservationHistoryByBookAsync(dto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<ReservationHistoryUserResponseDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<ReservationHistoryUserResponseDto>> GetReservationHistoryByUser(ReservationHistoryUserDto dto)
        {
            return await _reservationReportService.GetReservationHistoryByUserAsync(dto);
        }
    }
}
