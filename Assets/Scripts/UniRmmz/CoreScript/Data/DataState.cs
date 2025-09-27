using System;

namespace UniRmmz
{
    [Serializable]
    public class DataState : ITraitsObject, IMetadataContainer
    {
        public int Id => id;
        public int AutoRemovalTiming => autoRemovalTiming;
        public int ChanceByDamage => chanceByDamage;
        public string Description => description;
        public int IconIndex => iconIndex;
        public int MaxTurns => maxTurns;
        public string Message1 => message1;
        public string Message2 => message2;
        public string Message3 => message3;
        public string Message4 => message4;
        public int MinTurns => minTurns;
        public int Motion => motion;
        public string Name => name;
        public string Note => note;
        public RmmzMetadata Meta { get; set; }
        public int Overlay => overlay;
        public int Priority => priority;
        public bool ReleaseByDamage => releaseByDamage;
        public bool RemoveAtBattleEnd => removeAtBattleEnd;
        public bool RemoveByDamage => removeByDamage;
        public bool RemoveByRestriction => removeByRestriction;
        public bool RemoveByWalking => removeByWalking;
        public int Restriction => restriction;
        public int StepsToRemove => stepsToRemove;
        public DataTrait[] Traits => traits;
        public int MessageType => messageType;
        
        private int id;
        private int autoRemovalTiming;
        private int chanceByDamage;
        private string description;
        private int iconIndex;
        private int maxTurns;
        private string message1;
        private string message2;
        private string message3;
        private string message4;
        private int minTurns;
        private int motion;
        private string name;
        private string note;
        private int overlay;
        private int priority;
        private bool releaseByDamage;
        private bool removeAtBattleEnd;
        private bool removeByDamage;
        private bool removeByRestriction;
        private bool removeByWalking;
        private int restriction;
        private int stepsToRemove;
        private DataTrait[] traits;
        private int messageType;
    }
}