using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public class DataSkill : UsableItem
    {
        public string Message1 => message1;
        public string Message2 => message2;
        public int MpCost => mpCost;
        public int RequiredWtypeId1 => requiredWtypeId1;
        public int RequiredWtypeId2 => requiredWtypeId2;
        public int StypeId => stypeId;
        public int TpCost => tpCost;
        public int MessageType => messageType;
        
        public string message1;
        public string message2;
        public int mpCost;
        public int requiredWtypeId1;
        public int requiredWtypeId2;
        public int stypeId;
        public int tpCost;
        public int messageType;
    }
}