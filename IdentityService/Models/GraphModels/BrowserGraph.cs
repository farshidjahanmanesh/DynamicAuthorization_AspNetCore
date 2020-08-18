using System;

namespace SharedServices.GraphModel
{
    [Serializable]
    public class BrowserGraph
    {
        public int FireFox { get; set; }
        public int Safari { get; set; }
        public int Edge { get; set; }
        public int IE { get; set; }
        public int Chorome { get; set; }
        public int Other { get; set; }
    }
}
