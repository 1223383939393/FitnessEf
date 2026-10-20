using System;
using Microsoft.EntityFrameworkCore;
using FitnessEf;

// Создаем контекст для работы с БД
using var db = new FitnessContext();

Console.WriteLine("=== Задание 4: Список клиентов (Сортировка по Фамилии) ===");

try
{
    Console.WriteLine("=== Задание 6: Добавление нового клиента ===");

    // 1. Создаем объект нового клиента
    var newClient = new Client
    {
        FirstName = "Иван",
        LastName = "Тестовый",
        Phone = "+79991112233",
        Email = "testivan@mail.com",
        BirthDate = new DateOnly(1995, 5, 20),
        Status = "active"
    };

    // 2. Ставим в очередь на добавление
    db.Clients.Add(newClient);

    // 3. Отправляем команду INSERT в базу данных xz1
    await db.SaveChangesAsync();

    // EF Core автоматически заберет сгенерированный базой ID и запишет его в объект!
    Console.WriteLine($"Успешно! Клиент добавлен. Ему присвоен ID: {newClient.ClientId}");
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
