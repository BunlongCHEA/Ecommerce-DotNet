using ECommerceAPI.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly ICloudStorageService _cloudStorageService;
    private readonly ILogger<ProductService> _logger;
    public string _defaultImageUrl = "https://png.pngtree.com/png-vector/20221125/ourmid/pngtree-no-image-available-icon-flatvector-illustration-thumbnail-graphic-illustration-vector-png-image_40966590.jpg"; // Default image URL if none provided

    public ProductService(ApplicationDbContext context, ICloudStorageService cloudStorageService, ILogger<ProductService> logger)
    {
        _context = context;
        _cloudStorageService = cloudStorageService;
        _logger = logger;
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
                    .ThenInclude(c => c.Event) // Include related event data through coupon
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
            query = query.Where(p => p.Coupon != null && p.Coupon.EventId == eventId.Value
            );
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
                        EventId = p.Coupon != null ? p.Coupon.EventId : null, // Get EventId from Coupon relationship
                        EventName = p.Coupon != null && p.Coupon.Event != null ? p.Coupon.Event.Name : "No Event", // Get event name from Coupon → Event relationship
                        CouponCode = p.Coupon != null ? p.Coupon.Code : "No Coupon", // Get the coupon code
                        DiscountPercentage = p.Coupon != null ? p.Coupon.DiscountPercentage : 0, // Get the discount percentage
                        StoreName = p.Store != null ? p.Store.Name : "Unknown", // Get the store name
                        CategoryName = p.Category != null ? p.Category.Name : "Unknown", // Get the category name
                        SubCategoryName = p.SubCategory != null ? p.SubCategory.Name : "Unknown", // Get the subcategory name

                        // Additional coupon/event information
                        CouponIsActive = p.Coupon != null ? p.Coupon.IsActive : false,
                        CouponStartDate = p.Coupon != null ? p.Coupon.StartDate : null,
                        CouponDurationValidity = p.Coupon != null ? p.Coupon.DurationValidity : 0,
                        EventStartDate = p.Coupon != null && p.Coupon.Event != null ? p.Coupon.Event.StartDate : null,
                        EventEndDate = p.Coupon != null && p.Coupon.Event != null ? p.Coupon.Event.EndDate : null
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
                                        .ThenInclude(c => c.Event) // Include related event data through coupon
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
                                        EventId = p.Coupon != null ? p.Coupon.EventId : null, // Get EventId from Coupon relationship
                                        EventName = p.Coupon != null && p.Coupon.Event != null ? p.Coupon.Event.Name : "No Event", // Get event name from Coupon → Event relationship
                                        CouponCode = p.Coupon != null ? p.Coupon.Code : "No Coupon", // Get the coupon code
                                        DiscountPercentage = p.Coupon != null ? p.Coupon.DiscountPercentage : 0, // Get the discount percentage
                                        StoreName = p.Store != null ? p.Store.Name : "Unknown", // Get the store name
                                        CategoryName = p.Category != null ? p.Category.Name : "Unknown", // Get the category name
                                        SubCategoryName = p.SubCategory != null ? p.SubCategory.Name : "Unknown", // Get the subcategory name
                                        
                                        // Additional information
                                        CouponIsActive = p.Coupon != null ? p.Coupon.IsActive : false,
                                        CouponStartDate = p.Coupon != null ? p.Coupon.StartDate : null,
                                        CouponDurationValidity = p.Coupon != null ? p.Coupon.DurationValidity : 0,
                                        EventStartDate = p.Coupon != null && p.Coupon.Event != null ? p.Coupon.Event.StartDate : null,
                                        EventEndDate = p.Coupon != null && p.Coupon.Event != null ? p.Coupon.Event.EndDate : null,
                                        EventDescription = p.Coupon != null && p.Coupon.Event != null ? p.Coupon.Event.Description : null,
                                        EventImageUrl = p.Coupon != null && p.Coupon.Event != null ? p.Coupon.Event.ImageUrl : null
                                    })
                                    .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return new NotFoundObjectResult($"Product with ID {id} not found.");
        }
        return new OkObjectResult(product);
    }

    public async Task<ActionResult<Product>> CreateProduct(Product product, string userId, IFormFile imageFile)
    {
        if (product == null || string.IsNullOrEmpty(product.Name))
        {
            return new BadRequestObjectResult("Invalid Create product data. Some fields cannot be null or missing.");
        }
        if (product.Price <= 0)
        {
            return new BadRequestObjectResult("Invalid product price. Price must be greater than 0.");
        }

        // Handle image upload if provided
        if (imageFile != null && imageFile.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            try
            {
                var imageUrl = await _cloudStorageService.UploadImageAsync(imageFile, fileName);
                product.ImageUrl = imageUrl;
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Failed to upload image: {ex.Message}");
            }
        }

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return new CreatedAtActionResult(nameof(GetProduct), nameof(Product), new { id = product.Id }, product);
    }

    // PUT: api/product/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product, string userId, IFormFile imageFile)
    {
        if (id != product.Id || string.IsNullOrEmpty(product.Name))
        {
            return new BadRequestObjectResult("Invalid Product column data OR mismatch Product ID");
        }
        if (product.Price <= 0)
        {
            return new BadRequestObjectResult("Invalid product price. Price must be greater than 0.");
        }

        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
        {
            return new NotFoundObjectResult($"Product with ID {id} not found.");
        }

        // Handle image upload if new image is provided
        if (imageFile != null && imageFile.Length > 0)
        {
            // Delete old image if exists
            if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
            {
                var oldFileName = Path.GetFileName(new Uri(existingProduct.ImageUrl).LocalPath);
                await _cloudStorageService.DeleteImageAsync(oldFileName);
            }

            // Upload new image
            var fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            try
            {
                var imageUrl = await _cloudStorageService.UploadImageAsync(imageFile, fileName);
                product.ImageUrl = imageUrl;
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Failed to upload image: {ex.Message}");
            }
        }
        else
        {
            // Keep existing image URL if no new image provided
            product.ImageUrl = existingProduct.ImageUrl;
        }

        // _context.Entry(product).State = EntityState.Modified;
        _context.Entry(existingProduct).CurrentValues.SetValues(product);
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

        // Delete associated image from cloud storage
        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            var fileName = Path.GetFileName(new Uri(product.ImageUrl).LocalPath);
            await _cloudStorageService.DeleteImageAsync(fileName);
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
    
    public async Task<ActionResult<IEnumerable<Product>>> CreateBatchProducts(IEnumerable<ProductDto> productDtos, string userId)
    {
        if (productDtos == null || !productDtos.Any())
        {
            return new BadRequestObjectResult("Product list cannot be null, empty or mismatch.");
        }

        var products = new List<Product>();
        var imageUploadTasks = new Dictionary<string, IFormFile>();

        // Validate all products and prepare image uploads
        int index = 0;
        foreach (var productDto in productDtos)
        {
            if (string.IsNullOrEmpty(productDto.Name))
            {
                return new BadRequestObjectResult("Product name cannot be empty");
            }

            if (productDto.Price <= 0)
            {
                return new BadRequestObjectResult($"Invalid price for product '{productDto.Name}'. Price must be greater than 0.");
            }

            // If there's an image file, add it to upload tasks
            if (productDto.ImageFile != null)
            {
                imageUploadTasks[$"product_{index}"] = productDto.ImageFile;
                _logger.LogInformation("Prepared image for upload for product '{ProductName}'", productDto.Name);
            }

            // Convert DTO to Product entity
            var product = new Product
            {
                Name = productDto.Name,
                Price = productDto.Price,
                Description = productDto.Description,
                ImageUrl = string.Empty, // Will be set after image upload
                CategoryId = productDto.CategoryId,
                SubCategoryId = productDto.SubCategoryId,
                CouponId = productDto.CouponId,
                StoreId = productDto.StoreId,
                // EventId = productDto.EventId
            };

            products.Add(product);
            index++;
        }

        // Upload all images in batch
        Dictionary<string, string> uploadResults = new Dictionary<string, string>();
        if (imageUploadTasks.Any())
        {
            try
            {
                uploadResults = await _cloudStorageService.UploadBatchImagesAsync(imageUploadTasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload batch images");
                return new BadRequestObjectResult("Failed to upload one or more images. Please try again.");
            }
        }

        // Set image URLs for products
        for (int i = 0; i < products.Count; i++)
        {
            var key = $"product_{i}";
            if (uploadResults.ContainsKey(key) && !string.IsNullOrEmpty(uploadResults[key]))
            {
                products[i].ImageUrl = uploadResults[key];
                Console.WriteLine($"Uploaded image for product '{products[i].Name}': {products[i].ImageUrl}");
            }
            else
            {
                // Set default image or handle missing image
                products[i].ImageUrl = _defaultImageUrl;
            }

            // Set audit fields from BaseEntity
            // products[i].CreatedBy = userId;
            // products[i].CreatedDate = DateTime.UtcNow;
            // products[i].ModifiedBy = userId;
            // products[i].ModifiedDate = DateTime.UtcNow;
        }

        try
        {
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Successfully created {Count} products by user {UserId}", products.Count, userId);
            return new OkObjectResult(products);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save batch products to database");
            return new BadRequestObjectResult("Failed to save products to database. Please try again.");
        }
    }

    // public async Task<ActionResult<IEnumerable<Product>>> CreateBatchProducts(IEnumerable<Product> products, string userId)
    // {
    //     if (products == null || !products.Any())
    //     {
    //         return new BadRequestObjectResult("Product list cannot be null or empty.");
    //     }

    //     // Validate all products
    //     foreach (var product in products)
    //     {
    //         if (string.IsNullOrEmpty(product.Name))
    //         {
    //             return new BadRequestObjectResult("Product name cannot be empty");
    //         }

    //         if (product.Price <= 0)
    //         {
    //             return new BadRequestObjectResult($"Invalid price for product '{product.Name}'. Price must be greater than 0.");
    //         }
    //     }

    //     await _context.Products.AddRangeAsync(products);
    //     await _context.SaveChangesAsync();
    //     return new OkObjectResult(products);
    // }
}