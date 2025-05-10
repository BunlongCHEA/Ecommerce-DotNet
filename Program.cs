using ECommerceAPI.Data;
using ECommerceAPI.Models;
// using EcommerceAPI.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Make API to support CORS for Vue.js app for FrontEnd.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => 
        { 
            policy.WithOrigins("http://localhost:5173") // Vite default port
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add Controllers
builder.Services.AddControllers();

// Add Email services
// builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// Add Entity Framework Core with SQL Server Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services
// builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddDefaultTokenProviders();
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Configure Identity options here
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure Role-based Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

// Configure JWT authentication
// jwtIssuer and jwtKey issue from appsettings.json
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

// Null check for jwtKey and jwtIssuer
if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
{
    throw new InvalidOperationException("JWT Key / Issuer settings are not configured properly or missing.");
}

builder.Services.AddAuthentication(options =>
{
    // Register JWT as the default way to check identity for authentication and challenges (like 401 Unauthorized).
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Setting rules for what makes a JWT valid.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,  // Only accept tokens issued by a specific server
        ValidateAudience = false,   // Optional: You can skip checking who the token is intended for (e.g., mobile, web)
        ValidateLifetime = true,   // Reject tokens that are expired (exp claim in JWT). if not set, the token will be valid forever.
        ValidateIssuerSigningKey = true,    // Check that the token is signed using your private key, and not forged.
        ValidIssuer = jwtIssuer,    // Your app's issuer name — usually something like "your-app" or a domain name.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)) // The secret key used to sign JWTs. Must match the one used when generating tokens.
    };
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowFrontend"); // Enable CORS for the specified policy

// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();