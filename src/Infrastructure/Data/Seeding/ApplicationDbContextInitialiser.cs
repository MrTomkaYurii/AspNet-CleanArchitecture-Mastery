using Bogus;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.Seeding;

/// <summary>
/// Ініціалізатор бази даних та генератор початкових тестових даних (Seeding) за допомогою бібліотеки Bogus.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ (Генерація тестових даних через Bogus):
/// Замість ручного хардкоду декількох однакових записів типу "Test 1", "Test 2",
/// бібліотека Bogus дозволяє згенерувати реалістичні, багаті, локалізовані дані (у нашому випадку українською мовою "uk").
/// 
/// Ми демонструємо:
/// 1. Створення списків завдань (TodoLists) з випадковими підтримуваними кольорами (Colour Value Object).
/// 2. Генерацію фізичних адрес в Україні (Address Value Object).
/// 3. Генерацію дочірніх завдань (TodoItems) з різними пріоритетами, нотатками та датами нагадувань.
/// 4. Фіксований Randomizer.Seed для відтворюваності (детермінованості) однакового набору даних при кожному запуску.
/// </summary>
public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    /// <summary>
    /// Створення бази даних або застосування міграцій.
    /// </summary>
    public async Task InitialiseAsync()
    {
        try
        {
            // Для навчального проекту використовуємо EnsureCreatedAsync (або MigrateAsync при міграціях)
            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка під час ініціалізації бази даних SQLite.");
            throw;
        }
    }

    /// <summary>
    /// Наповнення бази даних тестовими даними за допомогою Bogus, якщо таблиці порожні.
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Помилка під час виконання Seeding за допомогою Bogus.");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Перевіряємо, чи база вже містить дані
        if (await _context.TodoLists.AnyAsync())
        {
            _logger.LogInformation("База даних вже містить записи. Сидинг пропущено.");
            return;
        }

        _logger.LogInformation("Починаємо генерацію тестових даних через Bogus...");

        // Встановлюємо детерміноване зерно для повторюваності результатів
        Randomizer.Seed = new Random(42);

        // Набір доступних кольорів з нашого Value Object
        var availableColours = new[]
        {
            Colour.Blue,
            Colour.Green,
            Colour.Orange,
            Colour.Purple,
            Colour.Red,
            Colour.Yellow
        };

        // Генератор адрес (Value Object Address) українською мовою
        var addressFaker = new Faker<Address>("uk")
            .CustomInstantiator(f => new Address(
                f.Address.StreetAddress(),
                f.Address.City(),
                f.Address.State(),
                "Україна",
                f.Address.ZipCode()));

        // Генератор окремого завдання (TodoItem)
        var taskTitles = new[]
        {
            "Провести код-рев'ю нового функціоналу",
            "Налаштувати CI/CD пайплайн для автоматичного деплою",
            "Оновити документацію Clean Architecture",
            "Покрити юніт-тестами хендлери MediatR",
            "Оптимізувати SQL-запити через EF Core Explain",
            "Налаштувати моніторинг помилок через Serilog",
            "Провести зустріч ретроспективи спринту",
            "Підготувати реліз для продакшн середовища",
            "Зробити бекап бази даних",
            "Оновити NuGet залежності до актуальних версій"
        };

        var todoItemFaker = new Faker<TodoItem>("uk")
            .RuleFor(i => i.Title, f => f.PickRandom(taskTitles))
            .RuleFor(i => i.Note, f => f.Lorem.Sentence(f.Random.Int(5, 12)))
            .RuleFor(i => i.Priority, f => f.PickRandom<PriorityLevel>())
            .RuleFor(i => i.Reminder, f => f.Date.Soon(14))
            .RuleFor(i => i.Done, f => f.Random.Bool(0.3f)); // ~30% вже виконані

        // Назви списків завдань
        var listTopics = new[]
        {
            "🚀 Реліз проекту Clean Architecture",
            "💼 Робочі завдання команди бекенду",
            "📚 Навчальні матеріали та лекції",
            "🏠 Домашні справи та закупівлі",
            "🛠 Технічний борг та рефакторинг"
        };

        var todoLists = new List<TodoList>();

        foreach (var topic in listTopics)
        {
            var list = new TodoList
            {
                Title = topic,
                Colour = new Faker().PickRandom(availableColours),
                Location = addressFaker.Generate()
            };

            // Генеруємо від 3 до 6 завдань для кожного списку
            var itemsCount = new Faker().Random.Int(3, 6);
            for (int i = 0; i < itemsCount; i++)
            {
                var item = todoItemFaker.Generate();
                list.Items.Add(item);
            }

            todoLists.Add(list);
        }

        _context.TodoLists.AddRange(todoLists);

        await _context.SaveChangesAsync();

        _logger.LogInformation(
            "Успішно згенеровано та збережено {ListCount} списків та {ItemCount} завдань через Bogus!",
            todoLists.Count,
            todoLists.Sum(l => l.Items.Count));
    }
}
