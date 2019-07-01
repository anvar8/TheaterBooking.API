using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheaterBooking.Helpers
{
    public class BookingParams: PageParams
    {  

        public string ShowName { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }
    }
}
