using ECommerceAPI.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(
        int pageNumber = 1,
        int pageSize = 10,
        string searchQuery = "",
        string categoryIds = "",
        string subCategoryIds = "",
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int? eventId = null
    )
    {
        // Sample api request: /api/product?pageNumber=2&pageSize=100&searchQuery=shirt&categoryIds=1,3&minPrice=10&maxPrice=50
        // {
        // "totalCount": 247,
        // "pageSize": 100,
        // "currentPage": 2,
        // "totalPages": 3,
        // "products": [{All product details}]
        // }

        // Validate pageSize and set allowed values (10, 100, 200, 500)
        pageSize = pageSize switch
        {
            10 or 20 or 50 or 100 => pageSize,
            _ => 10 // Default to 10 if invalid
        };

        // Convert comma-separated IDs to lists of integers
        var categoryIdList = string.IsNullOrEmpty(categoryIds) ? new List<int>() : categoryIds.Split(',').Select(int.Parse).ToList();

        var subCategoryIdList = string.IsNullOrEmpty(subCategoryIds) ? new List<int>() : subCategoryIds.Split(',').Select(int.Parse).ToList();

        // Start with the base query
        var query = _context.Products
                .Include(p => p.Category) // Include related category data
                .Include(p => p.SubCategory) // Include related subcategory data
                .Include(p => p.Coupon) // Include related coupon data
                .Include(p => p.Store) // Include related store data
                .AsQueryable();

        // Apply search query if provided
        if (!string.IsNullOrEmpty(searchQuery))
        {
            // Treats uppercase and lowercase versions of the same letter as equal. Ex: "SHIRT", "shirt", and "Shirt" would all match
            query = query.Where(p => p.Name.Contains(searchQuery));
        }

        if (categoryIdList.Any())
        {
            query = query.Where(p => categoryIdList.Contains(p.CategoryId));
        }

        if (subCategoryIdList.Any())
        {
            query = query.Where(p => subCategoryIdList.Contains(p.SubCategoryId));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (eventId.HasValue)
        {
            query = query.Where(p => p.EventId == eventId.Value);
        }

        // Get the total count of products for pagination from vue3
        var totalCount = await query.CountAsync();

        // Apply pagination
        var products = await query
                    .Skip((pageNumber - 1) * pageSize)  // Skip previous pages. Ex: If pageNumber is 2 and pageSize is 10: Skip previous 10 items (start from the 11th item)
                    .Take(pageSize)                     // Take only items for current page. Ex: If pageSize is 10, it takes the next 10 items after skipping.
                    .Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Price,
                        p.ImageUrl,
                        p.CategoryId,
                        p.SubCategoryId,
                        p.CouponId,
                        p.StoreId,
                        p.EventId,
                        EventName = p.Event != null ? p.Event.Name : "No Event", // Get the event name
                        CouponCode = p.Coupon != null ? p.Coupon.Code : "No Coupon", // Get the coupon code
                        DiscountPercentage = p.Coupon != null ? p.Coupon.DiscountPercentage : 0, // Get the discount percentage
                        StoreName = p.Store != null ? p.Store.Name : "Unknown", // Get the store name
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown", // Get the category name
                        SubCategoryName = p.SubCategory != null ? p.SubCategory.Name : "Unknown", // Get the subcategory name
                    })
                    .ToListAsync();

        // Return the products along with pagination metadata
        return new OkObjectResult(
            new
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                Products = products
            }
        );
    }

    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products
                                    .Include(p => p.Category) // Include related category data
                                    .Include(p => p.SubCategory) // Include related subcategory data
                                    .Include(p => p.Coupon) // Include related coupon data
                                    .Include(p => p.Store) // Include related store data
                                    .Select(p => new
                                    {
                                        p.Id,
                                        p.Name,
                                        p.Description,
                                        p.Price,
                                        p.ImageUrl,
                                        p.CategoryId,
                                        p.SubCategoryId,
                                        p.CouponId,
                                        p.StoreId,
                                        p.EventId,
                                        EventName = p.Event != null ? p.Event.Name : "No Event", // Get the event name
                                        CouponCode = p.Coupon != null ? p.Coupon.Code : "No Coupon", // Get the coupon code
                                        DiscountPercentage = p.Coupon != null ? p.Coupon.DiscountPercentage : 0, // Get the discount percentage
                                        StoreName = p.Store != null ? p.Store.Name : "Unknown", // Get the store name
                                        CategoryName = p.Category != null ? p.Category.Name : "Unknown", // Get the category name
                                        SubCategoryName = p.SubCategory != null ? p.SubCategory.Name : "Unknown", // Get the subcategory name
                                    })
                                    .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return new NotFoundObjectResult($"Product with ID {id} not found.");
        }
        return new OkObjectResult(product);
    }

    public async Task<ActionResult<Product>> CreateProduct(Product product, string userId)
    {
        if (product == null || string.IsNullOrEmpty(product.Name))
        {
            return new BadRequestObjectResult("Invalid Create product data. Some fields cannot be null or missing.");
        }
        if (product.Price <= 0)
        {
            return new BadRequestObjectResult("Invalid product price. Price must be greater than 0.");
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return new CreatedAtActionResult(nameof(GetProduct), nameof(Product), new { id = product.Id }, product);
    }

    // PUT: api/product/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product, string userId)
    {
        if (id != product.Id || string.IsNullOrEmpty(product.Name))
        {
            return new BadRequestObjectResult("Invalid Product column data OR mismatch Product ID");
        }
        if (product.Price <= 0)
        {
            return new BadRequestObjectResult("Invalid product price. Price must be greater than 0.");
        }

        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return new ContentResult
        {
            Content = "Product updated successfully",
            ContentType = "text/plain",
            StatusCode = 200
        };
    }

    public async Task<IActionResult> DeleteProduct(int id, string userId)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return new NotFoundObjectResult($"Product with ID {id} not found.");
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return new ContentResult
        {
            Content = "Product updated successfully",
            ContentType = "text/plain",
            StatusCode = 200
        };
    }

    public async Task<ActionResult<IEnumerable<Product>>> CreateBatchProducts(IEnumerable<Product> products, string userId)
    {
        if (products == null || !products.Any())
        {
            return new BadRequestObjectResult("Product list cannot be null or empty.");
        }
        
        // Validate all products
        foreach (var product in products)
        {
            if (string.IsNullOrEmpty(product.Name))
            {
                return new BadRequestObjectResult("Product name cannot be empty");
            }

            if (product.Price <= 0)
            {
                return new BadRequestObjectResult($"Invalid price for product '{product.Name}'. Price must be greater than 0.");
            }
        }

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();
        return new OkObjectResult(products);
    }
}