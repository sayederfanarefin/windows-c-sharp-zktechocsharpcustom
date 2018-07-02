using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Info
{
    internal class Device
    {
        public string DeviceName { get; set; }
        public string DeviceId { get; set; }
        public string IP { get; set; }
        public string Port { get; set; }
        public string Mac { get; set; }
        public Int64 Id { get; set; }
        public DateTime updatedAt { get; set; }
        public Boolean status { get; set; }
    }
}
