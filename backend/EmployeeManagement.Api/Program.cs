using System.Text.Json;
using System.Text.Json.Serialization;
using EmployeeManagement.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- Half 1: register services ----------

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // C# uses FirstName; the React app uses first_name. This converts between them
        // automatically, in both directions.
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;

        // Send/receive enums as text ("Active") instead of numbers (0).
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // When validation fails, reply as { "error": "...", "errors": {...} }
        // because the React app reads data.error.
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value!.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors
                        .Select(e => entry.Key.StartsWith('$') || string.IsNullOrEmpty(e.ErrorMessage)
                            // "$.status" style keys come from JSON that couldn't be read at all
                            // (e.g. status = "Maybe"); replace the technical message with a clear one.
                            ? $"Invalid value for '{entry.Key.TrimStart('$', '.')}'."
                            : e.ErrorMessage)
                        .ToArray());

            // Prefer the JSON-format error if there is one, otherwise the first rule that failed.
            var firstError = errors
                .OrderByDescending(entry => entry.Key.StartsWith('$'))
                .First().Value.First();

            return new BadRequestObjectResult(new { error = firstError, errors });
        };
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        // If SQL Server is briefly unreachable (e.g. still starting in Docker), retry instead of crashing.
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddOpenApi();

// ---------- Half 2: the request pipeline ----------

var app = builder.Build();

// Apply any pending EF Core migrations at startup.
// Inside Docker there is no "dotnet ef database update", so the app creates/updates
// its own database. If everything is already applied, this does nothing.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
