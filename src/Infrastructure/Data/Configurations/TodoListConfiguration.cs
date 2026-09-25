using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

/// <summary>
/// Конфігурація сутності TodoList через Entity Framework Core Fluent API.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Fluent API та Value Objects):
/// 1. Чому Fluent API, а не Data Annotations:
///    Це зберігає шар Domain чистим від сторонніх залежностей та атрибутів конкретної ORM.
/// 
/// 2. ДВА СПОСОБИ МАПІНГУ VALUE OBJECTS У EF CORE:
///    а) HasConversion (для одинарних Value Objects, як Colour):
///       Перетворює об'єкт Colour у рядок hex-коду "#FFFFFF" при збереженні в БД
///       та відновлює об'єкт Colour через фабричний метод Colour.From(code) при читанні.
///       Це класичний підхід Jason Taylor Clean Architecture!
/// 
///    б) OwnsOne (для комплексних Value Objects з декількома полями, як Address):
///       Вбудовує всі поля адреси (Street, City, State, Country, ZipCode) у ту саму таблицю TodoLists
///       з префіксом Location_ без створення зайвої пов'язаної таблиці.
/// </summary>
public class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
{
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
        builder.ToTable("TodoLists");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();

        // 1. Спосіб мапінгу Value Object: Value Converter для Colour
        builder.Property(t => t.Colour)
            .HasConversion(
                colour => colour.Code,
                code => Colour.From(code))
            .HasColumnName("Colour")
            .HasMaxLength(10)
            .IsRequired();

        // 2. Спосіб мапінгу Value Object: OwnsOne для комплексного Address
        builder.OwnsOne(b => b.Location, ab =>
        {
            ab.Property(a => a.Street).HasColumnName("Location_Street").HasMaxLength(150);
            ab.Property(a => a.City).HasColumnName("Location_City").HasMaxLength(100);
            ab.Property(a => a.State).HasColumnName("Location_State").HasMaxLength(100);
            ab.Property(a => a.Country).HasColumnName("Location_Country").HasMaxLength(100);
            ab.Property(a => a.ZipCode).HasColumnName("Location_ZipCode").HasMaxLength(20);
        });

        // Каскадне видалення: видалення списку видаляє всі його завдання
        builder.HasMany(t => t.Items)
            .WithOne(i => i.List)
            .HasForeignKey(i => i.ListId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
