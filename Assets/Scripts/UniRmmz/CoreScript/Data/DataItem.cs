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
        
        protected bool consumable;
        protected int itypeId;
        protected int price;
    }
}