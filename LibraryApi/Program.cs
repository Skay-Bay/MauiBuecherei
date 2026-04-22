using LibraryApi.Data;
using Microsoft.EntityFrameworkCore;
using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ResponseHeaderEncodingSelector = _ => System.Text.Encoding.UTF8;
});

// Datenbankkontext registrieren
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controller hinzufügen
builder.Services.AddControllers();

// OpenAPI-Dokumentation (standardmäßig in .NET 9+ verfügbar)
builder.Services.AddOpenApi();

// Eigenen Service für Nummerngenerierung registrieren
builder.Services.AddScoped<INummernGenerator, NummernGenerator>();

// ----- CORS aktivieren (wichtig für Aufrufe aus der MVC-App) -----
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowMvcApp",
        policy =>
        {
            // Ersetzen Sie die Ports durch die tatsächlichen URLs Ihrer MVC-App
            policy.WithOrigins("https://localhost:5001", "http://localhost:5000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

// HTTP-Pipeline
if (app.Environment.IsDevelopment())
{
    // OpenAPI-Endpunkt unter /openapi.json verfügbar machen
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// CORS muss vor Authorization und MapControllers stehen
app.UseCors("AllowMvcApp");

app.UseAuthorization();

app.MapControllers();

app.Run();