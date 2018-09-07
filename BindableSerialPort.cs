using System.ComponentModel;
using System.IO.Ports;
using System.Runtime.CompilerServices;

namespace BlueEyes
{
    public class BindableSerialPort : SerialPort, INotifyPropertyChanged
    {
        #region SerialPort
        public new string PortName
        {
            get { return base.PortName; }
            set
            {
                base.PortName = value;
                NotifyPropertyChanged();
            }
        }

        public new void Close()
        {
            base.Close();
            NotifyPropertyChanged("IsOpen");
        }

        public new void Open()
        {
            base.Open();
            NotifyPropertyChanged("IsOpen");
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(member, val)) return;

            member = val;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
