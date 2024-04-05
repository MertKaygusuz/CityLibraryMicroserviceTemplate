using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookReservationReportApi.Dtos.ReservationReport
{
    public class ActiveBookReservationsFilterDto
    {
        public string UserName { get; set; }

        public string BookTitle { get; set; }
    }
}