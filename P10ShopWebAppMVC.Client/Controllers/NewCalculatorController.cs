using Microsoft.AspNetCore.Mvc;
using P10ShopWebAppMVC.Client.Models;

namespace P10ShopWebAppMVC.Client.Controllers;

/// <summary>
/// Ten sam kalkulator z silnie typowanym modelem (OperationModel) i Tag Helperami asp-for.
/// Dwie akcje Index rozróżniają atrybuty [HttpGet] i [HttpPost]; formularz bez action wysyła POST pod ten sam adres.
/// </summary>
public class NewCalculatorController : Controller
{
    // GET: /NewCalculator – model z wartościami startowymi
    [HttpGet]
    public IActionResult Index()
    {
        var operationModel = new OperationModel
        {
            Number1 = 8,
            Number2 = 10
        };

        return View(operationModel);
    }

    // POST: /NewCalculator – model binding sam tworzy OperationModel z pól Number1 i Number2
    [HttpPost]
    public IActionResult Index(OperationModel operation)
    {
        operation.Result = operation.Number1 + operation.Number2;
        return View(operation);
    }
}
