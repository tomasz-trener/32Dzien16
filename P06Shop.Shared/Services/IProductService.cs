using P06Shop.Shared.Models;

namespace P06Shop.Shared.Services;

/// <summary>
/// Kontrakt serwisu produktów – pełny CRUD (Create, Read, Update, Delete).
/// Ten sam interfejs implementuje:
/// - serwer (P05Shop.API): dane z bazy przez EF Core,
/// - klient (ProductHttpService): wywołania HTTP do API.
/// ViewModel w WPF zna tylko interfejs, więc nie wie, skąd biorą się dane.
/// </summary>
public interface IProductService
{
    Task<ServiceResponse<List<Product>>> GetProductsAsync();

    Task<ServiceResponse<Product>> GetProductAsync(int id);

    Task<ServiceResponse<Product>> CreateProductAsync(Product product);

    Task<ServiceResponse<Product>> UpdateProductAsync(Product product);

    Task<ServiceResponse<bool>> DeleteProductAsync(int id);
}
