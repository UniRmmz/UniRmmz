using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UniRmmz
{
    [Serializable]
    public partial class DataArmor : EquipableItem
    {
        public int AtypeId => atypeId;
        public override int Price => price;
        
        private int atypeId;
        private int price;
    }
}