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
        public static readonly KeyValueMap<byte, string> AD_Type = new KeyValueMap<byte, string>
        {
            {0x01, "Flags"},
            {0x02, "Incomplete List of 16-bit Service Class UUIDs" },
            {0x03, "Complete List of 16-bit Service Class UUIDs" },
            {0x04, "Incomplete List of 32-bit Service Class UUIDs" },
            {0x05, "Complete List of 32-bit Service Class UUIDs" },
            {0x06, "Incomplete List of 128-bit Service Class UUIDs" },
            {0x07, "Complete List of 128-bit Service Class UUIDs" },
            {0x08, "Shortened Local Name"},
            {0x09, "Complete Local Name"},
            {0x0A, "Tx Power Level" },
            {0x0D, "Class of Device" },
            {0x0E, "Simple Pairing Hash C" },
            {0x0F, "Simple Pairing Randomizer R" },
            {0x10, "Device ID" },
            {0x11, "Security Manager Out of Band Flags" },
            {0x12, "Slave Connection Interval Range" }
            // etc.
        };
    }
}
