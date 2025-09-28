using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public partial class DataCommonEvent
    {
        public int Id => id;
        public List<DataEventCommand> List => list;
        public string Name => name;
        public int SwitchId => switchId;
        public int Trigger => trigger;
        
        protected int id;
        protected List<DataEventCommand> list;
        protected string name;
        protected int switchId;
        protected int trigger;        
    }
    
    [Serializable]
    public partial class DataEventCommand
    {
        public int Code => code;
        public int Indent => indent;
        public object[] Parameters => parameters;
        
        protected int code;
        protected int indent;
        protected object[] parameters;
    }
}