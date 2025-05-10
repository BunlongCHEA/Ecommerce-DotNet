using ECommerceAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Products
                                        .Include(p => p.Category) // Include related category data
                                        .Include(p => p.SubCategory) // Include related subcategory data
                                        .Include(p => p.Coupon) // Include related coupon data
                                        .Include(p => p.Store) // Include related store data
                                        .Select(p => new {
                                            p.Id,
                                            p.Name,
                                            p.Description,
                                            p.Price,
                                            p.ImageUrl,
                                            p.CategoryId,
                                            p.SubCategoryId,
                                            p.CouponId,
                                            p.StoreId,
                                            CouponCode = p.Coupon != null ? p.Coupon.Code : "No Coupon", // Get the coupon code
                                            DiscountAmount = p.Coupon != null ? p.Coupon.DiscountAmount : 0, // Get the discount amount
                                            StoreName = p.Store != null ? p.Store.Name : "Unknown", // Get the store name
                                            CategoryName = p.Category != null ? p.Category.Name : "Unknown", // Get the category name
                                            SubCategoryName = p.SubCategory != null ? p.SubCategory.Name : "Unknown", // Get the subcategory name
                                        })
                                        .ToListAsync();
            return Ok(products);
        }

        // GET: api/product/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.Category) // Include related category data
                                        .Include(p => p.SubCategory) // Include related subcategory data
                                        .Include(p => p.Coupon) // Include related coupon data
                                        .Include(p => p.Store) // Include related store data
                                        .Select(p => new {
                                            p.Id,
                                            p.Name,
                                            p.Description,
                                            p.Price,
                                            p.ImageUrl,
                                            p.CategoryId,
                                            p.SubCategoryId,
                                            p.CouponId,
                                            p.StoreId,
                                            CouponCode = p.Coupon != null ? p.Coupon.Code : "No Coupon", // Get the coupon code
                                            DiscountAmount = p.Coupon != null ? p.Coupon.DiscountAmount : 0, // Get the discount amount
                                            StoreName = p.Store != null ? p.Store.Name : "Unknown", // Get the store name
                                            CategoryName = p.Category != null ? p.Category.Name : "Unknown", // Get the category name
                                            SubCategoryName = p.SubCategory != null ? p.SubCategory.Name : "Unknown", // Get the subcategory name
                                        })
                                        .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        // POST: api/product
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            if (product == null || string.IsNullOrEmpty(product.Name))
            {
                return BadRequest("Invalid Create product data. Some fields cannot be null or missing.");
            }
            if ( product.Price <= 0 ) 
            {
                return BadRequest("Invalid product price. Price must be greater than 0.");
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/product/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
        {
            if (id != product.Id || string.IsNullOrEmpty(product.Name))
            {
                return BadRequest("Invalid Product column data OR mismatch Product ID");
            }
            if ( product.Price <= 0 ) 
            {
                return BadRequest("Invalid product price. Price must be greater than 0.");
            }

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/product/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}