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

    public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(
        string userId,
        string userRole,
        int pageNumber = 1,
        int pageSize = 10,
        string searchQuery = null,
        string searchType = null,
        string[] statuses = null
    )
    {
        // Start with the base query
        var query = _context.OrderItems
                    .Include(oi => oi.Product) // Include related product data
                    .Include(oi => oi.Order) // Include related order data
                    .Include(oi => oi.Order.Shipment) // Include related shipment data
                    .Include(oi => oi.Product.Coupon) // Include related coupon data for product
                    .Include(oi => oi.Order.CouponUserList) // Include related coupon user list data
                    .Where(oi => oi.Order != null && oi.Order.Payment != null)
                    .AsQueryable();

        // Filter by user role --  && oi.Order.Payment.UserId == int.Parse(userId)
        if (userRole != "Admin")
        {
            query = query.Where(oi => oi.Order.Payment.UserId == int.Parse(userId)); // Ensure the order item belongs to the logged-in user
        }

        // Apply search query if provided
        if (!string.IsNullOrEmpty(searchQuery))
        {
            if (searchType == "orderNumber")
            {
                query = query.Where(o => o.Order != null && o.Order.OrderNumber != null && o.Order.OrderNumber.Contains(searchQuery));
            }
            else
            {
                query = query.Where(o => o.Order != null
                                         && o.Order.Shipment != null
                                         && o.Order.Shipment.TrackingNumber != null
                                         && o.Order.Shipment.TrackingNumber.Contains(searchQuery));
            }
        }

        if (statuses != null && statuses.Length > 0)
        {
            query = query.Where(o => o.Order != null && statuses.Contains(o.Order.Status));
        }

        // Get distinct OrderIds to ensure we're grouping by orders
        var distinctOrderIds = await query.Select(oi => oi.OrderId).Distinct().ToListAsync();

        // Get total count of distinct orders (not order items)
        var totalCount = distinctOrderIds.Count;

        // Apply pagination to the distinct order IDs
        var paginatedOrderIds = distinctOrderIds
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

        var orderItems = await query
                        .Where(oi => paginatedOrderIds.Contains(oi.OrderId)) // Filter order items by paginated order IDs
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
                            ProductDiscountPercentage = oi.Product != null && oi.Product.Coupon != null
                                ? oi.Product.Coupon.DiscountPercentage 
                                : 0, // Include the discount from the coupon
                            CouponDiscountPercentage =
                                oi.Order != null && oi.Order.CouponUserList != null && oi.Order.CouponUserList.Coupon != null
                                ? oi.Order.CouponUserList.Coupon.DiscountPercentage
                                : 0,
                            ShipmentId = oi.Order != null && oi.Order.Shipment != null ? oi.Order.Shipment.Id : 0, // Include Shipment ID if available
                            OrderDate = oi.Order != null ? oi.Order.OrderDate : (DateTime?)null,
                            PackedDate = oi.Order != null ? oi.Order.PackedDate : (DateTime?)null,
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
            string message = userRole == "Admin" 
                ? "No order items found in the system." 
                : "No order items found for the logged-in user.";
            return new NotFoundObjectResult(message);
        }
        
        return new OkObjectResult(new
        {
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
            OrderItems = orderItems
        });
    }

    public async Task<ActionResult<OrderItem>> GetOrderItem(int id, string userId, string userRole)
    {
        var query = _context.OrderItems
                    .Include(oi => oi.Product) // Include related product data
                    .Include(oi => oi.Order) // Include related order 
                    .Include(oi => oi.Order.Shipment) // Include related shipment data
                    .Include(oi => oi.Product.Coupon) // Include related coupon data for product
                    .Include(oi => oi.Order.CouponUserList) // Include related coupon user list data
                        .ThenInclude(cur => cur.Coupon) // Include related coupon data
                    .Include(oi => oi.Order.Shipment.Location) // Include location data
                        .ThenInclude(l => l.LocationRegion) // Include region data
                            .ThenInclude(lr => lr.LocationCountry) // Include country data
                    .Where(oi => oi.Order != null && oi.Order.Payment != null && oi.OrderId == id) // Base filter for valid orders with specific OrderId
                    .AsQueryable();

        // Apply user-specific filter only if the user is not an Admin
        if (userRole != "Admin")
        {
            query = query.Where(oi => oi.Order.Payment.UserId == int.Parse(userId)); // Ensure the order item belongs to the logged-in user
        }

        var orderItems = await query
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
                            ProductDiscountPercentage = oi.Product != null && oi.Product.Coupon != null
                                ? oi.Product.Coupon.DiscountPercentage 
                                : 0, // Include the discount from the coupon
                            CouponDiscountPercentage =
                                oi.Order != null && oi.Order.CouponUserList != null && oi.Order.CouponUserList.Coupon != null
                                ? oi.Order.CouponUserList.Coupon.DiscountPercentage
                                : 0,
                            OrderDate = oi.Order != null ? oi.Order.OrderDate : (DateTime?)null,
                            PackedDate = oi.Order != null ? oi.Order.PackedDate : (DateTime?)null,
                            CompletedDate = oi.Order != null ? oi.Order.CompletedDate : (DateTime?)null,
                            CancelledDate = oi.Order != null ? oi.Order.CancelledDate : (DateTime?)null,
                            ShipmentId = oi.Order != null && oi.Order.Shipment != null ? oi.Order.Shipment.Id : 0, // Include Shipment ID if available
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
            string message = userRole == "Admin" 
                ? $"Order with ID {id} not found in the system."
                : $"Order with ID {id} not found or does not belong to the user.";
            return new NotFoundObjectResult(message);
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