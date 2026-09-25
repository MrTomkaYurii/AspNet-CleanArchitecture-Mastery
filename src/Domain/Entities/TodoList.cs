using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Агрегат списку завдань (TodoList).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Clean Architecture & DDD):
/// 1. TodoList є коренем агрегату (Aggregate Root). Він об'єднує сутності TodoItem під спільним контекстом.
/// 2. Містить два Value Objects:
///    - Colour: колір візуального оформлення списку;
///    - Address: фізична адреса/локація, пов'язана зі списком (наприклад, "Офіс Київ", "Дім").
/// 3. Успадкований від BaseAuditableEntity, що автоматично додає поля аудиту (дату створення, автора тощо).
/// </summary>
public class TodoList : BaseAuditableEntity
{
    /// <summary>
    /// Назва списку завдань (наприклад, "Робочі завдання", "Покупки").
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Колір оформлення списку (Value Object).
    /// За замовчуванням білий.
    /// </summary>
    public Colour Colour { get; set; } = Colour.White;

    /// <summary>
    /// Локація/адреса, до якої прив'язаний цей список (комплексний Value Object).
    /// Може бути null, якщо локація не вказана.
    /// </summary>
    public Address? Location { get; set; }

    /// <summary>
    /// Колекція дочірніх завдань у цьому списку.
    /// </summary>
    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}
