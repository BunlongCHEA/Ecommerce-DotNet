using ECommerceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ShipmentService : IShipmentService
{
    private readonly ApplicationDbContext _context;

    public ShipmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ActionResult<IEnumerable<Shipment>>> GetShipments(string userId)
    {
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

        if (shipments == null || shipments.Count == 0)
        {
            return new NotFoundObjectResult("No shipments found for the logged-in user.");
        }

        return new OkObjectResult(shipments);
    }

    public async Task<ActionResult<Shipment>> GetShipment(int id, string userId)
    {
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
            return new NotFoundObjectResult("Shipment not found or does not belong to the logged-in user.");
        }

        return new OkObjectResult(shipment);
    }

    public async Task<ActionResult<Shipment>> CreateShipment(Shipment shipment, string userId)
    {
        if (shipment == null || string.IsNullOrEmpty(shipment.ShipmentTypeId.ToString()))
        {
            return new BadRequestObjectResult("Invalid Create Shipment data. Some fields cannot be null or missing.");
        }

        _context.Shipments.Add(shipment);
        await _context.SaveChangesAsync();
        return new CreatedAtActionResult(nameof(GetShipment), nameof(Shipment), new { id = shipment.Id }, shipment);
    }

    public async Task<IActionResult> UpdateShipment(int id, Shipment shipment, string userId)
    {
        if (shipment == null || string.IsNullOrEmpty(shipment.ShipmentTypeId.ToString()))
        {
            return new BadRequestObjectResult("Invalid Shipment data OR mismatch Shipment Type ID.");
        }

        if (id != shipment.Id)
        {
            return new BadRequestObjectResult("ID mismatch in the request.");
        }

        _context.Entry(shipment).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return new NoContentResult();
    }
    
    public async Task<IActionResult> DeleteShipment(int id, string userId)
    {
        var shipment = await _context.Shipments.FindAsync(id);
        if (shipment == null)
        {
            return new NotFoundObjectResult("Shipment not found.");
        }

        _context.Shipments.Remove(shipment);
        await _context.SaveChangesAsync();

        return new NoContentResult();
    }
}