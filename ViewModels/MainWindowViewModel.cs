using BlueEyes.Models;
using BlueEyes.Utilities;
using BlueEyes.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace BlueEyes.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        #region Fields
        // Application Resources
        private Bluegiga.BGLib bglib = (Bluegiga.BGLib)Application.Current.FindResource("BGLib");
        private BLEPeripheralCollection discoveredDevices = (BLEPeripheralCollection)Application.Current.FindResource("BLEPeripherals");
        private LogWindow logWindow = (LogWindow)Application.Current.FindResource("LogWindow");

        private DiscoveredDeviceViewModel _discoveredDevicesVM = new DiscoveredDeviceViewModel();
        private LogViewModel _log = new LogViewModel();
        private SerialPortModel _port = new SerialPortModel();
        private SerialNameModel _selectedPort;
        private byte[] cmd = new Byte[] { };
        private bool _logIsVisible;

        // Commands
        private ICommand _exitCommand;
        private ICommand _serialOpenCloseCommand;
        //private ICommand _viewLogCommand;
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            logWindow.DataContext = this;
            LogIsVisible = Properties.Settings.Default.ShowLog;
            if (LogIsVisible)
            {
                logWindow.Show();
            }

            // Event handlers
            _port.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandler);
            bglib.BLEEventGAPScanResponse += new Bluegiga.BLE.Events.GAP.ScanResponseEventHandler(BLEEventGAPScanResponse);
        }
        #endregion

        #region Properties
        public DiscoveredDeviceViewModel DiscoveredDevices
        {
            get { return _discoveredDevicesVM; }
            set { SetProperty(ref _discoveredDevicesVM, value); }
        }

        public bool Port_IsOpen
        {
            get { return _port.IsOpen; }
        }

        public SerialPortModel Port
        {
            get { return _port; }
        }

        public SerialNameModel SelectedPort
        {
            get { return _selectedPort; }
            set { SetProperty(ref _selectedPort, value); }
        }

        public LogViewModel Log
        {
            get { return _log; }
            set { SetProperty(ref _log, value); }
        }

        public bool LogIsVisible
        {
            get { return _logIsVisible; }
            set { SetProperty(ref _logIsVisible, value); }
        }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(Exit);
                }
                return _exitCommand;
            }
            set { _exitCommand = value; }
        }

        public ICommand SerialOpenCloseCommand
        {
            get
            {
                if (_serialOpenCloseCommand == null)
                {
                    _serialOpenCloseCommand = new RelayCommand(SerialOpenClose);
                }
                return _serialOpenCloseCommand;
            }
            set { _serialOpenCloseCommand = value; }
        }

        /*public ICommand ViewLogCommand
        {
            get
            {
                if (_viewLogCommand == null)
                {
                    _viewLogCommand = new RelayCommand(ViewLog);
                }
                return _viewLogCommand;
            }
            set { _viewLogCommand = value; }
        }*/
        #endregion

        #region BLE Event Handlers
        public void BLEEventGAPScanResponse(object sender, Bluegiga.BLE.Events.GAP.ScanResponseEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Log.WriteLine("ble_evt_gap_scan_response: ", string.Format("rssi={0}, packet_type={1}, sender={2}, address_type={3}, bond={4}, data={5}",
                    (SByte)e.rssi,
                    e.packet_type,
                    BitConverter.ToString(e.sender),
                    e.address_type,
                    e.bond,
                    BitConverter.ToString(e.data)));

                BLEPeripheral responder = null;

                // Check if responder has been discovered already
                foreach (BLEPeripheral p in discoveredDevices)
                {
                    if (p.Address.SequenceEqual(e.sender))
                    {
                        responder = p;
                    }
                }

                // If the responder is undiscovereed, add it to the collection
                if (responder == null)
                {
                    responder = new BLEPeripheral(e.sender);
                    discoveredDevices.Add(responder);
                }

                // Connectable Advertisement Packet (0x00)
                /*if (e.packet_type == (byte)0 && responder.AdvertisedServices == null)
                {
                    responder.AdvertisedServices = GAPExtractServices(e.data);
                    log.WriteLine("Advertised services extracted");
                    if (responder.IsCompatible)
                    {
                        compatibleDevices.Add(responder);
                    }
                }*/

                // Scan Response Packet (0x04)
                if (e.packet_type == (byte)4 && responder.Name == null)
                {
                    if (e.data.Length > 2)
                    {
                        responder.Name = Encoding.ASCII.GetString(e.data, 2, e.data.Length - 2);
                    }
                    else
                    {
                        responder.Name = "(unnamed)";
                    }
                }
            });
        }
        #endregion

        #region Methods
        private void BLEScan()
        {
            // Set scan parameters
            int scan_interval = 125;    // in ms
            int scan_window = 125;   // in ms
            byte active = 1;
            cmd = bglib.BLECommandGAPSetScanParameters(
                Convert.ToUInt16(scan_interval * 1000 / 625),
                Convert.ToUInt16(scan_window * 1000 / 625),
                active); // 125ms interval, 125ms window, active scanning
            Log.WriteLine("ble_cmd_gap_set_scan_parameters: ", string.Format("scan_interval={0}; scan_window={1}, active={2}", scan_interval, scan_window, active));
            bglib.SendCommand(_port.ToSerialPort(), cmd);

            // Begin discovery mode
            byte mode = 1;
            cmd = bglib.BLECommandGAPDiscover(mode); // generic discovery mode
            Log.WriteLine("ble_cmd_gap_discover: ", string.Format("mode={0}", mode));
            bglib.SendCommand(_port.ToSerialPort(), cmd);
        }

        private void Exit(object obj)
        {
            Application.Current.Shutdown();
        }

        private void SerialDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            Byte[] inData = new Byte[sp.BytesToRead];

            // Read all available bytes from serial port in one chunk
            try
            {
                sp.Read(inData, 0, inData.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                // TODO: Close serial port if it is open?
                return;
            }

            // Parse all bytes read through BGLib parser
            for (int i = 0; i < inData.Length; i++)
            {
                bglib.Parse(inData[i]);
            }
        }

        private void SerialOpenClose(object obj)
        {
            if (_port.IsOpen)
            {
                Log.WriteLine(string.Format("Closing {0}...", _selectedPort.PortName));
                _port.Close();
                NotifyPropertyChanged("Port_IsOpen");
                Log.WriteLine(string.Format(_selectedPort.PortName + " closed"));
            }
            else // Is Closed
            {
                Log.WriteLine(string.Format("Opening {0}...", _selectedPort.PortName));
                _port.PortName = _selectedPort.PortName;
                _port.Open();
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();
                NotifyPropertyChanged("Port_IsOpen");
                Log.WriteLine(string.Format(_selectedPort.PortName + " opened"));

                BLEScan();
            }
        }

        /*private void ViewLog(object obj)
        {
            if (_logIsVisible)
            {
                logWindow.Hide();
            }
            else
            {
                logWindow.Show();
            }
        }*/
        #endregion
    }
}
