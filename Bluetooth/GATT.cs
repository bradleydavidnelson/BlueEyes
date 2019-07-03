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

        public static byte[] ModelNumberString
        { get { return BitConverter.GetBytes((ushort)0x2A24); } }

        public static byte[] SerialNumberString
        { get { return BitConverter.GetBytes((ushort)0x2A25); } }

        public static byte[] FirmwareRevisionString
        { get { return BitConverter.GetBytes((ushort)0x2A26); } }

        public static byte[] HardwareRevisionString
        { get { return BitConverter.GetBytes((ushort)0x2A27); } }

        public static byte[] SoftwareRevisionString
        { get { return BitConverter.GetBytes((ushort)0x2A28); } }

        public static byte[] ManufacturerNameString
        { get { return BitConverter.GetBytes((ushort)0x2A29); } }

        // etc...
    }

    public static class Custom
    {
        // Main service
        public static byte[] CustomService
        { get { return new byte[] { 0x80, 0x64, 0xBF, 0xCC, 0x8A, 0x5F, 0x11, 0x90, 0x4F, 0x48, 0x28, 0xA8, 0x18, 0xA3, 0x51, 0xA4 }; } }

        public static byte[] Data
        { get { return new byte[] { 0x66, 0xFE, 0xD5, 0x33, 0xF9, 0xB2, 0x14, 0xB1, 0xE7, 0x11, 0x20, 0x51, 0xC6, 0x74, 0x35, 0xAC }; } }

        public static byte[] LPM
        { get { return new byte[] { 0x5B, 0x97, 0xAC, 0xDE, 0x8A, 0xE4, 0x2B, 0x89, 0xFA, 0x46, 0xEF, 0x75, 0xBA, 0x16, 0x22, 0x93 }; } }

        public static byte[] Battery
        { get { return new Byte[] { 0xA0, 0xDB, 0xD3, 0x6A, 0x00, 0xA6, 0x7B, 0x85, 0xE7, 0x11, 0xA4, 0x70, 0xA6, 0x32, 0x1E, 0x75 }; } }

        public static byte[] Temperature
        { get { return new Byte[] { 0xA1, 0xDB, 0xD3, 0x6A, 0x00, 0xA6, 0x7B, 0x85, 0xE7, 0x11, 0xA4, 0x70, 0xA6, 0x32, 0x1E, 0x74 }; } }

        // Rheostat Service
        public static byte[] RheostatCalibrationService
        { get { return new byte[] { 0x69, 0x14, 0xFB, 0x69, 0x92, 0x52, 0xB6, 0x9E, 0xE8, 0x11, 0x50, 0x9A, 0x50, 0x36, 0x8D, 0x7B }; } }

        public static byte[] RheostatValue
        { get { return new byte[] { 0x66, 0xFE, 0xD5, 0x33, 0xF9, 0xB2, 0x14, 0xB1, 0xE7, 0x11, 0x1B, 0x55, 0xB6, 0xE3, 0xFA, 0x2A }; } }

        public static byte[] RheostatMaxValue
        { get { return new byte[] { 0x59, 0x14, 0xFB, 0x69, 0x92, 0x52, 0xB6, 0x9E, 0xE8, 0x11, 0x51, 0x9A, 0xB4, 0xE6, 0x01, 0x6A }; } }

        public static byte[] RheostatSaveCommand
        { get { return new byte[] { 0x59, 0x14, 0xFB, 0x69, 0x92, 0x52, 0xB6, 0x9E, 0xE8, 0x11, 0x51, 0x9A, 0x12, 0xE8, 0x01, 0x6A }; } }

        // Battery Service
        public static byte[] BatteryService
        { get { return new byte[] { 0x59, 0x5A, 0x08, 0xE4, 0x86, 0x2A, 0x9E, 0x8F, 0xE9, 0x11, 0xA4, 0x75, 0xB2, 0x50, 0x3B, 0x76 }; } }

        public static byte[] ChargeState
        { get { return new byte[] { 0x3E, 0x3D, 0x66, 0xBE, 0x81, 0x16, 0x23, 0xA9, 0xE9, 0x11, 0x35, 0x73, 0x76, 0xEF, 0x79, 0x7D }; } }

        public static byte[] BatteryVoltage
        { get { return new byte[] { 0x3F, 0x3D, 0x66, 0xBE, 0x81, 0x16, 0x23, 0xA9, 0xE9, 0x11, 0x35, 0x73, 0x76, 0xEF, 0x79, 0x7D }; } }

        // Piezoelectric Service
        public static byte[] PiezoelectricService
        { get { return new byte[] { 0xBC, 0xBE, 0x1B, 0xE0, 0x7A, 0xFA, 0xC0, 0xAD, 0xE7, 0x11, 0xB4, 0x74, 0xC4, 0xAE, 0xE2, 0x5A }; } }

        public static byte[] PiezoMode
        { get { return new byte[] { 0xBB, 0xBE, 0x1B, 0xE0, 0x7A, 0xFA, 0xC0, 0xAD, 0xE8, 0x11, 0xB4, 0x74, 0x32, 0xB8, 0x3E, 0xA0 }; } }

        public static byte[] PiezoDriveControl
        { get { return new byte[] { 0xBC, 0xBE, 0x1B, 0xE0, 0x7A, 0xFA, 0xC0, 0xAD, 0xE8, 0x11, 0xB4, 0x74, 0x32, 0xB8, 0x3E, 0xA0 }; } }

        public static byte[] PiezoVoltage
        { get { return new byte[] { 0xBD, 0xBE, 0x1B, 0xE0, 0x7A, 0xFA, 0xC0, 0xAD, 0xE8, 0x11, 0xB4, 0x74, 0x32, 0xB8, 0x3E, 0xA0 }; } }
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
