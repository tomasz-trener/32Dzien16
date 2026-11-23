using Microsoft.AspNetCore.Mvc;
using P05Shop.API.Dto;
using P05Shop.API.Models;

namespace P05Shop.API.Controllers;

/// <summary>
/// Kontroler z szablonu, rozbudowany o przykłady routingu i źródeł parametrów.
/// Służy do nauki: czasowniki HTTP, [FromRoute], [FromQuery], [FromBody], kody odpowiedzi.
/// Adres bazowy: api/weatherforecast
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    // GET api/weatherforecast
    [HttpGet]
    public IEnumerable<WeatherForecast> Get() => Generate(5);

    // GET api/weatherforecast/3        – parametr z trasy (route)
    [HttpGet("{days:int}")]
    public IEnumerable<WeatherForecast> GetDays([FromRoute] int days) => Generate(days);

    // GET api/weatherforecast/search?days=3&minTemperature=10   – parametry z query string
    [HttpGet("search")]
    public IEnumerable<WeatherForecast> Search([FromQuery] int days = 5, [FromQuery] int minTemperature = -20) =>
        Generate(days).Where(f => f.TemperatureC >= minTemperature);

    // GET api/weatherforecast/filter?name=Warszawa&country=PL   – query string zbindowany do obiektu
    [HttpGet("filter")]
    public string Filter([FromQuery] CityDto city) => $"Miasto: {city.Name}, kraj: {city.Country}";

    // POST api/weatherforecast/cities   – dane w treści żądania (JSON) + kod 201 Created
    [HttpPost("cities")]
    public IActionResult AddCity([FromBody] CityDto city)
    {
        if (string.IsNullOrWhiteSpace(city.Name))
        {
            return BadRequest("Pole name jest wymagane.");   // 400
        }

        int newId = Random.Shared.Next(100, 999);

        // 201 + nagłówek Location z adresem nowego zasobu
        return CreatedAtAction(nameof(GetCity), new { id = newId }, city);
    }

    // GET api/weatherforecast/cities/7
    [HttpGet("cities/{id:int}")]
    public ActionResult<CityDto> GetCity(int id)
    {
        if (id <= 0)
        {
            return NotFound();   // 404
        }

        return Ok(new CityDto { Name = "Warszawa", Country = "PL" });   // 200
    }

    private static WeatherForecast[] Generate(int days) =>
        Enumerable.Range(1, days).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
}
