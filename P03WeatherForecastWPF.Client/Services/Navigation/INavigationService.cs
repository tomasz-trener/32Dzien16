using P06Shop.Shared.Models;

namespace P03WeatherForecastWPF.Client.Services.Navigation;

/// <summary>
/// ViewModel nie może tworzyć okien (nie zna WPF), ale może poprosić o ich otwarcie.
/// Interfejs opisuje "co", implementacja w WPF wie "jak".
/// </summary>
public interface INavigationService
{
    void ShowProductsWindow();

    /// <summary>
    /// Otwiera okno szczegółów jako dialog modalny i edytuje przekazany obiekt.
    /// Zwraca true, gdy użytkownik kliknął "Zapisz" i dane są poprawne.
    /// </summary>
    bool ShowProductDetailsDialog(Product product);
}
