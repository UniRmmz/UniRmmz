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
        
        private string message1;
        private string message2;
        private int mpCost;
        private int requiredWtypeId1;
        private int requiredWtypeId2;
        private int stypeId;
        private int tpCost;
        private int messageType;
    }
}