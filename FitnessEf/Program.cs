using System;
using Microsoft.EntityFrameworkCore;
using FitnessEf;

// Создаем контекст для работы с БД
using var db = new FitnessContext();

Console.WriteLine("=== Задание 4: Список клиентов (Сортировка по Фамилии) ===");

try
{
    // Задание 5: Выбираем только активных, сортируем по фамилии, 
    // пропускаем 0 (первая страница) и берем ровно 5 записей
    var clientsPage = await db.Clients
        .Where(c => c.Status == "active")
        .OrderBy(c => c.LastName)
        .Skip(0)
        .Take(5)
        .ToListAsync();

    Console.WriteLine($"=== Задание 5: Список активных клиентов (Страница 1, топ-5) ===");

    foreach (var client in clientsPage)
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
