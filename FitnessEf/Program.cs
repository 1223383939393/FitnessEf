using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FitnessEf;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("==================================================");
    Console.WriteLine("=== УПРАВЛЕНИЕ КЛИЕНТАМИ ФИТНЕС-КЛУБА (EF CORE) ===");
    Console.WriteLine("==================================================");
    Console.WriteLine("1. Список клиентов (по алфавиту)");
    Console.WriteLine("2. Найти клиента по ID");
    Console.WriteLine("3. Поиск по фрагменту фамилии");
    Console.WriteLine("4. Добавить нового клиента");
    Console.WriteLine("5. Изменить телефон или статус");
    Console.WriteLine("6. Удалить клиента");
    Console.WriteLine("7. Показать общую статистику");
    Console.WriteLine("0. Выход из программы");
    Console.Write("Выберите пункт меню: ");

    string choice = Console.ReadLine() ?? string.Empty;
    if (choice == "0") break;

    switch (choice)
    {
        case "1": await ListClientsAsync(); break;
        case "2": await FindClientByIdAsync(); break;
        case "3": await FindClientByLastNameAsync(); break;
        case "4": await AddClientAsync(); break;
        case "5": await UpdateClientAsync(); break;
        case "6": await DeleteClientAsync(); break;
        case "7": await ShowStatisticsAsync(); break;
        default: Console.WriteLine("[Ошибка] Неизвестный пункт меню. Попробуйте снова."); break;
    }
}

// 1. Вывод списка клиентов с сортировкой по фамилии и имени
async Task ListClientsAsync()
{
    Console.WriteLine("\n--- Список клиентов ---");
    await using var db = new FitnessContext();

    var clients = await db.Clients
        .AsNoTracking()
        .OrderBy(c => c.LastName)
        .ThenBy(c => c.FirstName)
        .ToListAsync();

    if (!clients.Any())
    {
        Console.WriteLine("В базе данных пока нет клиентов.");
        return;
    }

    foreach (var c in clients)
    {
        Console.WriteLine($"ID: {c.ClientId} | {c.LastName} {c.FirstName} | Статус: {c.Status} | Тел: {c.Phone ?? "не указан"}");
    }
}

// 2. Поиск клиента по идентификатору с проверкой на существование
async Task FindClientByIdAsync()
{
    Console.Write("\nВведите ID клиента: ");
    if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
    {
        Console.WriteLine("[Ошибка] ID должен быть положительным целым числом.");
        return;
    }

    await using var db = new FitnessContext();
    var client = await db.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == id);

    if (client is null)
    {
        Console.WriteLine($"[Инфо] Клиент с ID {id} не найден в системе.");
        return;
    }

    Console.WriteLine($"\n[Найдено] ID: {client.ClientId}");
    Console.WriteLine($"ФИО: {client.LastName} {client.FirstName}");
    Console.WriteLine($"Статус: {client.Status} | Дата регистрации: {client.RegistrationDate}");
    Console.WriteLine($"Телефон: {client.Phone ?? "не указан"} | Email: {client.Email ?? "не указан"}");
}

// 3. Поиск по фрагменту фамилии
async Task FindClientByLastNameAsync()
{
    Console.Write("\nВведите фрагмент фамилии для поиска: ");
    string term = Console.ReadLine()?.Trim() ?? string.Empty;

    if (string.IsNullOrEmpty(term))
    {
        Console.WriteLine("[Ошибка] Строка поиска не может быть пустой.");
        return;
    }

    await using var db = new FitnessContext();
    // Базовый поиск по подстроке (чувствителен к регистру в зависимости от настроек БД)
    var foundClients = await db.Clients
        .AsNoTracking()
        .Where(c => c.LastName.Contains(term))
        .OrderBy(c => c.LastName)
        .ToListAsync();

    if (!foundClients.Any())
    {
        Console.WriteLine($"По запросу '{term}' ничего не найдено.");
        return;
    }

    Console.WriteLine($"\nНайдено совпадений: {foundClients.Count}");
    foreach (var c in foundClients)
    {
        Console.WriteLine($"ID: {c.ClientId} | {c.LastName} {c.FirstName} (Тел: {c.Phone ?? "не указан"})");
    }
}

