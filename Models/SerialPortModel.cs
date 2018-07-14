using BlueEyes.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlueEyes.Models
{
    public class SerialPortModel : BindableBase
    {
        private SerialPort _sp;

        #region Constructors
        public SerialPortModel()
        {
            _sp = new SerialPort("COM1",115200,Parity.None,8,StopBits.One);
        }
        #endregion

        #region Properties
        public int BaudRate
        {
            get { return _sp.BaudRate; }
            set
            {
                _sp.BaudRate = value;
                NotifyPropertyChanged();
            }
        }

        public int DataBits
        {
            get { return _sp.DataBits; }
            set
            {
                _sp.DataBits = value;
                NotifyPropertyChanged();
            }
        }

        public Handshake Handshake
        {
            get { return _sp.Handshake; }
            set
            {
                _sp.Handshake = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsOpen
        {
            get { return _sp.IsOpen; }
        }

        public Parity Parity
        {
            get { return _sp.Parity; }
            set
            {
                _sp.Parity = value;
                NotifyPropertyChanged();
            }
        }

        public string PortName
        {
            get { return _sp.PortName; }
            set
            {
                _sp.PortName = value;
                NotifyPropertyChanged();
            }
        }

        public StopBits StopBits
        {
            get { return _sp.StopBits; }
            set
            {
                _sp.StopBits = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public void Close()
        {
            _sp.Close();
            NotifyPropertyChanged("IsOpen");
        }

        public void DiscardInBuffer()
        {
            _sp.DiscardInBuffer();
        }

        public void DiscardOutBuffer()
        {
            _sp.DiscardOutBuffer();
        }

        public void Open()
        {
            _sp.Open();
            NotifyPropertyChanged("IsOpen");
        }
        #endregion

        #region Events
        public event SerialDataReceivedEventHandler DataReceived
        {
            add { _sp.DataReceived += value; }
            remove { _sp.DataReceived -= value; }
        }
        #endregion
    }
}
