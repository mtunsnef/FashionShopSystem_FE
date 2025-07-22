using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FashionShopSystem_FE.Pages.Client
{
    public class PaymentSuccessModel : PageModel
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
        public string Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string OrderCode { get; set; }

        public PayOSResponseSuccessModel ResponseData { get; set; }

        public IActionResult OnGet()
        {
            ResponseData = new PayOSResponseSuccessModel
            {
                Amount = Amount,
                IsSuccess = true,
                CreatedAt = DateTime.Now,
                Status = Status ?? "Thành công"
            };

            return Page();
        }
    }

    public class PayOSResponseSuccessModel
    {
        public int Amount { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string ItemName => "Thanh toán";
        public string PaymentMethod => "PayOS";

        public string Status { get; set; } = "Thành công";
    }
}
