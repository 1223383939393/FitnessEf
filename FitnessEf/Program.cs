using System;
using Microsoft.EntityFrameworkCore;
using FitnessEf;

try
{
    await using var db = new FitnessContext();

    Console.WriteLine("=== Задание 11.1 и 11.4: Диагностика SQL и асинхронное чтение ===");

    // Формируем LINQ-запрос с фильтрацией по активному статусу
    var query = db.Clients
        .AsNoTracking() // Отключаем трекинг для легковесного чтения (Задание 11.3)
        .Where(c => c.Status == "active")
        .OrderBy(c => c.LastName);

    // Выводим сгенерированный SQL-запрос в консоль до его выполнения
    Console.WriteLine("Сгенерированный SQL-запрос:");
    Console.WriteLine(query.ToQueryString());
    Console.WriteLine(new string('-', 60));

    // Асинхронно выполняем запрос (Задание 11.2)
    var activeClients = await query.ToListAsync();
    Console.WriteLine($"Успешно загружено активных клиентов: {activeClients.Count}\n");


    Console.WriteLine("=== Задание 11.3: Сравнение отслеживаемого и нетрекаемого запроса ===");

    // Пытаемся найти клиента для изменения с отслеживанием (Change Tracker)
    var clientToUpdate = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == 1);
    if (clientToUpdate is not null)
    {
        Console.WriteLine($"Текущий статус клиента ID 1: {clientToUpdate.Status}");
        // Объект отслеживается, при вызове SaveChangesAsync изменения уйдут в базу автоматически
        clientToUpdate.Status = "active";
        await db.SaveChangesAsync();
        Console.WriteLine("Статус проверен/обновлен с использованием Change Tracker.");
    }
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Ошибка конфигурации контекста: {ex.Message}");
}
catch (DbUpdateException ex)
{
    Console.WriteLine("Ошибка при сохранении или изменении данных в БД.");
    Console.WriteLine($"Детали: {ex.GetBaseException().Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
}

Console.WriteLine("\nНажмите любую клавишу для завершения...");
Console.ReadKey();
