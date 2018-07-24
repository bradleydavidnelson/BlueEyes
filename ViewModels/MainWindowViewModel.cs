using BlueEyes.Models;
using BlueEyes.Utilities;
using BlueEyes.Views;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private const int MAX_CONNECTIONS = 3;

        // Application Resources
        private Bluegiga.BGLib bglib = (Bluegiga.BGLib)Application.Current.FindResource("BGLib");
        private BLEPeripheralCollection discoveredDevices = (BLEPeripheralCollection)Application.Current.FindResource("BLEPeripherals");
        private SerialPortModel _port = (SerialPortModel)Application.Current.FindResource("Port");
        private LogWindow logWindow = (LogWindow)Application.Current.FindResource("LogWindow");

        private ConcurrentDictionary<ushort, BLEPeripheral> connectedPeripherals = new ConcurrentDictionary<ushort, BLEPeripheral>();
        private DiscoveredDeviceViewModel _discoveredDevicesVM = new DiscoveredDeviceViewModel();
        private LogViewModel _log = new LogViewModel();
        private SerialNameModel _selectedPort;
        private byte[] cmd = new Byte[] { };
        private bool _logIsVisible;

        // Commands
        private ICommand _exitCommand;
        private RelayCommand<CancelEventArgs> _logClosingCommand;
        private ICommand _serialOpenCloseCommand;
        private ICommand _logViewCommand;
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            logWindow.DataContext = this;
            LogIsVisible = Properties.Settings.Default.ShowLog;

            // Event handlers
            _port.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandler);
            bglib.BLEEventGAPScanResponse += new Bluegiga.BLE.Events.GAP.ScanResponseEventHandler(BLEGAPScanResponseEvent);
            bglib.BLEEventConnectionStatus += new Bluegiga.BLE.Events.Connection.StatusEventHandler(BLEConnectionStatusEvent);
            bglib.BLEEventConnectionDisconnected += new Bluegiga.BLE.Events.Connection.DisconnectedEventHandler(BLEConnectionDisconnectedEvent);
            bglib.BLEEventATTClientGroupFound += new Bluegiga.BLE.Events.ATTClient.GroupFoundEventHandler(BLEATTClientGroupFoundEvent);
            bglib.BLEEventATTClientProcedureCompleted += new Bluegiga.BLE.Events.ATTClient.ProcedureCompletedEventHandler(BLEATTClientProcedureCompletedEvent);
            bglib.BLEEventATTClientFindInformationFound += new Bluegiga.BLE.Events.ATTClient.FindInformationFoundEventHandler(BLEATTClientFindInformationFoundEvent);
            bglib.BLEEventATTClientAttributeValue += new Bluegiga.BLE.Events.ATTClient.AttributeValueEventHandler(BLEATTClientAttributeValueEvent);
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

        public RelayCommand<CancelEventArgs> LogClosingCommand
        {
            get
            {
                if (_logClosingCommand == null)
                {
                    _logClosingCommand = new RelayCommand<CancelEventArgs>(LogClosing);
                }
                return _logClosingCommand;
            }
        }

        public bool LogIsVisible
        {
            get { return _logIsVisible; }
            set { SetProperty(ref _logIsVisible, value); }
        }

        public ICommand LogViewCommand
        {
            get
            {
                if (_logViewCommand == null)
                {
                    _logViewCommand = new RelayCommand(LogView);
                }
                return _logViewCommand;
            }
            set { _logViewCommand = value; }
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
        #endregion

        #region BLE Event Handlers
        public void BLEGAPScanResponseEvent(object sender, Bluegiga.BLE.Events.GAP.ScanResponseEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                MessageWriter.LogWrite(string.Format("ble_evt_gap_scan_response: rssi={0}, packet_type={1}, sender={2}, address_type={3}, bond={4}, data={5}",
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
                        break;
                    }
                }

                // If the responder is undiscovereed, add it to the collection
                if (responder == null)
                {
                    responder = new BLEPeripheral(e.sender);
                    discoveredDevices.Add(responder);
                }

                // Right now, I only care about the name
                if (responder.Name == null)
                {
                    byte[] remainingData = e.data;
                    while (remainingData.Length > 0)
                    {
                        ushort elementLength = remainingData[0];
                        ushort adType = remainingData[1];
                        byte[] element = remainingData.Skip(2).Take(elementLength - 1).ToArray();
                        
                        // Complete local name
                        if (adType == 0x09)
                        {
                            responder.Name = Encoding.ASCII.GetString(element);
                        }

                        remainingData = remainingData.Skip(elementLength + 1).ToArray();
                    }
                }
            });
        }

        public void BLEConnectionStatusEvent(object sender, Bluegiga.BLE.Events.Connection.StatusEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                MessageWriter.LogWrite(string.Format("ble_evt_connection_status: connection={0}, flags={1}, address=[ {2}], address_type={3}, conn_interval={4}, timeout={5}, latency={6}, bonding={7}",
                    e.connection,
                    e.flags,
                    BitConverter.ToString(e.address),
                    e.address_type,
                    e.conn_interval,
                    e.timeout,
                    e.latency,
                    e.bonding));

                // Identify peripheral
                BLEPeripheral source = null;

                // Check if responder has been discovered already
                foreach (BLEPeripheral p in discoveredDevices)
                {
                    if (p.Address.SequenceEqual(e.address))
                    {
                        source = p;
                        break;
                    }
                }

                if (source == null)
                {
                    return;
                }

                // Has a new connection been established?
                if ((e.flags & 0x05) == 0x05)
                {
                    // Add to the connection dictionary
                    connectedPeripherals.TryAdd(e.connection, source);

                    // The connection is established
                    source.ConnectionState = BLEPeripheral.ConnState.Connected;
                    MessageWriter.LogWrite("Connected to " + source.Name);

                    // Perform service discovery
                    ushort start = 0x0001;
                    ushort end = 0xFFFF;
                    byte[] uuid = new byte[] { 0x00, 0x28 }; // "service" UUID is 0x2800 (little-endian for UUID uint8array)
                    cmd = bglib.BLECommandATTClientReadByGroupType(e.connection, start, end, uuid);
                    MessageWriter.LogWrite(string.Format("ble_cmd_att_client_read_by_group_type: connection={0}, start={1}, end={2}, uuid={3}",
                        e.connection,
                        start,
                        end,
                        BitConverter.ToString(uuid)));
                    bglib.SendCommand(_port.ToSerialPort(), cmd);
                }
            });
        }

        public void BLEConnectionDisconnectedEvent(object sender, Bluegiga.BLE.Events.Connection.DisconnectedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                MessageWriter.LogWrite(string.Format("ble_evt_connection_disconnected: connection={0}, reason={1}",
                    e.connection,
                    e.reason));

                BLEPeripheral source;

                connectedPeripherals.TryRemove(e.connection, out source);
                source.ConnectionState = BLEPeripheral.ConnState.Disconnected;

                // Stop advertising
                cmd = bglib.BLECommandGAPEndProcedure();
                MessageWriter.LogWrite("ble_cmd_gap_end_procedure");
                bglib.SendCommand(_port.ToSerialPort(), cmd);

                // Reset GAP Mode
                cmd = bglib.BLECommandGAPSetMode(0, 0);
                MessageWriter.LogWrite("ble_cmd_gap_set_mode: discover=0, connect=0");
                bglib.SendCommand(_port.ToSerialPort(), cmd);

                BLEScan();
            });
        }

        public void BLEATTClientFindInformationFoundEvent(object sender, Bluegiga.BLE.Events.ATTClient.FindInformationFoundEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                MessageWriter.LogWrite(string.Format("ble_evt_attclient_find_information_found: connection={0}, chrhandle={1}, uuid={2}",
                    e.connection,
                    e.chrhandle,
                    BitConverter.ToString(e.uuid)));

                BLEPeripheral peripheral = connectedPeripherals[e.connection];

                peripheral.AttributesFound = true;
            });
        }

        public void BLEATTClientGroupFoundEvent(object sender, Bluegiga.BLE.Events.ATTClient.GroupFoundEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                MessageWriter.LogWrite(string.Format("ble_evt_att_client_group_found: connection={0}, start={1}, end={2}, uuid={3}",
                    e.connection,
                    e.start,
                    e.end,
                    BitConverter.ToString(e.uuid)));

                BLEPeripheral peripheral = connectedPeripherals[e.connection];
            });
        }

        public void BLEATTClientProcedureCompletedEvent(object sender, Bluegiga.BLE.Events.ATTClient.ProcedureCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                MessageWriter.LogWrite(string.Format("ble_evt_attclient_procedure_completed: connection={0}, result={1}, chrhandle={2}",
                    e.connection,
                    e.result,
                    e.chrhandle));

                BLEPeripheral peripheral = connectedPeripherals[e.connection];

                if (!peripheral.AttributesFound)
                {
                    // Find all attributes
                    ushort start = 0x0001;
                    ushort end = 0xFFFF;
                    cmd = bglib.BLECommandATTClientFindInformation(e.connection, start, end);
                    MessageWriter.LogWrite(string.Format("ble_cmd_att_client_find_information: connection={0}, start={1}, end={2}",
                        e.connection,
                        start,
                        end));
                    bglib.SendCommand(_port.ToSerialPort(), cmd);
                }
            });
        }

        public void BLEATTClientAttributeValueEvent(object sender, Bluegiga.BLE.Events.ATTClient.AttributeValueEventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                MessageWriter.LogWrite(string.Format("ble_evt_attclient_attribute_value: connection={0}, atthandle={1}, type={2}, value={3}",
                    e.connection,
                    e.atthandle,
                    e.type,
                    BitConverter.ToString(e.value)));
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
            MessageWriter.LogWrite(string.Format("ble_cmd_gap_set_scan_parameters: scan_interval={0}; scan_window={1}, active={2}", scan_interval, scan_window, active));
            bglib.SendCommand(_port.ToSerialPort(), cmd);

            // Begin discovery mode
            byte mode = 1;
            cmd = bglib.BLECommandGAPDiscover(mode); // generic discovery mode
            MessageWriter.LogWrite(string.Format("ble_cmd_gap_discover: mode={0}", mode));
            bglib.SendCommand(_port.ToSerialPort(), cmd);
        }

        private void Exit(object obj)
        {
            Application.Current.Shutdown();
        }

        public void LogClosing(CancelEventArgs e)
        {
            // Hide instead of close
            e.Cancel = true;
            LogIsVisible = false;
        }

        private void LogView(object obj)
        {
            if (_logIsVisible)
            {
                logWindow.Hide();
            }
            else
            {
                logWindow.Show();
            }
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
                MessageWriter.LogWrite(string.Format("Closing {0}...", _selectedPort.PortName));
                _port.Close();
                NotifyPropertyChanged("Port_IsOpen");
                MessageWriter.LogWrite(_selectedPort.PortName + " closed");
            }
            else // Is Closed
            {
                MessageWriter.LogWrite(string.Format("Opening {0}...", _selectedPort.PortName));
                _port.PortName = _selectedPort.PortName;
                _port.Open();
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();
                NotifyPropertyChanged("Port_IsOpen");
                MessageWriter.LogWrite(_selectedPort.PortName + " opened");

                BLEScan();
            }
        }
        #endregion
    }
}
