using BlueEyes.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluetooth
{
    public static class GenericAccessProfile
    {
        public static readonly KeyValueMap<byte, string> AssignedNumbers = new KeyValueMap<byte, string>
        {
            {0x01, "Flags"},
            {0x08, "Shortened Local Name"},
            {0x09, "Complete Local Name"}
        };
    }
}
