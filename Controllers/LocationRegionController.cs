using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationRegionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationRegionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/locationregion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationRegion>>> GetLocationRegions()
        {
            var regions = await _context.LocationRegions.ToListAsync();
            return Ok(regions);
        }

        // GET: api/locationregion/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationRegion>> GetLocationRegion(int id)
        {
            var region = await _context.LocationRegions.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }
            return Ok(region);
        }

        // POST: api/locationregion
        [HttpPost]
        public async Task<ActionResult<LocationRegion>> CreateLocationRegion(LocationRegion region)
        {
            if (region == null || string.IsNullOrEmpty(region.Region))
            {
                return BadRequest("Invalid Create Location Region data. Some fields cannot be null or missing.");
            }

            _context.LocationRegions.Add(region);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLocationRegion), new { id = region.Id }, region);
        }

        // PUT: api/locationregion/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocationRegion(int id, LocationRegion region)
        {
            if (id != region.Id || string.IsNullOrEmpty(region.Region))
            {
                return BadRequest("Invalid Location Region data OR mismatch Location Region ID.");
            }

            _context.Entry(region).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Content("Location Region updated successfully");
        }

        // DELETE: api/locationregion/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocationRegion(int id)
        {
            var region = await _context.LocationRegions.FindAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            _context.LocationRegions.Remove(region);
            await _context.SaveChangesAsync();
            return Ok(region);
        }
    }
}