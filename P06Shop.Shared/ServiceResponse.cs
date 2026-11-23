namespace P06Shop.Shared;

/// <summary>
/// Jednolita "koperta" odpowiedzi serwisów i API.
/// Zamiast zwracać gołe dane albo rzucać wyjątkiem, zawsze zwracamy:
/// czy się udało (Success), dane (Data) i komunikat (Message).
/// Klient sprawdza Success, zanim użyje Data.
/// </summary>
public class ServiceResponse<T>
{
    public T? Data { get; set; }

    public bool Success { get; set; } = true;

    public string Message { get; set; } = string.Empty;

    // Metody fabryczne – czytelniejsze niż ustawianie trzech właściwości ręcznie
    public static ServiceResponse<T> Ok(T data, string message = "") =>
        new() { Data = data, Success = true, Message = message };

    public static ServiceResponse<T> Fail(string message) =>
        new() { Data = default, Success = false, Message = message };
}
