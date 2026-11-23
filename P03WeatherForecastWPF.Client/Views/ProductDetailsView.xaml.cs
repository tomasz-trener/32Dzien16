using System.Windows;
using P03WeatherForecastWPF.Client.ViewModels;

namespace P03WeatherForecastWPF.Client.Views;

public partial class ProductDetailsView : Window
{
    public ProductDetailsView(ProductDetailsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        // Okno modalne: ViewModel prosi o zamknięcie, okno ustawia DialogResult (wynik ShowDialog)
        viewModel.CloseRequested += result =>
        {
            DialogResult = result;
            Close();
        };
    }
}
