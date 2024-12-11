using System.Numerics;
using Lab5.Network.Common;
using Lab5.Network.Common.CarApi;
using Lab5.Network.Common.UserApi;

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
