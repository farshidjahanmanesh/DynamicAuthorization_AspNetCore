using System;

namespace IdentityLearning.Models
{


    [Serializable]
    public class DeviceGraphData
    {
        public int IOS { get; set; }
        public int Android { get; set; }
        public int Desktop { get; set; }
        public int Other { get; set; }
    }
}
