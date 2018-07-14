using BlueEyes.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.Models
{
    class SerialNameModel : BindableBase
    {
        private string _name;
        private string _friendlyName;

        #region Constructors
        public SerialNameModel(string portName, string friendlyPortName)
        {
            _name = portName;
            _friendlyName = friendlyPortName;
        }
        #endregion

        #region Properties
        public string FriendlyPortName
        {
            get { return _friendlyName; }
            set
            {
                _friendlyName = value;
                NotifyPropertyChanged();
            }
        }

        public string PortName
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
