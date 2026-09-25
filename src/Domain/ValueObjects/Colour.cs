using Domain.Common;
using Domain.Exceptions;

namespace Domain.ValueObjects;

/// <summary>
/// Об'єкт-значення (Value Object), що представляє колір оформлення списку.
/// Класичний приклад із шаблону Jason Taylor Clean Architecture.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// Замість того, щоб зберігати колір як простий рядок (primitive obsession), ми створюємо
/// повноцінний тип Colour. Це дає:
/// 1. Гарантію валідності: колір не може бути створений у невалідному стані (Domain Invariant).
/// 2. Список допустимих кольорів (White, Red, Orange, Yellow, Green, Blue, Purple, Grey).
/// 3. Явне приведення типів (explicit/implicit casting) для зручності.
/// </summary>
public class Colour : ValueObject
{
    static Colour()
    {
    }

    private Colour()
    {
    }

    private Colour(string code)
    {
        Code = code;
    }

    /// <summary>
    /// Фабричний метод для створення кольору з hex-коду.
    /// Перевіряє, чи код підтримується системою.
    /// </summary>
    public static Colour From(string code)
    {
        var colour = new Colour { Code = code };

        if (!SupportedColours.Contains(colour))
        {
            throw new UnsupportedColourException(code);
        }

        return colour;
    }

    public static Colour White => new("#FFFFFF");
    public static Colour Red => new("#FF5733");
    public static Colour Orange => new("#FFC300");
    public static Colour Yellow => new("#FFFF66");
    public static Colour Green => new("#CCFF99");
    public static Colour Blue => new("#6666FF");
    public static Colour Purple => new("#9966CC");
    public static Colour Grey => new("#999999");

    /// <summary>
    /// Hex-код кольору.
    /// </summary>
    public string Code { get; private set; } = "#FFFFFF";

    public static implicit operator string(Colour colour)
    {
        return colour.ToString();
    }

    public static explicit operator Colour(string code)
    {
        return From(code);
    }

    public override string ToString()
    {
        return Code;
    }

    /// <summary>
    /// Перелік підтримуваних кольорів.
    /// </summary>
    protected static IEnumerable<Colour> SupportedColours
    {
        get
        {
            yield return White;
            yield return Red;
            yield return Orange;
            yield return Yellow;
            yield return Green;
            yield return Blue;
            yield return Purple;
            yield return Grey;
        }
    }

    /// <summary>
    /// Визначаємо, які поля беруть участь у порівнянні об'єктів-значень.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Code;
    }
}
