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

        public int id;
        public string description;
        public int iconIndex;
        public string name;
        public string note;
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
        
        public int animationId;
        public int occasion;
        public DataDamage damage;
        public List<DataEffect> effects;
        public int hitType;
        public int repeats;
        public int scope;
        public int speed;
        public int successRate;
        public int tpGain;
    }
    
    public class EquipableItem : DataCommonItem, ITraitsObject
    {
        public int EtypeId => etypeId;
        public int[] Params => @params;
        public DataTrait[] Traits => traits;
        
        public int etypeId;
        public int[] @params;
        public DataTrait[] traits;
    }
}