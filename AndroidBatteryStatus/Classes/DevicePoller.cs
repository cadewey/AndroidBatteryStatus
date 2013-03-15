using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AndroidBatteryStatus
{
    public class DevicePoller
    {
        //Settings for the poller
        public int PollingInterval = 1000 * 60  * 5;

        private object _lockObj = new object();
        private DeviceInfo _info;
        private IDisplayReceiver _receiver;

        private Regex _reModel = new Regex(@"ro\.product\.model=(.*)");
        private Regex _reACPower = new Regex(@"\s*AC powered: (true|false)");
        private Regex _reUSBPower = new Regex(@"\s*USB powered: (true|false)");
        private Regex _rePowerLevel = new Regex(@"\s*level: (\d+)");

        Dictionary<Regex, KeyValuePair<string, Type>> _regexMapping;

        private System.Threading.Timer _pollingTimer;

        public DevicePoller(IDisplayReceiver recv)
        {
            _receiver = recv;

            _info = new DeviceInfo();
            _pollingTimer = new System.Threading.Timer(new System.Threading.TimerCallback(PollingIntervalElapsed), null, 0, PollingInterval);

            _regexMapping = new Dictionary<Regex, KeyValuePair<string, Type>>() { 
                { _rePowerLevel, new KeyValuePair<string, Type>("PowerLevel", typeof(int))}, 
                { _reACPower, new KeyValuePair<string, Type>("ACPower", typeof(bool)) },
                { _reUSBPower, new KeyValuePair<string, Type>("USBPower", typeof(bool)) }
            };
        }

        public DevicePoller(IDisplayReceiver recv, int interval)
        {
            _receiver = recv;
            PollingInterval = 1000 * 60 * interval;

            _info = new DeviceInfo();
            _pollingTimer = new System.Threading.Timer(new System.Threading.TimerCallback(PollingIntervalElapsed), null, 0, PollingInterval);

            _regexMapping = new Dictionary<Regex, KeyValuePair<string, Type>>() { 
                { _rePowerLevel, new KeyValuePair<string, Type>("PowerLevel", typeof(int))}, 
                { _reACPower, new KeyValuePair<string, Type>("ACPower", typeof(bool)) },
                { _reUSBPower, new KeyValuePair<string, Type>("USBPower", typeof(bool)) }
            };
        }

        private void DoPoll()
        {
            lock (_lockObj)
            {
                if (!PollForModel())
                {
                    _info.Reset();
                }

                PollForBatteryInfo();

                _info.LastCheckedTimestamp = DateTime.Now.ToString(@"MM/dd/yyyy h:mm:ss tt");
                _receiver.UpdateDisplay(_info);
            }
        }

        private bool PollForModel()
        {
            string buildProp = new AdbCommand().RunCommand(@"shell cat /system/build.prop");

            if (String.IsNullOrEmpty(buildProp) || buildProp.StartsWith("ERROR"))
                return false;

            var m = _reModel.Matches(buildProp);

            if (m == null || m.Count == 0 || m[0].Groups.Count < 2)
                return false;

            _info.Model = m[0].Groups[1].Value.Trim();
            _info.DeviceConnected = true;

            return true;
        }

        private void PollForBatteryInfo()
        {
            
            string batteryInfo = new AdbCommand().RunCommand(@"shell dumpsys battery");

            if (String.IsNullOrEmpty(batteryInfo) || batteryInfo.StartsWith("ERROR"))
                return;

            foreach (KeyValuePair<Regex, KeyValuePair<string, Type>> kvp in _regexMapping)
            {
                var m = kvp.Key.Matches(batteryInfo);

                if (m == null || m.Count == 0 || m[0].Groups.Count < 2)
                    continue;

                string value = m[0].Groups[1].Value.Trim();
                _info.GetType().GetProperty(kvp.Value.Key).SetValue(_info, System.Convert.ChangeType(value, kvp.Value.Value), null);
            }
        }

        private void PollingIntervalElapsed(object o)
        {
            DoPoll();
        }
    }
}
