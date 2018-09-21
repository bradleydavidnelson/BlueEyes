using BlueEyes.Models;
using BlueEyes.Utilities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace BlueEyes.ViewModels
{
    class CalibrateRheostatViewModel : BindableBase
    {
        #region Fields
        private Bluegiga.BGLib bglib = (Bluegiga.BGLib)Application.Current.FindResource("BGLib");
        private BLEPeripheralCollection _connectedPeripherals = (BLEPeripheralCollection)Application.Current.FindResource("ConnectedPeripherals");

        private DispatcherTimer _sliderTimer = new DispatcherTimer();
        private ushort _rheostatMax = 1023;
        private ushort _rheostatPrev = 0;
        private ushort _rheostatValue = 0;
        private BLEPeripheral _selectedPeripheral = null;
        private ICommand _saveValueCommand;
        private ICommand _sendValueCommand;
        private ICommand _sliderDelayCommand;
        #endregion

        #region Constructors
        public CalibrateRheostatViewModel()
        {
            SelectedPeripheral = ConnectedPeripherals[0];

            _sliderTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            _sliderTimer.Tick += CheckSlider;
        }
        #endregion

        #region Properties
        public BLEPeripheralCollection ConnectedPeripherals
        {
            get { return _connectedPeripherals; }
            private set { SetProperty(ref _connectedPeripherals, value); }
        }

        public bool HasSaveCharacteristic
        {
            get
            {
                if (_selectedPeripheral == null)
                    return false;
                return (_selectedPeripheral.Characteristics.ContainsKey("RheostatSaveCommand"));
            }
        }

        public ushort RheostatMax
        {
            get { return _rheostatMax; }
            private set { SetProperty(ref _rheostatMax, value); }
        }
        
        public ushort RheostatValue
        {
            get { return _rheostatValue; }
            private set { SetProperty(ref _rheostatValue, value); }
        }

        public BLEPeripheral SelectedPeripheral
        {
            get { return _selectedPeripheral; }
            set {
                SetProperty(ref _selectedPeripheral, value);
                NotifyPropertyChanged("HasSaveCharacteristic");
            }
        }

        public ICommand SaveValueCommand
        {
            get
            {
                if (_saveValueCommand == null)
                {
                    _saveValueCommand = new RelayCommand(SaveValue);
                }
                return _saveValueCommand;
            }
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

        public ICommand SliderDelayCommand
        {
            get
            {
                if (_sliderDelayCommand == null)
                {
                    _sliderDelayCommand = new RelayCommand(SliderDelay);
                }
                return _sliderDelayCommand;
            }
        }
        #endregion

        #region Methods
        private void CheckSlider(object sender, EventArgs e)
        {
            _sliderTimer.Stop();

            if (RheostatValue != _rheostatPrev)
            {
                SendValue(null);
            }
        }

        private void SaveValue(object obj)
        {
            // Save value
            if (SelectedPeripheral.Characteristics.ContainsKey("RheostatSaveCommand"))
            {
                byte connection = SelectedPeripheral.Connection;
                ushort handle = SelectedPeripheral.Services["RheostatCalibrationService"].Characteristics["RheostatSaveCommand"].ValueAttribute.Handle;
                byte[] value = new byte[] { 0x01 };
                byte[] cmd = bglib.BLECommandATTClientAttributeWrite(connection, handle, value);
                MessageWriter.LogWrite("ble_cmd_att_client_attribute_write: ", string.Format("handle={0}, att_handle={1}, data={2}",
                    connection,
                    handle,
                    BitConverter.ToString(value)));
                MessageWriter.BLEWrite(cmd);
            }
        }

        private void SendValue(object obj)
        {
            _rheostatPrev = RheostatValue;
            // Convert to byte array
            byte[] value = BitConverter.GetBytes(RheostatValue);
            if (BitConverter.IsLittleEndian)
            {
                value = value.Reverse().ToArray();
            }

            // Set command bits to write to wiper (command 1)
            value[0] = (byte)(value[0] | 0x04);

            // Write rheostat value to peripheral
            if (SelectedPeripheral.Services.ContainsKey("RheostatCalibrationService"))
            {
                byte connection = SelectedPeripheral.Connection;
                ushort handle = SelectedPeripheral.Services["RheostatCalibrationService"].Characteristics["RheostatValue"].ValueAttribute.Handle;
                byte[] cmd = bglib.BLECommandATTClientAttributeWrite(connection, handle, value);
                MessageWriter.LogWrite("ble_cmd_att_client_attribute_write: ", string.Format("handle={0}, att_handle={1}, data={2}",
                    connection,
                    handle,
                    BitConverter.ToString(value)));
                MessageWriter.BLEWrite(cmd);
            }
            else
            {   // Legacy Write to rheostat (TODO V3 remove)
                byte connection = SelectedPeripheral.Connection;
                ushort handle = SelectedPeripheral.Characteristics["RheostatValue"].ValueAttribute.Handle;
                byte[] cmd = bglib.BLECommandATTClientAttributeWrite(connection, handle, value);
                MessageWriter.LogWrite("ble_cmd_att_client_attribute_write: ", string.Format("handle={0}, att_handle={1}, data={2}",
                    connection,
                    handle,
                    BitConverter.ToString(value)));
                MessageWriter.BLEWrite(cmd);
                return;
            }
        }

        private void SliderDelay(object obj)
        {
            _sliderTimer.IsEnabled = true;
        }
        #endregion
    }
}
