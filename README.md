# Описание проекта
Данный проект представляет собой реализацию API для управления автомобилями и отправки сообщений через сетевые протоколы TCP и UDP. Он включает в себя классы, интерфейсы и серверную логику для работы с данными о автомобилях и обмена сообщениями.
# Структура проекта
## Общие компоненты
Класс Car
```c#
using System;

namespace Lab5.Network.Common.CarApi
{
    public class Car
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Manufacturer { get; set; }
        public int Year { get; set; }
        public bool Available { get; set; }

        public Car(int id, string model, string manufacturer, int year, bool available)
        {
            Id = id;
            Model = model;
            Manufacturer = manufacturer;
            Year = year;
            Available = available;
        }
    }
}
```
Описание: Класс Car представляет собой модель автомобиля. Он содержит свойства, такие как Id, Model, Manufacturer, Year и Available, которые описывают характеристики автомобиля. Конструктор класса позволяет инициализировать эти свойства при создании нового объекта.

Интерфейс ICarApi
```c#
using System.Threading.Tasks;

namespace Lab5.Network.Common.CarApi
{
    public interface ICarApi
    {
        Task<Car[]> GetAllAsync();
        Task<Car?> GetAsync(int id);
        Task<bool> AddAsync(Car newCar);
        Task<bool> UpdateAsync(int id, Car updateCar);
        Task<bool> DeleteAsync(int id);
    }
}
```
Описание: Интерфейс ICarApi определяет методы для взаимодействия с данными о автомобилях. Методы включают:
GetAllAsync(): получение всех автомобилей.
GetAsync(int id): получение автомобиля по его идентификатору.
AddAsync(Car newCar): добавление нового автомобиля.
UpdateAsync(int id, Car updateCar): обновление данных существующего автомобиля.
DeleteAsync(int id): удаление автомобиля по его идентификатору.

Интерфейс IUserApi
```c#
namespace Lab5.Network.Common.UserApi
{
    public interface IUserApi 
    {
        Task<bool> AddAsync(User newUser);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateAsync(int id, User updateUser);
        Task<User?> GetAsync(int id);
        Task<User[]> GetAllAsync();
    }
}
```
Описание: Интерфейс IUserApi предназначен для управления пользователями. Он включает методы для добавления, удаления, обновления и получения пользователей.

Перечисление CommandCode
```c#
namespace Lab5.Network.Common
{
    public enum CommandCode : byte
    {
        ReadCar,
        ReadAllCars,
        AddCar,
        EditCar,
        DeleteCar,
        SendMessage
    }
}
```
Описание: Перечисление CommandCode определяет набор команд, которые могут быть отправлены через API. Каждая команда соответствует определённому действию, например, чтение данных о автомобиле или отправка сообщения.

Интерфейс IMessageApi
```c#
namespace Lab5.Network.Common
{
    public interface IMessageApi
    {
        Task<bool> SendMessage(string message);
    }
}
```
Описание: Интерфейс IMessageApi определяет метод для отправки текстовых сообщений. Этот интерфейс используется в реализации UDP.

