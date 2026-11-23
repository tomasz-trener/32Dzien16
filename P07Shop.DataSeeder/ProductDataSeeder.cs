using Bogus;
using P06Shop.Shared.Models;

namespace P07Shop.DataSeeder;

/// <summary>
/// Generator przykładowych produktów (biblioteka Bogus).
/// UseSeed(1234) sprawia, że za każdym razem powstają TE SAME dane –
/// to warunek konieczny, gdy dane trafiają do migracji (HasData w DataContext):
/// EF Core porównuje je z poprzednią migracją i każda różnica byłaby nową migracją.
/// Dlatego też data jest z zamkniętego przedziału, a nie "ostatnie 2 lata od dziś".
/// </summary>
public static class ProductDataSeeder
{
    public static List<Product> GenerateProducts(int count = 100)
    {
        var faker = new Faker<Product>("en")
            .UseSeed(1234)
            .RuleFor(p => p.Id, f => f.IndexFaker + 1)
            .RuleFor(p => p.Title, f => f.Commerce.ProductName())
            .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
            .RuleFor(p => p.Barcode, f => f.Commerce.Ean13()[..12])              // 12 znaków, jak w bazie
            .RuleFor(p => p.Price, f => Math.Round(f.Random.Decimal(1, 1500), 2))   // część produktów > 1000 (reguła usuwania)
            .RuleFor(p => p.ReleaseDate, f => f.Date.Between(new DateTime(2020, 1, 1), new DateTime(2025, 12, 31)).Date);

        return faker.Generate(count);
    }
}
