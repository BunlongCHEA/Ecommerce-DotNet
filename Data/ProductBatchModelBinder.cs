using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

public class ProductBatchModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
            throw new ArgumentNullException(nameof(bindingContext));

        var products = new List<ProductDto>();
        var form = bindingContext.HttpContext.Request.Form;

        // Get the products JSON data
        var productsJson = form["products"].FirstOrDefault();
        if (!string.IsNullOrEmpty(productsJson))
        {
            var productList = JsonSerializer.Deserialize<List<ProductDto>>(productsJson, new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            });

            if (productList != null)
            {
                // Match files with products
                for (int i = 0; i < productList.Count; i++)
                {
                    var product = productList[i];
                    
                    // Look for file with key like "files[0]", "files[1]", etc.
                    var fileKey = $"files[{i}]";
                    var matchedFile = form.Files.FirstOrDefault(f => f.Name == fileKey);
                    if (matchedFile != null)
                    {
                        product.ImageFile = matchedFile;
                        Console.WriteLine($"Matched file {fileKey} to product {i}: {product.Name}");
                    }
                    
                    products.Add(product);
                }
            }
        }

        bindingContext.Result = ModelBindingResult.Success(products);
        return Task.CompletedTask;
    }
}

public class ProductBatchModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(List<ProductDto>))
        {
            return new ProductBatchModelBinder();
        }
        return null;
    }
}