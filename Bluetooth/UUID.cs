using System;
using System.Linq;
using System.Reflection;

namespace Bluetooth
{
    public static class Services
    {
        public static byte[] GenericAccess
        { get { return BitConverter.GetBytes((ushort)0x1800); } }

        public static byte[] GenericAttribute
        { get { return BitConverter.GetBytes((ushort)0x1801); } }

        public static byte[] ImmediateAlert
        { get { return BitConverter.GetBytes((ushort)0x1802); } }

        public static byte[] LinkLoss
        { get { return BitConverter.GetBytes((ushort)0x1803); } }

        public static byte[] TxPower
        { get { return BitConverter.GetBytes((ushort)0x1804); } }

        public static byte[] CurrentTime
        { get { return BitConverter.GetBytes((ushort)0x1805); } }

        public static byte[] ReferenceTimeUpdate
        { get { return BitConverter.GetBytes((ushort)0x1806); } }

        public static byte[] NextDSTChange
        { get { return BitConverter.GetBytes((ushort)0x1807); } }

        public static byte[] Glucose
        { get { return BitConverter.GetBytes((ushort)0x1808); } }

        public static byte[] HealthThermometer
        { get { return BitConverter.GetBytes((ushort)0x1809); } }

        public static byte[] DeviceInformation
        { get { return BitConverter.GetBytes((ushort)0x180A); } }

        public static byte[] HeartRate
        { get { return BitConverter.GetBytes((ushort)0x180D); } }

        public static byte[] PhoneAlertStatus
        { get { return BitConverter.GetBytes((ushort)0x180E); } }

        public static byte[] BatteryService
        { get { return BitConverter.GetBytes((ushort)0x180F); } }

        public static byte[] BloodPressure
        { get { return BitConverter.GetBytes((ushort)0x1810); } }

        public static byte[] AlertNotification
        { get { return BitConverter.GetBytes((ushort)0x1811); } }

        public static byte[] HumanInterfaceDevice
        { get { return BitConverter.GetBytes((ushort)0x1812); } }

        public static byte[] ScanParameters
        { get { return BitConverter.GetBytes((ushort)0x1813); } }

        public static byte[] RunningSpeedAndCadence
        { get { return BitConverter.GetBytes((ushort)0x1814); } }

        public static byte[] AutomationIO
        { get { return BitConverter.GetBytes((ushort)0x1815); } }

        public static byte[] CyclingSpeedAndCadence
        { get { return BitConverter.GetBytes((ushort)0x1816); } }

        public static byte[] CyclingPower
        { get { return BitConverter.GetBytes((ushort)0x1818); } }

        public static byte[] LocationAndNavigation
        { get { return BitConverter.GetBytes((ushort)0x1819); } }

        public static byte[] EnvironmentalSensing
        { get { return BitConverter.GetBytes((ushort)0x181A); } }

        public static byte[] BodyComposition
        { get { return BitConverter.GetBytes((ushort)0x181B); } }

        public static byte[] UserData
        { get { return BitConverter.GetBytes((ushort)0x181C); } }

        public static byte[] WeightScale
        { get { return BitConverter.GetBytes((ushort)0x181D); } }

        public static byte[] BondManagementService
        { get { return BitConverter.GetBytes((ushort)0x181E); } }

        public static byte[] ContinuousGlucoseMonitoring
        { get { return BitConverter.GetBytes((ushort)0x181F); } }

        public static byte[] InternetProtocolSupport
        { get { return BitConverter.GetBytes((ushort)0x1820); } }

        public static byte[] IndoorPositioning
        { get { return BitConverter.GetBytes((ushort)0x1821); } }

        public static byte[] PulseOximeter
        { get { return BitConverter.GetBytes((ushort)0x1822); } }

        public static byte[] HTTPProxy
        { get { return BitConverter.GetBytes((ushort)0x1823); } }

        public static byte[] TransportDiscovery
        { get { return BitConverter.GetBytes((ushort)0x1824); } }

        public static byte[] ObjectTransfer
        { get { return BitConverter.GetBytes((ushort)0x1825); } }

        public static byte[] FitnessMachine
        { get { return BitConverter.GetBytes((ushort)0x1826); } }

        public static byte[] MeshProvisioning
        { get { return BitConverter.GetBytes((ushort)0x1827); } }

        public static byte[] MeshProxy
        { get { return BitConverter.GetBytes((ushort)0x1828); } }

        public static byte[] ReconnectionConfiguration
        { get { return BitConverter.GetBytes((ushort)0x1829); } }
    }

    public static class Declarations
    {
        public static byte[] PrimaryService
        { get { return BitConverter.GetBytes((ushort)0x2800); } }

        public static byte[] SecondaryService
        { get { return BitConverter.GetBytes((ushort)0x2801); } }

        public static byte[] Include
        { get { return BitConverter.GetBytes((ushort)0x2802); } }

        public static byte[] Characteristic
        { get { return BitConverter.GetBytes((ushort)0x2803); } }
    }

    public static class Descriptors
    {
        public static byte[] CharacteristicExtendedProperties
        { get { return BitConverter.GetBytes((ushort)0x2900); } }

        public static byte[] CharacteristicUserDescription
        { get { return BitConverter.GetBytes((ushort)0x2901); } }

        public static byte[] ClientCharacteristicConfiguration
        { get { return BitConverter.GetBytes((ushort)0x2902); } }

        public static byte[] ServerCharacteristicConfiguration
        { get { return BitConverter.GetBytes((ushort)0x2903); } }

        public static byte[] CharacteristicPresentationFormat
        { get { return BitConverter.GetBytes((ushort)0x2904); } }

