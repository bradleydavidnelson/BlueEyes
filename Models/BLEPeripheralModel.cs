using BlueEyes.Utilities;
using BlueEyes.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace BlueEyes.Models
{
    public class BLEPeripheralModel { }

    public class BLEPeripheral : BindableBase
    {
        #region Fields
        private Bluegiga.BGLib bglib = (Bluegiga.BGLib)Application.Current.FindResource("BGLib");
        private SerialPortModel port = (SerialPortModel)Application.Current.FindResource("Port");

        private double _adc1;
        private byte[] _address;
        private byte _addrType;
        private byte _connection;
        private ConnState _connectionState = ConnState.Disconnected;
        private ICommand _connectCommand;
        private bool _isConnected;
        private double _data;
        private LPM _lowPowerMode = LPM.Unknown;
        private ICommand _lpmCommand;
        private string _name;
        private double _temperature;
        private Stopwatch _uptime = new Stopwatch();
        private DispatcherTimer _uptimeTimer = new DispatcherTimer();

        // Attribute Database

        // Easy handles TODO depreciate
        public ushort attHandleData { get; set; }
        public ushort attHandleLPM { get; set; }
        public ushort attHandleCalibrate { get; set; }
        public ushort attHandleTemp { get; set; }
        public ushort attHandleRail { get; set; }
        public Queue<ushort> attHandleCCC { get; set; } = new Queue<ushort>();

        public enum ConnState { Disconnected, Connecting, Connected };
        public enum LPM { Unknown, Enabled, Disabled};
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
        public double ADC1
        {
            get { return _adc1; }
            set { SetProperty(ref _adc1, value); }
        }

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

        public bool ServiceIDsFound { get; set; }

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
            get
            {
                if (!IsConnected)
                {
                    return (byte)0xFF;
                }
                return _connection;
            }
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

        public double Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value); }
        }

        public long ElapsedMilliseconds
        {
            get
            {
                if (!_uptime.IsRunning)
                {
                    _uptime.Start();
                }
                return _uptime.ElapsedMilliseconds;
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public LPM LowPowerMode
        {
            get { return _lowPowerMode; }
            set
            {
                SetProperty(ref _lowPowerMode, value);
                NotifyPropertyChanged("LPMAction");
            }
        }

        public string LPMAction
        {
            get
            {
                if (LowPowerMode == LPM.Enabled)
                {
                    return "Wake";
                }
                if (LowPowerMode == LPM.Disabled)
                {
                    return "Sleep";
                }
                else
                {
                    return "";
                }
            }
        }

        public ICommand LPMCommand
        {
            get
            {
                if (_lpmCommand == null)
                {
                    _lpmCommand = new RelayCommand(SleepWake);
                }
                return _lpmCommand;
            }
            set { _lpmCommand = value; }
        }

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public Collection<Service> Services
        {
            get; set;
        }

        public double Temperature
        {
            get { return _temperature; }
            set { SetProperty(ref _temperature, value); }
        }

        public string Uptime
        {
            get
            {
                if (!_uptime.IsRunning)
                {
                    _uptime.Start();
                }
                if (!_uptimeTimer.IsEnabled)
                {
                    _uptimeTimer.Start();
                    _uptimeTimer.Interval = new TimeSpan(0, 0, 1);
                    _uptimeTimer.Tick += timerNotify;
                }

                TimeSpan t = _uptime.Elapsed;
                
                if (t.Days > 0)
                {
                    return string.Format("{0}:{1}:{2:00}:{3:00}", t.Days, t.Hours, t.Minutes, t.Seconds);
                }
                else if (t.Hours > 0)
                {
                    return string.Format("{0}:{1:00}:{2:00}", t.Hours, t.Minutes, t.Seconds);
                }
                else
                {
                    return string.Format("{0}:{1:00}", t.Minutes, t.Seconds);
                }
            }
        }
        #endregion

        #region Methods
        private void CheckConnection(object sender, EventArgs e)
        {
            if (ConnectionState != ConnState.Connected)
            {
                MessageWriter.LogWrite("Connection failed");
                ConnectionState = ConnState.Disconnected;
            }

            ((DispatcherTimer)sender).Stop();
        }

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

            // Initialize connecting timeout timer
            DispatcherTimer connectionTimer = new DispatcherTimer();
            connectionTimer.Tick += new EventHandler(CheckConnection);
            connectionTimer.Interval = new TimeSpan(0, 0, 3);
            connectionTimer.Start();

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

        private void LPMEnter()
        {
            // Wake up
            byte[] lpm_bit = new byte[] { 0x00 };
            Byte[] cmd = bglib.BLECommandATTClientAttributeWrite(Connection, attHandleLPM, lpm_bit);
            MessageWriter.LogWrite(string.Format("ble_cmd_att_client_attribute_write: connection={0}, handle={1}, data={2}",
                Connection, attHandleLPM, BitConverter.ToString(lpm_bit)));
            bglib.SendCommand(port.ToSerialPort(), cmd);
        }

        private void LPMExit()
        {
            // Wake up
            byte[] lpm_bit = new byte[] { 0x01 };
            Byte[] cmd = bglib.BLECommandATTClientAttributeWrite(Connection, attHandleLPM, lpm_bit);
            MessageWriter.LogWrite(string.Format("ble_cmd_att_client_attribute_write: connection={0}, handle={1}, data={2}",
                Connection, attHandleLPM, BitConverter.ToString(lpm_bit)));
            bglib.SendCommand(port.ToSerialPort(), cmd);
        }

        private void SleepWake(object obj)
        {
            if (LowPowerMode == LPM.Disabled)
            {
                LPMEnter();
            }
            if (LowPowerMode == LPM.Enabled)
            {
                LPMExit();
            }
        }

        private void timerNotify(object sender, EventArgs e)
        {
            NotifyPropertyChanged("Uptime");
        }

        public override string ToString()
        {
            return Name;
        }
        #endregion
    }
}
