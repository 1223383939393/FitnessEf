using System;
using Microsoft.EntityFrameworkCore;
using FitnessEf;

// Создаем контекст для работы с БД
using var db = new FitnessContext();

Console.WriteLine("=== Задание 4: Список клиентов (Сортировка по Фамилии) ===");

try
{
    // Запрос LINQ: выбираем всех клиентов и сортируем по LastName (фамилии)
    var clients = await db.Clients
        .OrderBy(c => c.LastName)
        .ToListAsync();

    Console.WriteLine($"Успешно подключено к базе xz1. Найдено записей: {clients.Count}\n");

    foreach (var client in clients)
    {
        Console.WriteLine($"ID: {client.ClientId} | {client.LastName} {client.FirstName} | Статус: {client.Status}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при чтении базы: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Детали: {ex.InnerException.Message}");
    }
}

Console.WriteLine("\nНажмите любую клавишу...");
Console.ReadKey();
