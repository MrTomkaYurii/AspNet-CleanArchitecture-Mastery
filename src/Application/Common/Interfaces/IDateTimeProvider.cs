namespace Application.Common.Interfaces;

/// <summary>
/// Інтерфейс постачальника поточної дати та часу.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// Замість прямого виклику DateTime.UtcNow або DateTime.Now у бізнес-логіці,
/// використовується IDateTimeProvider.
/// Це дозволяє в Unit-тестах "заморозити" або підставити будь-який час,
/// щоб точно протестувати часові умови (нагадування, терміни виконання, прострочені завдання).
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
