using System;
using System.Collections;
using System.Collections.Generic;

namespace BlueEyes.Utilities
{
    public class KeyValueMap<T1,T2> : IEnumerable<KeyValuePair<T1,T2>>
    {
        IDictionary<T1, T2> values = new Dictionary<T1, T2>();
        IDictionary<T2, T1> keys = new Dictionary<T2, T1>();

        public void Add(T1 key, T2 value)
        {
            if (values.ContainsKey(key) || keys.ContainsKey(value))
            {
                throw new ArgumentException("Name or value already exists.");
            }
            values.Add(key, value);
            keys.Add(value, key);
        }

        public T1 Get(T2 value)
        {
            if (keys.ContainsKey(value))
            {
                return keys[value];
            }
            else
            {
                return default(T1);
            }
        }

        public T2 Get(T1 key)
        {
            if (values.ContainsKey(key))
            {
                return values[key];
            }
            else
            {
                return default(T2);
            }
        }

        public bool TryGetValue(T1 key, out T2 value)
        {
            return values.TryGetValue(key, out value);
        }

        public bool TryGetName(T2 value, out T1 key)
        {
            return keys.TryGetValue(value, out key);
        }

        #region IEnumerable
        public IEnumerator<KeyValuePair<T1,T2>> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
