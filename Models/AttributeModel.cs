using BlueEyes.Utilities;

namespace BlueEyes.Models
{
    public class Attribute : BindableBase
    {
        private string _description = null;
        private ushort _handle;
        private byte[] _uuid = new byte[] { };

        public Attribute()
        { }

        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        public ushort Handle
        {
            get { return _handle; }
            set { SetProperty(ref _handle, value); }
        }

        public byte[] UUID
        {
            get { return _uuid; }
            set { SetProperty(ref _uuid, value); }
        }

        public override string ToString()
        {
            return _description;
        }
    }
}
