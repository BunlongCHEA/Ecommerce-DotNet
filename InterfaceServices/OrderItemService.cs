using ECommerceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class OrderItemService : IOrderItemService
{
    private readonly ApplicationDbContext _context;

    public OrderItemService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(string userId)
    {        
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
                            TotalFinalAmount = oi.Order != null ? oi.Order.TotalFinalAmount : 0,
                            OrderNumber = oi.Order != null ? oi.Order.OrderNumber : null,
                            Status = oi.Order != null ? oi.Order.Status : null,
                            ProductName = oi.Product != null ? oi.Product.Name : null,
                            ImageUrl = oi.Product != null ? oi.Product.ImageUrl : null,
                            DiscountAmount = oi.Product != null && oi.Product.Coupon != null
                                ? oi.Product.Coupon.DiscountAmount 
                                : 0, // Include the discount from the coupon
                            CouponDiscount =
                                oi.Order != null && oi.Order.CouponUserList != null && oi.Order.CouponUserList.Coupon != null
                                ? oi.Order.CouponUserList.Coupon.DiscountAmount
                                : 0,
                            OrderDate = oi.Order != null ? oi.Order.OrderDate : (DateTime?)null,
                            CompletedDate = oi.Order != null ? oi.Order.CompletedDate : (DateTime?)null,
                            CancelledDate = oi.Order != null ? oi.Order.CancelledDate : (DateTime?)null,
                            ShipmentDate = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.ShipmentDate 
                                : (DateTime?)null,
                            ExpectedDate = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.ExpectedDate 
                                : (DateTime?)null,
                            DelayedDateFrom = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.DelayedDateFrom 
                                : (DateTime?)null,
                            ArrivedDate = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.ArrivedDate 
                                : (DateTime?)null,
                            TrackingNumber = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.TrackingNumber 
                                : null, // Include the tracking number from the shipment
                            ShippingCost = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.ShippingCost 
                                : 0, // Include the shipping cost from the shipment
                            Address = oi.Order != null && oi.Order.Shipment != null && oi.Order.Shipment.Location != null
                                ? oi.Order.Shipment.Location.Address 
                                : null, // Include the address from the location
                            PostalCode = oi.Order != null && oi.Order.Shipment != null && oi.Order.Shipment.Location != null
                                ? oi.Order.Shipment.Location.PostalCode 
                                : null, // Include the postal code from the location
                            Region =
                                oi.Order != null && oi.Order.Shipment != null && oi.Order.Shipment.Location != null && oi.Order.Shipment.Location.LocationRegion != null
                                ? oi.Order.Shipment.Location.LocationRegion.Region 
                                : null, // Include the region from the location
                            CountryName =
                                oi.Order != null && oi.Order.Shipment != null && oi.Order.Shipment.Location != null && oi.Order.Shipment.Location.LocationRegion != null && oi.Order.Shipment.Location.LocationRegion.LocationCountry != null
                                ? oi.Order.Shipment.Location.LocationRegion.LocationCountry.CountryName 
                                : null, // Include the country from the location
                        })
                        .ToListAsync();

        if (orderItems == null || orderItems.Count == 0)
        {
            return new NotFoundObjectResult("No order items found for the logged-in user.");
        }
        return new OkObjectResult(orderItems);
    }

    public async Task<ActionResult<OrderItem>> GetOrderItem(int id, string userId)
    {
        var orderItems = await _context.OrderItems
                        .Include(oi => oi.Product) // Include related product data
                        .Include(oi => oi.Order) // Include related order 
                        // .ThenInclude(o => o.CouponUserList) // Include related coupon user list data
                        // .ThenInclude(cur => cur.Coupon) // Include related coupon data
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
                            TotalFinalAmount = oi.Order != null ? oi.Order.TotalFinalAmount : 0,
                            OrderNumber = oi.Order != null ? oi.Order.OrderNumber : null,
                            Status = oi.Order != null ? oi.Order.Status : null,
                            ProductName = oi.Product != null ? oi.Product.Name : null,
                            ImageUrl = oi.Product != null ? oi.Product.ImageUrl : null,
                            DiscountAmount = oi.Product != null && oi.Product.Coupon != null ? oi.Product.Coupon.DiscountAmount : 0, // Include the discount from the coupon
                            CouponDiscount =
                                oi.Order != null && oi.Order.CouponUserList != null && oi.Order.CouponUserList.Coupon != null
                                ? oi.Order.CouponUserList.Coupon.DiscountAmount
                                : 0,
                            OrderDate = oi.Order != null ? oi.Order.OrderDate : (DateTime?)null,
                            CompletedDate = oi.Order != null ? oi.Order.CompletedDate : (DateTime?)null,
                            CancelledDate = oi.Order != null ? oi.Order.CancelledDate : (DateTime?)null,
                            ShipmentDate = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.ShipmentDate 
                                : (DateTime?)null,
                            ExpectedDate = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.ExpectedDate 
                                : (DateTime?)null,
                            DelayedDateFrom = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.DelayedDateFrom 
                                : (DateTime?)null,
                            ArrivedDate = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.ArrivedDate 
                                : (DateTime?)null,
                            TrackingNumber = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.TrackingNumber 
                                : null, // Include the tracking number from the shipment
                            ShippingCost = oi.Order != null && oi.Order.Shipment != null
                                ? oi.Order.Shipment.ShippingCost 
                                : 0, // Include the shipping cost from the shipment
                            Address = oi.Order != null && oi.Order.Shipment != null && oi.Order.Shipment.Location != null
                                ? oi.Order.Shipment.Location.Address 
                                : null, // Include the address from the location
                            PostalCode = oi.Order != null && oi.Order.Shipment != null && oi.Order.Shipment.Location != null
                                ? oi.Order.Shipment.Location.PostalCode 
                                : null, // Include the postal code from the location
                            Region =
                                oi.Order != null && oi.Order.Shipment != null && oi.Order.Shipment.Location != null && oi.Order.Shipment.Location.LocationRegion != null
                                ? oi.Order.Shipment.Location.LocationRegion.Region 
                                : null, // Include the region from the location
                            CountryName =
                                oi.Order != null && oi.Order.Shipment != null && oi.Order.Shipment.Location != null && oi.Order.Shipment.Location.LocationRegion != null && oi.Order.Shipment.Location.LocationRegion.LocationCountry != null
                                ? oi.Order.Shipment.Location.LocationRegion.LocationCountry.CountryName 
                                : null, // Include the country from the location
                        })
                        .ToListAsync();

        if (orderItems == null)
        {
            return new NotFoundObjectResult("Order with ID {id} not found or does not belong to the user..");
        }
        return new OkObjectResult(orderItems);
    }

    public async Task<ActionResult<OrderItem>> CreateOrderItem(OrderItem orderItem, string userId)
    {
        if (orderItem == null || string.IsNullOrEmpty(orderItem.ProductId.ToString()) || string.IsNullOrEmpty(orderItem.OrderId.ToString()))
        {
            return new BadRequestObjectResult("Invalid Create OrderItem data. Some fields cannot be null or missing.");
        }

        _context.OrderItems.Add(orderItem);
        await _context.SaveChangesAsync();
        return new CreatedAtActionResult(nameof(GetOrderItem), nameof(OrderItem), new { id = orderItem.Id }, orderItem);
    }

    public async Task<IActionResult> UpdateOrderItem(int id, OrderItem orderItem, string userId)
    {
        if (orderItem == null || string.IsNullOrEmpty(orderItem.ProductId.ToString()) || string.IsNullOrEmpty(orderItem.OrderId.ToString()))
        {
            return new BadRequestObjectResult("Invalid Order Item data OR mismatch Product ID / Order ID.");
        }

        _context.Entry(orderItem).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return new NoContentResult();
    }

    public async Task<IActionResult> DeleteOrderItem(int id, string userId)
    {        
        var orderItem = await _context.OrderItems.FindAsync(id);
        if (orderItem == null)
        {
            return new NotFoundResult();
        }
        _context.OrderItems.Remove(orderItem);
        await _context.SaveChangesAsync();
        return new NoContentResult();
    }
}