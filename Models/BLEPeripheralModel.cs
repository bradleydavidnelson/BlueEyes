using BlueEyes.Utilities;
using BlueEyes.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace BlueEyes.Models
{
    public class BLEPeripheralModel { }

    public class BLEPeripheral : BindableBase
    {
        #region Fields
        // Global
        private Bluegiga.BGLib bglib = (Bluegiga.BGLib)Application.Current.FindResource("BGLib");
        private SerialPortModel port = (SerialPortModel)Application.Current.FindResource("Port");

        // Collections
        private ConcurrentDictionary<ushort,Attribute> _attributes = new ConcurrentDictionary<ushort,Attribute>();
        private ConcurrentDictionary<string,Service> _services = new ConcurrentDictionary<string,Service>();
        private ConcurrentDictionary<string,Characteristic> _characteristics = new ConcurrentDictionary<string,Characteristic>();
        public DynamicDictionary Serv = new DynamicDictionary();

        // misc
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
        private string _saveFile;
        private double _temperature;
        private Stopwatch _uptime = new Stopwatch();
        private DispatcherTimer _uptimeDispatcher = new DispatcherTimer();

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

            _uptimeDispatcher.Interval = new TimeSpan(0, 0, 1);
            _uptimeDispatcher.Tick += timerNotify;
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
                SetProperty(ref _address, value);
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

        public ConcurrentDictionary<ushort,Attribute> Attributes
        {
            get { return _attributes; }
            private set { SetProperty(ref _attributes, value); }
        }

        public ConcurrentDictionary<string,Characteristic> Characteristics
        {
            get { return _characteristics; }
            set { SetProperty(ref _characteristics, value); }
        }

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
            set
            {
                SetProperty(ref _connection, value);
                LowPowerMode = LPM.Unknown; // LPM may have changed
            }
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

        public string SaveFile
        {
            get
            {
                if (_saveFile == null)
                {
                    _saveFile = Properties.Settings.Default.SaveLocation + "\\" + Name + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second +".csv";
                }
                return _saveFile;
            }
            private set { _saveFile = value; }
        }

        public ConcurrentDictionary<string,Service> Services
        {
            get { return _services; }
            set { SetProperty(ref _services, value); }
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
                TimeSpan uptime = _uptime.Elapsed;

                if (uptime.Days > 0)
                {
                    return string.Format("{0}:{1}:{2:00}:{3:00}", uptime.Days, uptime.Hours, uptime.Minutes, uptime.Seconds);
                }
                else if (uptime.Hours > 0)
                {
                    return string.Format("{0}:{1:00}:{2:00}", uptime.Hours, uptime.Minutes, uptime.Seconds);
                }
                else
                {
                    return string.Format("{0}:{1:00}", uptime.Minutes, uptime.Seconds);
                }
            }
        }

        public long UptimeMilliseconds
        {
            get { return _uptime.ElapsedMilliseconds; }
        }
        #endregion

        #region Methods
        public void AddNewAttribute(ushort handle, byte[] uuid)
        {
            Attribute attr = new Attribute();
            attr.Handle = handle;
            attr.UUID = uuid;
            attr.Description = Bluetooth.Parser.Lookup(uuid);
            Attributes.TryAdd(handle, attr);
        }

        public bool TryAddService(Service s)
        {
            return Services.TryAdd(s.Description, s);
        }

        public void AddNewService(ushort start, ushort end, byte[] uuid)
        {
            Service s = new Service();
            s.Handle = start;
            s.UUID = uuid;
            s.Description = Bluetooth.Parser.Lookup(uuid);
            if (s.Description != null)
            {
                Services.TryAdd(s.Description, s);
            }
            else
            {
                Services.TryAdd(BitConverter.ToString(s.UUID), s);
            }

            PopulateService(s);
        }

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
            MessageWriter.LogWrite("ble_cmd_gap_connect_direct: ", string.Format("address={0}, address_type={1}, conn_interval_min={2}; conn_interval_max={3}, timeout={4}, latency={5}",
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
            if (LowPowerMode == LPM.Disabled)
            {
                MessageWriter.LogWrite(Name + " entering LPM...");
                LPMEnter();
            }

            // Disconnect
            byte[] cmd = bglib.BLECommandConnectionDisconnect(Connection);
            MessageWriter.LogWrite("ble_cmd_connection_disconnect: ", string.Format("connection={0}", Connection));
            bglib.SendCommand(port.ToSerialPort(), cmd);
        }

        public List<Service> GetServices()
        {
            return _services.Values.OrderBy(o => o.Handle).ToList();
        }

        private void LPMEnter()
        {
            // Wake up
            byte[] lpm_bit = new byte[] { 0x00 };
            Byte[] cmd = bglib.BLECommandATTClientAttributeWrite(Connection, attHandleLPM, lpm_bit);
            MessageWriter.LogWrite("ble_cmd_att_client_attribute_write: ", string.Format("connection={0}, handle={1}, data={2}",
                Connection, attHandleLPM, BitConverter.ToString(lpm_bit)));
            bglib.SendCommand(port.ToSerialPort(), cmd);

            // Stop tracking uptime
            _uptimeDispatcher.IsEnabled = false;
            if (_uptime.IsRunning == true)
            {
                _uptime.Reset();
                NotifyPropertyChanged("Uptime");
            }

            // New save file
            SaveFile = null;
        }

        private void LPMExit()
        {
            // Wake up
            byte[] lpm_bit = new byte[] { 0x01 };
            Byte[] cmd = bglib.BLECommandATTClientAttributeWrite(Connection, attHandleLPM, lpm_bit);
            MessageWriter.LogWrite("ble_cmd_att_client_attribute_write: ", string.Format("connection={0}, handle={1}, data={2}",
                Connection, attHandleLPM, BitConverter.ToString(lpm_bit)));
            bglib.SendCommand(port.ToSerialPort(), cmd);

            // Track uptime
            _uptimeDispatcher.IsEnabled = true;
            if (_uptime.IsRunning == false)
            {
                _uptime.Start();
            }
        }

        public void PopulateService(Service s)
        {
            Stack<Characteristic> chrstack = new Stack<Characteristic>();

            foreach (Attribute a in Attributes.Values)
            {
                if (a.Handle > s.Handle && a.Handle <= s.GroupEnd)
                {
                    // New characteristic declaration
                    if (a.UUID.SequenceEqual(Bluetooth.Declarations.Characteristic))
                    {
                        Characteristic chr = new Characteristic();
                        chr.Handle = a.Handle;
                        chr.UUID = a.UUID;
                        chr.ValueAttribute = Attributes[(ushort)(chr.Handle + 1)];
                        chr.Declaration = Attributes[a.Handle];
                        chr.Description = chr.ValueAttribute.Description;
                        if (chr.Description != null)
                        {
                            s.Characteristics.TryAdd(chr.Description, chr);
                            Characteristics.TryAdd(chr.Description, chr);
                        }
                        else
                        {
                            s.Characteristics.TryAdd(BitConverter.ToString(chr.UUID), chr);
                            Characteristics.TryAdd(BitConverter.ToString(chr.UUID), chr);
                        }
                        chrstack.Push(chr);
                    }
                    else
                    {
                        if (a.Description != null)
                        {
                            chrstack.Peek().Descriptors.TryAdd(a.Description, a);
                        }
                        else
                        {
                            chrstack.Peek().Descriptors.TryAdd(BitConverter.ToString(a.UUID), a);
                        }
                    }
                }
            }
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
