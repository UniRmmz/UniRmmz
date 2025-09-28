using System;

namespace UniRmmz
{
    [Serializable]
    public partial class DataState : ITraitsObject, IMetadataContainer
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
        
        protected int id;
        protected int autoRemovalTiming;
        protected int chanceByDamage;
        protected string description;
        protected int iconIndex;
        protected int maxTurns;
        protected string message1;
        protected string message2;
        protected string message3;
        protected string message4;
        protected int minTurns;
        protected int motion;
        protected string name;
        protected string note;
        protected int overlay;
        protected int priority;
        protected bool releaseByDamage;
        protected bool removeAtBattleEnd;
        protected bool removeByDamage;
        protected bool removeByRestriction;
        protected bool removeByWalking;
        protected int restriction;
        protected int stepsToRemove;
        protected DataTrait[] traits;
        protected int messageType;
    }
}