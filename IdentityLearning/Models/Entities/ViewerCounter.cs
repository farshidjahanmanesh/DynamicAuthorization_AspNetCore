using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityLearning.Models.Entities
{
    public class BrowserCounter:BrowserGraphData
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
    public class ViewerCounter
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
