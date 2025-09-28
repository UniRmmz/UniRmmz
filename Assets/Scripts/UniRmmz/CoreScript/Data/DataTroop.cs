using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UniRmmz
{
    [Serializable]
    public partial class DataTroop
    {
        public int Id => id;
        public DataTroopMember[] Members => members;
        public string Name => name;
        public DataTroopPage[] Pages => pages;
        
        protected int id;
        protected DataTroopMember[] members;
        protected string name;
        protected DataTroopPage[] pages;
    }

    [Serializable]
    public partial class DataTroopMember
    {
        public int EnemyId => enemyId;
        public float X => x;
        public float Y => y;
        public bool Hidden => hidden;
        
        protected int enemyId;
        protected float x;
        protected float y;
        protected bool hidden;
    }

    [Serializable]
    public partial class DataTroopPage
    {
        public DataTroopConditions Conditions => conditions;
        public List<DataEventCommand> List => list;
        public int Span => span;
        
        protected DataTroopConditions conditions;
        protected List<DataEventCommand> list;
        protected int span;
    }

    [Serializable]
    public partial class DataTroopConditions
    {
        public int ActorHp => actorHp;
        public int ActorId => actorId;
        public bool ActorValid => actorValid;
        public int EnemyHp => enemyHp;
        public int EnemyIndex => enemyIndex;
        public bool EnemyValid => enemyValid;
        public int SwitchId => switchId;
        public bool SwitchValid => switchValid;
        public int TurnA => turnA;
        public int TurnB => turnB;
        public bool TurnEnding => turnEnding;
        public bool TurnValid => turnValid;
        
        protected int actorHp;
        protected int actorId;
        protected bool actorValid;
        protected int enemyHp;
        protected int enemyIndex;
        protected bool enemyValid;
        protected int switchId;
        protected bool switchValid;
        protected int turnA;
        protected int turnB;
        protected bool turnEnding;
        protected bool turnValid;
    }
}