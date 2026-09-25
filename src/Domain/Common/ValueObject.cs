namespace Domain.Common;

/// <summary>
/// Базовий абстрактний клас для Value Objects (Об'єктів-значень) за патерном DDD.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (DDD - Value Object):
/// 1. Value Object НЕ має власного унікального ідентифікатора (Id).
/// 2. Він визначається виключно сукупністю своїх значень (властивостей).
///    Наприклад: дві купюри номіналом 100 грн мають однакову цінність. Якщо змінити значення,
///    це вже інший об'єкт-значення.
/// 3. Value Object завжди НЕЗМІННИЙ (Immutable). Після створення його властивості не можна змінювати.
///    Якщо потрібно змінити значення — створюється новий екземпляр.
/// 4. Два Value Object вважаються рівними (==), якщо всі їхні компоненти рівні.
/// 5. Базовий клас інкапсулює логіку порівняння через абстрактний метод GetEqualityComponents().
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Метод, який дочірній клас зобов'язаний реалізувати, повернувши послідовність усіх полів,
    /// що беруть участь у порівнянні на рівність.
    /// </summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
