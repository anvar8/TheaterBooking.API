using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheaterBooking.Data;
using TheaterBooking.Models.Shows;
using Microsoft.Extensions.DependencyInjection;
using TheaterBooking.Models.ShowTimes;
using TheaterBooking.Helpers;

namespace TheaterBooking.Services
{
    public class ShowService
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly HttpContextService _httpContextService;
        public ShowService(ApplicationDbContext context, IServiceProvider serviceProvider, 
            HttpContextService httpContextService)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _httpContextService = httpContextService;
        }

        public async Task<PagedList<Show>> GetAllShows(ShowParams showParams)
        {
            var shows = _context.Shows.AsQueryable();

            if (!string.IsNullOrEmpty(showParams.ShowName))
                shows = shows.Where(s => s.Name.ToLower().Contains(showParams.ShowName.ToLower()));
            if (!string.IsNullOrEmpty(showParams.StartPeriod.ToString()))
                shows = shows.Where(s => s.StartPeriod >= showParams.StartPeriod);
            if (!string.IsNullOrEmpty(showParams.EndPeriod.ToString()))
                shows = shows.Where(s => s.EndPeriod <= showParams.EndPeriod);
            if (!string.IsNullOrEmpty(showParams.StartDate.ToString()))
                shows = shows
                    .Include(s => s.ShowTimes
                    .Where(st => st.StartDate >= showParams.StartDate));
            if (!string.IsNullOrEmpty(showParams.EndDate.ToString()))
                shows = shows
                    .Include(s => s.ShowTimes
                    .Where(st => st.EndDate <= showParams.EndDate));
            return await PagedList<Show>.CreateAsync(shows.OrderBy(s => s.StartPeriod),
                showParams.PageNumber, showParams.PageSize);

        }
        public async Task<Show> GetShowById (Guid id)
        {
            var show = await _context.Shows
           .Include(s => s.ShowTimes)   
           .FirstOrDefaultAsync(s => s.Id == id);
            if (show == null)
                throw new ArgumentException("Not found");
            return show;
        }
        public async Task<Show> AddShow(Show show)
        {
            _context.Shows.Add(show);
            _context.ShowTimes.AddRange(show.ShowTimes);
            await _context.SaveChangesAsync();
            return show;
        }

        public async Task EditShow (Guid id, Show show)
        {
            if (id != show.Id) throw new ArgumentException("Id mismatch");
            var dbShow = await GetShowById(id);
            if (dbShow == null) throw new ArgumentException("Not found");
            _context.Entry(dbShow).CurrentValues.SetValues(show);          

            //delete showtimes that are not in updated show
            foreach (var existingShowTime in dbShow.ShowTimes)
            {
                if (!show.ShowTimes.Any(st => st.Id == existingShowTime.Id))
                {
                    dbShow.ShowTimes.Remove(existingShowTime);
                    _context.Entry(existingShowTime).State = EntityState.Deleted;
                    //notify users about cancelled show
                    //@Todo
                    //refactor use accumulator and then send out emails
                    await ShowCancelled(existingShowTime);                    
                }

            }
            // Update and Insert showtimes 
            foreach (var showTime in show.ShowTimes)
            {
                var existingShowTime = dbShow.ShowTimes
                    .Where(st => st.Id == showTime.Id)
                    .SingleOrDefault();
                if (existingShowTime != null)
                {
                    var initialTime = existingShowTime.StartDate;
                    // Update show time          
                    _context.Entry(existingShowTime).CurrentValues.SetValues(showTime);
                    _context.Entry(existingShowTime).State = EntityState.Modified;
                    if (initialTime != showTime.StartDate)
                    {
                        //@Todo
                        //refactor use accumulator and then send out emails
                        //notify users on updated showtime
                        await ShowTimeModified(existingShowTime);
                    }

                }
                else
                {                   
                    dbShow.ShowTimes.Add(showTime);
                    _context.Entry(showTime).State = EntityState.Added;
                }
            }
        }
        public async Task DeleteShow (Guid id)
        {
            var show = await GetShowById(id);
            // check if user is client
            _context.Shows.Remove(show);
            await _context.SaveChangesAsync();
        }

        private async Task ShowCancelled (ShowTime showTime)
        {
            var emailService = _serviceProvider.GetService<EmailSender>();
            var dbBookings = _context.Bookings.Where(b => b.ShowTimeId == showTime.Id);
            foreach (var booking in dbBookings)
            {
                var userEmail = await _httpContextService.
                    GetClientEmailByIdAsync(booking.UserId.ToString());
                await emailService.SendBookingCanceled(userEmail, 
                    showTime.Show.Name, showTime.StartDate);
            }
        }
        private async Task ShowTimeModified(ShowTime showTime)
        {
            var emailService = _serviceProvider.GetService<EmailSender>();
            var dbBookings = _context.Bookings.Where(b => b.ShowTimeId == showTime.Id);
            foreach (var booking in dbBookings)
            {
                var userEmail = await _httpContextService.
                    GetClientEmailByIdAsync(booking.UserId.ToString());
                await emailService.SendBookingDateChanged(userEmail,
                    showTime.Show.Name, showTime.StartDate);
            }
        }


    }
}
