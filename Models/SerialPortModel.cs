using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.Models
{
    public class SerialPortModel : INotifyPropertyChanged
    {
        private string _friendly_port_name;
        private string _port_name;

        public SerialPortModel(string portName, string friendlyPortName)
        {
            _port_name = portName;
            _friendly_port_name = friendlyPortName;
        }

        public string FriendlyPortName
        {
            get { return _friendly_port_name; }
            set
            {
                _friendly_port_name = value;
                NotifyPropertyChanged();
            }
        }

        public string PortName
        {
            get { return _port_name; }
            set
            {
                _port_name = value;
                NotifyPropertyChanged();
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
