using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public class DataWeapon : EquipableItem
    {
        public int AnimationId => animationId;
        public override int Price => price;
        public int WtypeId => wtypeId;
        
        public int animationId;
        public int price;
        public int wtypeId;
    }
}