using Microsoft.AspNetCore.Mvc;
using P06Shop.Shared.Models;
using P06Shop.Shared.Services;

namespace P11ShopWebAppMVCApi.Client.Controllers;

/// <summary>
/// CRUD produktów przez NASZE API (P05) – struktura akcji jak w wygenerowanym kontrolerze z P10,
/// ale zamiast ShopContext (baza) wstrzykujemy IProductService (HTTP). Kontroler nie wie, że pod spodem jest HTTP.
/// Każda metoda serwisu zwraca ServiceResponse&lt;T&gt;: sprawdzamy Success, do widoku przekazujemy Data,
/// a Message pokazujemy użytkownikowi (ModelState.AddModelError → asp-validation-summary, albo ViewBag).
/// </summary>
public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET: Products
    public async Task<IActionResult> Index()
    {
        var response = await _productService.GetProductsAsync();

        if (!response.Success || response.Data is null)
        {
            // API wyłączone / błąd – pokazujemy komunikat na liście zamiast strony z wyjątkiem
            ViewBag.ErrorMessage = response.Message;
            return View(new List<Product>());
        }

        return View(response.Data);
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var response = await _productService.GetProductAsync(id.Value);

        if (!response.Success || response.Data is null)
        {
            // API odpowiada 404 z ServiceResponse ("Nie znaleziono produktu o Id ...") – oddajemy 404
            return NotFound(response.Message);
        }

        return View(response.Data);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Products/Create
    // [Bind] – lista dozwolonych pól (ochrona przed overpostingiem), jak w kontrolerze wygenerowanym w P10
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Description,Barcode,Price,ReleaseDate")] Product product)
    {
        if (ModelState.IsValid)
        {
            var response = await _productService.CreateProductAsync(product);

            if (response.Success)
            {
                return RedirectToAction(nameof(Index));   // PRG: po zapisie przekierowanie na listę
            }

            // Błąd z API pokazujemy nad formularzem (<div asp-validation-summary="ModelOnly">)
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(product);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var response = await _productService.GetProductAsync(id.Value);

        if (!response.Success || response.Data is null)
        {
            return NotFound(response.Message);
        }

        return View(response.Data);
    }

    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Barcode,Price,ReleaseDate")] Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var response = await _productService.UpdateProductAsync(product);

            if (response.Success)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(product);
    }

    // GET: Products/Delete/5 – strona z potwierdzeniem (nigdy nie usuwamy przez GET)
    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var response = await _productService.GetProductAsync(id.Value);

        if (!response.Success || response.Data is null)
        {
            return NotFound(response.Message);
        }

        return View(response.Data);
    }

    // POST: Products/Delete/5
    // Dwie metody Delete(int) miałyby tę samą sygnaturę, więc ta nazywa się DeleteConfirmed,
    // a [ActionName("Delete")] zachowuje adres /Products/Delete/5.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var response = await _productService.DeleteProductAsync(id);

        if (response.Success)
        {
            return RedirectToAction(nameof(Index));
        }

        // API odmówiło (reguła biznesowa: cena > 1000 zł) – zostajemy na stronie Delete i pokazujemy powód
        var productResponse = await _productService.GetProductAsync(id);

        if (!productResponse.Success || productResponse.Data is null)
        {
            return NotFound(productResponse.Message);
        }

        ModelState.AddModelError(string.Empty, response.Message);
        return View(productResponse.Data);
    }
}