        public static byte[] CharacteristicAggregateFormat
        { get { return BitConverter.GetBytes((ushort)0x2905); } }

        public static byte[] ValidRange
        { get { return BitConverter.GetBytes((ushort)0x2906); } }

        // etc...
    }

    public static class Characteristics
    {
        public static byte[] DeviceName
        { get { return BitConverter.GetBytes((ushort)0x2A00); } }

        public static byte[] Appearance
        { get { return BitConverter.GetBytes((ushort)0x2A01); } }

        public static byte[] PeripheralPrivacyFlag
        { get { return BitConverter.GetBytes((ushort)0x2A02); } }

        public static byte[] ReconnectionAddress
        { get { return BitConverter.GetBytes((ushort)0x2A03); } }

        // etc...
    }

    public static class Custom
    {
        public static byte[] Data
        { get { return new byte[] { 0x66, 0xFE, 0xD5, 0x33, 0xF9, 0xB2, 0x14, 0xB1, 0xE7, 0x11, 0x20, 0x51, 0xC6, 0x74, 0x35, 0xAC }; } }

        public static byte[] LPM
        { get { return new byte[] { 0x5B, 0x97, 0xAC, 0xDE, 0x8A, 0xE4, 0x2B, 0x89, 0xFA, 0x46, 0xEF, 0x75, 0xBA, 0x16, 0x22, 0x93 }; } }

        public static byte[] Rheostat
        { get { return new Byte[] { 0x66, 0xFE, 0xD5, 0x33, 0xF9, 0xB2, 0x14, 0xB1, 0xE7, 0x11, 0x1B, 0x55, 0xB6, 0xE3, 0xFA, 0x2A }; } }

        public static byte[] Battery
        { get { return new Byte[] { 0xA0, 0xDB, 0xD3, 0x6A, 0x00, 0xA6, 0x7B, 0x85, 0xE7, 0x11, 0xA4, 0x70, 0xA6, 0x32, 0x1E, 0x75 }; } }

        public static byte[] Temperature
        { get { return new Byte[] { 0xA1, 0xDB, 0xD3, 0x6A, 0x00, 0xA6, 0x7B, 0x85, 0xE7, 0x11, 0xA4, 0x70, 0xA6, 0x32, 0x1E, 0x74 }; } }
    }

    public static class Parser
    {
        public static string Lookup(byte[] uuid)
        {
            // 16-bit UUIDs
            if (uuid.Length == 2)
            {
                if (BitConverter.IsLittleEndian)
                {
                    if (uuid.Last() == 0x18)
                    {
                        return LookupServices(uuid);
                    }

                    else if (uuid.Last() == 0x28)
                    {
                        return LookupDeclarations(uuid);
                    }

                    else if (uuid.Last() == 0x29)
                    {
                        return LookupDescriptors(uuid);
                    }

                    else if (uuid.Last() == 0x2A)
                    {
                        return LookupCharacteristics(uuid);
                    }

                    else
                        return null;
                }
                else
                {
                    if (uuid.First() == 0x18)
                    {
                        return LookupServices(uuid);
                    }

                    else if (uuid.First() == 0x28)
                    {
                        return LookupDeclarations(uuid);
                    }

                    else if (uuid.First() == 0x29)
                    {
                        return LookupDescriptors(uuid);
                    }

                    else if (uuid.First() == 0x2A)
                    {
                        return LookupCharacteristics(uuid);
                    }

                    else
                        return null;
                }
            }

            // 128-bit UUIDs
            else if (uuid.Length == 16)
            {
                return LookupCustom(uuid);
            }
            
            return null;
        }

        private static string LookupCharacteristics(byte[] uuid)
        {
            foreach (PropertyInfo pi in typeof(Characteristics).GetProperties())
            {
                if (pi.PropertyType == typeof(byte[]))
                {
                    if (uuid.SequenceEqual((byte[])pi.GetValue(null, null)))
                    {
                        return pi.Name;
                    }
                }
            }

            return null;
        }

        private static string LookupCustom(byte[] uuid)
        {
            foreach (PropertyInfo pi in typeof(Custom).GetProperties())
            {
                if (pi.PropertyType == typeof(byte[]))
                {
                    if (uuid.SequenceEqual((byte[])pi.GetValue(null, null)))
                    {
                        return pi.Name;
                    }
                }
            }

            return null;
        }

        private static string LookupDeclarations(byte[] uuid)
        {
            foreach (PropertyInfo pi in typeof(Declarations).GetProperties())
            {
                if (pi.PropertyType == typeof(byte[]))
                {
                    if (uuid.SequenceEqual((byte[])pi.GetValue(null, null)))
                    {
                        Console.WriteLine("Name is " + pi.Name);
                        return pi.Name;
                    }
                }
            }

            return null;
        }

        private static string LookupDescriptors(byte[] uuid)
        {
            foreach (PropertyInfo pi in typeof(Descriptors).GetProperties())
            {
                if (pi.PropertyType == typeof(byte[]))
                {
                    if (uuid.SequenceEqual((byte[])pi.GetValue(null, null)))
                    {
                        return pi.Name;
                    }
                }
            }

            return null;
        }

        private static string LookupServices(byte[] uuid)
        {
            foreach (PropertyInfo pi in typeof(Services).GetProperties())
            {
                if (pi.PropertyType == typeof(byte[]))
                {
                    if (uuid.SequenceEqual((byte[])pi.GetValue(null, null)))
                    {
                        return pi.Name;
                    }
                }
            }

            return null;
        }
    }
}
