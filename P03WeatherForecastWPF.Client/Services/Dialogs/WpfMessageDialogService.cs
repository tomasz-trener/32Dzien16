using System.Windows;
using P06Shop.Shared.Services;

namespace P03WeatherForecastWPF.Client.Services.Dialogs;

/// <summary>
/// Implementacja IMessageDialogService dla WPF – zwykły MessageBox.
/// W MAUI (dzień 18) ten sam interfejs zaimplementuje DisplayAlertAsync.
/// </summary>
public class WpfMessageDialogService : IMessageDialogService
{
    public void ShowMessage(string title, string message) =>
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public bool Confirm(string title, string question) =>
        MessageBox.Show(question, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
}
