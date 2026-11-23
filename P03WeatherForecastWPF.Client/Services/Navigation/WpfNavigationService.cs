using Microsoft.Extensions.DependencyInjection;
using P03WeatherForecastWPF.Client.ViewModels;
using P03WeatherForecastWPF.Client.Views;
using P06Shop.Shared.Models;

namespace P03WeatherForecastWPF.Client.Services.Navigation;

/// <summary>
/// Implementacja nawigacji dla WPF: prosi kontener DI o okno (ze wstrzykniętym ViewModelem)
/// i je pokazuje. Tylko ta klasa zna konkretne typy okien.
/// </summary>
public class WpfNavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public WpfNavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void ShowProductsWindow()
    {
        // Transient: za każdym razem nowe okno z nowym ProductsViewModel
        ShopProductsView window = _serviceProvider.GetRequiredService<ShopProductsView>();
        window.Show();
    }

    public bool ShowProductDetailsDialog(Product product)
    {
        // ViewModel dostaje produkt "z ręki" (parametr), a wszystkie inne zależności konstruktora
        // (gdy dojdą, np. serwis mowy w dniu 15) dołoży kontener – ActivatorUtilities łączy oba źródła
        var viewModel = ActivatorUtilities.CreateInstance<ProductDetailsViewModel>(_serviceProvider, product);
        var window = ActivatorUtilities.CreateInstance<ProductDetailsView>(_serviceProvider, viewModel);

        // ShowDialog blokuje okno nadrzędne do czasu zamknięcia; zwraca DialogResult
        return window.ShowDialog() == true;
    }
}
