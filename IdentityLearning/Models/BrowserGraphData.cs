using System;

namespace IdentityLearning.Models
{
    [Serializable]
    public class BrowserGraphData
    {
        public int FireFox { get; set; }
        public int Safari { get; set; }
        public int Edge { get; set; }
        public int IE { get; set; }
        public int Chorome { get; set; }
        public int Other { get; set; }
    }
}
