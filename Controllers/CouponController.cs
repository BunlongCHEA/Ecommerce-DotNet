using ECommerceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CouponController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/coupon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Coupon>>> GetCoupons([FromQuery] string? code = null)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var coupon = await _context.Coupons
                            .Where(c => c.Code == code)
                            .Select(c => new
                            {
                                c.Id,
                                c.Code,
                                c.Type,
                                c.DiscountAmount,
                                c.Desription,
                                c.IsActive,
                                c.StartDate,
                                c.DurationValidity,
                            })
                            .FirstOrDefaultAsync();
                if (coupon == null)
                {
                    return NotFound($"Coupon with code {code} not found or expired.");
                }
                return Ok(coupon);
            }

            var coupons = await _context.Coupons.ToListAsync();
            return Ok(coupons);
        }

        // GET: api/coupon/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Coupon>> GetCoupon(int id, [FromQuery] string? code = null)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var couponDetails = await _context.Coupons
                                    .Where(c => c.Code == code || c.Id == id)
                                    .Select(c => new
                                    {
                                        c.Id,
                                        c.Code,
                                        c.Type,
                                        c.DiscountAmount,
                                        c.Desription,
                                        c.IsActive,
                                        c.StartDate,
                                        c.DurationValidity,
                                    })
                                    .FirstOrDefaultAsync();
                if (couponDetails == null)
                {
                    return NotFound($"Coupon with code {code} not found or expired.");
                }
                return Ok(couponDetails);
            }

            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return Ok(coupon);
        }

        // POST: api/coupon
        [HttpPost]
        public async Task<ActionResult<Coupon>> CreateCoupon(Coupon coupon)
        {
            if (coupon == null || string.IsNullOrEmpty(coupon.Code))
            {
                return BadRequest("Invalid Create coupon data. Some fields cannot be null or missing.");
            }

            _context.Coupons.Add(coupon);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCoupon), new { id = coupon.Id }, coupon);
        }

        // PUT: api/coupon/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Coupon>> UpdateCoupon(int id, Coupon coupon)
        {
            if (id != coupon.Id || string.IsNullOrEmpty(coupon.Code))
            {
                return BadRequest("Invalid Coupon column data OR mismatch Coupon ID");
            }

            _context.Entry(coupon).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/coupon/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Coupon>> DeleteCoupon(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            if (coupon == null)
            {
                return NotFound();
            }
            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}