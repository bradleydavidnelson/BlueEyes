namespace BlueEyes.Models
{
    public class Attribute
    {
        private string _description = null;
        private ushort handle;
        private byte[] uuid = new byte[] { };

        public Attribute()
        { }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ushort Handle
        {
            get { return handle; }
            set { handle = value; }
        }

        public byte[] UUID
        {
            get { return uuid; }
            set { uuid = value; }
        }

        public override string ToString()
        {
            return _description;
        }
    }
}
