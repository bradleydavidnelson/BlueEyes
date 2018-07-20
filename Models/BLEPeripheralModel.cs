using BlueEyes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.Models
{
    public class BLEPeripheralModel { }

    public class BLEPeripheral : BindableBase
    {
        #region Fields
        private byte[] _address;
        private ConnState _connectionState = ConnState.Disconnected;
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
        
        public string AddressString
        {
            get { return BitConverter.ToString(_address); }
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
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}
