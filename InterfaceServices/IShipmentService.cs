using Microsoft.AspNetCore.Mvc;

public interface IShipmentService
{
    Task<ActionResult<IEnumerable<Shipment>>> GetShipments(string userId);
    Task<ActionResult<Shipment>> GetShipment(int id, string userId);
    Task<ActionResult<Shipment>> CreateShipment(Shipment shipment, string userId);
    Task<IActionResult> UpdateShipment(int id, Shipment shipment, string userId);
    Task<IActionResult> DeleteShipment(int id, string userId);
}