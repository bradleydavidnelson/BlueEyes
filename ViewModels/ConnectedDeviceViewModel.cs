using BlueEyes.Models;
using System.Windows;

namespace BlueEyes.ViewModels
{
    class ConnectedDeviceViewModel
    {
        private BLEPeripheralCollection _connectedDevices = (BLEPeripheralCollection)Application.Current.FindResource("ConnectedPeripherals");

        public BLEPeripheralCollection ConnectedDevices
        {
            get { return _connectedDevices; }
            set { _connectedDevices = value; }
        }
    }
}
