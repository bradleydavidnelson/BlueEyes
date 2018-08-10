using System.Collections.Concurrent;
using System.Dynamic;

namespace BlueEyes
{
    public class DynamicDictionary : DynamicObject
    {
        ConcurrentDictionary<string, object> dictionary = new ConcurrentDictionary<string, object>();

        public int Count
        {
            get { return dictionary.Count; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return dictionary.TryAdd(binder.Name, value);
        }
    }
}
