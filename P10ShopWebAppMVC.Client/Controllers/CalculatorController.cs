using Microsoft.AspNetCore.Mvc;

namespace P10ShopWebAppMVC.Client.Controllers;

/// <summary>
/// Adres /Calculator → ten kontroler. W MVC adres wskazuje kontroler i akcję (metodę),
/// a kontroler decyduje, który widok zwrócić i jakie dane mu przekazać.
/// Wersja "ręczna": IFormCollection (odpowiednik Request.Form z Razor Pages) i ViewBag.
/// </summary>
public class CalculatorController : Controller
{
    // GET: /Calculator – pusty formularz
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    // POST: /Calculator/Add – formularz wysłany przyciskiem
    [HttpPost]
    public IActionResult Add(IFormCollection form)
    {
        // TryParse zamiast int.Parse: puste pole nie wywraca aplikacji (FormatException)
        bool ok1 = int.TryParse(form["number1"], out int number1);
        bool ok2 = int.TryParse(form["number2"], out int number2);

        // ViewBag: dynamiczny obiekt na proste dane dla widoku (ViewData["X"] robi to samo jako słownik).
        // Wada: literówka w widoku wychodzi dopiero w działającej aplikacji – patrz NewCalculator z modelem.
        ViewBag.Number1 = number1;
        ViewBag.Number2 = number2;

        if (!ok1 || !ok2)
        {
            ViewBag.Error = "Podaj dwie liczby całkowite.";
            return View("Index");
        }

        ViewBag.MyResult = number1 + number2;

        // Akcja nazywa się Add, ale zwracamy widok Index – bez nazwy MVC szukałby Views/Calculator/Add.cshtml
        return View("Index");
    }
}
