using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc; // For ControllerBase, HttpPost, etc.
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;  // For Encoding.UTF8.GetBytes, especially when working with JWT tokens.
using ECommerceAPI.Models;
using System.Net;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailSender _emailSender; // Assuming you have an email sender service

        public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Validate the model
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Email))
            {
                return BadRequest(new { message = "Username, Password, and Email are required." });
            }
            
            // Check if username is already taken
            var existingUserName = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserName != null)
            {
                return BadRequest(new { message = "Username is already taken." });
            }

            // Check if email is already taken
            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                return BadRequest(new { message = "Email is already taken." });
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                Role = model.Role // Default role is User
            };

            var result = await _userManager.CreateAsync(user, model.Password); 

            if (result.Succeeded)
            {
                return Ok(new { message = $"User - {model.UserName} - Created" });
            }
            return BadRequest(result.Errors);
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Validate the model
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                return BadRequest(new { message = "Username and Password are required." });

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(user);

            // // Set userId in a cookie
            // HttpContext.Response.Cookies.Append("userId", user.Id.ToString(), new CookieOptions
            // {
            //     HttpOnly = true,
            //     Secure = false, // Set to true if using HTTPS
            //     SameSite = SameSiteMode.Strict,
            //     Expires = DateTimeOffset.UtcNow.AddDays(1) // Cookie valid for 1 day
            // });

            return Ok(new { token, userId = user.Id });
        }

        // POST: api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.ClientUrl))
                return BadRequest("Email and Client URL are required.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Ok(new { message = "If that email is registered, a reset link has been sent."});
            }

            // Generate password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodeToken = WebUtility.UrlEncode(token); // Encode the token for URL safety

            // Create the reset link, and pass a client URL to user to reset password by go to the link
            var resetLink = $"{model.ClientUrl}/reset-password?token={encodeToken}&email={model.Email}";
            var htmlTemplate = System.IO.File.ReadAllText("Templates/ResetPasswordEmail.html");
            var htmlMessage = htmlTemplate.Replace("{RESET_LINK}", resetLink); // Replace placeholder with actual link

            // <a href='{resetLink}'>Reset Password</a>
            await _emailSender.SendEmailAsync(
                model.Email,
                "Reset Password",
                htmlMessage
            );

            // Send email with the reset link (you need to implement this method in your email service)
            return Ok(new { message = "Password reset token has been sent" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest("Email is required.");

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Invalid request.");

            if (string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
                return BadRequest("Token and new password are required.");

            var result = await _userManager.ResetPasswordAsync(user, WebUtility.UrlDecode(model.Token), model.NewPassword);
            if (result.Succeeded)
                return Ok(new { message = "Password reset successfully!" });

            return BadRequest(result.Errors);
        }
        
        private string GenerateJwtToken(ApplicationUser user)
        {
            // Read JWT settings from appsettings.json
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtExpireMin = Convert.ToInt32(_configuration["Jwt:ExpirationInMinutes"]);

            // Null check
            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
            {
                throw new InvalidOperationException("JWT Key / Issuer settings are not configured properly or missing.");
            }

            if (!int.TryParse(_configuration["Jwt:ExpirationInMinutes"], out jwtExpireMin))
            {
                throw new InvalidOperationException("JWT ExpirationInMinutes setting is not configured properly or missing.");
            }

            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Id.ToString()))
            {
                throw new InvalidOperationException("UserName / UserID cannot be null when generating token.");
            }

            if (string.IsNullOrEmpty(user.Role))
            {
                throw new InvalidOperationException("User Role cannot be null when generating token.");
            }

            // Converts secret key into a byte[], wraps it in a SymmetricSecurityKey, and creates a signing credential with HMAC SHA-256
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Claims are key/value pairs embedded in the token payload. You can add more like roles, email, etc
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // Your internal user ID
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  // Unique token ID
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),  // Subject (username)
                new Claim(ClaimTypes.Role, user.Role) // User role (e.g., Admin, User)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,  // Who issued it (issuer)
                audience: null, // Optional: Who can read it (audience)
                claims: claims, // Claims (user data)
                expires: DateTime.Now.AddMinutes(jwtExpireMin), // Expiration
                signingCredentials: credentials // Signing credentials (key + algorithm)
            );
            
            // Converts the token object into the final string (Authorization: Bearer <token>)
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}