using FashionShopSystem_FE.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FashionShopSystem_FE.Pages.Client
{
    public class ShopDetailsModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        [BindProperty(SupportsGet = true)]
        public int id { get; set; }
        public ProductResponseDto product { get; set; }
        public ShopDetailsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public void OnGet()
        {
            var client = _httpClientFactory.CreateClient();
            var response = client.GetAsync($"https://localhost:7242/api/Product/Product/{id}");
            if (response.Result.IsSuccessStatusCode)
            {
                var products = response.Result.Content.ReadFromJsonAsync<ProductResponseDto>().Result;
                if (products != null)
                {
                    product = products;
                }
            }
            else
            {
                // Handle error response
                ModelState.AddModelError(string.Empty, "Không thể tải sản phẩm. Vui lòng thử lại sau.");
            }
        }
    }
}
