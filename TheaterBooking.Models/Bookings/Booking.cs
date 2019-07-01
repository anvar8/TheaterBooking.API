using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheaterBooking.Models.ShowTimes;
using TheaterBooking.Models.Users;

namespace TheaterBooking.Models.Bookings
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfTickets { get; set; }
        [ForeignKey(nameof(ShowTime))]
        public Guid ShowTimeId { get; set; }
        public ShowTime ShowTime { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }   
    }
}
