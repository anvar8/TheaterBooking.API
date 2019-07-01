using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TheaterBooking.Helpers;
using TheaterBooking.Services;

namespace TheaterBooking.Controllers
{
    [Authorize(Roles = "Administrator" )]
    [Authorize(Roles = "Client")]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly BookingService _service;

        public BookingsController(BookingService service)
        {
            _service = service;
        }

       // GET: api/Bookings
        [HttpGet]
        public async Task<IActionResult> GetBookings([FromQuery] BookingParams bookingParams)
        {
            var bookings = await _service.GetAllBookings(bookingParams);
            Response.AddPagination(bookings.CurrentPage,
                bookings.PageSize, bookings.TotalCount,
                bookings.TotalPages);
            return Ok(bookings);

        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var booking = await _service.GetBookingById(id);

            return Ok(booking);
        }

        // PUT: api/Bookings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(Guid id, Models.Bookings.Booking booking)
        {
            await _service.EditBooking(id, booking);

            return NoContent();
        }

        // POST: api/Bookings
        [HttpPost]
        public async Task<IActionResult> PostBooking(Models.Bookings.Booking booking)
        {
            await _service.AddBooking(booking);
            return CreatedAtAction("GetBooking", new { id = booking.Id }, booking);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(Guid id)
        {
            await _service.DeleteBooking(id);

            return Ok();
        }    

    }
}
