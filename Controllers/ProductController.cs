using ECommerceAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchQuery = "",
            [FromQuery] string categoryIds = "",
            [FromQuery] string subCategoryIds = "",
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null
        )
        {
            return await _productService.GetProducts(
                pageNumber, 
                pageSize, 
                searchQuery, 
                categoryIds, 
                subCategoryIds, 
                minPrice, 
                maxPrice
            );
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            return await _productService.GetProduct(id);
        }

        // // POST: api/product
        // [HttpPost]
        // public async Task<ActionResult<Product>> CreateProduct(Product product)
        // {
        //     if (product == null || string.IsNullOrEmpty(product.Name))
        //     {
        //         return BadRequest("Invalid Create product data. Some fields cannot be null or missing.");
        //     }
        //     if (product.Price <= 0)
        //     {
        //         return BadRequest("Invalid product price. Price must be greater than 0.");
        //     }

        //     _context.Products.Add(product);
        //     await _context.SaveChangesAsync();
        //     return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        // }

        // // PUT: api/product/{id}
        // [HttpPut("{id}")]
        // public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
        // {
        //     if (id != product.Id || string.IsNullOrEmpty(product.Name))
        //     {
        //         return BadRequest("Invalid Product column data OR mismatch Product ID");
        //     }
        //     if (product.Price <= 0)
        //     {
        //         return BadRequest("Invalid product price. Price must be greater than 0.");
        //     }

        //     _context.Entry(product).State = EntityState.Modified;
        //     await _context.SaveChangesAsync();
        //     return Content("Product updated successfully");
        // }

        // // DELETE: api/product/{id}
        // [HttpDelete("{id}")]
        // public async Task<ActionResult<Product>> DeleteProduct(int id)
        // {
        //     var product = await _context.Products.FindAsync(id);
        //     if (product == null)
        //     {
        //         return NotFound();
        //     }
        //     _context.Products.Remove(product);
        //     await _context.SaveChangesAsync();
        //     return NoContent();
        // }
    }
}