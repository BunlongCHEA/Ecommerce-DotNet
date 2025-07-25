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
    public class LocationCountryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationCountryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/locationcountry
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LocationCountry>>> GetLocationCountries()
        {
            var countries = await _context.LocationCountries.ToListAsync();
            return Ok(countries);
        }

        // GET: api/locationcountry/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LocationCountry>> GetLocationCountry(int id)
        {
            var country = await _context.LocationCountries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return Ok(country);
        }

        // POST: api/locationcountry
        [HttpPost]
        public async Task<ActionResult<LocationCountry>> CreateLocationCountry(LocationCountry country)
        {
            if (country == null || string.IsNullOrEmpty(country.CountryName))
            {
                return BadRequest("Invalid Create Location Country data. Some fields cannot be null or missing.");
            }

            _context.LocationCountries.Add(country);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLocationCountry), new { id = country.Id }, country);
        }

        // PUT: api/locationcountry/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLocationCountry(int id, LocationCountry country)
        {
            if (id != country.Id || string.IsNullOrEmpty(country.CountryName))
            {
                return BadRequest("Invalid Location Country data OR mismatch Location Country ID.");
            }

            _context.Entry(country).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Content("Location Country updated successfully");
        }

        // DELETE: api/locationcountry/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLocationCountry(int id)
        {
            var country = await _context.LocationCountries.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.LocationCountries.Remove(country);
            await _context.SaveChangesAsync();
            return Ok(country);
        }
    }
}