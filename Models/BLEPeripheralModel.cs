using BlueEyes.Utilities;
using BlueEyes.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Input;

namespace BlueEyes.Models
{
    public class BLEPeripheralModel { }

    public class BLEPeripheral : BindableBase
    {
        #region Fields
        private Bluegiga.BGLib bglib = (Bluegiga.BGLib)Application.Current.FindResource("BGLib");
        private SerialPortModel port = (SerialPortModel)Application.Current.FindResource("Port");

        private byte[] _address;
        private byte _addrType;
        private byte _connection;
        private ConnState _connectionState = ConnState.Disconnected;
        private ICommand _connectCommand;
        private bool _isConnected;
        private string _name;

        public enum ConnState { Disconnected, Connecting, Connected };
        #endregion

        #region Constructors
        public BLEPeripheral(byte[] address)
        {
            _address = address;
            NotifyPropertyChanged("Address");
            NotifyPropertyChanged("AddressString");
        }
        #endregion

        #region Properties
        public byte[] Address
        {
            get { return _address; }
            set
            {
                _address = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("AddressString");
            }
        }

        public byte AddressType
        {
            get { return _addrType; }
            set { SetProperty(ref _addrType, value); }
        }
        
        public string AddressString
        {
            get { return BitConverter.ToString(_address); }
        }

        public bool AttributesFound { get; set; }

        public string ConnectAction
        {
            get
            {
                if (_connectionState == ConnState.Disconnected)
                    return "Connect";

                else if (_connectionState == ConnState.Connected)
                    return "Disconnect";

                else
                    return "Connecting";
            }
        }

        public ICommand ConnectCommand
        {
            get
            {
                if (_connectCommand == null)
                {
                    _connectCommand = new RelayCommand(ConnectDisconnect);
                }
                return _connectCommand;
            }
            set { _connectCommand = value; }
        }

        public byte Connection
        {
            get { return _connection; }
            set { SetProperty(ref _connection, value); }
        }

        public ConnState ConnectionState
        {
            get { return _connectionState; }
            set
            {
                _connectionState = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ConnectAction");
                if (value == ConnState.Connected)
                {
                    _isConnected = true;
                }
                else
                {
                    _isConnected = false;
                }
                NotifyPropertyChanged("IsConnected");
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }
        #endregion

        #region Methods
        public void Connect()
        {
            // Connection parameters
            float conn_interval_min = 40; // in ms
            float conn_interval_max = 60; // in ms
            float timeout = 2560; // in ms
            byte latency = 0;

            // Form a direct connection
            byte[] cmd = bglib.BLECommandGAPConnectDirect(
                Address,
                AddressType,
                (ushort)(conn_interval_min / 1.25F),
                (ushort)(conn_interval_max / 1.25F),
                (ushort)(timeout / 10F),
                latency); // 125ms interval, 125ms window, active scanning
            MessageWriter.LogWrite(string.Format("ble_cmd_gap_connect_direct: adress={0}, address_type={1}, conn_interval_min={2}; conn_interval_max={3}, timeout={4}, latency={5}",
                BitConverter.ToString(Address),
                AddressType,
                conn_interval_min,
                conn_interval_max,
                timeout,
                latency));
            bglib.SendCommand(port.ToSerialPort(), cmd);

            ConnectionState = ConnState.Connecting;
        }

        private void ConnectDisconnect(object obj)
        {
            if (IsConnected)
            {
                Disconnect();
            }
            else
            {
                Connect();
            }
        }

        public void Disconnect()
        {
            // TODO: enter LPM before disconnecting

            // Disconnect
            byte[] cmd = bglib.BLECommandConnectionDisconnect(Connection);
            MessageWriter.LogWrite(string.Format("ble_cmd_connection_disconnect: connection={0}", Connection));
            bglib.SendCommand(port.ToSerialPort(), cmd);
        }
        #endregion
    }
}
