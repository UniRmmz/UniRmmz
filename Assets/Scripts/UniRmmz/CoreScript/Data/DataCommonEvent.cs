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
        
        private int id;
        private List<DataEventCommand> list;
        private string name;
        private int switchId;
        private int trigger;        
    }
    
    [Serializable]
    public partial class DataEventCommand
    {
        public int Code => code;
        public int Indent => indent;
        public object[] Parameters => parameters;
        
        private int code;
        private int indent;
        private object[] parameters;
    }
}