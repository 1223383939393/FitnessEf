using System;
using Microsoft.EntityFrameworkCore;
using FitnessEf;

// Создаем контекст для работы с БД
using var db = new FitnessContext();

//Console.WriteLine("=== Задание 4: Список клиентов (Сортировка по Фамилии) ===");

try
{
    Console.WriteLine("=== Задание 8: Фильтрация по NULL (Клиенты без Email) ===");

    // LINQ-запрос: ищем всех, у кого Email равен null
    var clientsWithoutEmail = await db.Clients
        .Where(c => c.Email == null)
        .OrderBy(c => c.LastName)
        .ToListAsync();

    Console.WriteLine($"Найдено клиентов без электронной почты: {clientsWithoutEmail.Count}\n");

    foreach (var client in clientsWithoutEmail)
    {
        Console.WriteLine($"ID: {client.ClientId} | {client.LastName} {client.FirstName} | Телефон: {client.Phone ?? "Не указан"}");
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
