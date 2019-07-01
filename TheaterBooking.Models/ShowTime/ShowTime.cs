using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheaterBooking.Models.Shows;

namespace TheaterBooking.Models.ShowTimes

{
    public class ShowTime
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TicketsAvailable { get; set; }
        public int PricePerTicket { get; set; }
        [ForeignKey(nameof(Show))]
        public Guid ShowId { get; set; }
        public Show Show { get; set; }
    }
}
