// P08Speech – Azure AI Speech: rozpoznawanie mowy (mikrofon → tekst) i synteza (tekst → głośnik).
//
// Klucz i region usługi NIE są w kodzie ani w appsettings.json – trzymamy je w User Secrets:
//   dotnet user-secrets init
//   dotnet user-secrets set "Speech:Key" "<klucz z portalu Azure>"
//   dotnet user-secrets set "Speech:Region" "westeurope"
// Plik secrets.json leży w profilu użytkownika (%APPDATA%\Microsoft\UserSecrets\<id>\secrets.json),
// więc nigdy nie trafi do repozytorium.

using Microsoft.Extensions.Configuration;
using P08Speech;

Console.OutputEncoding = System.Text.Encoding.UTF8;

IConfiguration configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()          // czyta secrets.json powiązany z tym projektem (UserSecretsId w .csproj)
    .AddEnvironmentVariables()          // alternatywa: zmienne środowiskowe Speech__Key i Speech__Region
    .Build();

string? key = configuration["Speech:Key"];
string? region = configuration["Speech:Region"];

if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(region))
{
    Console.WriteLine("Brak konfiguracji. Ustaw sekrety: dotnet user-secrets set \"Speech:Key\" \"...\" i \"Speech:Region\" \"westeurope\"");
    return;
}

var speech = new AzureSpeechClient(key, region);

while (true)
{
    Console.WriteLine();
    Console.WriteLine("1 – rozpoznaj mowę (powiedz coś do mikrofonu)");
    Console.WriteLine("2 – przeczytaj tekst na głos");
    Console.WriteLine("3 – rozpoznaj i powtórz");
    Console.WriteLine("Enter – koniec");
    Console.Write("> ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.WriteLine("Mów teraz...");
            Console.WriteLine($"Rozpoznano: {await speech.RecognizeOnceAsync()}");
            break;

        case "2":
            Console.Write("Tekst do przeczytania: ");
            await speech.SpeakAsync(Console.ReadLine() ?? string.Empty);
            break;

        case "3":
            Console.WriteLine("Mów teraz...");
            string text = await speech.RecognizeOnceAsync();
            Console.WriteLine($"Rozpoznano: {text}");
            await speech.SpeakAsync($"Powiedziałeś: {text}");
            break;

        default:
            return;
    }
}
