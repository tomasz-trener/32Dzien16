using Microsoft.AspNetCore.Mvc.RazorPages;
using P09ShopWebApp.Client.Data;
using P09ShopWebApp.Client.Models;

namespace P09ShopWebApp.Client.Pages;

/// <summary>
/// Ręczna lista produktów: ShopContext przychodzi przez DI (zarejestrowany w Program.cs),
/// nigdzie nie piszemy new ShopContext().
/// </summary>
public class ProductsListModel : PageModel
{
    private readonly ShopContext _context;

    public List<Product> Products { get; set; } = new();

    public ProductsListModel(ShopContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
        // EF Core wysyła SELECT ... FROM Products (widać w konsoli: "Executed DbCommand")
        Products = _context.Products.OrderBy(p => p.Id).ToList();
    }
}
