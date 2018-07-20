using BlueEyes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlueEyes.ViewModels
{
    class DiscoveredDeviceViewModel
    {
        private BLEPeripheralCollection _discoveredDevices = (BLEPeripheralCollection)Application.Current.FindResource("BLEPeripherals");

        public BLEPeripheralCollection DiscoveredDevices
        {
            get { return _discoveredDevices; }
            set { _discoveredDevices = value; }
        }
    }
}
