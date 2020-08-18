using System;

namespace SharedServices.GraphModel
{
    [Serializable]
    public class DeviceGraph
    {
        public int IOS { get; set; }
        public int Android { get; set; }
        public int Desktop { get; set; }
        public int Other { get; set; }
    }
}
