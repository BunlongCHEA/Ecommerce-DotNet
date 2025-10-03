using Microsoft.AspNetCore.Mvc;

public interface IProductService
{
    Task<ActionResult<IEnumerable<Product>>> GetProducts(
        int pageNumber = 1,
        int pageSize = 10,
        string searchQuery = "",
        string categoryIds = "",
        string subCategoryIds = "",
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int? eventId = null
    );
    Task<ActionResult<Product>> GetProduct(int id);
    Task<ActionResult<Product>> CreateProduct(Product product, string userId, IFormFile imageFile);
    Task<IActionResult> UpdateProduct(int id, Product product, string userId, IFormFile imageFile);
    Task<IActionResult> DeleteProduct(int id, string userId);
    Task<ActionResult<IEnumerable<Product>>> CreateBatchProducts(IEnumerable<Product> products, string userId);
}