using BlueEyes.Utilities;
using BlueEyes.ViewModels;
using BlueEyes.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BlueEyes.Models
{
    public class Attribute : BindableBase
    {
        private Bluegiga.BGLib bglib = (Bluegiga.BGLib)Application.Current.FindResource("BGLib");

        private string _description = null;
        private UInt32 _editableValue = 0;
        private ushort _handle;
        private BLEPeripheral _parentPeripheral;
        private byte[] _uuid = new byte[] { };
        private byte[] _value = new byte[] { };

        private ICommand editEntryCommand;
        private ICommand writeValueCommand;

        public Attribute()
        { }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public string DisplayValue
        { 
            get { return BitConverter.ToString(_value); }
            // TODO Set write
        }

        public UInt32 EditableValue
        {
            get { return _editableValue; }
            set { SetProperty(ref _editableValue, value); }
        }

        public ushort Handle
        {
            get { return _handle; }
            set { SetProperty(ref _handle, value); }
        }

        public BLEPeripheral ParentPeripheral
        {
            get { return _parentPeripheral; }
            set { SetProperty(ref _parentPeripheral, value); }
        }

        public byte[] UUID
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }

        public byte[] Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public ICommand EditEntryCommand
        {
            get
            {
                if (editEntryCommand == null)
                {
                    editEntryCommand = new RelayCommand(OpenGATTEditor);
                }
                return editEntryCommand;
            }
        }

        public ICommand WriteValueCommand
        {
            get
            {
                if (writeValueCommand == null)
                {
                    writeValueCommand = new RelayCommand(TryWriteValue);
                }
                return writeValueCommand;
            }
        }

        private void OpenGATTEditor(object obj)
        {
            // Create window
            MessageWriter.LogWrite("Opening attribute editor...");
            Window editor = new EditGATTValueWindow();
            editor.DataContext = this;

            // Check current value
            byte[] cmd = bglib.BLECommandATTClientReadByHandle(_parentPeripheral.Connection, _handle);
            MessageWriter.LogWrite("ble_cmd_att_client_read_by_handle: ", string.Format("connection={0}, handle={1}",
                _parentPeripheral.Connection, _handle));
            MessageWriter.BLEWrite(cmd);

            editor.Show();
        }

        private void TryWriteValue(object obj)
        {
            try {
                byte[] data = { BitConverter.GetBytes(EditableValue)[0] };
                byte[] cmd = bglib.BLECommandATTClientAttributeWrite(_parentPeripheral.Connection, _handle, data);
                MessageWriter.LogWrite("ble_cmd_att_client_attribute_write: ", string.Format("connection={0}, handle={1}, data={2}",
                    _parentPeripheral.Connection, Handle, BitConverter.ToString(data)));
                MessageWriter.BLEWrite(cmd);
            }
            catch (Exception ex)
            {
                MessageWriter.LogWrite("Error writing value!");
            }
        }

        public override string ToString()
        {
            return _description;
        }
    }
}
