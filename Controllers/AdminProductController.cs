using ECommerceAPI.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/admin/product")]
    [Authorize(Policy = "Admin")]
    public class AdminProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public AdminProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/admin/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchQuery = "",
            [FromQuery] string categoryIds = "",
            [FromQuery] string subCategoryIds = "",
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? eventId = null
        )
        {
            return await _productService.GetProducts(
                pageNumber,
                pageSize,
                searchQuery,
                categoryIds,
                subCategoryIds,
                minPrice,
                maxPrice,
                eventId
            );
        }

        // GET: api/admin/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            return await _productService.GetProduct(id);
        }

        // POST: api/admin/product
        // Product product
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] ProductDto productDto)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            // Convert DTO to Product entity
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                CategoryId = productDto.CategoryId,
                SubCategoryId = productDto.SubCategoryId,
                CouponId = productDto.CouponId,
                StoreId = productDto.StoreId,
                EventId = productDto.EventId
            };

            return await _productService.CreateProduct(product, userId, productDto.ImageFile);
        }

        // PUT: api/admin/product/{id}
        // Product product
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductDto productDto)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            // Convert DTO to Product entity
            var product = new Product
            {
                Id = id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                CategoryId = productDto.CategoryId,
                SubCategoryId = productDto.SubCategoryId,
                CouponId = productDto.CouponId,
                StoreId = productDto.StoreId,
                EventId = productDto.EventId
            };

            return await _productService.UpdateProduct(id, product, userId, productDto.ImageFile);
        }

        // DELETE: api/admin/product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            return await _productService.DeleteProduct(id, userId);
        }

        // POST: api/admin/product/batch
        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Product>>> CreateBatchProducts(IEnumerable<Product> products)
        {
            // Find the logged-in userId
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            var result = await _productService.CreateBatchProducts(products, userId);
            return Ok(result);
        }
    }
}