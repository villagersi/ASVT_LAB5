using System.Text.Json;
using Lab5.Network.Common;
using Lab5.Network.Common.CarApi;
using Lab5.Network.Common.UserApi;

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
        var command = new Command()
        {
            Code = (byte)CommandCode.EditCar,
            Arguments = new Dictionary<string, object?>()
            {
                ["Id"] = id,
                ["Data"] = updateCar
            }
        };

        var result = await netTcpClient.SendAsync(command);

        return result != null && result.Arguments["Data"]?.ToString() == "true";
    }
}