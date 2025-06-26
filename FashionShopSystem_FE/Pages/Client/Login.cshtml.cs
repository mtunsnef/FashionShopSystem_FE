using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace FashionShopSystem_FE.Pages.Client
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public LoginModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập địa chỉ email")]
            [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostEmailLoginAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                
                var loginData = new
                {
                    Email = Input.Email,
                    Password = Input.Password
                };

                var json = JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://localhost:7242/api/Authenticate/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    // Parse the response and extract token
                    var loginResult = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    
                    // Extract token from response
                    if (loginResult.TryGetProperty("token", out var tokenElement))
                    {
                        var token = tokenElement.GetString();
                        ViewData["token"] = token;
                    }
                    
                    // Extract user info if available
                    if (loginResult.TryGetProperty("user", out var userElement))
                    {
                        ViewData["UserInfo"] = userElement.GetRawText();
                    }
                    
                    TempData["SuccessMessage"] = "Đăng nhập thành công!";
                    ViewData["LoginSuccess"] = true;
                    return Page();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    
                    // Try to parse error message from API response
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
                        if (errorResponse.TryGetProperty("message", out var messageElement))
                        {
                            TempData["ErrorMessage"] = messageElement.GetString() ?? "Email hoặc mật khẩu không đúng";
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Email hoặc mật khẩu không đúng";
                        }
                    }
                    catch
                    {
                        TempData["ErrorMessage"] = "Email hoặc mật khẩu không đúng";
                    }
                    
                    return Page();
                }
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] = "Không thể kết nối đến máy chủ. Vui lòng thử lại sau.";
                return Page();
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại.";
                return Page();
            }
        }
    }
} 