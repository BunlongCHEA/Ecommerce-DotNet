using System.Security.Claims;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponUserListController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CouponUserListController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/couponuserlist
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CouponUserList>>> GetCouponUserLists([FromQuery] int? couponId = null, [FromQuery] int? userId = null)
        {
            if (!string.IsNullOrEmpty(userId.ToString()) && !string.IsNullOrEmpty(couponId.ToString()))
            {
                var couponUserListQuery = await _context.CouponUserLists
                                    .Include(c => c.Coupon)
                                    .Include(c => c.ApplicationUser)
                                    .Where(c => c.UserId == userId && c.CouponId == couponId)
                                    .Select(c => new
                                    {
                                        c.Id,
                                        c.CouponId,
                                        c.UserId,
                                        CouponCode = c.Coupon != null ? c.Coupon.Code : null,
                                        CouponType = c.Coupon != null ? c.Coupon.Type : null,
                                        CouponDiscountAmount = c.Coupon != null ? c.Coupon.DiscountAmount : 0,
                                        CouponIsActive = c.Coupon != null ? c.Coupon.IsActive : false,
                                        CouponStartDate = c.Coupon != null ? c.Coupon.StartDate : null,
                                        CouponDescription = c.Coupon != null ? c.Coupon.Desription : null,
                                        CouponDurationValidity = c.Coupon != null ? c.Coupon.DurationValidity : 0,
                                        c.IsUsed,
                                        c.UsedDate,
                                        c.ExpiryDate
                                    })
                                    .ToListAsync();

                if (couponUserListQuery == null)
                {
                    return NotFound($"CouponUserList with CouponId {couponId} and UserId {userId} not found.");
                }
                return Ok(couponUserListQuery); 
            }

            // Find the logged-in userId
            var userLoginId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"User Login ID: {userLoginId}");
            if (string.IsNullOrEmpty(userLoginId))
            {
                return Unauthorized("User not authenticated or valid.");
            }
            // foreach (var claim in User.Claims)
            // {
            //     Console.WriteLine($"{claim.Type} : {claim.Value}");
            // }
            
            var couponUserLists = await _context.CouponUserLists
                                    .Include(c => c.Coupon)
                                    .Include(c => c.ApplicationUser)
                                    .Where(c => c.UserId == int.Parse(userLoginId))
                                    .Select(c => new
                                    {
                                        c.Id,
                                        c.CouponId,
                                        c.UserId,
                                        CouponCode = c.Coupon != null ? c.Coupon.Code : null,
                                        CouponType = c.Coupon != null ? c.Coupon.Type : null,
                                        CouponDiscountAmount = c.Coupon != null ? c.Coupon.DiscountAmount : 0,
                                        CouponIsActive = c.Coupon != null ? c.Coupon.IsActive : false,
                                        CouponStartDate = c.Coupon != null ? c.Coupon.StartDate : null,
                                        CouponDescription = c.Coupon != null ? c.Coupon.Desription : null,
                                        CouponDurationValidity = c.Coupon != null ? c.Coupon.DurationValidity : 0,
                                        c.IsUsed,
                                        c.UsedDate,
                                        c.ExpiryDate
                                    })
                                    .ToListAsync();
            return Ok(couponUserLists);
        }

        // GET: api/couponuserlist/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CouponUserList>> GetCouponUserList(int id, [FromQuery] int? couponId = null, [FromQuery] int? userId = null)
        {
            if (!string.IsNullOrEmpty(userId.ToString()) && !string.IsNullOrEmpty(couponId.ToString()))
            {
                var couponUserListQuery = await _context.CouponUserLists
                                    .Include(c => c.Coupon)
                                    .Include(c => c.ApplicationUser)
                                    .Where(c => c.UserId == userId || c.CouponId == couponId || c.Id == id)
                                    .Select(c => new
                                    {
                                        c.Id,
                                        c.CouponId,
                                        c.UserId,
                                        CouponCode = c.Coupon != null ? c.Coupon.Code : null,
                                        CouponType = c.Coupon != null ? c.Coupon.Type : null,
                                        CouponDiscountAmount = c.Coupon != null ? c.Coupon.DiscountAmount : 0,
                                        CouponIsActive = c.Coupon != null ? c.Coupon.IsActive : false,
                                        CouponStartDate = c.Coupon != null ? c.Coupon.StartDate : null,
                                        CouponDescription = c.Coupon != null ? c.Coupon.Desription : null,
                                        CouponDurationValidity = c.Coupon != null ? c.Coupon.DurationValidity : 0,
                                        c.IsUsed,
                                        c.UsedDate,
                                        c.ExpiryDate
                                    })
                                    .FirstOrDefaultAsync();

                if (couponUserListQuery == null)
                {
                    return NotFound($"CouponUserList with CouponId {couponId} and UserId {userId} not found.");
                }
                return Ok(couponUserListQuery); 
            }

            // Find the logged-in userId
            var userLoginId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userLoginId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            var couponUserList = await _context.CouponUserLists
                                .Include(c => c.Coupon)
                                .Include(c => c.ApplicationUser)
                                .Where(c => c.UserId == int.Parse(userLoginId) && c.Id == id)
                                .Select(c => new
                                {
                                    c.Id,
                                    c.CouponId,
                                    c.UserId,
                                    CouponCode = c.Coupon != null ? c.Coupon.Code : null,
                                    CouponType = c.Coupon != null ? c.Coupon.Type : null,
                                    CouponDiscountAmount = c.Coupon != null ? c.Coupon.DiscountAmount : 0,
                                    CouponIsActive = c.Coupon != null ? c.Coupon.IsActive : false,
                                    CouponStartDate = c.Coupon != null ? c.Coupon.StartDate : null,
                                    CouponDescription = c.Coupon != null ? c.Coupon.Desription : null,
                                    CouponDurationValidity = c.Coupon != null ? c.Coupon.DurationValidity : 0,
                                    c.IsUsed,
                                    c.UsedDate,
                                    c.ExpiryDate
                                })
                                .FirstOrDefaultAsync();

            if (couponUserList == null)
            {
                return NotFound();
            }
            return Ok(couponUserList);
        }

        // POST: api/couponuserlist
        [HttpPost]
        public async Task<ActionResult<CouponUserList>> CreateCouponUserList(CouponUserList couponUserList)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            if (couponUserList == null || string.IsNullOrEmpty(couponUserList.CouponId.ToString()) || string.IsNullOrEmpty(couponUserList.UserId.ToString()))
            {
                return BadRequest("Invalid Create Coupon User List data. Some fields cannot be null or missing.");
            }

            _context.CouponUserLists.Add(couponUserList);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCouponUserList), new { id = couponUserList.Id }, couponUserList);
        }

        // PUT: api/couponuserlist/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCouponUserList(int id, CouponUserList couponUserList)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            if (id != couponUserList.Id)
            {
                Console.WriteLine($"ID Mismatch: {id} != {couponUserList.Id}");
                return BadRequest("Invalid Coupon User List data OR mismatch Coupon User List ID.");
            }

            _context.Entry(couponUserList).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/couponuserlist/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCouponUserList(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }
            
            var couponUserList = await _context.CouponUserLists.FindAsync(id);
            if (couponUserList == null)
            {
                return NotFound();
            }

            _context.CouponUserLists.Remove(couponUserList);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}