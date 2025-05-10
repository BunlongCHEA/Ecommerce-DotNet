using System.Security.Claims;
using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/payment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetPayments()
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Console.WriteLine($"User ID: {userId}");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }
            // foreach (var claim in User.Claims)
            // {
            //     Console.WriteLine($"{claim.Type} : {claim.Value}");
            // }

            // Return payments that belong to the logged-in user
            var payments = await _context.Payments
                            .Where(p => p.UserId == int.Parse(userId)) // Ensure the payment belongs to the logged-in user
                            .Select(p => new
                            {
                                p.Id,
                                p.PaymentMethod,
                                p.BankName,
                                p.AccountOrCardNumber,
                                p.CardExpireDate,
                                p.Balance,
                                p.UserId
                            })               
                            .ToListAsync();
            return Ok(payments);
        }

        // GET: api/payment/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetPayment(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated or valid.");
            }

            // Find the payment by id and ensure it belongs to the logged-in user
            var payment = await _context.Payments
                            .Where(p => p.Id == id && p.UserId == int.Parse(userId)) // Ensure the payment belongs to the logged-in user
                            .Select(p => new
                            {
                                p.Id,
                                p.PaymentMethod,
                                p.BankName,
                                p.AccountOrCardNumber,
                                p.CardExpireDate,
                                p.Balance,
                                p.UserId
                            })
                            .FirstOrDefaultAsync();
                            // .FirstOrDefaultAsync(p => p.Id == id && p.UserId == int.Parse(userId));
            if (payment == null)
            {
                return NotFound("Payment not found or you do not have access.");
            }
            return Ok(payment);
        }

        // POST: api/payment
        [HttpPost]
        public async Task<ActionResult<Payment>> CreatePayment(Payment payment)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || userId != payment.UserId.ToString())
            {
                return Unauthorized("User is not authenticated or valid.");
            }

            if (payment == null || string.IsNullOrEmpty(payment.UserId.ToString()) || string.IsNullOrEmpty(payment.AccountOrCardNumber))
            {
                return BadRequest("Invalid Create Payment data. Some fields cannot be null or missing.");
            }

            if (payment.CardExpireDate != null && payment.CardExpireDate < DateTime.Now)
            {
                return BadRequest("Your Card expiration date has been expired. Cannot create payment.");
            }

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }

        // PUT: api/payment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePayment(int id, Payment payment)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || userId != payment.UserId.ToString())
            {
                return Unauthorized("User is not authenticated or valid.");
            }

            if (id != payment.Id || string.IsNullOrEmpty(payment.UserId.ToString()) || string.IsNullOrEmpty(payment.AccountOrCardNumber))
            {
                return BadRequest("Invalid Payment data OR mismatch Payment ID.");
            }

            if (payment.CardExpireDate != null && payment.CardExpireDate < DateTime.Now)
            {
                return BadRequest("Your Card expiration date has been expired. Cannot update payment.");
            }

            _context.Entry(payment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/payment/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayment(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            // Find the payment by id and ensure it belongs to the logged-in user
            var payment = await _context.Payments
                            .FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            _context.Payments.Remove(payment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}