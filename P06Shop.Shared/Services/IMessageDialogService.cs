namespace P06Shop.Shared.Services;

/// <summary>
/// ViewModel chce pokazać komunikat albo zapytać "czy na pewno?", ale nie zna
/// MessageBox z WPF ani DisplayAlert z MAUI. Dlatego mówi przez interfejs,
/// a każda platforma ma własną implementację (WpfMessageDialogService dziś, MAUI w dniu 18).
/// Interfejs leży w Shared, bo ViewModele też będą współdzielone.
/// </summary>
public interface IMessageDialogService
{
    void ShowMessage(string title, string message);

    bool Confirm(string title, string question);
}
