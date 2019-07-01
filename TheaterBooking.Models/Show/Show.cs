using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheaterBooking.Models.ShowTimes;

namespace TheaterBooking.Models.Shows
{
    public class Show
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public ICollection<ShowTime> ShowTimes { get; set; }
    }
}
