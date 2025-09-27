using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            try
            {
                _metadata.Add(key, value);
            }
            catch (ArgumentException e)
            {
                // キーの重複？
                Debug.LogError(e.ToString());
            }
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