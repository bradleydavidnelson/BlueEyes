using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueEyes.Models
{
    public class Service : AttributeGroup
    {
        private ConcurrentDictionary<string,Characteristic> _characteristics = new ConcurrentDictionary<string,Characteristic>();
        private byte[] _groupUuid;

        #region Properties
        public ConcurrentDictionary<string, Characteristic> Characteristics
        {
            get { return _characteristics; }
            set { SetProperty(ref _characteristics, value); }
        }

        public byte[] GroupUUID
        {
            get { return _groupUuid; }
            set { SetProperty(ref _groupUuid, value); }
        }
        #endregion

        #region Methods
        public List<Characteristic> GetCharacteristics()
        {
            return _characteristics.Values.OrderBy(o => o.Handle).ToList();
        }
        #endregion
    }
}