## TCP реализация
Класс Program (Клиент)
```c#
using System;
using System.Threading.Tasks;
using Lab5.Network.Common;
using Lab5.Network.Common.CarApi;

internal class Program
{
    private static object _locker = new object();

    public static async Task Main(string[] args)
    {
        var serverAddress = new Uri("tcp://127.0.0.1:5555");
        var client = new NetTcpClient(serverAddress);

        Console.WriteLine($"Connect to server at {serverAddress}");

        await client.ConnectAsync();

        var carApi = new CarApiClient(client);

        await ManageCars(carApi);
        client.Dispose();
    }

    private static async Task ManageCars(ICarApi carApi)
    {
        PrintMenu();

        while (true)
        {
            var key = Console.ReadKey(true);

            PrintMenu();

            if (key.Key == ConsoleKey.D1)
            {
                var cars = await carApi.GetAllAsync();
                Console.WriteLine($"| Id |     Model     | Manufacturer | Year | Available |");
                foreach (var car in cars)
                {
                    Console.WriteLine($"| {car.Id,2} | {car.Model,12} | {car.Manufacturer,12} | {car.Year,4} | {car.Available,9} |");
                }
            }

            if (key.Key == ConsoleKey.D2)
            {
                Console.Write("Enter car id: ");
                var carIdString = Console.ReadLine();
                int.TryParse(carIdString, out var carId);
                var car = await carApi.GetAsync(carId);
                Console.WriteLine($"Id={car?.Id}, Model={car?.Model}, Manufacturer={car?.Manufacturer}, Year={car?.Year}, Available={car?.Available}");
            }

            if (key.Key == ConsoleKey.D3)
            {
                Console.Write("Enter car model: ");
                var modelName = Console.ReadLine() ?? "empty";

                Console.Write("Enter manufacturer: ");
                var manufacturerName = Console.ReadLine() ?? "unknown";

                Console.Write("Enter year: ");
                int.TryParse(Console.ReadLine(), out var year);

                var newCar = new Car(
                    id: 0,
                    model: modelName,
                    manufacturer: manufacturerName,
                    year: year,
                    available: true);

                var addResult = await carApi.AddAsync(newCar);
                Console.WriteLine(addResult ? "Car added successfully." : "Error adding car.");
            }

            if (key.Key == ConsoleKey.D4)
            {
                Console.Write("Enter car id to update: ");
                var carIdString = Console.ReadLine();
                int.TryParse(carIdString, out var carId);

                // Получаем текущие данные автомобиля
                var currentCar = await carApi.GetAsync(carId);
                if (currentCar != null)
                {
                    Console.Write($"Current Model ({currentCar.Model}): ");
                    var modelName = Console.ReadLine() ?? currentCar.Model;

                    Console.Write($"Current Manufacturer ({currentCar.Manufacturer}): ");
                    var manufacturerName = Console.ReadLine() ?? currentCar.Manufacturer;

                    Console.Write($"Current Year ({currentCar.Year}): ");
                    int.TryParse(Console.ReadLine(), out var year);
                    year = year == 0 ? currentCar.Year : year;

                    var updatedCar = new Car(id: carId,
                        model: modelName,
                        manufacturer: manufacturerName,
                        year: year,
                        available: currentCar.Available);

                    var updateResult = await carApi.UpdateAsync(carId, updatedCar);
                    Console.WriteLine(updateResult ? "Car updated successfully." : "Error updating car.");
                }
                else
                {
                    Console.WriteLine("Car not found.");
                }
            }

            if (key.Key == ConsoleKey.D5)
            {
                Console.Write("Enter car id to delete: ");
                var carIdString = Console.ReadLine();
                int.TryParse(carIdString, out var carId);

                var deleteResult = await carApi.DeleteAsync(carId);
                Console.WriteLine(deleteResult ? "Car deleted successfully." : "Error deleting car.");
            }

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }

        Console.ReadKey();
    }

    private static void PrintMenu()
    {
        lock (_locker)
        {
            Console.WriteLine("1 - Get all cars");
            Console.WriteLine("2 - Get car by id");
            Console.WriteLine("3 - Create car");
            Console.WriteLine("4 - Update car");
            Console.WriteLine("5 - Delete car");
            Console.WriteLine("Press Escape to exit.");
        }
    }
}
```
Описание: Этот класс реализует клиентскую часть приложения на основе TCP. Он подключается к серверу по указанному адресу и предоставляет пользователю меню для управления автомобилями.
Метод Main устанавливает соединение с сервером и вызывает метод управления автомобилями.
Метод ManageCars обрабатывает пользовательский ввод и выполняет соответствующие действия (получение всех автомобилей, добавление нового автомобиля, обновление или удаление существующего).
Метод PrintMenu отображает доступные команды на консоли.

