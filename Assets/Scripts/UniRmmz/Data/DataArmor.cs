using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UniRmmz
{
    [Serializable]
    public class DataArmor : EquipableItem
    {
        public int AtypeId => atypeId;
        public override int Price => price;
        
        public int atypeId;
        public int price;
    }
}