using ECommerceAPI.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductDto productDto)
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

            // Get IFormFile from base64 or existing file
            var imageFile = productDto.GetImageFile();

            return await _productService.CreateProduct(product, userId, imageFile);
        }

        // PUT: api/admin/product/{id}
        // Product product
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto productDto)
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

            var imageFile = productDto.GetImageFile();

            return await _productService.UpdateProduct(id, product, userId, imageFile);
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

        [HttpPost("batch")]
        public async Task<ActionResult<IEnumerable<Product>>> CreateBatchProducts([FromBody] List<ProductDto> productDtos)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated or valid.");
            }

            try
            {
                if (productDtos == null || !productDtos.Any())
                {
                    return BadRequest("Product list cannot be null or empty.");
                }

                Console.WriteLine($"Received {productDtos.Count} products for batch creation");

                // Convert each DTO's base64 image to IFormFile
                foreach (var dto in productDtos)
                {
                    if (!string.IsNullOrEmpty(dto.ImageBase64))
                    {
                        dto.ImageFile = dto.GetImageFile();
                    }
                }

                var result = await _productService.CreateBatchProducts(productDtos, userId);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Controller Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, "An error occurred while creating products. Please try again.");
            }
        }

        // POST: api/admin/product/batch
            // IEnumerable<Product> products
            // IEnumerable<ProductDto> productDtos
            // [FromForm] IEnumerable<ProductDto> productDtos
            // [ModelBinder(typeof(ProductBatchModelBinder))] List<ProductDto> productDtos
        // [HttpPost("batch")]
        // public async Task<ActionResult<IEnumerable<Product>>> CreateBatchProducts()
        // {
        //     // Find the logged-in userId
        //     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //     if (string.IsNullOrEmpty(userId))
        //     {
        //         return Unauthorized("User not authenticated or valid.");
        //     }

        //     try
        //     {
        //         var form = Request.Form;

        //         // Debug: Log all form keys
        //         Console.WriteLine("=== Form Data Debug ===");
        //         Console.WriteLine($"Form keys: {string.Join(", ", form.Keys)}");
        //         Console.WriteLine($"File keys: {string.Join(", ", form.Files.Select(f => f.Name))}");

        //         var productsJson = form["products"].FirstOrDefault();

        //         if (string.IsNullOrEmpty(productsJson))
        //         {
        //             Console.WriteLine("No products JSON found in form data");
        //             return BadRequest("Products data is required.");
        //         }

        //         Console.WriteLine($"Products JSON: {productsJson}");

        //         var productDtos = JsonSerializer.Deserialize<List<ProductDto>>(productsJson, new JsonSerializerOptions 
        //         { 
        //             PropertyNameCaseInsensitive = true 
        //         });

        //         if (productDtos == null || !productDtos.Any())
        //         {
        //             return BadRequest("Product list cannot be null or empty.");
        //         }

        //         Console.WriteLine($"Deserialized {productDtos.Count} products");

        //         // Match files with products
        //         for (int i = 0; i < productDtos.Count; i++)
        //         {
        //             var fileKey = $"files[{i}]";
        //             var matchedFile = form.Files.FirstOrDefault(f => f.Name == fileKey);
        //             if (matchedFile != null)
        //             {
        //                 productDtos[i].ImageFile = matchedFile;
        //                 Console.WriteLine($"Matched file {fileKey} to product {i}: {productDtos[i].Name} - File: {productDtos[i].ImageFile.FileName} ({productDtos[i].ImageFile.Length} bytes)");
        //             }
        //             else
        //             {
        //                 Console.WriteLine($"No file found for key {fileKey}");
        //             }
        //         }

        //         // Log each product DTO for debugging
        //         for (int i = 0; i < productDtos.Count; i++)
        //         {
        //             var dto = productDtos[i];
        //             Console.WriteLine($"Product {i}: Name={dto.Name}, Price={dto.Price}, ImageFile={dto.ImageFile?.FileName ?? "NULL"}, Size={dto.ImageFile?.Length ?? 0}");
        //         }

        //         var result = await _productService.CreateBatchProducts(productDtos, userId);
        //         return result;
        //     }
        //     catch (System.Text.Json.JsonException jsonEx)
        //     {
        //         Console.WriteLine($"JSON Deserialization Error: {jsonEx.Message}");
        //         return BadRequest($"Invalid JSON format: {jsonEx.Message}");
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Controller Exception: {ex.Message}");
        //         Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        //         return StatusCode(500, "An error occurred while creating products. Please try again.");
        //     }
        // }
    }
}