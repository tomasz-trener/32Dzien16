using Microsoft.AspNetCore.Mvc;
using P06Shop.Shared;
using P06Shop.Shared.Models;
using P06Shop.Shared.Services;

namespace P05Shop.API.Controllers;

/// <summary>
/// Kontroler REST dla produktów – pełny CRUD. Adres: api/products
/// Kontroler NIE zawiera logiki – przyjmuje żądanie, woła serwis, tłumaczy wynik na kod HTTP.
///
/// | Operacja | Czasownik | Adres           | Sukces | Błąd        |
/// |----------|-----------|-----------------|--------|-------------|
/// | lista    | GET       | api/products    | 200    | 500         |
/// | jeden    | GET       | api/products/5  | 200    | 404         |
/// | dodaj    | POST      | api/products    | 201    | 400         |
/// | zmień    | PUT       | api/products/5  | 200    | 400 / 404   |
/// | usuń     | DELETE    | api/products/5  | 200    | 400 / 404   |
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    // Kontener DI wstrzykuje implementację zarejestrowaną w Program.cs (AddScoped)
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // GET api/products
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
    {
        ServiceResponse<List<Product>> response = await _productService.GetProductsAsync();

        return response.Success
            ? Ok(response)                                                   // 200
            : StatusCode(StatusCodes.Status500InternalServerError, response); // 500
    }

    // GET api/products/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int id)
    {
        ServiceResponse<Product> response = await _productService.GetProductAsync(id);

        return response.Success
            ? Ok(response)          // 200
            : NotFound(response);   // 404 – nadal z ServiceResponse w treści, żeby klient znał powód
    }

    // POST api/products   (JSON produktu w treści; [ApiController] sam sprawdza atrybuty walidacji → 400)
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<Product>>> CreateProduct([FromBody] Product product)
    {
        ServiceResponse<Product> response = await _productService.CreateProductAsync(product);

        if (!response.Success)
        {
            return BadRequest(response);   // 400
        }

        // 201 Created + nagłówek Location: api/products/{nowe Id}
        return CreatedAtAction(nameof(GetProduct), new { id = response.Data!.Id }, response);
    }

    // PUT api/products/5
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ServiceResponse<Product>>> UpdateProduct(int id, [FromBody] Product product)
    {
        if (id != product.Id)
        {
            return BadRequest(ServiceResponse<Product>.Fail("Id w adresie i w treści żądania muszą być takie same."));
        }

        ServiceResponse<Product> response = await _productService.UpdateProductAsync(product);

        if (response.Success)
        {
            return Ok(response);   // 200
        }

        return response.Message.StartsWith("Nie znaleziono")
            ? NotFound(response)     // 404
            : BadRequest(response);  // 400
    }

    // DELETE api/products/5
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ServiceResponse<bool>>> DeleteProduct(int id)
    {
        ServiceResponse<bool> response = await _productService.DeleteProductAsync(id);

        if (response.Success)
        {
            return Ok(response);   // 200
        }

        return response.Message.StartsWith("Nie znaleziono")
            ? NotFound(response)     // 404
            : BadRequest(response);  // 400 – np. reguła "cena > 1000"
    }
}
