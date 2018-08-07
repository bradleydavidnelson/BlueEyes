using BlueEyes.Models;
using BlueEyes.Utilities;
using System.Windows;
using System.Windows.Input;

namespace BlueEyes.ViewModels
{
    class CalibrateRheostatViewModel : BindableBase
    {
        #region Fields
        private BLEPeripheralCollection _connectedPeripherals = (BLEPeripheralCollection)Application.Current.FindResource("ConnectedPeripherals");
        private BLEPeripheral _selectedPeripheral = null;
        private ICommand _sendValueCommand;
        #endregion

        #region Properties
        public BLEPeripheralCollection ConnectedPeripherals
        {
            get { return _connectedPeripherals; }
            set { SetProperty(ref _connectedPeripherals, value); }
        }

        public BLEPeripheral SelectedPeripheral
        {
            get { return _selectedPeripheral; }
            set { SetProperty(ref _selectedPeripheral, value); }
        }

        public ICommand SendValueCommand
        {
            get
            {
                if (_sendValueCommand == null)
                {
                    _sendValueCommand = new RelayCommand(SendValue);
                }
                return _sendValueCommand;
            }
        }
        #endregion

        #region Methods
        private void SendValue(object obj)
        {
            // TODO transmit calibration value
        }
        #endregion
    }
}
