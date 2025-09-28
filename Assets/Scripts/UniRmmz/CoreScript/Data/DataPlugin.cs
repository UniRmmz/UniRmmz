using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public partial class DataPlugin
    {
        public string Name => name;
        public bool Status => status;
        public string Description => description;
        public Dictionary<string, string> Parameters => parameters;
        
        protected string name;
        protected bool status;
        protected string description;
        protected Dictionary<string, string> parameters;
    }
}