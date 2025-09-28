using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public partial class DataSkill : UsableItem
    {
        public string Message1 => message1;
        public string Message2 => message2;
        public int MpCost => mpCost;
        public int RequiredWtypeId1 => requiredWtypeId1;
        public int RequiredWtypeId2 => requiredWtypeId2;
        public int StypeId => stypeId;
        public int TpCost => tpCost;
        public int MessageType => messageType;
        
        protected string message1;
        protected string message2;
        protected int mpCost;
        protected int requiredWtypeId1;
        protected int requiredWtypeId2;
        protected int stypeId;
        protected int tpCost;
        protected int messageType;
    }
}