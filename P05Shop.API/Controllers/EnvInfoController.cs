using Microsoft.AspNetCore.Mvc;

namespace P05Shop.API.Controllers;

/// <summary>
/// Diagnostyka: gdzie i w jakim środowisku działa API.
/// Przydaje się po wdrożeniu do Azure, żeby zobaczyć, że to naprawdę inna maszyna
/// (Linux w App Service, Windows Server na maszynie wirtualnej) i inne środowisko (Production).
/// Adres: api/envinfo
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EnvInfoController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public EnvInfoController(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Get()
    {
        // Connection string pokazujemy tylko częściowo – nigdy nie zwracamy haseł przez API
        string? connectionString = _configuration.GetConnectionString("DefaultConnection");
        string database = connectionString is null ? "(brak)" : connectionString.Split(';').FirstOrDefault(p => p.TrimStart().StartsWith("Database", StringComparison.OrdinalIgnoreCase) || p.TrimStart().StartsWith("Initial Catalog", StringComparison.OrdinalIgnoreCase)) ?? "(nieznana)";

        return Ok(new
        {
            EnvironmentName = _environment.EnvironmentName,       // Development / Production
            MachineName = Environment.MachineName,
            OperatingSystem = Environment.OSVersion.ToString(),
            Is64Bit = Environment.Is64BitOperatingSystem,
            ProcessorCount = Environment.ProcessorCount,
            DotNetVersion = Environment.Version.ToString(),
            CurrentDirectory = Environment.CurrentDirectory,
            UserName = Environment.UserName,
            ServerTimeUtc = DateTime.UtcNow,
            Database = database.Trim(),
            CustomSetting = _configuration["MyCustomSetting"] ?? "(nie ustawiono – ustaw w App Service → Environment variables)"
        });
    }
}
