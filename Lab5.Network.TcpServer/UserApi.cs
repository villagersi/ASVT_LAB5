using System.Collections.Concurrent;
using Lab5.Network.Common.CarApi;

public class CarApi : ICarApi
{
    private static readonly ConcurrentDictionary<int, Car> carRepository
        = new ConcurrentDictionary<int, Car>()
        {
            [1] = new Car(1, "Model S", "Tesla", 2020, true),
            [2] = new Car(2, "Mustang", "Ford", 2019, true),
            [3] = new Car(3, "Civic", "Honda", 2021, true),
        };
    private static int _lastId;

    public CarApi()
    {
        _lastId = carRepository.Count + 1;
    }

    public Task<bool> AddAsync(Car newCar)
    {
        newCar.Id = _lastId;

        if (carRepository.ContainsKey(_lastId))
        {
            return Task.FromResult(false);
        }

        var result = carRepository.TryAdd(_lastId, newCar);

        if (!result)
        {
            return Task.FromResult(false);
        }

        Interlocked.Increment(ref _lastId);
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        if (!carRepository.ContainsKey(id))
        {
            return Task.FromResult(false);
        }

        var result = carRepository.Remove(id, out _);
        return Task.FromResult(result);
    }

    public Task<Car[]> GetAllAsync()
    {
        return Task.FromResult(carRepository.Values.ToArray());
    }

    public Task<Car?> GetAsync(int id)
    {
        if (!carRepository.ContainsKey(id))
        {
            return Task.FromResult(default(Car));
        }

        return Task.FromResult<Car?>(carRepository[id]);
    }

    public Task<bool> UpdateAsync(int id, Car updateCar)
    {
        if (!carRepository.ContainsKey(id))
        {
            return Task.FromResult(false);
        }

        var result = carRepository.TryUpdate(id, updateCar, carRepository[id]);
        return Task.FromResult(result);
    }
}
