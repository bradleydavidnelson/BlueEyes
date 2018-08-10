using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.Models
{
    public class AttributeGroup : Attribute
    {
        private Attribute _declaration;
        private ushort _groupEnd;

        public Attribute Declaration
        {
            get { return _declaration; }
            set { SetProperty(ref _declaration, value); }
        }

        public ushort GroupEnd
        {
            get { return _groupEnd; }
            set { SetProperty(ref _groupEnd, value); }
        }
    }
}
