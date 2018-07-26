using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BlueEyes.Models
{
    class BLEPeripheralCollection : ObservableCollection<BLEPeripheral>
    {
        public BLEPeripheralCollection() : base()
        { }

        public bool TryRemoveByConnection(byte connection, out BLEPeripheral target)
        {
            foreach (BLEPeripheral p in this)
            {
                if (p.Connection == connection)
                {
                    target = p;
                    Remove(p);
                    return true;
                }
            }

            target = null;
            return false;
        }

        public bool TryGetConnection(byte connection, out BLEPeripheral target)
        {
            foreach (BLEPeripheral p in this)
            {
                if (p.Connection == connection)
                {
                    target = p;
                    return true;
                }
            }

            target = null;
            return false;
        }
    }
}
