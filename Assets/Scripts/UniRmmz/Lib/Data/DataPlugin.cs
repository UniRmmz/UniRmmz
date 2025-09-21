using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public class DataPlugin
    {
        public string Name => name;
        public bool Status => status;
        public string Description => description;
        public Dictionary<string, object> Parameters => parameters;
        
        private string name;
        private bool status;
        private string description;
        private Dictionary<string, object> parameters;
    }
}