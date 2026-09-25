namespace Domain.Exceptions;

/// <summary>
/// Доменний виняток (Domain Exception), який виникає при спробі створити колір з непідтримуваним кодом.
/// 
/// НАВЧАЛЬНЕ ПОЯСНЕННЯ:
/// Доменні винятки викидаються всередині ядра бізнес-логіки (Domain), коли порушується
/// доменний інваріант (бізнес-правило).
/// Вони не повинні залежати від HTTP-статусів (наприклад 400 Bad Request),
/// тому що шар Domain нічого не знає про Web або протокол HTTP.
/// Трансляція доменного винятку в статус відповіді відбувається на рівні Presentation / WebApi (у Middleware).
/// </summary>
public class UnsupportedColourException : Exception
{
    public UnsupportedColourException(string code)
        : base($"Колір із кодом \"{code}\" не підтримується системою.")
    {
    }
}
