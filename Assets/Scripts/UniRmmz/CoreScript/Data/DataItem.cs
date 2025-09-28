using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public partial class DataItem : UsableItem
    {
        public bool Consumable => consumable;
        public int ItypeId => itypeId;
        public override int Price => price;
        
        private bool consumable;
        private int itypeId;
        private int price;
    }
}