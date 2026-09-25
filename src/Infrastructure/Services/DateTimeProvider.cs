using Application.Common.Interfaces;

namespace Infrastructure.Services;

/// <summary>
/// Реалізація постачальника системного часу за допомогою DateTime.UtcNow.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