// 4. Добавить нового клиента с валидацией обязательных полей
async Task AddClientAsync()
{
    Console.WriteLine("\n--- Регистрация нового клиента ---");
    Console.Write("Имя (обязательно): ");
    string firstName = Console.ReadLine()?.Trim() ?? string.Empty;
    Console.Write("Фамилия (обязательно): ");
    string lastName = Console.ReadLine()?.Trim() ?? string.Empty;
    Console.Write("Телефон (необязательно): ");
    string? phone = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(phone)) phone = null;

    if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
    {
        Console.WriteLine("[Ошибка] Имя и Фамилия являются обязательными для заполнения!");
        return;
    }

    await using var db = new FitnessContext();
    var newClient = new Client
    {
        FirstName = firstName,
        LastName = lastName,
        Phone = phone,
        Status = "active", // Значение по умолчанию
        RegistrationDate = DateOnly.FromDateTime(DateTime.Today) // Текущая дата
    };

    try
    {
        db.Clients.Add(newClient);
        await db.SaveChangesAsync();
        Console.WriteLine($"[Успех] Клиент успешно зарегистрирован. Присвоен ID: {newClient.ClientId}");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine("[Ошибка] Не удалось сохранить клиента в базу данных.");
        Console.WriteLine($"Детали: {ex.GetBaseException().Message}");
    }
}

// 5. Изменение телефона или статуса существующего клиента
async Task UpdateClientAsync()
{
    Console.Write("\nВведите ID клиента для редактирования: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    await using var db = new FitnessContext();
    var client = await db.Clients.FirstOrDefaultAsync(c => c.ClientId == id);

    if (client is null)
    {
        Console.WriteLine("[Ошибка] Клиент с таким ID не найден.");
        return;
    }

    Console.WriteLine($"Редактируем клиента: {client.LastName} {client.FirstName}");
    Console.Write($"Новый телефон (оставьте пустым, чтобы оставить текущий '{client.Phone ?? "нет"}'): ");
    string newPhone = Console.ReadLine()?.Trim() ?? string.Empty;

    Console.Write($"Новый статус (active/inactive, оставьте пустым для сохранения текущего '{client.Status}'): ");
    string newStatus = Console.ReadLine()?.Trim() ?? string.Empty;

    if (!string.IsNullOrEmpty(newPhone)) client.Phone = newPhone;
    if (!string.IsNullOrEmpty(newStatus)) client.Status = newStatus;

    if (!string.IsNullOrEmpty(newPhone) || !string.IsNullOrEmpty(newStatus))
    {
        await db.SaveChangesAsync();
        Console.WriteLine("[Успех] Данные клиента успешно обновлены.");
    }
    else
    {
        Console.WriteLine("Изменений не внесено.");
    }
}

// 6. Удаление клиента с обработкой ошибок ограничений внешнего ключа (FK)
async Task DeleteClientAsync()
{
    Console.Write("\nВведите ID клиента для УДАЛЕНИЯ: ");
    if (!int.TryParse(Console.ReadLine(), out int id)) return;

    await using var db = new FitnessContext();
    var client = await db.Clients.FindAsync(id);

    if (client is null)
    {
        Console.WriteLine("[Ошибка] Клиент не найден.");
        return;
    }

    Console.Write($"Вы уверены, что хотите удалить клиента {client.LastName} {client.FirstName}? (y/n): ");
    if (Console.ReadLine()?.ToLower() != "y")
    {
        Console.WriteLine("Удаление отменено.");
        return;
    }

    try
    {
        db.Clients.Remove(client);
        await db.SaveChangesAsync();
        Console.WriteLine("[Успех] Клиент полностью удален из базы данных.");
    }
    catch (DbUpdateException ex)
    {
        Console.WriteLine("\n[Ошибка удаления] Нарушено ограничение целостности базы данных!");
        Console.WriteLine("Причина: У данного клиента есть связанные посещения в таблице визитов. Удаление заблокировано (DeleteBehavior.Restrict).");
        // Логируем системную ошибку для диагностики
        Console.WriteLine($"Системный лог: {ex.GetBaseException().Message}");
    }
}

// 7. Статистика (Общее количество клиентов и количество активных)
async Task ShowStatisticsAsync()
{
    Console.WriteLine("\n--- Статистика базы данных ---");
    await using var db = new FitnessContext();

    int totalClients = await db.Clients.CountAsync();
    int activeClients = await db.Clients.CountAsync(c => c.Status == "active");
    int totalVisits = await db.Visits.CountAsync();

    Console.WriteLine($"Всего зарегистрировано клиентов: {totalClients}");
    Console.WriteLine($"Из них имеют статус 'active'  : {activeClients}");
    Console.WriteLine($"Всего посещений зафиксировано : {totalVisits}");
}
