using Microsoft.AspNetCore.Mvc;

public interface IOrderItemService
{
    Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(string userId);
    Task<ActionResult<OrderItem>> GetOrderItem(int id, string userId);
    Task<ActionResult<OrderItem>> CreateOrderItem(OrderItem order, string userId);
    Task<IActionResult> UpdateOrderItem(int id, OrderItem order, string userId);
    Task<IActionResult> DeleteOrderItem(int id, string userId);
}