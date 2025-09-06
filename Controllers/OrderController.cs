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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _OrderService;

        public OrderController(IOrderService orderService)
        {
            _OrderService = orderService;
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
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
            
            return await _OrderService.GetOrders(userId);
        }

        // GET: api/order/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
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

            return await _OrderService.GetOrder(id, userId, userRole);
        }

        // POST: api/order
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            return await _OrderService.CreateOrder(order, userId);
        }

        // // PUT: api/order/{id}
        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateOrder(int id, Order order)
        // {
        //     // Find the logged-in userId
        //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     if (string.IsNullOrEmpty(userId))
        //     {
        //         return Unauthorized("User not authenticated or valid.");
        //     }

        //     return await _OrderService.UpdateOrder(id, order, userId);
        // }

        // // DELETE: api/order/{id}
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteOrder(int id)
        // {
        //     // Find the logged-in userId
        //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     if (string.IsNullOrEmpty(userId))
        //     {
        //         return Unauthorized("User not authenticated or valid.");
        //     }

        //     return await _OrderService.DeleteOrder(id, userId);
        // }
    }
}