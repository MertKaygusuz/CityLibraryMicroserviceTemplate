using BookReservationReportApi.Services.ReservationReport.Interfaces;
using CityLibrary.Shared.SharedModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookReservationReportApi.Controllers.Record
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RecordController(IReservationRecordService reservationRecordService) : ControllerBase
    {
        private readonly IReservationRecordService _reservationRecordService = reservationRecordService;

        [HttpPost]
        public async Task<IActionResult> CreateReservation(ActiveBookReservationModel dto)
        {
            await _reservationRecordService.ReservationCreateAsync(dto);
            return NoContent();
        }
    }
}