using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    public class RmmzMetadata
    {
        private Dictionary<string, object> _metadata = new();

        public void Add(string key)
        {
            _metadata.Add(key, true);
        }
        
        public void Add(string key, object value)
        {
            _metadata.Add(key, value);
        }

        public string Value(string key)
        {
            return Convert.ToString(_metadata.GetValueOrDefault(key));
        }
        
        public bool Contains(string key)
        {
            return _metadata.ContainsKey(key);
        }
    }
}