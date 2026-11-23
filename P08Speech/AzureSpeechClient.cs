using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace P08Speech;

/// <summary>
/// Cienka otoczka na SDK Azure AI Speech. Dokumentacja:
/// https://learn.microsoft.com/azure/ai-services/speech-service/
/// </summary>
public class AzureSpeechClient
{
    private readonly SpeechConfig _config;

    public AzureSpeechClient(string key, string region)
    {
        _config = SpeechConfig.FromSubscription(key, region);
        _config.SpeechRecognitionLanguage = "pl-PL";
        _config.SpeechSynthesisVoiceName = "pl-PL-ZofiaNeural";   // inne: pl-PL-MarekNeural, pl-PL-AgnieszkaNeural
    }

    /// <summary>
    /// Nasłuchuje mikrofonu do pierwszej pauzy (maks. ok. 15 s) i zwraca rozpoznany tekst.
    /// Gdy nic nie rozpoznano albo wystąpił błąd (np. zły klucz), zwraca opis zamiast rzucać wyjątek.
    /// </summary>
    public async Task<string> RecognizeOnceAsync()
    {
        using var audio = AudioConfig.FromDefaultMicrophoneInput();
        using var recognizer = new SpeechRecognizer(_config, audio);

        SpeechRecognitionResult result = await recognizer.RecognizeOnceAsync();

        return result.Reason switch
        {
            ResultReason.RecognizedSpeech => result.Text,
            ResultReason.NoMatch => "(nie rozpoznano mowy)",
            ResultReason.Canceled => DescribeCancellation(CancellationDetails.FromResult(result)),
            _ => $"({result.Reason})"
        };
    }

    /// <summary>Czyta tekst przez domyślny głośnik.</summary>
    public async Task SpeakAsync(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        using var synthesizer = new SpeechSynthesizer(_config);   // domyślne wyjście audio = głośnik
        using SpeechSynthesisResult result = await synthesizer.SpeakTextAsync(text);

        if (result.Reason == ResultReason.Canceled)
        {
            var details = SpeechSynthesisCancellationDetails.FromResult(result);
            Console.WriteLine($"Synteza przerwana: {details.Reason} {details.ErrorDetails}");
        }
    }

    private static string DescribeCancellation(CancellationDetails details) =>
        details.Reason == CancellationReason.Error
            ? $"(błąd {details.ErrorCode}: {details.ErrorDetails} – sprawdź klucz i region)"
            : $"(anulowano: {details.Reason})";
}
