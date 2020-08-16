using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityLearning.Models
{
    [Serializable]
    public class ViewerVisitorGraphData
    {
        public ViewerVisitorGraphData()
        {
            Counts = new List<int>();
            Dates = new List<string>();
        }
        public List<int> Counts { get; set; }
        public List<string> Dates { get; set; }
    }
}
