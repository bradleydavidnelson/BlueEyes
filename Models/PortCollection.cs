using BlueEyes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlueEyes.Models
{
    class SerialPortCollection : ObservableCollection<SerialNameModel>
    {
        public SerialPortCollection() : base()
        {
            try
            {
                // search for ports
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"Select * From Win32_SerialPort"))
                {
                    foreach (ManagementObject port in searcher.Get())
                    {
                        Add(new SerialNameModel((string)port["DeviceID"], (string)port["Caption"]));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
