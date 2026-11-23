using Microsoft.EntityFrameworkCore;
using P05Shop.API.Data;
using P06Shop.Shared;
using P06Shop.Shared.Models;
using P06Shop.Shared.Services;

namespace P05Shop.API.Services;

/// <summary>
/// Implementacja IProductService po stronie SERWERA – dane z bazy przez EF Core.
/// Wczoraj była tu statyczna lista; kontroler nie zauważył zmiany, bo zna tylko interfejs.
/// Serwis jest Scoped, DataContext też: jeden kontekst na jedno żądanie HTTP.
/// </summary>
public class ProductService : IProductService
{
    /// <summary>Reguła biznesowa: produktów droższych niż ta kwota nie wolno usuwać.</summary>
    public const decimal MaxDeletablePrice = 1000m;

    private readonly DataContext _dataContext;

    public ProductService(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
    {
        try
        {
            // AsNoTracking: tylko czytamy, EF nie musi śledzić zmian (szybciej, mniej pamięci)
            List<Product> products = await _dataContext.Products
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .ToListAsync();

            return ServiceResponse<List<Product>>.Ok(products, $"Pobrano {products.Count} produktów.");
        }
        catch (Exception ex)
        {
            return ServiceResponse<List<Product>>.Fail($"Błąd podczas pobierania produktów: {ex.Message}");
        }
    }

    public async Task<ServiceResponse<Product>> GetProductAsync(int id)
    {
        Product? product = await _dataContext.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);

        return product is null
            ? ServiceResponse<Product>.Fail($"Nie znaleziono produktu o Id {id}.")
            : ServiceResponse<Product>.Ok(product);
    }

    public async Task<ServiceResponse<Product>> CreateProductAsync(Product product)
    {
        try
        {
            product.Id = 0;   // Id nadaje baza (IDENTITY) – ignorujemy to, co przysłał klient

            _dataContext.Products.Add(product);
            await _dataContext.SaveChangesAsync();   // tu wykonuje się INSERT i product.Id dostaje wartość

            return ServiceResponse<Product>.Ok(product, "Produkt został dodany.");
        }
        catch (DbUpdateException ex)
        {
            return ServiceResponse<Product>.Fail($"Błąd zapisu do bazy: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task<ServiceResponse<Product>> UpdateProductAsync(Product product)
    {
        try
        {
            Product? existing = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == product.Id);

            if (existing is null)
            {
                return ServiceResponse<Product>.Fail($"Nie znaleziono produktu o Id {product.Id}.");
            }

            // Przepisujemy pola na śledzony obiekt – EF wygeneruje UPDATE tylko dla zmienionych kolumn
            existing.Title = product.Title;
            existing.Description = product.Description;
            existing.Barcode = product.Barcode;
            existing.Price = product.Price;
            existing.ReleaseDate = product.ReleaseDate;

            await _dataContext.SaveChangesAsync();

            return ServiceResponse<Product>.Ok(existing, "Produkt został zaktualizowany.");
        }
        catch (DbUpdateException ex)
        {
            return ServiceResponse<Product>.Fail($"Błąd zapisu do bazy: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task<ServiceResponse<bool>> DeleteProductAsync(int id)
    {
        Product? product = await _dataContext.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return ServiceResponse<bool>.Fail($"Nie znaleziono produktu o Id {id}.");
        }

        // Reguła biznesowa należy do serwisu, nie do kontrolera ani klienta
        if (product.Price > MaxDeletablePrice)
        {
            return ServiceResponse<bool>.Fail($"Nie można usunąć produktu droższego niż {MaxDeletablePrice:0} zł (cena: {product.Price:0.00} zł).");
        }

        _dataContext.Products.Remove(product);
        await _dataContext.SaveChangesAsync();

        return ServiceResponse<bool>.Ok(true, "Produkt został usunięty.");
    }
}
