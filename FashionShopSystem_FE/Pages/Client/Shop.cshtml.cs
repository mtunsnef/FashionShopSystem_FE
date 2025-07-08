using FashionShopSystem_FE.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace FashionShopSystem_FE.Pages.Client
{
    public class ShopModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        [BindProperty(SupportsGet = true)]
        public List<ProductResponseDto> Products { get; set; } = new();

        public List<CategoryResponseDto> Categories { get; set; } = new();
        public List<string> brands { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? Price { get; set; } // Đây là filter price dạng 1–6

        [BindProperty(SupportsGet = true)]
        public decimal? minPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? maxPrice { get; set; }



        public ShopModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync(int? categoryid, string? keyword, string? sort, string? brand, decimal? price)
        {
            var client = _httpClientFactory.CreateClient();

            // Gán giá theo range từ Price
            if (Price.HasValue)
            {
                switch (Price.Value)
                {
                    case 1:
                        minPrice = 0; maxPrice = 50000;
                        break;
                    case 2:
                        minPrice = 50000; maxPrice = 200000;
                        break;
                    case 3:
                        minPrice = 200000; maxPrice = 300000;
                        break;
                    case 4:
                        minPrice = 300000; maxPrice = 400000;
                        break;
                    case 5:
                        minPrice = 400000; maxPrice = 500000;
                        break;
                    case 6:
                        minPrice = 500000; maxPrice = null;
                        break;
                }
            }

            // Build query URL
            var queryParams = new List<string>();
            if (categoryid.HasValue)
                queryParams.Add($"categoryid={categoryid}");
            if (!string.IsNullOrWhiteSpace(keyword))
                queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
            if (!string.IsNullOrWhiteSpace(sort))
                queryParams.Add($"sort={Uri.EscapeDataString(sort)}");
            if (!string.IsNullOrWhiteSpace(brand))
                queryParams.Add($"brand={Uri.EscapeDataString(brand)}");
            if (Price.HasValue)
                queryParams.Add($"price={Price.Value}");
            if (minPrice.HasValue)
                queryParams.Add($"minPrice={minPrice.Value}");
            if (maxPrice.HasValue)
                queryParams.Add($"maxPrice={maxPrice.Value}");

            var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var productUrl = $"https://localhost:7242/api/Product/Product{query}";

            try
            {
                // Get product list
                var response = await client.GetAsync(productUrl);
                if (response.IsSuccessStatusCode)
                {
                    var products = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>();
                    if (products != null)
                        Products = products;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không thể tải sản phẩm. Vui lòng thử lại sau.");
                }

                // Get categories
                var categoryResponse = await client.GetAsync("https://localhost:7242/api/Category/Category");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categories = await categoryResponse.Content.ReadFromJsonAsync<List<CategoryResponseDto>>();
                    if (categories != null)
                        Categories = categories;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không thể tải danh mục. Vui lòng thử lại sau.");
                }

                // Get brand list
                var brandResponse = await client.GetAsync("https://localhost:7242/api/Product/Brand");
                if (brandResponse.IsSuccessStatusCode)
                {
                    var brandlist = await brandResponse.Content.ReadFromJsonAsync<List<string>>();
                    if (brandlist != null)
                        brands = brandlist;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không thể tải thương hiệu. Vui lòng thử lại sau.");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }
}