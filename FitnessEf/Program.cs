using System;
using Microsoft.EntityFrameworkCore;
using FitnessEf;

using var db = new FitnessContext();

try
{
    Console.WriteLine("=== Задание 9: Работа со связью Клиент — Посещения ===\n");

    // 1. Пробуем добавить новое посещение для клиента с ID = 1 (если он есть в базе)
    int testClientId = 1;
    bool clientExists = await db.Clients.AnyAsync(c => c.ClientId == testClientId);

    if (clientExists)
    {
        var newVisit = new Visit
        {
            ClientId = testClientId,
            VisitDate = DateOnly.FromDateTime(DateTime.Today),
            Note = "Первичное Задание через Program.cs"
        };

        db.Visits.Add(newVisit);
        await db.SaveChangesAsync();
        Console.WriteLine($"[Успех] Добавлено посещение для Клиента с ID {testClientId}.\n");
    }
    else
    {
        Console.WriteLine($"[Предупреждение] Клиент с ID {testClientId} не найден. Пропускаем добавление посещения.\n");
    }

    // 2. Жадная загрузка (Eager Loading) клиентов вместе с их посещениями
    Console.WriteLine("Выводим всех клиентов и их визиты:");
    var clientsWithVisits = await db.Clients
        .Include(c => c.Visits) // Подгружаем связанные посещения из таблицы visits
        .OrderBy(c => c.ClientId)
        .ToListAsync();

    foreach (var client in clientsWithVisits)
    {
        Console.WriteLine($"Клиент: {client.ClientId} | {client.LastName} {client.FirstName} (Статус: {client.Status})");

        if (client.Visits.Count == 0)
        {
            Console.WriteLine("  -> Посещений пока нет.");
        }
        else
        {
            foreach (var visit in client.Visits)
            {
                Console.WriteLine($"  -> [Визит ID: {visit.VisitId}] Дата: {visit.VisitDate} | Заметка: {visit.Note ?? "нет"}");
            }
        }
        Console.WriteLine(new string('-', 50));
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Детали: {ex.InnerException.Message}");
    }
}

Console.WriteLine("\nНажмите любую клавишу...");
Console.ReadKey();
