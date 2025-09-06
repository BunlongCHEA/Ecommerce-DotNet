using ECommerceAPI.Data;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure for production with proper forwarded headers handling
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                              Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Make API to support CORS for Vue.js app for FrontEnd && SignalR for Chat && Swagger
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // policy.AllowAnyOrigin()
        policy.WithOrigins(
                "https://ecommercevue.bunlong.site", // frontend domain
                "https://www.ecommercevue.bunlong.site", // www variant
                "https://ecommerceapi.bunlong.site", // backend domain
                "https://www.ecommerceapi.bunlong.site", // www variant
                "http://localhost:5173", // Development
                "https://localhost:5173" // Development HTTPS
            )
            // .AllowAnyHeader()
            .AllowAnyMethod()
            .WithHeaders("x-signalr-user-agent", "content-type", "authorization", "accept", "user-agent")
            .AllowCredentials()
            .SetIsOriginAllowedToAllowWildcardSubdomains(); // This helps with SignalR;
    });
});

// Add Controllers
builder.Services.AddControllers();

// Add Email services
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();

// Add Entity Framework Core with SQL Server Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MongoDB Configuration
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// MongoDB Service
builder.Services.AddSingleton<IMongoDbService, MongoDbService>();

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Configure Identity options here
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add Services and its implementations
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IChatImageService, ChatImageService>();

// Add SignalR for real-time communication
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = !builder.Environment.IsProduction();; // Enable detailed errors for debugging
});

builder.Services.AddScoped<IChatService, ChatService>();

// Configure Role-based Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});

// Configure JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
{
    throw new InvalidOperationException("JWT Key / Issuer settings are not configured properly or missing.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chathub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ECommerce API",
        Version = "v1",
        Description = "API for ECommerce application"
    });

    // Configure Swagger for HTTPS in production
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Use forwarded headers (important for GKE with Google-managed SSL)
app.UseForwardedHeaders();

// Add CSP middleware - ADD THIS BEFORE OTHER MIDDLEWARE
app.Use(async (context, next) =>
{
    // Set CSP header to allow WebSocket connections
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self' http: https: data: blob: 'unsafe-inline'; " +
        "connect-src 'self' ws: wss: http: https:; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline'");
    
    await next();
});

// Configure the HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ECommerce API V1");
    c.RoutePrefix = "swagger";
    
    // Configure Swagger UI for HTTPS in production
    if (app.Environment.IsProduction())
    {
        c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    }
});

app.MapHealthChecks("/health");

app.UseRouting();
app.UseCors("AllowFrontend");

// The load balancer handles HTTPS termination
// if (!app.Environment.IsProduction())
// {
//     app.UseHttpsRedirection();
// }

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();