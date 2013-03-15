using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AndroidBatteryStatus
{
    public partial class fmMain : Form, IDisplayReceiver
    {
        private DevicePoller _poller;

        public fmMain()
        {
            InitializeComponent();

            _poller = new DevicePoller(this);
        }

        #region IDisplayReceiver Members

        public void UpdateDisplay(DeviceInfo info)
        {
            if (lblTitle.InvokeRequired)
            {
                lblTitle.Invoke(new Action(
                    delegate() { UpdateDisplay(info); }
                ));
            }
            else
            {
                if (info.DeviceConnected)
                {
                    lblTitle.Text = "Battery Stats (" + info.Model + ")";
                    lblACPower.Text = info.ACPower ? "Yes" : "No";
                    lblUSBPower.Text = info.USBPower ? "Yes" : "No";
                    lblLevel.Text = info.PowerLevel.ToString() + "%";
                    lblLastChecked.Text = info.LastCheckedTimestamp;
                }
                else
                {
                    lblTitle.Text = "Battery Stats (No Device)";
                    lblACPower.Text = "--";
                    lblUSBPower.Text = "--";
                    lblLevel.Text = "--";
                    lblLastChecked.Text = info.LastCheckedTimestamp;
                }
            }
        }

        #endregion
    }
}
