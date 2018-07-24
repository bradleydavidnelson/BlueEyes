using BlueEyes.Models;
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
