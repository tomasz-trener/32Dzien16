using System.ComponentModel.DataAnnotations;

namespace P06Shop.Shared.Models;

/// <summary>
/// Produkt w sklepie. Klasa leży w bibliotece wspólnej, bo używa jej
/// zarówno API (serwer, jako encja bazy danych), jak i każdy klient (WPF, web, Blazor, MAUI).
///
/// Atrybuty DataAnnotations pełnią DWIE role:
/// 1) walidacja danych w klientach i w API ([ApiController] sprawdza je automatycznie),
/// 2) część z nich ([Required], [MaxLength]) EF Core tłumaczy na schemat bazy (NOT NULL, nvarchar(100)).
/// Ustawienia tylko bazodanowe (precyzja decimal, dane początkowe) są we Fluent API w DataContext.
/// </summary>
public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa jest wymagana.")]
    [MaxLength(100, ErrorMessage = "Nazwa może mieć maksymalnie 100 znaków.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Opis jest wymagany.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kod kreskowy jest wymagany.")]
    [MaxLength(12, ErrorMessage = "Kod kreskowy może mieć maksymalnie 12 znaków.")]
    public string Barcode { get; set; } = string.Empty;

    /// <summary>Cena w PLN. decimal, nie double – pieniądze wymagają dokładnej arytmetyki dziesiętnej.</summary>
    [Range(0.01, 100000, ErrorMessage = "Cena musi być w przedziale 0,01–100 000.")]
    public decimal Price { get; set; }

    public DateTime ReleaseDate { get; set; } = DateTime.Today;
}
