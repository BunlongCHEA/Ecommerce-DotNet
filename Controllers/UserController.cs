using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc; // For ControllerBase, HttpPost, etc.
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;  // For Encoding.UTF8.GetBytes, especially when working with JWT tokens.
using ECommerceAPI.Models;
using System.Net;
using ECommerceAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        // private readonly UserManager<ApplicationUser> _userManager;
        // private readonly IConfiguration _configuration;
        // private readonly IEmailSender _emailSender; // Assuming you have an email sender service

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
        {
            var users = await _context.ApplicationUsers.ToListAsync();

            return Ok(users);
        }

        // GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetUser(int id)
        {
            var user = await _context.ApplicationUsers
                        .Where(u => u.Id == id) // Ensure the user is of role "User"
                        .Select(u => new
                        {
                            u.Id,
                            u.UserName,
                            u.Email,
                            u.FirstName,
                            u.LastName,
                        })
                        .FirstOrDefaultAsync();

            Console.WriteLine($"User ID UserController: {id}");
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

    }
}