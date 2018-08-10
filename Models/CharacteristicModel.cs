using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;

namespace BlueEyes.Models
{
    /// <summary>
    /// A Characteristic is an attribute containing: a value, properties, and descriptors
    /// </summary>
    public class Characteristic : AttributeGroup
    {
        private ConcurrentDictionary<string, Attribute> _descriptors = new ConcurrentDictionary<string, Attribute>();
        private double _value;
        private Attribute _valueAttribute = new Attribute();

        public ConcurrentDictionary<string,Attribute> Descriptors
        {
            get { return _descriptors; }
            set { SetProperty(ref _descriptors, value); }
        }

        public double Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }

        public Attribute ValueAttribute
        {
            get { return _valueAttribute; }
            set { SetProperty(ref _valueAttribute, value); }
        }

        public List<Attribute> GetDescriptors()
        {
            return _descriptors.Values.OrderBy(o => o.Handle).ToList();
        }
    }
}
