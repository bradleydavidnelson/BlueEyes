using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.Models
{
    public class Characteristic : Attribute
    {
        private Collection<Attribute> _attributes = new Collection<Attribute>();

        public Collection<Attribute> Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }
    }
}
