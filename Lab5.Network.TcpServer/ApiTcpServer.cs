using System.Text.Json;
using Lab5.Network.Common;
using Lab5.Network.Common.CarApi;

public class ApiTcpServer : TcpServerBase
{
    private readonly CarApi carApi;

    public ApiTcpServer(CarApi carApi, Uri listAddress)
        : base(listAddress)                                                         
    {
        this.carApi = carApi;
    }

    protected override async Task<Command> ProcessCommandAsync(Command command)
    {
        var commandCode = (CommandCode)command!.Code;
        Console.WriteLine($"+ command: {commandCode}");
        switch (commandCode)
        {
            case CommandCode.AddCar:
                var carData = command.Arguments["Data"]?.ToString() ?? "{}";
                var addCar = JsonSerializer.Deserialize<Car>(carData);
                var addResult = await carApi.AddAsync(addCar!);
                return new Command()
                {
                    Code = (byte)CommandCode.AddCar,
                    Arguments = new Dictionary<string, object?>()
                    {
                        ["Data"] = addResult
                    }
                };
            case CommandCode.ReadCar:
                var id = command.Arguments["Id"]?.ToString() ?? "1";
                var carId = Convert.ToInt32(id);
                var car = await carApi.GetAsync(carId);
                return new Command()
                {
                    Code = (byte)CommandCode.ReadCar,
                    Arguments = new Dictionary<string, object?>()
                    {
                        ["Data"] = car
                    }
                };
            case CommandCode.ReadAllCars:
                var cars = await carApi.GetAllAsync();
                return new Command()
                {
                    Code = (byte)CommandCode.ReadAllCars,
                    Arguments = new Dictionary<string, object?>()
                    {
                        ["Data"] = cars
                    }
                };
            case CommandCode.EditCar:
                var editCarId = Convert.ToInt32(command.Arguments["Id"]);
                var editCarData = command.Arguments["Data"]?.ToString() ?? "{}";
                var editCar = JsonSerializer.Deserialize<Car>(editCarData);
                var updateResult = await carApi.UpdateAsync(editCarId, editCar!);
                return new Command()
                {
                    Code = (byte)CommandCode.EditCar,
                    Arguments = new Dictionary<string, object?>()
                    {
                        ["Data"] = updateResult
                    }
                };
            case CommandCode.DeleteCar:
                var deleteCarId = Convert.ToInt32(command.Arguments["Id"]);
                var deleteResult = await carApi.DeleteAsync(deleteCarId);
                return new Command()
                {
                    Code = (byte)CommandCode.DeleteCar,
                    Arguments = new Dictionary<string, object?>()
                    {
                        ["Data"] = deleteResult
                    }
                };
            default:
                return new Command()
                {
                    Code = 0,
                    Arguments = new Dictionary<string, object?>()
                    {
                        ["Error"] = $"error code {command!.Code}"
                    }
                };
        }
    }
}
