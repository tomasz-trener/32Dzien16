namespace P03WeatherForecastWPF.Client.Configuration;

/// <summary>
/// Sekcja "SpeechSettings". Key i Region to SEKRETY – nie wpisujemy ich do appsettings.json,
/// tylko do User Secrets (prawy przycisk na projekcie → Manage User Secrets):
/// { "SpeechSettings": { "Key": "...", "Region": "westeurope" } }
/// Language może być w appsettings.json, bo nie jest tajny.
/// </summary>
public class SpeechSettings
{
    public const string SectionName = "SpeechSettings";

    public string Key { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string Language { get; set; } = "pl-PL";
}
