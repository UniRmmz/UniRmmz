using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UniRmmz
{
    [Serializable]
    public partial class DataEnemy : ITraitsObject, IMetadataContainer
    {
        public int Id => id;
        public List<DataEnemyAction> Actions => actions;
        public int BattlerHue => battlerHue;
        public string BattlerName => battlerName;
        public List<DataDropItem> DropItems => dropItems;
        public int Exp => exp;
        public DataTrait[] Traits => traits;
        public int Gold => gold;
        public string Name => name;
        public string Note => note;
        public RmmzMetadata Meta { get; set; }
        public int[] Params => @params;
        
        protected int id;
        protected List<DataEnemyAction> actions;
        protected int battlerHue;
        protected string battlerName;
        protected List<DataDropItem> dropItems;
        protected int exp;
        protected DataTrait[] traits;
        protected int gold;
        protected string name;
        protected string note;
        protected int[] @params;
    }

    [Serializable]
    public partial class DataEnemyAction
    {
        public int SkillId => skillId;
        public int ConditionType => conditionType;
        public float ConditionParam1 => conditionParam1;
        public float ConditionParam2 => conditionParam2;
        public int Rating => rating;
        
        protected int skillId;
        protected int conditionType;
        protected float conditionParam1;
        protected float conditionParam2;
        protected int rating;
    }

    [Serializable]
    public partial class DataDropItem
    {
        public int Kind => kind;
        public int DataId => dataId;
        public int Denominator => denominator;
        
        protected int kind;
        protected int dataId;
        protected int denominator;
    }
}