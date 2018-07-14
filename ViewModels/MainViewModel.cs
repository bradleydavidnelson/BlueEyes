using BlueEyes.Commands;
using BlueEyes.Models;
using BlueEyes.Resources;
using BlueEyes.Views;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace BlueEyes.ViewModels
{
    class MainViewModel : BindableBase
    {
        public LogView LogWindow = new LogView();
        private SerialPortModel _port = new SerialPortModel();
        private SerialNameModel _selectedPort;

        // Commands
        private ICommand hiButtonCommand;
        private ICommand toggleExecuteCommand { get; set; }
        private ICommand _exitCommand;
        private ICommand _serialOpenCloseCommand;
        private ICommand _viewLogCommand;

        private bool _can_execute = true;

        #region Constructors
        public MainViewModel()
        {
            HiButtonCommand = new DelegateCommand(ShowMessage, param => _can_execute);
            toggleExecuteCommand = new DelegateCommand(ChangeCanExecute);
            
            _port.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceivedHandler);
        }
        #endregion

        #region Properties
        public string HiButtonContent
        {
            get { return "click to hi"; }
        }

        public bool CanExecute
        {
            get { return _can_execute; }
            set
            {
                if (_can_execute != value)
                {
                    _can_execute = value;
                }
            }
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
            set
            {
                _selectedPort = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ToggleExecuteCommand
        {
            get
            {
                if (toggleExecuteCommand == null)
                {
                    toggleExecuteCommand = new DelegateCommand(ChangeCanExecute);
                }
                return toggleExecuteCommand;
            }
            set { toggleExecuteCommand = value; }
        }

        public ICommand HiButtonCommand
        {
            get
            {
                if (hiButtonCommand == null)
                {
                    hiButtonCommand = new DelegateCommand(ShowMessage, param => _can_execute);
                }
                return hiButtonCommand;
            }
            set { hiButtonCommand = value; }
        }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new DelegateCommand(Exit);
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
                    _serialOpenCloseCommand = new DelegateCommand(SerialOpenClose);
                }
                return _serialOpenCloseCommand;
            }
            set { _serialOpenCloseCommand = value; }
        }

        public ICommand ViewLogCommand
        {
            get
            {
                if (_viewLogCommand == null)
                {
                    _viewLogCommand = new DelegateCommand(ViewLog);
                }
                return _viewLogCommand;
            }
            set { _viewLogCommand = value; }
        }
        #endregion

        #region Methods
        public void ChangeCanExecute(object obj)
        {
            _can_execute = !_can_execute;
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
                //TODO: bglib.Parse(inData[i]);
            }
        }

        private void SerialOpenClose(object obj)
        {
            if (_port.IsOpen)
            {
                _port.Close();
                NotifyPropertyChanged("Port_IsOpen");
            }
            else
            {
                _port.PortName = _selectedPort.PortName;
                _port.Open();
                _port.DiscardInBuffer();
                _port.DiscardOutBuffer();
                NotifyPropertyChanged("Port_IsOpen");
            }
        }

        public void ShowMessage(object obj)
        {
            MessageBox.Show(obj.ToString());
        }

        private void ViewLog(object obj)
        {
            if (LogWindow.IsVisible)
            {
                LogWindow.Hide();
            }
            else
            {
                LogWindow.Show();
            }
        }
        #endregion
    }
}
