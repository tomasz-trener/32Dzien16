namespace P05Shop.API.Dto;

/// <summary>
/// DTO (Data Transfer Object): kształt danych przesyłanych między klientem a API.
/// Model binding ASP.NET Core wypełnia go z query stringa ([FromQuery]) lub z JSON ([FromBody]).
/// </summary>
public class CityDto
{
    public string Name { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;
}
