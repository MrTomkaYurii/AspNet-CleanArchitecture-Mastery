using System.Reflection;
using Application;
using Infrastructure;
using Infrastructure.Data.Seeding;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Composition Root у Clean Architecture):
// Проект WebApi є точкою входу (Composition Root).
// Він збирає докупи всі шари, викликаючи їхні методи розширення:
// 1. AddApplicationServices - реєструє MediatR, валідатори, Pipeline Behaviors.
// 2. AddInfrastructureServices - підключає SQLite, EF Core, Seeder, Interceptors.
// 3. WebApi налаштовує свій шар: контролери, Swagger UI, Scalar UI, глобальні фільтри.
// =========================================================================

// 1. Реєстрація шару Application
builder.Services.AddApplicationServices();

// 2. Реєстрація шару Infrastructure
builder.Services.AddInfrastructureServices(builder.Configuration);

// 3. Реєстрація контролерів та OpenAPI (Swagger / Scalar)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Налаштування генерації Swagger документації
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clean Architecture API",
        Version = "v1",
        Description = "Навчальний проект: Clean Architecture, CQRS, MediatR, Result Pattern на .NET 10",
        Contact = new OpenApiContact
        {
            Name = "Clean Architecture Course",
            Email = "tomka.yuriy@gmail.com"
        }
    });

    // Підключення XML-документації WebApi для відображення описів ендпоінтів у Swagger UI
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

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
    // Генерує /openapi/v1.json (вбудований .NET 10 OpenAPI)
    app.MapOpenApi();

    // 1. Класичний Swagger та Swagger UI (доступний за адресою /swagger)
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Clean Architecture API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Clean Architecture API - Swagger UI";
    });

    // 2. Сучасний інтерактивний UI для тестування API (доступний за адресою /scalar/v1)
    app.MapScalarApiReference(options =>
    {
        options.Title = "Clean Architecture API - Scalar UI";
        options.Theme = ScalarTheme.Purple;
    });

    // Зручний редирект з кореня "/" одразу на Swagger UI
    app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
