using System.Windows;
using P03WeatherForecastWPF.Client.ViewModels;

namespace P03WeatherForecastWPF.Client.Views;

public partial class ShopProductsView : Window
{
    public ShopProductsView(ProductsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Po otwarciu okna od razu pobieramy produkty (zamiast czekać na kliknięcie)
        Loaded += async (_, _) => await viewModel.LoadProductsCommand.ExecuteAsync(null);
    }
}
