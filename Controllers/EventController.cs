using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Data;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EventController> _logger;

        public EventController(ApplicationDbContext context, ILogger<EventController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/event/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveEvents()
        {
            try
            {
                var currentDate = DateTimeOffset.UtcNow;
                
                var activeEvents = await _context.Events
                    .Where(e => e.StartDate <= currentDate && e.EndDate >= currentDate)
                    .OrderBy(e => e.StartDate)
                    .Select(e => new
                    {
                        id = e.Id,
                        name = e.Name,
                        imageUrl = e.ImageUrl,
                        description = e.Description,
                        startDate = e.StartDate,
                        endDate = e.EndDate
                    })
                    .ToListAsync();

                return Ok(activeEvents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active events");
                return StatusCode(500, new { message = "Error getting active events" });
            }
        }

        // GET: api/event
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            try
            {
                var events = await _context.Events
                    .OrderBy(e => e.StartDate)
                    .Select(e => new
                    {
                        id = e.Id,
                        name = e.Name,
                        imageUrl = e.ImageUrl,
                        description = e.Description,
                        startDate = e.StartDate,
                        endDate = e.EndDate
                    })
                    .ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting events");
                return StatusCode(500, new { message = "Error getting events" });
            }
        }
    }
}