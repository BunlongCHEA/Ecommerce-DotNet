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
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
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
            
            var orders = await _context.Orders
                        .Include(o => o.Payment) // Include Payment for each order
                        .Include(o => o.Shipment) // Include Shipment for each order
                        .Include(o => o.CouponUserList) // Include CouponUserList for each order
                        .Where(o => o.Payment != null && o.Payment.UserId == int.Parse(userId)) // Ensure the order belongs to the logged-in user & payment is not null
                        .Select(o => new
                        {
                            o.Id,
                            o.OrderNumber,
                            o.OrderDate,
                            o.Status,
                            o.AmountAfterDiscount,
                            o.TotalQuantity,
                            o.TotalAmount,
                            o.CancelledDate,
                            o.CompletedDate,
                            o.ShipmentId,
                            o.PaymentId,
                            o.CouponUserListId,
                            CouponIsUsed = o.CouponUserList != null ? o.CouponUserList.IsUsed : false, // include if the coupon is used
                            CouponExpiryDate = o.CouponUserList != null ? o.CouponUserList.ExpiryDate : null, // include the expiry date of the coupon
                            CouponCode = o.CouponUserList != null ? o.CouponUserList.Coupon.Code : null, // include the coupon code
                            CouponDiscountAmount = o.CouponUserList != null ? o.CouponUserList.Coupon.DiscountAmount : 0, // include the discount from the coupon
                            CouponDescription = o.CouponUserList != null ? o.CouponUserList.Coupon.Desription : null, // include the description from the coupon
                        })
                        .ToListAsync();

            if (orders == null || orders.Count == 0)
            {
                return NotFound("No orders found for the logged-in user.");
            }

            return Ok(orders);
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

            var order = await _context.Orders
                            .Include(o => o.Payment) // Include Payment for the order
                            .Include(o => o.Shipment) // Include Shipment for the order
                            .Include(o => o.CouponUserList) // Include CouponUserList for each order
                            .Where(o => o.Id == id && o.Payment != null && o.Payment.UserId == int.Parse(userId)) // Ensure the order belongs to the logged-in user & payment is not null
                            .Select(o => new
                            {
                                o.Id,
                                o.OrderNumber,
                                o.OrderDate,
                                o.Status,
                                o.AmountAfterDiscount,
                                o.TotalQuantity,
                                o.TotalAmount,
                                o.CancelledDate,
                                o.CompletedDate,
                                o.ShipmentId,
                                o.PaymentId,
                                o.CouponUserListId,
                                CouponIsUsed = o.CouponUserList != null ? o.CouponUserList.IsUsed : false, // include if the coupon is used
                                CouponExpiryDate = o.CouponUserList != null ? o.CouponUserList.ExpiryDate : null, // include the expiry date of the coupon
                                CouponCode = o.CouponUserList != null ? o.CouponUserList.Coupon.Code : null, // include the coupon code
                                CouponDiscountAmount = o.CouponUserList != null ? o.CouponUserList.Coupon.DiscountAmount : 0, // include the discount from the coupon
                                CouponDescription = o.CouponUserList != null ? o.CouponUserList.Coupon.Desription : null, // include the description from the coupon
                            })
                            .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound("Order not found or add.");
            }
            return Ok(order);
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

            if (order == null || string.IsNullOrEmpty(order.PaymentId.ToString()))
            {
                return BadRequest("Invalid Create Order data. Some fields cannot be null or missing.");
            }

            // Check if couponUserListId is null or 0 and handle it
            if (order.CouponUserListId == null || order.CouponUserListId == 0)
            {
                order.CouponUserListId = null; // Explicitly set to null to avoid validation errors
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        // PUT: api/order/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            if (id != order.Id || string.IsNullOrEmpty(order.PaymentId.ToString()))
            {
                return BadRequest("Invalid Order data OR mismatch Payment ID.");
            }

            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/order/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}