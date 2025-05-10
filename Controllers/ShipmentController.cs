using System.Security.Claims;
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
    public class ShipmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShipmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/shipment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shipment>>> GetShipments()
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
            
            var shipments = await _context.Shipments
                            .Include(s => s.ShipmentType) // Include related shipment type data
                            .Include(s => s.Location) // Include related location data
                            .Where(s => s.Location != null && s.Location.UserId == int.Parse(userId)) // Ensure the shipment belongs to the logged-in user
                            .Select(s => new
                            {
                                s.Id,
                                s.TrackingNumber,
                                s.ShipmentDate,
                                s.ExpectedDate,
                                s.DelayedDateFrom,
                                s.ArrivedDate,
                                s.ShippingCost,
                                ShipmentTypeId = s.ShipmentType != null ? s.ShipmentType.Id : 0,
                                LocationId = s.Location != null ? s.Location.Id : 0,
                            })
                            .ToListAsync();
            return Ok(shipments);
        }

        // GET: api/shipment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Shipment>> GetShipment(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            var shipment = await _context.Shipments
                            .Include(s => s.ShipmentType) // Include related shipment type data
                            .Include(s => s.Location) // Include related location data
                            .Where(s => s.Id == id && s.Location != null && s.Location.UserId == int.Parse(userId)) // Ensure the shipment belongs to the logged-in user
                            .Select(s => new
                            {
                                s.Id,
                                s.TrackingNumber,
                                s.ShipmentDate,
                                s.ExpectedDate,
                                s.DelayedDateFrom,
                                s.ArrivedDate,
                                s.ShippingCost,
                                ShipmentTypeId = s.ShipmentType != null ? s.ShipmentType.Id : 0,
                                LocationId = s.Location != null ? s.Location.Id : 0,
                            })
                            .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return NotFound();
            }
            return Ok(shipment);
        }

        // POST: api/shipment
        [HttpPost]
        public async Task<ActionResult<Shipment>> CreateShipment(Shipment shipment)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated or valid.");
            }

            if (shipment == null || string.IsNullOrEmpty(shipment.ShipmentTypeId.ToString()))
            {
                return BadRequest("Invalid Create Shipment data. Some fields cannot be null or missing.");
            }

            _context.Shipments.Add(shipment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetShipment), new { id = shipment.Id }, shipment);
        }

        // PUT: api/shipment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipment(int id, Shipment shipment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated or valid.");
            }

            if (shipment == null || string.IsNullOrEmpty(shipment.ShipmentTypeId.ToString()))
            {
                return BadRequest("Invalid Shipment data OR mismatch Shipment Type ID.");
            }

            if (id != shipment.Id)
            {
                return BadRequest("ID mismatch in the request.");
            }

            _context.Entry(shipment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/shipment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipment(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated or valid.");
            }

            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }

            _context.Shipments.Remove(shipment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}