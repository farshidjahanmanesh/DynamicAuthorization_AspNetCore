using System;
using System.Collections.Generic;

namespace SharedServices.GraphModel
{
    [Serializable]
    public class ViewerVisitorGraph
    {
        public ViewerVisitorGraph()
        {
            Counts = new List<int>();
            Dates = new List<string>();
        }
        public List<int> Counts { get; set; }
        public List<string> Dates { get; set; }
    }
}
