using Microsoft.AspNetCore.Mvc.RazorPages;

namespace P09ShopWebApp.Client.Pages;

/// <summary>
/// Najprostszy kalkulator: formularz czytamy RĘCZNIE z Request.Form.
/// Celowo bez model bindingu – żeby zobaczyć, co framework robi za nas (patrz CalculatorBinding).
/// </summary>
public class CalculatorModel : PageModel
{
    public int Number1 { get; set; }

    public int Number2 { get; set; }

    public int Result { get; set; }

    // Komunikat zamiast pustego "else" – użytkownik widzi, co wpisał źle
    public string? ErrorMessage { get; set; }

    // GET: /Calculator – pusty formularz. Etap A (formularz method="get"): dane są w ADRESIE,
    // np. /Calculator?Number1=2&Number2=3, i czytamy je z Request.Query.
    public void OnGet()
    {
        if (Request.Query.ContainsKey("Number1"))
        {
            Calculate(Request.Query["Number1"], Request.Query["Number2"]);
        }
    }

    // POST: /Calculator – Etap B (formularz method="post"): dane są w TREŚCI żądania, czytamy je z Request.Form
    public void OnPost()
    {
        var form = Request.Form;
        Calculate(form["Number1"], form["Number2"]);
    }

    private void Calculate(string? text1, string? text2)
    {
        // Pola formularza to zawsze TEKST. TryParse zamiast Parse: użytkownik może wpisać cokolwiek.
        bool ok1 = int.TryParse(text1, out int number1);
        bool ok2 = int.TryParse(text2, out int number2);

        // HTTP jest bezstanowe: po każdym żądaniu serwer buduje stronę od nowa,
        // więc wartości do pól musimy odesłać sami, inaczej po wysłaniu pola będą puste.
        Number1 = number1;
        Number2 = number2;

        if (ok1 && ok2)
        {
            Result = number1 + number2;
        }
        else
        {
            ErrorMessage = "Podaj dwie liczby całkowite.";
        }
    }
}
