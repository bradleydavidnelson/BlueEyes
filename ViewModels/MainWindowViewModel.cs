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
        // Application Resources
        private Bluegiga.BGLib bglib = (Bluegiga.BGLib)Application.Current.FindResource("BGLib");
        private BLEPeripheralCollection discoveredPeripherals = (BLEPeripheralCollection)Application.Current.FindResource("DiscoveredPeripherals");
        private BLEPeripheralCollection connectedDevices = (BLEPeripheralCollection)Application.Current.FindResource("ConnectedPeripherals");
        private BindableSerialPort _selectedPort = new BindableSerialPort();

        private ConnectedDeviceViewModel _connectedDevicesVM = new ConnectedDeviceViewModel();
        private DiscoveredDeviceViewModel _discoveredDevicesVM = new DiscoveredDeviceViewModel();
        private byte[] cmd = new byte[] { };

        // Commands
        private ICommand _exitCommand;
        private ICommand _getGattCommand;
        private ICommand _openCalibrationWindowCommand;
        private ICommand _serialOpenCloseCommand;
        private ICommand _setSaveLocationCommand;
        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            Messenger.Default.Register<GenericMessage<byte[]>>(this, (action) => ReceiveMessage(action));

            // Event handlers
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
            get { return SelectedPort.IsOpen; }
        }

        public BindableSerialPort SelectedPort
        {
            get { return _selectedPort; }
            set { SetProperty(ref _selectedPort, value); }
        }

        public bool Settings_ShowLog
        {
            get { return Properties.Settings.Default.ShowLog; }
            set
            {
                Properties.Settings.Default.ShowLog = value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged();
            }
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

                BLEPeripheral peripheral = FindDiscoveredPeripheral(e.sender);

                // If the responder is undiscovereed, add it to the collection
                if (peripheral == null)
                {
                    peripheral = new BLEPeripheral(e.sender);
                    discoveredPeripherals.Add(peripheral);
                }

                // Update RSSI
                peripheral.RSSI = e.rssi;
                peripheral.Bond = e.bond;

                // Parse packet
                byte[] remainingData = e.data;
                while (remainingData.Length > 0)
                {
                    ushort elementLength = remainingData[0];
                    byte adType = remainingData[1];
                    byte[] element = remainingData.Skip(2).Take(elementLength - 1).ToArray();

                    if (!peripheral.AdData.ContainsKey(adType))
                    {
                        peripheral.AdData.Add(adType, BitConverter.ToString(element));
                    }

                    // Flags
                    if (adType == Bluetooth.GenericAccessProfile.AD_Type.Get("Flags"))
                    {
                        peripheral.Flags = element.FirstOrDefault();
                    }

                    // Complete local name
                    if (adType == Bluetooth.GenericAccessProfile.AD_Type.Get("Shortened Local Name"))
                    {
                        peripheral.ShortenedLocalName = Encoding.ASCII.GetString(element);
                    }

                    // Complete local name
                    if (adType == Bluetooth.GenericAccessProfile.AD_Type.Get("Complete Local Name"))
                    {
                        peripheral.Name = Encoding.ASCII.GetString(element);
                    }

                    remainingData = remainingData.Skip(elementLength + 1).ToArray();
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
                BLEPeripheral peripheral = FindDiscoveredPeripheral(e.address);

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
                bglib.SendCommand(SelectedPort, cmd);

                // Reset GAP Mode
                cmd = bglib.BLECommandGAPSetMode(0, 0);
                MessageWriter.LogWrite("ble_cmd_gap_set_mode: ","discover=0, connect=0");
                bglib.SendCommand(SelectedPort, cmd);

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
                    MessageWriter.DebugWrite("Unable to find connection " + e.connection);
                    return;
                }

                peripheral.AddNewAttribute(e.chrhandle, e.uuid);

                if (e.uuid.SequenceEqual(Bluetooth.Descriptors.ClientCharacteristicConfiguration))
                {
                    peripheral.attHandleCCC.Enqueue(e.chrhandle);
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

                if (peripheral.Characteristics.ContainsKey("LPM"))
                {
                    if (e.chrhandle == peripheral.Characteristics["LPM"].ValueAttribute.Handle)
                    {
                        // Check sleep mode
                        cmd = bglib.BLECommandATTClientReadByHandle(e.connection, peripheral.Characteristics["LPM"].ValueAttribute.Handle);
                        MessageWriter.LogWrite("ble_cmd_att_client_read_by_handle: ", string.Format("connection={0}, handle={1}",
                            e.connection, peripheral.Characteristics["LPM"].ValueAttribute.Handle));
                        bglib.SendCommand(SelectedPort, cmd);
                    }
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
                if (e.atthandle == peripheral.Characteristics["Data"].ValueAttribute.Handle)
                {
                    // Decode packet
                    // The following decodes each pair of bytes into a single data point
                    double[] val = new double[e.value.Length/2];
                    for (int i = 0; i < val.Length; i++)
                    {
                        short raw = BitConverter.ToInt16(e.value, 2 * i);
                        val[i] = raw * 2500 / 2047;
                    }

                    peripheral.SetCharacteristicValue("Data", val.Average());

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
                        peripheral.SetCharacteristicValue("Battery", data * 2500 / 2047);
                    }
                }

                // TEMPERATURE PACKET
                if (e.atthandle == peripheral.Characteristics["Temperature"].ValueAttribute.Handle)
                {
                    short value = BitConverter.ToInt16(e.value, 0);

                    peripheral.SetCharacteristicValue("Temperature", (value * 2500 / 2047) * 10 / 45 - 169); // Not really characterized, but it should be close
                }

                // Low Power Mode
                if (e.atthandle == peripheral.Characteristics["LPM"].ValueAttribute.Handle)
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
                bglib.SendCommand(SelectedPort, cmd);
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
                bglib.SendCommand(SelectedPort, cmd);
            }

            else if (!peripheral.Services.ContainsKey("CustomService"))
            {
                MessageWriter.LogWrite("Invalid device selected");
                peripheral.Disconnect();
            }

            else if (peripheral.attHandleCCC.Count > 0)
            {
                // Enable indications
                byte[] indications = new byte[] { 0x03, 0x00 };
                byte[] cmd = bglib.BLECommandATTClientAttributeWrite(peripheral.Connection, peripheral.attHandleCCC.Peek(), indications);
                MessageWriter.LogWrite("ble_cmd_att_client_attribute_write: ", string.Format("connection={0}, att_handle={1}, data={2}",
                    peripheral.Connection, peripheral.attHandleCCC.Dequeue(), BitConverter.ToString(indications)));
                bglib.SendCommand(SelectedPort, cmd);
            }

            // Is the low power mode state known?
            else if (peripheral.Characteristics["LPM"].ValueAttribute.Handle > 0 && peripheral.LowPowerMode == BLEPeripheral.LPM.Unknown)
            {
                // Check sleep mode
                cmd = bglib.BLECommandATTClientReadByHandle(peripheral.Connection, peripheral.Characteristics["LPM"].ValueAttribute.Handle);
                MessageWriter.LogWrite("ble_cmd_att_client_read_by_handle: ", string.Format("connection={0}, handle={1}",
                    peripheral.Connection, peripheral.Characteristics["LPM"].ValueAttribute.Handle));
                bglib.SendCommand(SelectedPort, cmd);
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
            bglib.SendCommand(SelectedPort, cmd);

            // Begin discovery mode
            byte mode = 1;
            cmd = bglib.BLECommandGAPDiscover(mode); // generic discovery mode
            MessageWriter.LogWrite("ble_cmd_gap_discover: ", string.Format("mode={0}", mode));
            bglib.SendCommand(SelectedPort, cmd);
        }

        private void Exit(object obj)
        {
            Application.Current.Shutdown();
        }

        private BLEPeripheral FindDiscoveredPeripheral(byte[] address)
        {
            // Search discovered devices for address
            foreach (BLEPeripheral p in discoveredPeripherals)
            {
                if (p.Address.SequenceEqual(address))
                {
                    return p;
                }
            }

            // If peripheral was not found
            return null;
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

        private void OpenCalibrationWindow(object obj)
        {
            MessageWriter.LogWrite("Opening calibration window...");
            CalibrationWindow cal = new CalibrationWindow();
            cal.Show();
        }

        private void ReceiveMessage(GenericMessage<byte[]> message)
        {
            bglib.SendCommand(SelectedPort, message.Content);
        }

        [STAThread]
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

        [STAThread]
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
            // Close port
            if (SelectedPort.IsOpen)
            {
                MessageWriter.LogWrite(string.Format("Closing {0}...", SelectedPort.PortName));
                SelectedPort.Close();
                NotifyPropertyChanged("Port_IsOpen");
                MessageWriter.LogWrite(SelectedPort.PortName + " closed");

                SelectedPort.DataReceived -= new SerialDataReceivedEventHandler(SerialDataReceivedHandler);

                discoveredPeripherals.Clear();
            }

            // Open port
            else
            {
                MessageWriter.LogWrite(string.Format("Opening {0}...", SelectedPort.PortName));
                SelectedPort.PortName = SelectedPort.PortName;
                SelectedPort.Open();
                SelectedPort.DiscardInBuffer();
                SelectedPort.DiscardOutBuffer();
                NotifyPropertyChanged("Port_IsOpen");
                MessageWriter.LogWrite(SelectedPort.PortName + " opened");

                SelectedPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandler);

                BLEScan();
            }
        }

        private void WriteData(BLEPeripheral peripheral, double[] newData)
        {
            long timestamp = peripheral.UptimeMilliseconds;

            for (int k = 0; k < newData.Length; k++)
            {
                long period = 333;

                // Name
                string outputLine = peripheral.Name;

                // Uptime
                double measurementTime = timestamp - ((period / newData.Length) * (newData.Length - k + 1));
                outputLine += string.Format(",{0}", measurementTime);

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

                try
                {
                    using (StreamWriter output = new StreamWriter(peripheral.SaveFile, true))
                    {
                        output.WriteLine(outputLine);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageWriter.LogWrite("Error writing file. Check save location.");
                }
            }
        }
        #endregion
    }
}
