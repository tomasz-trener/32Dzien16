using Microsoft.EntityFrameworkCore;
using P06Shop.Shared.Models;
using P07Shop.DataSeeder;

namespace P05Shop.API.Data;

/// <summary>
/// Kontekst bazy danych (EF Core). Jeden DbSet = jedna tabela.
/// Connection string przychodzi z Program.cs (AddDbContext → UseSqlServer),
/// więc ta klasa nie wie, z jaką bazą pracuje (LocalDB dziś, Azure SQL w dniu 15).
/// </summary>
public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    /// <summary>
    /// Fluent API: ustawienia TYLKO bazodanowe. Walidacja ([Required], [MaxLength])
    /// jest na klasie Product i EF Core też ją czyta.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");

            // decimal(8,2): 6 cyfr przed przecinkiem, 2 po – bez tego EF ostrzega o obcinaniu wartości
            entity.Property(p => p.Price).HasPrecision(8, 2);

            // Dane początkowe (seed): trafiają do migracji jako INSERT-y.
            // Muszą być deterministyczne (stały seed w Bogus) i mieć ustawione Id.
            entity.HasData(ProductDataSeeder.GenerateProducts());
        });
    }
}
