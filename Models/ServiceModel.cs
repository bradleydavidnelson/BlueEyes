using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.Models
{
    public class Service : Attribute
    {
        private Collection<Attribute> _attributes = new Collection<Attribute>();
        private Collection<Characteristic> _characteristics = new Collection<Characteristic>();
        private byte[] _groupUuid = new byte[] { };

        public Collection<Attribute> Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

        public Collection<Characteristic> Characteristics
        {
            get { return _characteristics; }
            set { _characteristics = value; }
        }

        public byte[] GroupUUID
        {
            get { return _groupUuid; }
            set { _groupUuid = value; }
        }
    }
}
