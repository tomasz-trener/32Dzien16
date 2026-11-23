namespace P10ShopWebAppMVC.Client.Models;

/// <summary>
/// Silnie typowany model dla widoku NewCalculator – zamiast ViewBag.
/// Kompilator i IntelliSense pilnują nazw właściwości.
/// </summary>
public class OperationModel
{
    public int Number1 { get; set; }

    public int Number2 { get; set; }

    public int Result { get; set; }
}
