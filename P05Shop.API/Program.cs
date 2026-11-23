using Microsoft.EntityFrameworkCore;
using P05Shop.API.Data;
using P05Shop.API.Services;
using P06Shop.Shared.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ----- 1. Rejestracja serwisów w kontenerze DI (ten sam mechanizm co w WPF z dnia 12) -----

builder.Services.AddControllers();

// OpenAPI: opis naszego API w formacie JSON (dawniej Swagger) pod /openapi/v1.json
builder.Services.AddOpenApi();

// Baza danych: DataContext dostaje connection string z konfiguracji
// (appsettings.Development.json → LocalDB; w dniu 15 User Secrets → Azure SQL).
// AddDbContext rejestruje kontekst jako Scoped: jeden na żądanie HTTP.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Nasz serwis. Cykle życia:
//  AddSingleton – jedna instancja na cały czas działania aplikacji
//  AddScoped    – jedna instancja na jedno żądanie HTTP (najczęstszy wybór dla serwisów z danymi)
//  AddTransient – nowa instancja przy każdym wstrzyknięciu
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// ----- 2. Pipeline: kolejne kroki obsługi każdego żądania (kolejność ma znaczenie) -----

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();                 // /openapi/v1.json
    app.MapScalarApiReference();      // /scalar – interaktywna dokumentacja (następca Swagger UI)
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();                 // adresy z atrybutów [Route] i [HttpGet] w kontrolerach

app.Run();
