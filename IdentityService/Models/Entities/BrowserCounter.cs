
using SharedServices.GraphModel;
using System;

namespace SharedServices.Models.Entities
{
    public class BrowserCounter:BrowserGraph
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
}
