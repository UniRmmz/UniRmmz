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
        
        public int id;
        public int autoRemovalTiming;
        public int chanceByDamage;
        public string description;
        public int iconIndex;
        public int maxTurns;
        public string message1;
        public string message2;
        public string message3;
        public string message4;
        public int minTurns;
        public int motion;
        public string name;
        public string note;
        public int overlay;
        public int priority;
        public bool releaseByDamage;
        public bool removeAtBattleEnd;
        public bool removeByDamage;
        public bool removeByRestriction;
        public bool removeByWalking;
        public int restriction;
        public int stepsToRemove;
        public DataTrait[] traits;
        public int messageType;
    }
}