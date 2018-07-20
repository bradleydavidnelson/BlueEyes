using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.Models
{
    class BLEPeripheralCollection : ObservableCollection<BLEPeripheral>
    {
        public BLEPeripheralCollection() : base()
        { }
    }
}
