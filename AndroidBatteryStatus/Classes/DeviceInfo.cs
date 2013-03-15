using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AndroidBatteryStatus
{
    public class DeviceInfo
    {
        //Attributes that contain data from the plugged device
        public bool DeviceConnected { get; set; }
        public int PowerLevel { get; set; }
        public bool ACPower { get; set; }
        public bool USBPower { get; set; }
        public string Model { get; set; }
        public string LastCheckedTimestamp { get; set; }

        public DeviceInfo()
        {
            Reset();
        }

        public void Reset()
        {
            Model = "No Device";
            LastCheckedTimestamp = "--";
            ACPower = false;
            USBPower = false;
            PowerLevel = 0;
            DeviceConnected = false;
        }
    }
}
