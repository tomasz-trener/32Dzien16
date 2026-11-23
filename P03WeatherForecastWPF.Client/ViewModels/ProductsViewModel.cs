using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using P03WeatherForecastWPF.Client.Services.Navigation;
using P06Shop.Shared;
using P06Shop.Shared.Models;
using P06Shop.Shared.Services;

namespace P03WeatherForecastWPF.Client.ViewModels;

/// <summary>
/// ViewModel okna z produktami – pełny CRUD przez IProductService.
/// Nie wie, że pod spodem jest HttpClient i nasze API (ProductHttpService z P06Shop.Shared),
/// nie zna też okien (INavigationService) ani MessageBox (IMessageDialogService).
/// </summary>
public partial class ProductsViewModel : ObservableObject
{
    private readonly IProductService _productService;
    private readonly INavigationService _navigationService;
    private readonly IMessageDialogService _messageDialogService;

    public ProductsViewModel(
        IProductService productService,
        INavigationService navigationService,
        IMessageDialogService messageDialogService)
    {
        _productService = productService;
        _navigationService = navigationService;
        _messageDialogService = messageDialogService;
    }

    public ObservableCollection<Product> Products { get; } = [];

    /// <summary>Zaznaczony wiersz. Zmiana odświeża CanExecute komend Edytuj i Usuń.</summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditProductCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
    public partial Product? SelectedProduct { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadProductsCommand))]
    [NotifyCanExecuteChangedFor(nameof(NewProductCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditProductCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteProductCommand))]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    public partial string? ErrorMessage { get; set; }

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "Kliknij „Odśwież”, aby pobrać produkty z API.";

    // ----- READ -----

    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task LoadProductsAsync()
    {
        await RunAsync(async () =>
        {
            ServiceResponse<List<Product>> response = await _productService.GetProductsAsync();

            Products.Clear();

            if (!response.Success || response.Data is null)
            {
                ErrorMessage = response.Message;
                return;
            }

            foreach (Product product in response.Data)
            {
                Products.Add(product);
            }

            StatusMessage = $"Pobrano {Products.Count} produktów.";
        });
    }

    // ----- CREATE -----

    [RelayCommand(CanExecute = nameof(IsNotBusy))]
    private async Task NewProductAsync()
    {
        var product = new Product();

        // Dialog edytuje obiekt "w miejscu"; false = Anuluj
        if (!_navigationService.ShowProductDetailsDialog(product))
        {
            return;
        }

        await RunAsync(async () =>
        {
            ServiceResponse<Product> response = await _productService.CreateProductAsync(product);

            if (!response.Success)
            {
                _messageDialogService.ShowMessage("Nie udało się dodać produktu", response.Message);
                return;
            }

            int newId = response.Data!.Id;
            await LoadProductsAsync();
            SelectedProduct = Products.FirstOrDefault(p => p.Id == newId);
            StatusMessage = $"{response.Message} (Id {newId})";
        });
    }

    // ----- UPDATE -----

    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private async Task EditProductAsync()
    {
        if (SelectedProduct is null)
        {
            return;
        }

        // Edytujemy KOPIĘ: gdy użytkownik kliknie Anuluj, lista zostaje bez zmian
        Product copy = Clone(SelectedProduct);

        if (!_navigationService.ShowProductDetailsDialog(copy))
        {
            return;
        }

        await RunAsync(async () =>
        {
            ServiceResponse<Product> response = await _productService.UpdateProductAsync(copy);

            if (!response.Success)
            {
                _messageDialogService.ShowMessage("Nie udało się zapisać zmian", response.Message);
                return;
            }

            await LoadProductsAsync();
            SelectedProduct = Products.FirstOrDefault(p => p.Id == copy.Id);
            StatusMessage = response.Message;
        });
    }

    // ----- DELETE -----

    [RelayCommand(CanExecute = nameof(CanEditOrDelete))]
    private async Task DeleteProductAsync()
    {
        if (SelectedProduct is null)
        {
            return;
        }

        if (!_messageDialogService.Confirm("Usuwanie produktu", $"Czy na pewno usunąć „{SelectedProduct.Title}”?"))
        {
            return;
        }

        await RunAsync(async () =>
        {
            ServiceResponse<bool> response = await _productService.DeleteProductAsync(SelectedProduct.Id);

            if (!response.Success)
            {
                // Tu zobaczymy regułę z API: "Nie można usunąć produktu droższego niż 1000 zł"
                _messageDialogService.ShowMessage("Nie udało się usunąć produktu", response.Message);
                return;
            }

            await LoadProductsAsync();
            StatusMessage = response.Message;
        });
    }

    // ----- pomocnicze -----

    private bool IsNotBusy() => !IsBusy;

    private bool CanEditOrDelete() => !IsBusy && SelectedProduct is not null;

    /// <summary>Wspólna obudowa operacji: IsBusy, czyszczenie błędu, obsługa wyjątków.</summary>
    private async Task RunAsync(Func<Task> operation)
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Nieoczekiwany błąd: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static Product Clone(Product source) => new()
    {
        Id = source.Id,
        Title = source.Title,
        Description = source.Description,
        Barcode = source.Barcode,
        Price = source.Price,
        ReleaseDate = source.ReleaseDate
    };
}
