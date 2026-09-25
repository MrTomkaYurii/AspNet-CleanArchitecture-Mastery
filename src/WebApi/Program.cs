using Application;
using Infrastructure;
using Infrastructure.Data.Seeding;
using Scalar.AspNetCore;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Composition Root у Clean Architecture):
// Проект WebApi є точкою входу (Composition Root).
// Він збирає докупи всі шари, викликаючи їхні методи розширення:
// 1. AddApplicationServices - реєструє MediatR, валідатори, Pipeline Behaviors.
// 2. AddInfrastructureServices - підключає SQLite, EF Core, Seeder, Interceptors.
// 3. WebApi налаштовує свій шар: контролери, OpenAPI документацію, глобальні фільтри.
// =========================================================================

// 1. Реєстрація шару Application
builder.Services.AddApplicationServices();

// 2. Реєстрація шару Infrastructure
builder.Services.AddInfrastructureServices(builder.Configuration);

// 3. Реєстрація контролерів та OpenAPI (Swagger / Scalar)
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// =========================================================================
// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Автоматична ініціалізація та Seeding через Bogus):
// Під час старту додатка створюється окремий IServiceScope:
// - Перевіряється наявність бази SQLite (EnsureCreatedAsync);
// - Якщо таблиці порожні, Bogus генерує реалістичні україномовні дані.
// =========================================================================
using (var scope = app.Services.CreateScope())
{
    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
    await initialiser.InitialiseAsync();
    await initialiser.SeedAsync();
}

// =========================================================================
// НАВЧАЛЬНЕ ПОЯСНЕННЯ (HTTP Request Pipeline / Middleware):
// Порядок підключення Middleware має вирішальне значення:
// ExceptionHandlingMiddleware стоїть першим, щоб перехоплювати будь-які помилки
// з наступних компонентів конвеєра.
// =========================================================================

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Генерує /openapi/v1.json
    app.MapOpenApi();

    // Сучасний інтерактивний UI для тестування API (доступний за адресою /scalar/v1)
    app.MapScalarApiReference(options =>
    {
        options.Title = "Clean Architecture API - Навчальний проект";
        options.Theme = ScalarTheme.Purple;
    });

    // Зручний редирект з кореня "/" одразу на документацію Scalar
    app.MapGet("/", () => Results.Redirect("/scalar/v1")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
