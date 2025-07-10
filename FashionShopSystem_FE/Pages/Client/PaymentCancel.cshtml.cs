using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FashionShopSystem_FE.Pages.Client
{
    public class PaymentCancelModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int OrderId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Amount { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Code { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Cancel { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string OrderCode { get; set; }

        public PayOSResponseCancelModel ResponseData { get; set; }

        public IActionResult OnGet()
        {
            ResponseData = new PayOSResponseCancelModel
            {
                Amount = Amount,
                IsSuccess = false,
                CreatedAt = DateTime.Now,
                Status = Status ?? "Đã hủy"
            };

            return Page();
        }
    }

    public class PayOSResponseCancelModel
    {
        public int Amount { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string ItemName => "Thanh toán";
        public string PaymentMethod => "PayOS";

        public string Status { get; set; } = "Đã hủy";
    }
}
