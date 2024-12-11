using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
