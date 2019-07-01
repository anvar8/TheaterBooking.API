using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheaterBooking.Data;
using TheaterBooking.Models.Bookings;
using Microsoft.Extensions.DependencyInjection;
using TheaterBooking.Helpers;

namespace TheaterBooking.Services
{
    public class BookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpContextService _httpContextService;
        private readonly IServiceProvider _serviceProvider;


        public BookingService(ApplicationDbContext context, HttpContextService httpContextService, 
            IServiceProvider serviceProvider)
        {
            _context = context;
            _httpContextService = httpContextService;
            _serviceProvider = serviceProvider;
         
        }
        public async Task<PagedList<Booking>> GetAllBookings(BookingParams bookingParams)
        {
            var bookings =  _context.Bookings
                .Include(b => b.ShowTime)
                .Include(b => b.User).AsQueryable();
              
            if (!string.IsNullOrEmpty(bookingParams.ShowName))
                bookings = bookings.Where(b => 
                b.ShowTime.Show.Name.ToLower().Contains(bookingParams.ShowName.ToLower()));
            if (!string.IsNullOrEmpty(bookingParams.StartDate.ToString()))
                bookings = bookings.Where(b => b.ShowTime.StartDate >= bookingParams.StartDate);
            if (!string.IsNullOrEmpty(bookingParams.EndDate.ToString()))
                bookings = bookings.Where(b => b.ShowTime.StartDate <= bookingParams.EndDate);

            // check if the user is client 
            if (_httpContextService.IsClient())
            {
                // show only the client's bookings
                var userId = _httpContextService.GetUserId();
                bookings = bookings.Where(b => b.UserId == userId).AsQueryable();
            }
            return await PagedList<Booking>.CreateAsync(bookings.OrderBy(b => b.ShowTime.StartDate), 
                bookingParams.PageNumber, bookingParams.PageSize);
        }
        public async Task<Booking> GetBookingById(Guid id)
        {
            var booking = await _context.Bookings
                .Include(b => b.ShowTime)
                .Include(b => b.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
                throw new ArgumentException("Booking Not found");

            if (_httpContextService.IsClient())
            {
                var userId = _httpContextService.GetUserId();
                if (booking.UserId != userId)
                    throw new ArgumentException ("Action not allowed");
            }
            return booking;
        }

        public async Task<Booking> AddBooking(Booking booking)
        {
            // validate booking
            if (booking.ShowTime.StartDate > booking.ShowTime.EndDate)
                throw new ArgumentException("Wrong Date");

            if (booking.ShowTime.StartDate < DateTime.Now)
                throw new ArgumentException("Wrong Date");    
            var dbShowTime = await _context.ShowTimes.FindAsync(booking.ShowTimeId);
            booking.ShowTime = dbShowTime ?? throw new ArgumentException("Show not found");
         
            if (dbShowTime.TicketsAvailable < booking.NumberOfTickets)
            {
                throw new ArgumentException("Limit of tickets reached");
            }
            dbShowTime.TicketsAvailable -= booking.NumberOfTickets;
            // identify user's Id
            var userId = _httpContextService.GetUserId();
            // identify user's Email
            var userEmail = _httpContextService.GetUserEmail();
            booking.UserId = userId;
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();     
            return booking;
        }
        public async Task EditBooking(Guid id, Booking booking)
        {
            if (id != booking.Id)
                throw new ArgumentException("Id mismatch");

            var dbBooking = await GetBookingById(id);
            if (dbBooking == null)
                throw new ArgumentException("Booking not found");


            if (_httpContextService.IsClient())
            {
                var userId = _httpContextService.GetUserId();

                if (dbBooking.UserId == userId)
                    throw new ArgumentException("Action not allowed.");
            }

            _context.Entry(dbBooking).CurrentValues.SetValues(booking);
            await _context.SaveChangesAsync();
  

        }
        public async Task DeleteBooking(Guid id)
        {
            var booking = await GetBookingById(id);
            // check if user is client
            if (_httpContextService.IsClient())
            {
                var userId = _httpContextService.GetUserId();
                if (booking.UserId == userId)
                    throw new ArgumentException("Action not allowed.");
            }
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            var emailService = _serviceProvider.GetService<EmailSender>();
            var userEmail = await _httpContextService.GetClientEmailByIdAsync(booking.UserId);
            await emailService.SendBookingCanceled(userEmail,
                booking.ShowTime.Show.Name, booking.ShowTime.StartDate);
        }


    }
}
