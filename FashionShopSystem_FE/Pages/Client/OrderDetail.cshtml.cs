using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FashionShopSystem_FE.Pages.Client
{
    public class OrderDetailModel : PageModel
    {
        public int OrderId { get; set; }

        public void OnGet(int orderId)
        {
            OrderId = orderId;
        }
    }
} 