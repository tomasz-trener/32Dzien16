namespace P03WeatherForecastWPF.Client.Services.Speech;

/// <summary>
/// Rozpoznawanie mowy z mikrofonu. ViewModel zna tylko ten interfejs,
/// więc w testach (albo bez klucza Azure) można podstawić atrapę.
/// </summary>
public interface ISpeechService
{
    /// <summary>Nasłuchuje do pierwszej pauzy i zwraca rozpoznany tekst (pusty, gdy nic nie rozpoznano).</summary>
    Task<string> RecognizeOnceAsync();
}
