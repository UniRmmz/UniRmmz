using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UniRmmz
{
    [Serializable]
    public class DataEnemy : ITraitsObject, IMetadataContainer
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
        
        public int id;
        public List<DataEnemyAction> actions;
        public int battlerHue;
        public string battlerName;
        public List<DataDropItem> dropItems;
        public int exp;
        public DataTrait[] traits;
        public int gold;
        public string name;
        public string note;
        public int[] @params;
    }

    [Serializable]
    public class DataEnemyAction
    {
        public int SkillId => skillId;
        public int ConditionType => conditionType;
        public float ConditionParam1 => conditionParam1;
        public float ConditionParam2 => conditionParam2;
        public int Rating => rating;
        
        public int skillId;
        public int conditionType;
        public float conditionParam1;
        public float conditionParam2;
        public int rating;
    }

    [Serializable]
    public class DataDropItem
    {
        public int Kind => kind;
        public int DataId => dataId;
        public int Denominator => denominator;
        
        public int kind;
        public int dataId;
        public int denominator;
    }
}