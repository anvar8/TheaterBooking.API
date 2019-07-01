using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheaterBooking.Helpers
{
    public class ShowParams: PageParams
    {
        public string ShowName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }
    }
}
