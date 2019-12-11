using BlueEyes.Models;
using BlueEyes.Utilities;
using BlueEyes.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlueEyes.ViewModels
{
    class ConnectedDeviceViewModel : BindableBase
    {
        private BLEPeripheralCollection _connectedDevices = (BLEPeripheralCollection)Application.Current.FindResource("ConnectedPeripherals");
        private GATTViewModel _GATTVM = new GATTViewModel();

        public BLEPeripheralCollection ConnectedDevices
        {
            get { return _connectedDevices; }
            set { _connectedDevices = value; }
        }

        public GATTViewModel GATT
        {
            get { return _GATTVM; }
            set { SetProperty(ref _GATTVM, value); }
        }
    }
}
