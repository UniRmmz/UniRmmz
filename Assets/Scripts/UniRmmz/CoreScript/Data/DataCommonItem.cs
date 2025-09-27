using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace UniRmmz
{
    [Serializable]
    public class DataCommonItem : IMetadataContainer
    {
        public int Id => id;
        public string Description => description;
        public int IconIndex => iconIndex;
        public string Name => name;
        public virtual int Price => 0;
        public string Note => note;
        public RmmzMetadata Meta { get; set; }

        private int id;
        private string description;
        private int iconIndex;
        private string name;
        private string note;
    }
    
    public class UsableItem : DataCommonItem 
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
        
        private int animationId;
        private int occasion;
        private DataDamage damage;
        private List<DataEffect> effects;
        private int hitType;
        private int repeats;
        private int scope;
        private int speed;
        private int successRate;
        private int tpGain;
    }
    
    public class EquipableItem : DataCommonItem, ITraitsObject
    {
        public int EtypeId => etypeId;
        public int[] Params => @params;
        public DataTrait[] Traits => traits;
        
        private int etypeId;
        private int[] @params;
        private DataTrait[] traits;
    }
}