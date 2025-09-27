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
        
        private int id;
        private List<DataEnemyAction> actions;
        private int battlerHue;
        private string battlerName;
        private List<DataDropItem> dropItems;
        private int exp;
        private DataTrait[] traits;
        private int gold;
        private string name;
        private string note;
        private int[] @params;
    }

    [Serializable]
    public class DataEnemyAction
    {
        public int SkillId => skillId;
        public int ConditionType => conditionType;
        public float ConditionParam1 => conditionParam1;
        public float ConditionParam2 => conditionParam2;
        public int Rating => rating;
        
        private int skillId;
        private int conditionType;
        private float conditionParam1;
        private float conditionParam2;
        private int rating;
    }

    [Serializable]
    public class DataDropItem
    {
        public int Kind => kind;
        public int DataId => dataId;
        public int Denominator => denominator;
        
        private int kind;
        private int dataId;
        private int denominator;
    }
}