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

        public ShopModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync(int? categoryid, string? keyword, string? sort)
        {
            var client = _httpClientFactory.CreateClient();

            // Tạo URL linh hoạt theo filter
            var queryParams = new List<string>();
            if (categoryid.HasValue)
                queryParams.Add($"categoryid={categoryid}");
            if (!string.IsNullOrWhiteSpace(keyword))
                queryParams.Add($"keyword={Uri.EscapeDataString(keyword)}");
            if (!string.IsNullOrWhiteSpace(sort))
                queryParams.Add($"sort={Uri.EscapeDataString(sort)}");

            var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var productUrl = $"https://localhost:7242/api/Product/Product{query}";

            try
            {
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
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi: " + ex.Message);
            }
        }
    }
}
