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
    public class OrderItemController : ControllerBase
    {
        private readonly IOrderItemService _OrderItemService;

        public OrderItemController(IOrderItemService orderItemService)
        {
            _OrderItemService = orderItemService;
        }

        // GET: api/orderitem
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
        // {
        //     // Find the logged-in userId
        //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     Console.WriteLine($"User ID: {userId}");
        //     if (string.IsNullOrEmpty(userId))
        //     {
        //         return Unauthorized("User not authenticated or valid.");
        //     }
        //     foreach (var claim in User.Claims)
        //     {
        //         Console.WriteLine($"{claim.Type} : {claim.Value}");
        //     }
            
        //     return await _OrderItemService.GetOrderItems(userId);
        // }

        // GET: api/orderitem/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            // Get user role
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            Console.WriteLine($"User Role: {userRole}");
            
            if (string.IsNullOrEmpty(userRole))
            {
                return Unauthorized("User role not found.");
            }

            return await _OrderItemService.GetOrderItem(id, userId, userRole);
        }

        // POST: api/admin/orderitem
        [HttpPost]
        public async Task<ActionResult<OrderItem>> CreateOrderItem(OrderItem orderItem)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            return await _OrderItemService.CreateOrderItem(orderItem, userId);
        }

        // // PUT: api/orderitem/{id}
        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateOrderItem(int id, OrderItem orderItem)
        // {
        //     // Find the logged-in userId
        //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     if (string.IsNullOrEmpty(userId))
        //     {
        //         return Unauthorized("User not authenticated or valid.");
        //     }

        //     return await _OrderItemService.UpdateOrderItem(id, orderItem, userId);
        // }

        // // DELETE: api/orderitem/{id}
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteOrderItem(int id)
        // {
        //     // Find the logged-in userId
        //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     if (string.IsNullOrEmpty(userId))
        //     {
        //         return Unauthorized("User not authenticated or valid.");
        //     }

        //     return await _OrderItemService.DeleteOrderItem(id, userId);
        // }
    }
}