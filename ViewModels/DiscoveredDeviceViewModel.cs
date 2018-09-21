using BlueEyes.Models;
using System.Windows;
using System.Windows.Input;

namespace BlueEyes.ViewModels
{
    class DiscoveredDeviceViewModel
    {
        private BLEPeripheralCollection _discoveredDevices = (BLEPeripheralCollection)Application.Current.FindResource("DiscoveredPeripherals");

        public BLEPeripheralCollection DiscoveredDevices
        {
            get { return _discoveredDevices; }
            set { _discoveredDevices = value; }
        }
    }
}
