namespace Domain.Common;

/// <summary>
/// Базовий клас для сутностей, які підлягають аудиту (відстеженню створення та модифікації).
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// Замість того, щоб у кожному сервісі вручну прописувати Created = DateTime.UtcNow,
/// у Clean Architecture це вирішується за допомогою EF Core Interceptor (перехоплювача).
/// Перехоплювач під час виконання SaveChangesAsync автоматично знаходить усі сутності,
/// успадковані від BaseAuditableEntity, і самостійно заповнює ці поля.
/// </summary>
public abstract class BaseAuditableEntity : BaseEntity
{
    /// <summary>
    /// Дата та час створення запису у форматі UTC.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Ідентифікатор користувача або системи, що створила запис.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Дата та час останнього оновлення запису у форматі UTC.
    /// </summary>
    public DateTime? LastModifiedAtUtc { get; set; }

    /// <summary>
    /// Ідентифікатор користувача або системи, що востаннє оновила запис.
    /// </summary>
    public string? LastModifiedBy { get; set; }
}
