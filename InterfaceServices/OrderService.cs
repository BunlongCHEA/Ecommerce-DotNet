using ECommerceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ActionResult<IEnumerable<Order>>> GetOrders(string userId)
    {
        var orders = await _context.Orders
                    .Include(o => o.Payment) // Include Payment for each order
                    .Include(o => o.Shipment) // Include Shipment for each order
                    .Include(o => o.CouponUserList) // Include CouponUserList for each order
                    .Where(o => o.Payment != null && o.Payment.UserId == int.Parse(userId)) // Ensure the order belongs to the logged-in user & payment is not null
                    .Select(o => new
                    {
                        o.Id,
                        o.OrderNumber,
                        o.Status,
                        o.TotalFinalAmount,
                        o.TotalQuantity,
                        o.TotalAmount,
                        o.OrderDate,
                        o.PackedDate,
                        o.CancelledDate,
                        o.CompletedDate,
                        ShipmentDate = o.Shipment != null ? o.Shipment.ShipmentDate : null, // include the shipment date
                        ExpectedDate = o.Shipment != null ? o.Shipment.ExpectedDate : null, // include the expected date
                        DelayedDateFrom = o.Shipment != null ? o.Shipment.DelayedDateFrom : null, // include the delayed date
                        ArrivedDate = o.Shipment != null ? o.Shipment.ArrivedDate : null, // include the arrived date
                        o.ShipmentId,
                        o.PaymentId,
                        o.CouponUserListId,
                        CouponIsUsed = o.CouponUserList != null ? o.CouponUserList.IsUsed : false, // include if the coupon is used
                        CouponExpiryDate = o.CouponUserList != null ? o.CouponUserList.ExpiryDate : null, // include the expiry date of the coupon
                        CouponCode = o.CouponUserList != null && o.CouponUserList.Coupon != null
                            ? o.CouponUserList.Coupon.Code 
                            : null, // include the coupon code
                        CouponDiscountPercentage = o.CouponUserList != null && o.CouponUserList.Coupon != null
                            ? o.CouponUserList.Coupon.DiscountPercentage 
                            : 0, // include the discount from the coupon
                        CouponDescription = o.CouponUserList != null && o.CouponUserList.Coupon != null
                            ? o.CouponUserList.Coupon.Desription 
                            : null, // include the description from the coupon
                        AccountOrCardNumber = o.Payment != null ? o.Payment.AccountOrCardNumber : null, // include the account or card number
                        TrackingNumber = o.Shipment != null ? o.Shipment.TrackingNumber : null, // include the tracking number
                        ShippingCost = o.Shipment != null ? o.Shipment.ShippingCost : 0, // include the shipping cost
                        Address = o.Shipment != null && o.Shipment.Location != null
                            ? o.Shipment.Location.Address 
                            : null, // include the address
                        PostalCode = o.Shipment != null && o.Shipment.Location != null
                            ? o.Shipment.Location.PostalCode 
                            : null, // include the postal code
                        Region = o.Shipment != null && o.Shipment.Location != null && o.Shipment.Location.LocationRegion != null
                            ? o.Shipment.Location.LocationRegion.Region 
                            : null, // include the region
                        CountryName = o.Shipment != null && o.Shipment.Location != null && o.Shipment.Location.LocationRegion != null && o.Shipment.Location.LocationRegion.LocationCountry != null
                            ? o.Shipment.Location.LocationRegion.LocationCountry.CountryName 
                            : null, // include the country name
                    })
                    .ToListAsync();

        if (orders == null || orders.Count == 0)
        {
            return new NotFoundObjectResult("No orders found for the logged-in user.");
        }

        return new OkObjectResult(orders);
    }

    public async Task<ActionResult<Order>> GetOrder(int id, string userId)
    {
        var order = await _context.Orders
                        .Include(o => o.Payment) // Include Payment for the order
                        .Include(o => o.Shipment) // Include Shipment for the order
                        .Include(o => o.CouponUserList) // Include CouponUserList for each order
                        .Where(o => o.Id == id && o.Payment != null && o.Payment.UserId == int.Parse(userId)) // Ensure the order belongs to the logged-in user & payment is not null
                        .Select(o => new
                        {
                            o.Id,
                            o.OrderNumber,
                            o.Status,
                            o.TotalFinalAmount,
                            o.TotalQuantity,
                            o.TotalAmount,
                            o.OrderDate,
                            o.PackedDate,
                            o.CancelledDate,
                            o.CompletedDate,
                            ShipmentDate = o.Shipment != null ? o.Shipment.ShipmentDate : null, // include the shipment date
                            ExpectedDate = o.Shipment != null ? o.Shipment.ExpectedDate : null, // include the expected date
                            DelayedDateFrom = o.Shipment != null ? o.Shipment.DelayedDateFrom : null, // include the delayed date
                            ArrivedDate = o.Shipment != null ? o.Shipment.ArrivedDate : null, // include the arrived date
                            o.ShipmentId,
                            o.PaymentId,
                            o.CouponUserListId,
                            CouponIsUsed = o.CouponUserList != null ? o.CouponUserList.IsUsed : false, // include if the coupon is used
                            CouponExpiryDate = o.CouponUserList != null ? o.CouponUserList.ExpiryDate : null, // include the expiry date of the coupon
                            CouponCode = o.CouponUserList != null && o.CouponUserList.Coupon != null
                                ? o.CouponUserList.Coupon.Code 
                                : null, // include the coupon code
                            CouponDiscountPercentage = o.CouponUserList != null && o.CouponUserList.Coupon != null
                                ? o.CouponUserList.Coupon.DiscountPercentage 
                                : 0, // include the discount from the coupon
                            CouponDescription = o.CouponUserList != null && o.CouponUserList.Coupon != null
                                ? o.CouponUserList.Coupon.Desription 
                                : null, // include the description from the coupon
                            AccountOrCardNumber = o.Payment != null ? o.Payment.AccountOrCardNumber : null, // include the account or card number
                            TrackingNumber = o.Shipment != null ? o.Shipment.TrackingNumber : null, // include the tracking number
                            ShippingCost = o.Shipment != null ? o.Shipment.ShippingCost : 0, // include the shipping cost
                            Address = o.Shipment != null && o.Shipment.Location != null
                                ? o.Shipment.Location.Address 
                                : null, // include the address
                            PostalCode = o.Shipment != null && o.Shipment.Location != null
                                ? o.Shipment.Location.PostalCode 
                                : null, // include the postal code
                            Region = o.Shipment != null && o.Shipment.Location != null && o.Shipment.Location.LocationRegion != null
                                ? o.Shipment.Location.LocationRegion.Region 
                                : null, // include the region
                            CountryName = o.Shipment != null && o.Shipment.Location != null && o.Shipment.Location.LocationRegion != null && o.Shipment.Location.LocationRegion.LocationCountry != null
                                ? o.Shipment.Location.LocationRegion.LocationCountry.CountryName 
                                : null, // include the country name
                        })
                        .FirstOrDefaultAsync();

        if (order == null)
        {
            return new NotFoundObjectResult($"Order with ID {id} not found or does not belong to the user.");
        }
        return new OkObjectResult(order);
    }

    public async Task<ActionResult<Order>> CreateOrder(Order order, string userId)
    {
        if (order == null || string.IsNullOrEmpty(order.PaymentId.ToString()))
        {
            return new BadRequestObjectResult("Invalid Create Order data. Some fields cannot be null or missing.");
        }

        if (order.CouponUserListId == null || order.CouponUserListId == 0)
        {
            order.CouponUserListId = null;
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return new CreatedAtActionResult(nameof(GetOrder), nameof(Order), new { id = order.Id }, order);
    }

    public async Task<IActionResult> UpdateOrder(int id, Order order, string userId)
    {
        // if (id != order.Id)
        // {
        //     return new BadRequestObjectResult("Invalid Order data OR mismatch Payment ID.");
        // }

        _context.Entry(order).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return new NoContentResult();
    }

    public async Task<IActionResult> DeleteOrder(int id, string userId)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return new NotFoundResult();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();
        return new NoContentResult();
    }
}