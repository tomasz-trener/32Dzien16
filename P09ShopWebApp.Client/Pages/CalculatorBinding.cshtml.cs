using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P09ShopWebApp.Client.Pages;

/// <summary>
/// Ten sam kalkulator, ale framework robi za nas to, co w Calculator pisaliśmy ręcznie:
/// [BindProperty] mapuje pola formularza na właściwości (model binding),
/// asp-page-handler wybiera metodę OnPostXxx, a [TempData] przenosi wynik do kolejnego żądania.
/// </summary>
public class CalculatorBindingModel : PageModel
{
    // Przy POST framework znajdzie pole name="Number1", zamieni tekst na int i wpisze tutaj.
    // Domyślnie działa tylko dla POST; dla GET trzeba [BindProperty(SupportsGet = true)].
    [BindProperty]
    public int Number1 { get; set; }

    [BindProperty]
    public int Number2 { get; set; }

    // TempData: schowek, który przeżywa JEDNO kolejne żądanie i znika po odczytaniu (domyślnie cookie).
    // Dlatego po kliknięciu kolejnego przycisku poprzedni wynik jest widoczny jeszcze raz, a potem znika.
    [TempData]
    public int AdditionResult { get; set; }

    [TempData]
    public int SubtractionResult { get; set; }

    [TempData]
    public int MultiplicationResult { get; set; }

    public void OnGet()
    {
    }

    // Konwencja: OnPost + nazwa handlera z asp-page-handler="Add"
    public void OnPostAdd()
    {
        AdditionResult = Number1 + Number2;
    }

    public void OnPostSubtract()
    {
        SubtractionResult = Number1 - Number2;
    }

    public void OnPostMultiply()
    {
        MultiplicationResult = Number1 * Number2;
    }
}
