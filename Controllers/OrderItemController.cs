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
        private readonly ApplicationDbContext _context;

        public OrderItemController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/orderitem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems()
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
            
            var orderItems = await _context.OrderItems
                            .Include(oi => oi.Product) // Include related product data
                            .Include(oi => oi.Order) // Include related order data
                            .ThenInclude(o => o.CouponUserList) // Include related coupon user list data
                            .ThenInclude(cur => cur.Coupon) // Include related coupon data
                            // .Include(oi => oi.Product != null ? oi.Product.Coupon : null) // Include related coupon data with null check
                            .Where(oi => oi.Order != null && oi.Order.Payment != null && oi.Order.Payment.UserId == int.Parse(userId)) // Ensure the order item belongs to the logged-in user
                            .Select(oi => new
                            {
                                oi.Id,
                                oi.ProductId,
                                oi.OrderId,
                                oi.Quantity,
                                oi.Price,
                                oi.TotalPrice,
                                oi.Order.AmountAfterDiscount,
                                OrderNumber = oi.Order != null ? oi.Order.OrderNumber : null,
                                Status = oi.Order != null ? oi.Order.Status : null,
                                ProductName = oi.Product != null ? oi.Product.Name : null,
                                ImageUrl = oi.Product != null ? oi.Product.ImageUrl : null,
                                DiscountAmount = oi.Product.Coupon != null ? oi.Product.Coupon.DiscountAmount : 0, // Assuming you want to include the discount from the coupon
                                CouponDiscount = oi.Order != null && oi.Order.CouponUserList != null && oi.Order.CouponUserList.Coupon != null 
                                    ? oi.Order.CouponUserList.Coupon.DiscountAmount 
                                    : 0,
                                oi.Order.OrderDate,
                                oi.Order.CompletedDate,
                                oi.Order.CancelledDate,
                                oi.Order.Shipment.ShipmentDate,
                                oi.Order.Shipment.ExpectedDate,
                                oi.Order.Shipment.DelayedDateFrom,
                                oi.Order.Shipment.ArrivedDate,
                            })
                            .ToListAsync();
            return Ok(orderItems);
        }

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

            var orderItems = await _context.OrderItems
                            .Include(oi => oi.Product) // Include related product data
                            .Include(oi => oi.Order) // Include related order 
                            .ThenInclude(o => o.CouponUserList) // Include related coupon user list data
                            .ThenInclude(cur => cur.Coupon) // Include related coupon data
                            // .Include(oi => oi.Product != null ? oi.Product.Coupon : null) // Include related coupon data with null check
                            .Where(oi => oi.Order != null && oi.Order.Payment != null && oi.Order.Payment.UserId == int.Parse(userId) && oi.OrderId == id) // Ensure the order item belongs to the logged-in user
                            .Select(oi => new
                            {
                                oi.Id,
                                oi.ProductId,
                                oi.OrderId,
                                oi.Quantity,
                                oi.Price,
                                oi.TotalPrice,
                                oi.Order.AmountAfterDiscount,
                                OrderNumber = oi.Order != null ? oi.Order.OrderNumber : null,
                                Status = oi.Order != null ? oi.Order.Status : null,
                                ProductName = oi.Product != null ? oi.Product.Name : null,
                                ImageUrl = oi.Product != null ? oi.Product.ImageUrl : null,
                                DiscountAmount = oi.Product.Coupon != null ? oi.Product.Coupon.DiscountAmount : 0, // Include the discount from the coupon
                                CouponDiscount = oi.Order != null && oi.Order.CouponUserList != null && oi.Order.CouponUserList.Coupon != null 
                                    ? oi.Order.CouponUserList.Coupon.DiscountAmount 
                                    : 0,
                                oi.Order.OrderDate,
                                oi.Order.CompletedDate,
                                oi.Order.CancelledDate,
                                oi.Order.Shipment.ShipmentDate,
                                oi.Order.Shipment.ExpectedDate,
                                oi.Order.Shipment.DelayedDateFrom,
                                oi.Order.Shipment.ArrivedDate,
                            })
                            .ToListAsync();

            if (orderItems == null)
            {
                return NotFound();
            }
            return Ok(orderItems);
        }

        // POST: api/orderitem
        [HttpPost]
        public async Task<ActionResult<OrderItem>> CreateOrderItem(OrderItem orderItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            if (orderItem == null || string.IsNullOrEmpty(orderItem.ProductId.ToString()) || string.IsNullOrEmpty(orderItem.OrderId.ToString()))
            {
                return BadRequest("Invalid Create OrderItem data. Some fields cannot be null or missing.");
            }

            _context.OrderItems.Add(orderItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrderItem), new { id = orderItem.Id }, orderItem);
        }

        // PUT: api/orderitem/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItem(int id, OrderItem orderItem)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            if (orderItem == null || string.IsNullOrEmpty(orderItem.ProductId.ToString()) || string.IsNullOrEmpty(orderItem.OrderId.ToString()))
            {
                return BadRequest("Invalid Order Item data OR mismatch Product ID / Order ID.");
            }

            _context.Entry(orderItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/orderitem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItem(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }
            
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            _context.OrderItems.Remove(orderItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}