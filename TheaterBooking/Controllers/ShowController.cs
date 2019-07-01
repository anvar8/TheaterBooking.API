using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TheaterBooking.Helpers;
using TheaterBooking.Models.Shows;
using TheaterBooking.Services;


namespace TheaterBooking.Controllers
{
    [Authorize (Roles = "Administrator")]
    [Route("api/[controller]")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        private readonly ShowService _service;
        public ShowController(ShowService service)
        {
            _service = service;
        }

        // GET: api/shows
        [HttpGet]       
        [AllowAnonymous]
        public async Task<IActionResult> GetShows(ShowParams showParams)
        {
            var shows = await _service.GetAllShows(showParams);
            Response.AddPagination(shows.CurrentPage,
                shows.PageSize, shows.TotalCount,
                shows.TotalPages);
            return Ok(shows);
        }
        // GET: api/shows/5
        [HttpGet("{id}")]   
        public async Task<IActionResult> GetShow(Guid id)
        {
            var show = await _service.GetShowById(id);
            return Ok(show);
        }
        // PUT: api/shows/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShow(Guid id, Show show)
        {
            await _service.EditShow(id, show);
            return NoContent();
        }
        // POST: api/shows
        [HttpPost]
        public async Task<IActionResult> PostShow(Show show)
        {
            await _service.AddShow(show);
            return CreatedAtAction("GetShow", new { id = show.Id }, show);
        }
        // DELETE: api/shows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShow(Guid id)
        {
            await _service.DeleteShow(id);
            return Ok();
        }

    }
}
