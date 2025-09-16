using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public class DataItem : UsableItem
    {
        public bool Consumable => consumable;
        public int ItypeId => itypeId;
        public override int Price => price;
        
        public bool consumable;
        public int itypeId;
        public int price;
    }
}