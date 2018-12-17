using BlueEyes.Models;
using BlueEyes.Utilities;
using BlueEyes.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BlueEyes.ViewModels
{
    class ConnectedDeviceViewModel
    {
        private BLEPeripheralCollection _connectedDevices = (BLEPeripheralCollection)Application.Current.FindResource("ConnectedPeripherals");
        private ICommand selectColumnsCommand;

        public BLEPeripheralCollection ConnectedDevices
        {
            get { return _connectedDevices; }
            set { _connectedDevices = value; }
        }

        public ICommand SelectColumnsCommand
        {
            get
            {
                if (selectColumnsCommand == null)
                {
                    selectColumnsCommand = new RelayCommand(OpenColumnSelector);
                }
                return selectColumnsCommand;
            }
        }

        private void OpenColumnSelector(object obj)
        {
            Window selector = new DataDisplaySelectorView();
            selector.Show();
        }
    }
}
