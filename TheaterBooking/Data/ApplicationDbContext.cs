using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TheaterBooking.Models.Bookings;
using TheaterBooking.Models.Shows;
using TheaterBooking.Models.ShowTimes;
using TheaterBooking.Models.Users;

namespace TheaterBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options)
          : base(options)
        {
        }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Show> Shows { get; set; }
        public DbSet<ShowTime> ShowTimes { get; set; }
    
    }
}
