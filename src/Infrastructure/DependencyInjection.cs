using Application.Common.Interfaces;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Data.Seeding;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
/// Реєстрація служб шару Infrastructure у контейнері Dependency Injection.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// 1. Підключення до бази даних через UseSqlite із передачею рядка з'єднання.
/// 2. Реєстрація інтерфейсу IApplicationDbContext -> спрямована на конкретний ApplicationDbContext.
/// 3. Реєстрація сидера бази даних (ApplicationDbContextInitialiser) та перехоплювачів.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? "Data Source=CleanArchitecture.db";

        // Реєстрація перехоплювачів (Interceptors)
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();

        // Реєстрація контексту бази даних з провайдером SQLite
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlite(connectionString);
        });

        // Інверсія залежностей: Application залежить від інтерфейсу IApplicationDbContext,
        // а Infrastructure надає конкретну реалізацію ApplicationDbContext.
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Реєстрація сидера даних (Bogus)
        services.AddScoped<ApplicationDbContextInitialiser>();

        // Реєстрація постачальника часу
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
