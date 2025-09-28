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
        
        private int id;
        private DataTroopMember[] members;
        private string name;
        private DataTroopPage[] pages;
    }

    [Serializable]
    public partial class DataTroopMember
    {
        public int EnemyId => enemyId;
        public float X => x;
        public float Y => y;
        public bool Hidden => hidden;
        
        private int enemyId;
        private float x;
        private float y;
        private bool hidden;
    }

    [Serializable]
    public partial class DataTroopPage
    {
        public DataTroopConditions Conditions => conditions;
        public List<DataEventCommand> List => list;
        public int Span => span;
        
        private DataTroopConditions conditions;
        private List<DataEventCommand> list;
        private int span;
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
        
        private int actorHp;
        private int actorId;
        private bool actorValid;
        private int enemyHp;
        private int enemyIndex;
        private bool enemyValid;
        private int switchId;
        private bool switchValid;
        private int turnA;
        private int turnB;
        private bool turnEnding;
        private bool turnValid;
    }
}