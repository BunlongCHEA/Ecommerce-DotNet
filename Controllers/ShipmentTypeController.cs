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
    public class ShipmentTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ShipmentTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/shipmenttype
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShipmentType>>> GetShipmentTypes()
        {
            var shipmentTypes = await _context.ShipmentTypes.ToListAsync();
            return Ok(shipmentTypes);
        }

        // GET: api/shipmenttype/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ShipmentType>> GetShipmentType(int id)
        {
            var shipmentType = await _context.ShipmentTypes.FindAsync(id);
            if (shipmentType == null)
            {
                return NotFound();
            }
            return Ok(shipmentType);
        }

        // POST: api/shipmenttype
        [HttpPost]
        public async Task<ActionResult<ShipmentType>> CreateShipmentType(ShipmentType shipmentType)
        {
            if (shipmentType == null || string.IsNullOrEmpty(shipmentType.Type))
            {
                return BadRequest("Invalid Create Shipment Type data. Some fields cannot be null or missing.");
            }

            _context.ShipmentTypes.Add(shipmentType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetShipmentType), new { id = shipmentType.Id }, shipmentType);
        }

        // PUT: api/shipmenttype/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShipmentType(int id, ShipmentType shipmentType)
        {
            if (shipmentType == null || string.IsNullOrEmpty(shipmentType.Type))
            {
                return BadRequest("Invalid Shipment Type data OR mismatch Shipment Type ID.");
            }
            _context.Entry(shipmentType).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Content("Shipment Type updated successfully");
        }

        // DELETE: api/shipmenttype/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipmentType(int id)
        {
            var shipmentType = await _context.ShipmentTypes.FindAsync(id);
            if (shipmentType == null)
            {
                return NotFound();
            }

            _context.ShipmentTypes.Remove(shipmentType);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}