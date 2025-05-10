using System.Security.Claims;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/location
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Location>>> GetLocations()
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"User ID: {userId}");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} : {claim.Value}");
            }

            var locations = await _context.Locations
                                .Include(l => l.LocationRegion) // Include related region data
                                .Include(l => l.LocationRegion != null ? l.LocationRegion.LocationCountry : null) // Include related country data with null check
                                .Where(l => l.UserId == int.Parse(userId)) // Ensure the location belongs to the logged-in user
                                .Select(l => new
                                {
                                    l.Id,
                                    l.PostalCode,
                                    l.Address,
                                    CountryId = l.LocationRegion != null ? l.LocationRegion.CountryId : (int?)null,
                                    CountryName = l.LocationRegion != null && l.LocationRegion.LocationCountry != null ? l.LocationRegion.LocationCountry.CountryName : "Unknown",
                                    CountryCode = l.LocationRegion != null && l.LocationRegion.LocationCountry != null ? l.LocationRegion.LocationCountry.CountryCode : "Unknown",
                                    l.RegionId,
                                    Region = l.LocationRegion != null ? l.LocationRegion.Region : "Unknown",
                                    l.UserId
                                })
                                .ToListAsync();
            return Ok(locations);
        }

        // GET: api/location/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Location>> GetLocation(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            var location = await _context.Locations
                            .Include(l => l.LocationRegion) // Include related region data
                            .Include(l => l.LocationRegion != null ? l.LocationRegion.LocationCountry : null) // Include related country data with null check
                            .Where(l => l.Id == id && l.UserId == int.Parse(userId)) // Ensure the location belongs to the logged-in user
                            .Select(l => new
                            {
                                l.Id,
                                l.PostalCode,
                                l.Address,
                                CountryId = l.LocationRegion != null ? l.LocationRegion.CountryId : (int?)null,
                                CountryName = l.LocationRegion != null && l.LocationRegion.LocationCountry != null ? l.LocationRegion.LocationCountry.CountryName : "Unknown",
                                CountryCode = l.LocationRegion != null && l.LocationRegion.LocationCountry != null ? l.LocationRegion.LocationCountry.CountryCode : "Unknown",
                                l.RegionId,
                                Region = l.LocationRegion != null ? l.LocationRegion.Region : "Unknown",
                                l.UserId
                            })
                            .FirstOrDefaultAsync();

            if (location == null)
            {
                return NotFound();
            }
            return Ok(location);
        }

        // POST: api/location
        [HttpPost]
        public async Task<ActionResult<Location>> CreateLocation(Location location)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || userId != location.UserId.ToString())
            {
                return Unauthorized("User is not authenticated or valid.");
            }

            if (location == null || string.IsNullOrEmpty(location.UserId.ToString()) || string.IsNullOrEmpty(location.RegionId.ToString()))
            {
                return BadRequest("Invalid Create Location data. Some fields cannot be null or missing.");
            }

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, location);
        }

        // PUT: api/location/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocation(int id, Location location)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || userId != location.UserId.ToString())
            {
                return Unauthorized("User is not authenticated or valid.");
            }

            if (location == null || string.IsNullOrEmpty(location.UserId.ToString()) || string.IsNullOrEmpty(location.RegionId.ToString()))
            {
                return BadRequest("Invalid Location data OR mismatch Location ID.");
            }

            _context.Entry(location).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/location/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated or valid.");
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}