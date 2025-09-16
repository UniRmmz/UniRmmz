using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public class DataCommonEvent
    {
        public int Id => id;
        public List<DataEventCommand> List => list;
        public string Name => name;
        public int SwitchId => switchId;
        public int Trigger => trigger;
        
        public int id;
        public List<DataEventCommand> list;
        public string name;
        public int switchId;
        public int trigger;        
    }
    
    [Serializable]
    public class DataEventCommand
    {
        public int Code => code;
        public int Indent => indent;
        public object[] Parameters => parameters;
        
        public int code;
        public int indent;
        public object[] parameters;
    }
}