using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
