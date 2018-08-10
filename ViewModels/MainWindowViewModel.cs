using BlueEyes.Models;
using BlueEyes.Utilities;
using BlueEyes.Views;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace BlueEyes.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        #region Fields
        private const int MAX_CONNECTIONS = 3;

        // Application Resources
        private Bluegiga.BGLib bglib = (Bluegiga.BGLib)Application.Current.FindResource("BGLib");
        private BLEPeripheralCollection discoveredDevices = (BLEPeripheralCollection)Application.Current.FindResource("DiscoveredPeripherals");
        private BLEPeripheralCollection connectedDevices = (BLEPeripheralCollection)Application.Current.FindResource("ConnectedPeripherals");
        private SerialPortModel _port = (SerialPortModel)Application.Current.FindResource("Port");
        private LogWindow logWindow = (LogWindow)Application.Current.FindResource("LogWindow");

        private ConnectedDeviceViewModel _connectedDevicesVM = new ConnectedDeviceViewModel();
        private DiscoveredDeviceViewModel _discoveredDevicesVM = new DiscoveredDeviceViewModel();
        private LogViewModel _log = new LogViewModel();
        private SerialNameModel _selectedPort;
        private byte[] cmd = new Byte[] { };
        private bool _logIsVisible;

        // Commands
        private ICommand _exitCommand;
        private ICommand _getGattCommand;
        private RelayCommand<CancelEventArgs> _logClosingCommand;
        private ICommand _openCalibrationWindowCommand;
        private ICommand _serialOpenCloseCommand;
        private ICommand _setSaveLocationCommand;
        private ICommand _logViewCommand;
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            logWindow.DataContext = this;
            LogIsVisible = Properties.Settings.Default.ShowLog;

            Messenger.Default.Register<GenericMessage<byte[]>>(this, (action) => ReceiveMessage(action));

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

        #region Properties - Commands
        public ICommand GetGATTCommand
        {
            get
            {
                if(_getGattCommand == null)
                {
                    _getGattCommand = new RelayCommand(GetGATT);
                }
                return _getGattCommand;
            }
            set { SetProperty(ref _getGattCommand, value); }
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

        public ICommand OpenCalibrationWindowCommand
        {
            get
            {
                if (_openCalibrationWindowCommand == null)
                {
                    _openCalibrationWindowCommand = new RelayCommand(OpenCalibrationWindow);
                }
                return _openCalibrationWindowCommand;
            }
            private set { _openCalibrationWindowCommand = value; }
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

        public ICommand SetSaveLocationCommand
        {
            get
            {
                if (_setSaveLocationCommand == null)
                {
                    _setSaveLocationCommand = new RelayCommand(SetSaveLocation);
                }
                return _setSaveLocationCommand;
            }
        }
        #endregion

        #region Properties - Other
        public ConnectedDeviceViewModel ConnectedDevices
        {
            get { return _connectedDevicesVM; }
            set { SetProperty(ref _connectedDevicesVM, value); }
        }

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
        #endregion

        #region BLE Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BLEGAPScanResponseEvent(object sender, Bluegiga.BLE.Events.GAP.ScanResponseEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                MessageWriter.LogWrite("ble_evt_gap_scan_response: ", string.Format("rssi={0}, packet_type={1}, sender={2}, address_type={3}, bond={4}, data={5}",
                    e.rssi,
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

                // Identify device name
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BLEConnectionStatusEvent(object sender, Bluegiga.BLE.Events.Connection.StatusEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                MessageWriter.LogWrite("ble_evt_connection_status: ", string.Format("connection={0}, flags={1}, address=[ {2}], address_type={3}, conn_interval={4}, timeout={5}, latency={6}, bonding={7}",
                    e.connection,
                    e.flags,
                    BitConverter.ToString(e.address),
                    e.address_type,
                    e.conn_interval,
                    e.timeout,
                    e.latency,
                    e.bonding));

                // Identify peripheral
                BLEPeripheral peripheral = null;

                // Check if responder has been discovered already
                foreach (BLEPeripheral p in discoveredDevices)
                {
                    if (p.Address.SequenceEqual(e.address))
                    {
                        peripheral = p;
                        break;
                    }
                }

                if (peripheral == null)
                {
                    return;
                }

                // Has a new connection been established?
                if ((e.flags & 0x05) == 0x05)
                {
                    // Add to collection
                    if (!connectedDevices.Contains(peripheral))
                    {
                        connectedDevices.Add(peripheral);
                    }

                    // The connection is established
                    peripheral.ConnectionState = BLEPeripheral.ConnState.Connected;
                    MessageWriter.LogWrite("Connected to " + peripheral.Name);

                    BLEPerformNextTask(peripheral);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BLEConnectionDisconnectedEvent(object sender, Bluegiga.BLE.Events.Connection.DisconnectedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                MessageWriter.LogWrite("ble_evt_connection_disconnected: ", string.Format("connection={0}, reason={1}",
                    e.connection,
                    e.reason));

                BLEPeripheral peripheral;
                if (connectedDevices.TryRemoveByConnection(e.connection, out peripheral))
                {
                    peripheral.ConnectionState = BLEPeripheral.ConnState.Disconnected;
                }

                // Stop advertising
                cmd = bglib.BLECommandGAPEndProcedure();
                MessageWriter.LogWrite("ble_cmd_gap_end_procedure","");
                bglib.SendCommand(_port.ToSerialPort(), cmd);

                // Reset GAP Mode
                cmd = bglib.BLECommandGAPSetMode(0, 0);
                MessageWriter.LogWrite("ble_cmd_gap_set_mode: ","discover=0, connect=0");
                bglib.SendCommand(_port.ToSerialPort(), cmd);

                BLEScan();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BLEATTClientFindInformationFoundEvent(object sender, Bluegiga.BLE.Events.ATTClient.FindInformationFoundEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                MessageWriter.LogWrite("ble_evt_attclient_find_information_found: ", string.Format("connection={0}, chrhandle={1}, uuid={2}",
                    e.connection,
                    e.chrhandle,
                    BitConverter.ToString(e.uuid)));

                BLEPeripheral peripheral;
                if (!connectedDevices.TryGetConnection(e.connection, out peripheral))
                {
                    MessageWriter.LogWrite("Unable to find connection " + e.connection);
                    return;
                }

                peripheral.AddNewAttribute(e.chrhandle, e.uuid);

                if (e.uuid.SequenceEqual(Bluetooth.Custom.Data))
                {
                    peripheral.attHandleData = e.chrhandle;
                }

                if (e.uuid.SequenceEqual(Bluetooth.Custom.LPM))
                {
                    peripheral.attHandleLPM = e.chrhandle;
                }

                if (e.uuid.SequenceEqual(Bluetooth.Custom.RheostatValue))
                {
                    peripheral.attHandleCalibrate = e.chrhandle;
                }

                if (e.uuid.SequenceEqual(Bluetooth.Custom.Battery))
                {
                    peripheral.attHandleRail = e.chrhandle;
                }

                if (e.uuid.SequenceEqual(Bluetooth.Custom.Temperature))
                {
                    peripheral.attHandleTemp = e.chrhandle;
                }

                else if (e.uuid.SequenceEqual(Bluetooth.Descriptors.ClientCharacteristicConfiguration))
                {
                    peripheral.attHandleCCC.Enqueue(e.chrhandle);
                    MessageWriter.LogWrite("Enqueue " + e.chrhandle + "; Count = " + peripheral.attHandleCCC.Count);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BLEATTClientGroupFoundEvent(object sender, Bluegiga.BLE.Events.ATTClient.GroupFoundEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                MessageWriter.LogWrite("ble_evt_att_client_group_found: ", string.Format("connection={0}, start={1}, end={2}, uuid={3}",
                    e.connection,
                    e.start,
                    e.end,
                    BitConverter.ToString(e.uuid)));

                BLEPeripheral peripheral;
                if (!connectedDevices.TryGetConnection(e.connection, out peripheral))
                {
                    MessageWriter.LogWrite("Unable to find connection " + e.connection);
                    return;
                }

                string str = "Hi";

                
                Service s = new Service();
                s.Handle = e.start;
                s.GroupEnd = e.end;
                s.GroupUUID = e.uuid;
                s.Declaration = peripheral.Attributes[e.start];
                s.Description = Bluetooth.Parser.Lookup(e.uuid);
                if (s.Description == null)
                {
                    s.Description = BitConverter.ToString(s.UUID);
                }
                if (peripheral.TryAddService(s))
                {
                    peripheral.PopulateService(s);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BLEATTClientProcedureCompletedEvent(object sender, Bluegiga.BLE.Events.ATTClient.ProcedureCompletedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                MessageWriter.LogWrite("ble_evt_attclient_procedure_completed: ", string.Format("connection={0}, result={1}, chrhandle={2}",
                    e.connection,
                    e.result,
                    e.chrhandle));

                BLEPeripheral peripheral;
                if (!connectedDevices.TryGetConnection(e.connection, out peripheral))
                {
                    MessageWriter.LogWrite("Unable to find connection " + e.connection);
                    return;
                }

                BLEPerformNextTask(peripheral);

                if (e.chrhandle == peripheral.attHandleLPM)
                {
                    // Check sleep mode
                    cmd = bglib.BLECommandATTClientReadByHandle(e.connection, peripheral.attHandleLPM);
                    MessageWriter.LogWrite("ble_cmd_att_client_read_by_handle: ", string.Format("connection={0}, handle={1}",
                        e.connection, peripheral.attHandleLPM));
                    bglib.SendCommand(_port.ToSerialPort(), cmd);
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BLEATTClientAttributeValueEvent(object sender, Bluegiga.BLE.Events.ATTClient.AttributeValueEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                MessageWriter.LogWrite("ble_evt_attclient_attribute_value: ", string.Format("connection={0}, atthandle={1}, type={2}, value={3}",
                    e.connection,
                    e.atthandle,
                    e.type,
                    BitConverter.ToString(e.value)));

                BLEPeripheral peripheral;
                if (!connectedDevices.TryGetConnection(e.connection, out peripheral))
                {
                    MessageWriter.LogWrite("Unable to find connection " + e.connection);
                    return;
                }

                // DATA PACKET
                if (e.atthandle == peripheral.attHandleData)
                {
                    // Decode packet
                    // The following decodes each pair of bytes into a single data point
                    double[] val = new double[e.value.Length/2];
                    for (int i = 0; i < val.Length; i++)
                    {
                        short raw = BitConverter.ToInt16(e.value, 2 * i);
                        val[i] = raw * 2500 / 2047;
                    }

                    peripheral.Characteristics["Data"].Value = val.Average();

                    WriteData(peripheral, val);
                }

                // Battery
                if (e.atthandle == peripheral.Characteristics["Battery"].ValueAttribute.Handle)
                {
                    if (e.value.Length == 2)
                    {
                        // Get data
                        short data = BitConverter.ToInt16(e.value, 0);

                        // Calculate rail
                        peripheral.Characteristics["Battery"].Value = data * 2500 / 2047;
                    }
                }

                // TEMPERATURE PACKET
                if (e.atthandle == peripheral.attHandleTemp)
                {
                    short value = BitConverter.ToInt16(e.value, 0);

                    peripheral.Characteristics["Temperature"].Value = (value * 2500 / 2047) * 10 / 45 - 169; // Not really characterized, but it should be close
                }

                // Low Power Mode
                if (e.atthandle == peripheral.attHandleLPM)
                {
                    // Sleep bit
                    if ((e.value[0] & 0x01) == 0x00) // Device is asleep
                    {
                        peripheral.LowPowerMode = BLEPeripheral.LPM.Enabled;
                    }
                    else // Device is awake
                    {
                        peripheral.LowPowerMode = BLEPeripheral.LPM.Disabled;
                    }
                }
            });
        }
        #endregion

        #region Methods
        private void BLEPerformNextTask(BLEPeripheral peripheral)
        {
            // Find all attributes
            if (peripheral.Attributes.Count == 0)
            {
                ushort start = 0x0001;
                ushort end = 0xFFFF;
                cmd = bglib.BLECommandATTClientFindInformation(peripheral.Connection, start, end);
                MessageWriter.LogWrite("ble_cmd_att_client_find_information: ", string.Format("connection={0}, start={1}, end={2}",
                    peripheral.Connection,
                    start,
                    end));
                bglib.SendCommand(_port.ToSerialPort(), cmd);
            }

            // Have attributes been found?
            else if (peripheral.Services.Count == 0)
            {
                // Perform service discovery
                ushort start = 0x0001;
                ushort end = 0xFFFF;
                byte[] primaryService = new byte[] { 0x00, 0x28 };
                cmd = bglib.BLECommandATTClientReadByGroupType(peripheral.Connection, start, end, primaryService);
                MessageWriter.LogWrite("ble_cmd_att_client_read_by_group_type: ", string.Format("connection={0}, start={1}, end={2}, uuid={3}",
                    peripheral.Connection,
                    start,
                    end,
                    BitConverter.ToString(primaryService)));
                bglib.SendCommand(_port.ToSerialPort(), cmd);
            }

            else if (peripheral.attHandleCCC.Count > 0)
            {
                // Enable indications
                byte[] indications = new byte[] { 0x03, 0x00 };
                byte[] cmd = bglib.BLECommandATTClientAttributeWrite(peripheral.Connection, peripheral.attHandleCCC.Peek(), indications);
                MessageWriter.LogWrite("ble_cmd_att_client_attribute_write: ", string.Format("connection={0}, att_handle={1}, data={2}",
                    peripheral.Connection, peripheral.attHandleCCC.Dequeue(), BitConverter.ToString(indications)));
                bglib.SendCommand(_port.ToSerialPort(), cmd);
            }

            // Is the low power mode state known?
            else if (peripheral.attHandleLPM > 0 && peripheral.LowPowerMode == BLEPeripheral.LPM.Unknown)
            {
                // Check sleep mode
                cmd = bglib.BLECommandATTClientReadByHandle(peripheral.Connection, peripheral.attHandleLPM);
                MessageWriter.LogWrite("ble_cmd_att_client_read_by_handle: ", string.Format("connection={0}, handle={1}",
                    peripheral.Connection, peripheral.attHandleLPM));
                bglib.SendCommand(_port.ToSerialPort(), cmd);
            }
        }

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
            MessageWriter.LogWrite("ble_cmd_gap_set_scan_parameters: ", string.Format("scan_interval={0}; scan_window={1}, active={2}", scan_interval, scan_window, active));
            bglib.SendCommand(_port.ToSerialPort(), cmd);

            // Begin discovery mode
            byte mode = 1;
            cmd = bglib.BLECommandGAPDiscover(mode); // generic discovery mode
            MessageWriter.LogWrite("ble_cmd_gap_discover: ", string.Format("mode={0}", mode));
            bglib.SendCommand(_port.ToSerialPort(), cmd);
        }

        private void Exit(object obj)
        {
            Application.Current.Shutdown();
        }

        private void GetGATT(object obj)
        {
            MessageWriter.LogWrite("GATT Database:");
            foreach (BLEPeripheral p in connectedDevices)
            {
                MessageWriter.LogWrite("Peripheral: " + p.Name);
                foreach (Service s in p.GetServices())
                {
                    MessageWriter.LogWrite(s.Declaration.Handle + " \t" + BitConverter.ToString(s.GroupUUID) + " \t" + s.Description);
                    foreach (Characteristic c in s.GetCharacteristics())
                    {
                        MessageWriter.LogWrite(c.Declaration.Handle + " \t\t" + BitConverter.ToString(c.UUID) + " \t" + c.Description);
                        foreach (Models.Attribute a in c.GetDescriptors())
                        {
                            MessageWriter.LogWrite(a.Handle + " \t\t\t" + BitConverter.ToString(a.UUID) + " \t" + a.Description);
                        }
                    }
                }
            }
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

        private void OpenCalibrationWindow(object obj)
        {
            MessageWriter.LogWrite("Opening calibration window...");
            CalibrationWindow cal = new CalibrationWindow();
            cal.Show();
        }

        private void ReceiveMessage(GenericMessage<byte[]> message)
        {
            bglib.SendCommand(_port.ToSerialPort(), message.Content);
        }

        private void SerialDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                SerialPort sp = (SerialPort)sender;
                
                // Read all available bytes from serial port in one chunk
                try
                {
                    byte[] inData = new Byte[sp.BytesToRead];
                    sp.Read(inData, 0, inData.Length);

                    // Parse all bytes read through BGLib parser
                    for (int i = 0; i < inData.Length; i++)
                    {
                        bglib.Parse(inData[i]);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }
            });
        }

        private void SetSaveLocation(object obj)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.Description = "Please select directory to save log files in.";
            System.Windows.Forms.DialogResult result = folderBrowser.ShowDialog();

            // If a directory was specified...
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default.SaveLocation = folderBrowser.SelectedPath;
                Properties.Settings.Default.Save();

                MessageWriter.LogWrite("Data will be logged to: " + Properties.Settings.Default.SaveLocation);
            }

            // If a directory was not specified...
            else if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                if (!Directory.Exists(Properties.Settings.Default.SaveLocation))
                {
                    MessageBox.Show("No valid directory has been specified. Data will not be recorded.");
                }
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

                discoveredDevices.Clear();
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

        private void WriteData(BLEPeripheral peripheral, double[] newData)
        {
            // Write data to output file
            using (StreamWriter output = new StreamWriter(peripheral.SaveFile, true))
            {
                for (int k = 0; k < newData.Length; k++)
                {
                    long freq = 30;

                    // Uptime
                    string outputLine = string.Format("{0}", peripheral.UptimeMilliseconds - (((newData.Length - 1) - k) / freq));

                    // Name
                    outputLine += "," + peripheral.Name;

                    // Main data
                    outputLine += "," + newData[k];

                    // Battery level
                    if (peripheral.Characteristics.ContainsKey("Battery"))
                    {
                        outputLine += string.Format(",{0}", peripheral.Characteristics["Battery"].Value);
                    }

                    // Temperature
                    if (peripheral.Characteristics.ContainsKey("Temperature"))
                    {
                        outputLine += string.Format(",{0:0.0}", peripheral.Characteristics["Temperature"].Value);
                    }
                    output.WriteLine(outputLine);
                }
            }
        }
        #endregion
    }
}
