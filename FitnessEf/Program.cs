using System;
using Microsoft.EntityFrameworkCore;
using FitnessEf;

// Создаем контекст для работы с БД
using var db = new FitnessContext();

//Console.WriteLine("=== Задание 4: Список клиентов (Сортировка по Фамилии) ===");

try
{
    Console.WriteLine("=== Задание 7: Изменение и удаление клиента ===");

    // 1. Ищем в базе нашего только что созданного клиента по его ID (69)
    var clientToEdit = await db.Clients.FindAsync(69);

    if (clientToEdit != null)
    {
        Console.WriteLine($"Нашли клиента: {clientToEdit.LastName}. Старый телефон: {clientToEdit.Phone}");

        // 2. Меняем ему телефон (ЭТО ИЗМЕНЕНИЕ)
        clientToEdit.Phone = "+70000000000";
        await db.SaveChangesAsync();
        Console.WriteLine($"Телефон успешно изменен на: {clientToEdit.Phone}");

        // 3. А теперь давай его удалим (ЭТО УДАЛЕНИЕ)
        db.Clients.Remove(clientToEdit);
        await db.SaveChangesAsync();
        Console.WriteLine("Клиент успешно удален из базы данных xz1.");
    }
    else
    {
        Console.WriteLine("Клиент с ID 69 не найден в базе!");
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
