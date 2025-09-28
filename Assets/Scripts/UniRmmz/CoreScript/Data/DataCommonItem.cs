using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace UniRmmz
{
    [Serializable]
    public partial class DataCommonItem : IMetadataContainer
    {
        public int Id => id;
        public string Description => description;
        public int IconIndex => iconIndex;
        public string Name => name;
        public virtual int Price => 0;
        public string Note => note;
        public RmmzMetadata Meta { get; set; }

        protected int id;
        protected string description;
        protected int iconIndex;
        protected string name;
        protected string note;
    }
    
    public partial class UsableItem : DataCommonItem 
    {
        public int AnimationId => animationId;
        public int Occasion => occasion;
        public DataDamage Damage => damage;
        public List<DataEffect> Effects => effects;
        public int HitType => hitType;
        public int Repeats => repeats;
        public int Scope => scope;
        public int Speed => speed;
        public int SuccessRate => successRate;
        public int TpGain => tpGain;
        
        protected int animationId;
        protected int occasion;
        protected DataDamage damage;
        protected List<DataEffect> effects;
        protected int hitType;
        protected int repeats;
        protected int scope;
        protected int speed;
        protected int successRate;
        protected int tpGain;
    }
    
    public partial class EquipableItem : DataCommonItem, ITraitsObject
    {
        public int EtypeId => etypeId;
        public int[] Params => @params;
        public DataTrait[] Traits => traits;
        
        protected int etypeId;
        protected int[] @params;
        protected DataTrait[] traits;
    }
}