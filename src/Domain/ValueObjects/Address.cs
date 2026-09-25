using Domain.Common;

namespace Domain.ValueObjects;

/// <summary>
/// Об'єкт-значення (Value Object), що представляє фізичну адресу (наприклад, локація списку завдань чи офісу).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Комплексний Value Object):
/// Цей клас чудово демонструє порівняння за декількома полями:
/// Якщо хоча б одне поле (місто чи вулиця) відрізняється — це інша адреса.
/// 
/// В Entity Framework Core такі об'єкти налаштовуються через Fluent API за допомогою:
/// `builder.OwnsOne(x => x.Address)`
/// Це автоматично розгортає поля адреси у колонки таблиці сутності-власника без необхідності
/// створення окремої таблиці Addresses з зовнішніми ключами!
/// </summary>
public class Address : ValueObject
{
    public string Street { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;

    // Конструктор без параметрів потрібен для Entity Framework Core
    private Address() { }

    public Address(string street, string city, string state, string country, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    /// <summary>
    /// Всі компоненти адреси беруть участь у визначенні рівності двох об'єктів.
    /// </summary>
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return ZipCode;
    }

    public override string ToString()
    {
        return $"{Street}, {City}, {State}, {Country} {ZipCode}".Trim(',', ' ');
    }
}
