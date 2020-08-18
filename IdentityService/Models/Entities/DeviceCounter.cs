
using SharedServices.GraphModel;
using System;

namespace SharedServices.Models.Entities
{
    public class DeviceCounter:DeviceGraph
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
}
