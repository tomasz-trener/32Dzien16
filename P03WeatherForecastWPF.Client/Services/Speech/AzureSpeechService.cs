using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;
using P03WeatherForecastWPF.Client.Configuration;

namespace P03WeatherForecastWPF.Client.Services.Speech;

/// <summary>
/// Implementacja na Azure AI Speech. Klucz i region pochodzą z sekcji "SpeechSettings"
/// (User Secrets w czasie nauki; po wdrożeniu – zmienne środowiskowe lub Azure Key Vault).
/// </summary>
public class AzureSpeechService : ISpeechService
{
    private readonly SpeechSettings _settings;

    public AzureSpeechService(IOptions<SpeechSettings> options)
    {
        _settings = options.Value;
    }

    public async Task<string> RecognizeOnceAsync()
    {
        if (string.IsNullOrWhiteSpace(_settings.Key) || string.IsNullOrWhiteSpace(_settings.Region))
        {
            throw new InvalidOperationException(
                "Brak klucza Azure Speech. Ustaw: dotnet user-secrets set \"SpeechSettings:Key\" \"...\" oraz \"SpeechSettings:Region\" \"westeurope\".");
        }

        SpeechConfig config = SpeechConfig.FromSubscription(_settings.Key, _settings.Region);
        config.SpeechRecognitionLanguage = _settings.Language;

        using var audio = AudioConfig.FromDefaultMicrophoneInput();
        using var recognizer = new SpeechRecognizer(config, audio);

        SpeechRecognitionResult result = await recognizer.RecognizeOnceAsync();

        return result.Reason switch
        {
            ResultReason.RecognizedSpeech => result.Text,
            ResultReason.NoMatch => string.Empty,
            ResultReason.Canceled => throw new InvalidOperationException(
                $"Rozpoznawanie przerwane: {CancellationDetails.FromResult(result).ErrorDetails}"),
            _ => string.Empty
        };
    }
}