Класс CarApiClient
```c#
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Lab5.Network.Common;
using Lab5.Network.Common.CarApi;

public class CarApiClient : ICarApi
{
    private readonly NetTcpClient netTcpClient;

    public CarApiClient(NetTcpClient netTcpClient)
    {
        this.netTcpClient = netTcpClient;
    }

    public async Task<bool> AddAsync(Car newCar)
    {
        var command = new Command()
        {
            Code = (byte)CommandCode.AddCar,
            Arguments = new Dictionary<string, object?>()
            {
                ["Data"] = newCar
            }
        };

        var result = await netTcpClient.SendAsync(command);

        if (result == null)
        {
            return false;
        }

        var addResult = result.Arguments["Data"]?.ToString();
        return bool.TryParse(addResult, out var addResultValue) && addResultValue;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var command = new Command()
        {
            Code = (byte)CommandCode.DeleteCar,
            Arguments = new Dictionary<string, object?>()
            {
                ["Id"] = id
            }
        };

        var result = await netTcpClient.SendAsync(command);
        return result != null && result.Arguments["Data"]?.ToString() == "true";
    }

    public async Task<Car[]> GetAllAsync()
    {
        var command = new Command()
        {
            Code = (byte)CommandCode.ReadAllCars
        };

        var result = await netTcpClient.SendAsync(command);

        if (result == null)
        {
            return Array.Empty<Car>();
        }

        var carsJson = result.Arguments["Data"]?.ToString();
        return carsJson != null ?
            JsonSerializer.Deserialize<Car[]>(carsJson)! :
            Array.Empty<Car>();
    }

    public async Task<Car?> GetAsync(int id)
    {
        var command = new Command()
        {
            Code = (byte)CommandCode.ReadCar,
            Arguments = new Dictionary<string, object?>()
            {
                ["Id"] = id
            }
        };

        var result = await netTcpClient.SendAsync(command);

        if (result == null)
        {
            return null;
        }

        var carJson = result.Arguments["Data"]?.ToString();
        return carJson != null ?
            JsonSerializer.Deserialize<Car>(carJson) : null;
    }

    public async Task<bool> UpdateAsync(int id, Car updateCar)
    {
       ...
       // Код аналогичен коду выше для обновления автомобиля.
       ...
   }
}
```
Описание: Класс CarApiClient реализует интерфейс ICarApi и отвечает за взаимодействие с TCP-сервером. Он отправляет команды серверу и обрабатывает ответы.
Каждый метод формирует команду с соответствующим кодом и аргументами, отправляет её на сервер и обрабатывает результат.

## UDP реализация
Класс Program (UDP клиент)
```c#
// See https://aka.ms/new-console-template for more information
using Lab5.Network.Common;

internal class Program
{
    private static object _locker = new object();

    private static async Task Main(string[] args)
    {
        var serverAdress = new Uri("udp://127.0.0.1:7777");
        var client = new NetUdpClient(serverAdress);
        Console.WriteLine($"Connect to server at {serverAdress}");

        var messageApi = new MessageApiClient(client);
        await ManageMessages(messageApi);
        client.Dispose();
    }

    private static async Task ManageMessages(IMessageApi messageApi)
    {
        PrintMenu();

        while (true)
        {
            var key = Console.ReadKey(true);

            PrintMenu();

            if (key.Key == ConsoleKey.D1)
            {
                Console.Write("Enter message: ");
                var message = Console.ReadLine() ?? string.Empty;
                await messageApi.SendMessage(message);
                Console.WriteLine($"Message sent: {message}");
            }

            if (key.Key == ConsoleKey.D2)
            {
                Console.Write("Enter your wish!!!: ");
                var message = Console.ReadLine() ?? string.Empty;
                await messageApi.SendMessage(message);
                Console.WriteLine($"Your message sent to Santa Claus: {message}");
            }

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
        Console.ReadKey();
    }

    private static void PrintMenu()
    {
        lock (_locker)
        {
            Console.WriteLine("1 - Send message");
            Console.WriteLine("2 - Send wish to Santa");
            Console.WriteLine("-------");
        }
    }
}
```
Описание: Этот класс реализует клиентскую часть приложения на основе UDP. Он подключается к серверу по указанному адресу и предоставляет пользователю возможность отправлять сообщения.
Метод Main устанавливает соединение с сервером и вызывает метод управления сообщениями.
Метод ManageMessages обрабатывает пользовательский ввод и отправляет сообщения на сервер.
Метод PrintMenu отображает доступные команды на консоли.

## Заключение
Этот проект демонстрирует основы работы с сетевыми протоколами TCP и UDP в C#. Он может быть расширен для поддержки дополнительных функций или интеграции с другими системами.
