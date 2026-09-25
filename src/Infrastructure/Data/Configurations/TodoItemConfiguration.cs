using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Конфігурація сутності TodoItem через Entity Framework Core Fluent API.
/// </summary>
public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        // Назва таблиці
        builder.ToTable("TodoItems");

        // Первинний ключ
        builder.HasKey(t => t.Id);

        // Налаштування обов'язковості та довжини заголовка
        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        // Опис (нотатка) може містити до 1000 символів
        builder.Property(t => t.Note)
            .HasMaxLength(1000);

        // Пріоритет зберігається у вигляді цілого числа (int)
        builder.Property(t => t.Priority)
            .IsRequired();

        // Індекс по полю ListId для прискорення пошуку завдань конкретного списку
        builder.HasIndex(t => t.ListId);
    }
}
