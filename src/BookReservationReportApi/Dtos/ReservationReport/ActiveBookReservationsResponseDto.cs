using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityLibrary.Shared.SharedEnums;

namespace BookReservationReportApi.Dtos.ReservationReport
{
    public class ActiveBookReservationsResponseDto
    {
        public DateTime DeliveryDateToUser { get; set; }

        public DateTime AvailableAt => DeliveryDateToUser.AddDays(7);

        public string UserName { get; set; }

        public string UserFullName { get; set; }

        public string BookTitle { get; set; }

        public short EditionNumber { get; set; }

        public BookCoverTypes CoverType { get; set; }
    }
}