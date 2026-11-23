using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using P03WeatherForecastWPF.Client.Services.Speech;
using P06Shop.Shared.Models;

namespace P03WeatherForecastWPF.Client.ViewModels;

/// <summary>
/// ViewModel okna szczegółów (nowy produkt albo edycja istniejącego).
/// Nie woła API – tylko zbiera dane i sprawdza je atrybutami z klasy Product.
/// O zapis prosi ProductsViewModel po zamknięciu dialogu.
/// Od dnia 15: przycisk "Dyktuj" wpisuje opis z mikrofonu (Azure Speech przez ISpeechService).
/// </summary>
public partial class ProductDetailsViewModel : ObservableObject
{
    private readonly ISpeechService _speechService;

    // Product przychodzi "z ręki" (ActivatorUtilities w WpfNavigationService), ISpeechService – z kontenera
    public ProductDetailsViewModel(Product product, ISpeechService speechService)
    {
        Product = product;
        _speechService = speechService;
    }

    /// <summary>Edytowany obiekt. Kontrolki bindują się do jego właściwości (Product.Title itd.).</summary>
    public Product Product { get; }

    public string WindowTitle => Product.Id == 0 ? "Nowy produkt" : $"Edycja produktu #{Product.Id}";

    [ObservableProperty]
    public partial string? ValidationErrors { get; set; }

    /// <summary>True podczas nasłuchiwania mikrofonu.</summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(DictateDescriptionCommand))]
    public partial bool IsListening { get; set; }

    /// <summary>
    /// Okno subskrybuje to zdarzenie i zamyka się z podanym wynikiem (true = zapisz).
    /// ViewModel nie zna klasy Window, więc nie może zamknąć okna sam.
    /// </summary>
    public event Action<bool>? CloseRequested;

    [RelayCommand]
    private void Save()
    {
        // Ta sama walidacja, którą wykona API ([Required], [MaxLength], [Range]) – ale bez wysyłania żądania
        var context = new ValidationContext(Product);
        var results = new List<ValidationResult>();

        if (!Validator.TryValidateObject(Product, context, results, validateAllProperties: true))
        {
            ValidationErrors = string.Join(Environment.NewLine, results.Select(r => r.ErrorMessage));
            return;
        }

        ValidationErrors = null;
        CloseRequested?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel() => CloseRequested?.Invoke(false);

    /// <summary>Rozpoznaje mowę i wstawia tekst do opisu produktu.</summary>
    [RelayCommand(CanExecute = nameof(CanDictate))]
    private async Task DictateDescriptionAsync()
    {
        IsListening = true;
        ValidationErrors = null;

        try
        {
            string text = await _speechService.RecognizeOnceAsync();

            if (string.IsNullOrWhiteSpace(text))
            {
                ValidationErrors = "Nie rozpoznano mowy – spróbuj jeszcze raz.";
                return;
            }

            // Product to zwykła klasa (bez INotifyPropertyChanged), więc po zmianie w kodzie
            // musimy sami powiedzieć widokowi, że Product.Description ma nową wartość
            Product.Description = string.IsNullOrWhiteSpace(Product.Description) ? text : $"{Product.Description} {text}";
            OnPropertyChanged(nameof(Product));
        }
        catch (Exception ex)
        {
            ValidationErrors = ex.Message;
        }
        finally
        {
            IsListening = false;
        }
    }

    private bool CanDictate() => !IsListening;
}
