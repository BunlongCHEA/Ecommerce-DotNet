using Microsoft.AspNetCore.Mvc;

public interface IOrderService
{
    Task<ActionResult<IEnumerable<Order>>> GetOrders(string userId);
    Task<ActionResult<Order>> GetOrder(int id, string userId);
    Task<ActionResult<Order>> CreateOrder(Order order, string userId);
    Task<IActionResult> UpdateOrder(int id, Order order, string userId);
    Task<IActionResult> DeleteOrder(int id, string userId);
}