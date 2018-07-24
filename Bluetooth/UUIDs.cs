using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bluetooth
{
    public struct Attribute
    {
        public static byte[] PrimaryService
        {
            get { return BitConverter.GetBytes((ushort)0x2800); }
        }

        public static byte[] SecondaryService
        {
            get { return BitConverter.GetBytes((ushort)0x2801); }
        }

        public static byte[] Include
        {
            get { return BitConverter.GetBytes((ushort)0x2802); }
        }

        public static byte[] Characteristic
        {
            get { return BitConverter.GetBytes((ushort)0x2803); }
        }
    }
}
