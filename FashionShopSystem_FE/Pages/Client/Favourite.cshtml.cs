using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace FashionShopSystem_FE.Pages.Client;
public class FavouriteModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public FavouriteModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public List<FavouriteResponseDto> Favourites { get; set; }

    public async Task OnGetAsync()
    {
    }


    public async Task<IActionResult> OnPostRemoveAsync(int id)
    {
        var client = _clientFactory.CreateClient();
        var response = await client.DeleteAsync($"https://localhost:7242/api/Favourite/DeleteFavourite/{id}");

        return RedirectToPage();
    }
}

// DTOs
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
}

public class FavouriteResponseDto
{
    public int FavoriteId { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public ProductDto Product { get; set; }
}

public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Price { get; set; }
    public string ImageUrl { get; set; }
}
