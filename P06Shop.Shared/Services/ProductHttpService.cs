using System.Net.Http.Json;
using System.Text.Json;
using P06Shop.Shared.Models;

namespace P06Shop.Shared.Services;

/// <summary>
/// Implementacja IProductService po stronie KLIENTA: woła nasze API przez HTTP.
/// Leży w bibliotece wspólnej, bo użyje jej każdy klient: WPF (dziś),
/// MVC (dzień 16), Blazor (dzień 17) i MAUI (dzień 18).
/// Adres bazowy API (https://localhost:7127/) ustawia klient przy rejestracji HttpClient.
///
/// Wszystkie metody przechodzą przez SendAsync&lt;T&gt;, który zamienia problemy
/// (brak połączenia, 404, 400, 500) na ServiceResponse z komunikatem – klient nigdy nie dostaje wyjątku.
/// </summary>
public class ProductHttpService : IProductService
{
    private const string BasePath = "api/products";

    private readonly HttpClient _httpClient;

    public ProductHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<ServiceResponse<List<Product>>> GetProductsAsync() =>
        SendAsync<List<Product>>(() => _httpClient.GetAsync(BasePath));

    public Task<ServiceResponse<Product>> GetProductAsync(int id) =>
        SendAsync<Product>(() => _httpClient.GetAsync($"{BasePath}/{id}"));

    public Task<ServiceResponse<Product>> CreateProductAsync(Product product) =>
        SendAsync<Product>(() => _httpClient.PostAsJsonAsync(BasePath, product));

    public Task<ServiceResponse<Product>> UpdateProductAsync(Product product) =>
        SendAsync<Product>(() => _httpClient.PutAsJsonAsync($"{BasePath}/{product.Id}", product));

    public Task<ServiceResponse<bool>> DeleteProductAsync(int id) =>
        SendAsync<bool>(() => _httpClient.DeleteAsync($"{BasePath}/{id}"));

    /// <summary>
    /// Wspólna obsługa każdego wywołania:
    /// - wyjątek sieciowy (API wyłączone, zły adres) → Fail("Brak połączenia z API"),
    /// - odpowiedź z ServiceResponse w treści (200, 404, 400 z naszego kontrolera) → zwracamy ją,
    /// - inna odpowiedź (np. 400 z walidacji [ApiController], 500 bez treści) → Fail z kodem i tekstem.
    /// </summary>
    private static async Task<ServiceResponse<T>> SendAsync<T>(Func<Task<HttpResponseMessage>> send)
    {
        HttpResponseMessage response;

        try
        {
            response = await send();
        }
        catch (HttpRequestException ex)
        {
            return ServiceResponse<T>.Fail($"Brak połączenia z API: {ex.Message}");
        }
        catch (TaskCanceledException)
        {
            return ServiceResponse<T>.Fail("Przekroczono czas oczekiwania na odpowiedź API.");
        }

        using (response)
        {
            string content = await response.Content.ReadAsStringAsync();

            if (content.Length > 0 && content.TrimStart().StartsWith('{'))
            {
                try
                {
                    ServiceResponse<T>? parsed = JsonSerializer.Deserialize<ServiceResponse<T>>(content, JsonOptions);

                    // Odpowiedź ma kształt ServiceResponse (jest pole "success") – zwracamy ją, nawet gdy to 404/400
                    if (parsed is not null && content.Contains("\"success\"", StringComparison.OrdinalIgnoreCase))
                    {
                        return parsed;
                    }
                }
                catch (JsonException)
                {
                    // treść nie jest naszym ServiceResponse – obsłużymy niżej
                }
            }

            if (response.IsSuccessStatusCode)
            {
                return ServiceResponse<T>.Fail("API zwróciło odpowiedź w nieoczekiwanym formacie.");
            }

            // 400 z automatycznej walidacji [ApiController] ma format ProblemDetails z polem "errors"
            string? validationErrors = TryReadValidationErrors(content);

            return ServiceResponse<T>.Fail(validationErrors
                ?? $"API odpowiedziało kodem {(int)response.StatusCode} ({response.ReasonPhrase}). {Shorten(content)}");
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>Z ProblemDetails {"errors":{"Title":["Nazwa jest wymagana."]}} robi czytelną listę komunikatów.</summary>
    private static string? TryReadValidationErrors(string content)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(content);

            if (!document.RootElement.TryGetProperty("errors", out JsonElement errors))
            {
                return null;
            }

            var messages = errors.EnumerateObject()
                .SelectMany(property => property.Value.EnumerateArray().Select(m => m.GetString()))
                .Where(m => !string.IsNullOrWhiteSpace(m));

            return "Błędy walidacji: " + string.Join(" ", messages);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string Shorten(string text) =>
        text.Length <= 300 ? text : text[..300] + "…";
}
