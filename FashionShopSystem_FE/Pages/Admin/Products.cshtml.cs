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

        public async Task OnGetAsync()
        {
            try
            {
                // Load categories from API
                var response = await _httpClient.GetAsync("https://localhost:7242/api/Category/Category");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Categories = JsonSerializer.Deserialize<List<Category>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Category>();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't break the page
                Console.WriteLine($"Error loading categories: {ex.Message}");
                Categories = new List<Category>();
            }
        }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
} 