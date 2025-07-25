using System.Security.Claims;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/admin/orderitem")]
    // Apply the "Admin" policy to the entire controller
    [Authorize(Policy = "Admin")]
    public class AdminOrderItemController : ControllerBase
    {
        private readonly IOrderItemService _OrderItemService;

        public AdminOrderItemController(IOrderItemService orderItemService)
        {
            _OrderItemService = orderItemService;
        }

        // GET: api/admin/orderitem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchQuery = null, 
            [FromQuery] string searchType = null,
            [FromQuery] string[] statuses = null
        )
        {
            // Validate pageSize and set allowed values (10, 100, 200, 500)
            pageSize = pageSize switch
            {
                10 or 20 or 50 or 100 => pageSize,
                _ => 10 // Default to 10 if invalid
            };

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
            
            return await _OrderItemService.GetOrderItems(userId, pageNumber, pageSize, searchQuery, searchType, statuses);
        }

        // GET: api/admin/orderitem/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderItem>> GetOrderItem(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            return await _OrderItemService.GetOrderItem(id, userId);
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

        // PUT: api/admin/orderitem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, OrderItem orderItem)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            return await _OrderItemService.UpdateOrderItem(id, orderItem, userId);
        }

        // DELETE: api/admin/orderitem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            return await _OrderItemService.DeleteOrderItem(id, userId);
        }
    }
}