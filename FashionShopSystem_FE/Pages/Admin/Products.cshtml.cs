using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace FashionShopSystem_FE.Pages.Admin
{
    public class ProductsModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ProductsModel(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public List<Category> Categories { get; set; } = new List<Category>();
        public Product? EditingProduct { get; set; }

        public async Task OnGetAsync(int? productId = null)
        {
            try
            {
                // Load categories from API
                var categoriesResponse = await _httpClient.GetAsync("https://localhost:7242/api/Category/Category");
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    var categoriesJson = await categoriesResponse.Content.ReadAsStringAsync();
                    Categories = JsonSerializer.Deserialize<List<Category>>(categoriesJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Category>();
                }

                // Load specific product if productId is provided (for editing)
                if (productId.HasValue)
                {
                    var token = Request.Cookies["token"] ?? ""; // Get token from cookies if stored there
                    var productResponse = await _httpClient.GetAsync($"https://localhost:7242/api/Product/Product/{productId.Value}");
                    if (productResponse.IsSuccessStatusCode)
                    {
                        var productJson = await productResponse.Content.ReadAsStringAsync();
                        EditingProduct = JsonSerializer.Deserialize<Product>(productJson, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                Console.WriteLine($"Error loading data: {ex.Message}");
                Categories = new List<Category>();
                EditingProduct = null;
            }
        }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
} 