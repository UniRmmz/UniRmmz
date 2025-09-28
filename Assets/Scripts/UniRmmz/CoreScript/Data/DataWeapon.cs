using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public partial class DataWeapon : EquipableItem
    {
        public int AnimationId => animationId;
        public override int Price => price;
        public int WtypeId => wtypeId;
        
        protected int animationId;
        protected int price;
        protected int wtypeId;
    }
}