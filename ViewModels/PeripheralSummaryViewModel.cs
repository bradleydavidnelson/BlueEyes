using BlueEyes.Models;
using BlueEyes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BlueEyes.ViewModels
{
    class PeripheralSummaryViewModel : BindableBase
    {
        #region Fields
        private Bluegiga.BGLib bglib = (Bluegiga.BGLib)Application.Current.FindResource("BGLib");
        private BLEPeripheralCollection _connectedPeripherals = (BLEPeripheralCollection)Application.Current.FindResource("ConnectedPeripherals");

        private BLEPeripheral _selectedPeripheral = null;
        #endregion
    }
}
