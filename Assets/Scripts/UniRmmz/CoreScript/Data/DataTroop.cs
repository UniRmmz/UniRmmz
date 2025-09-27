using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UniRmmz
{
    [Serializable]
    public class DataTroop
    {
        public int Id => id;
        public DataTroopMember[] Members => members;
        public string Name => name;
        public DataTroopPage[] Pages => pages;
        
        public int id;
        public DataTroopMember[] members;
        public string name;
        public DataTroopPage[] pages;
    }

    [Serializable]
    public class DataTroopMember
    {
        public int EnemyId => enemyId;
        public float X => x;
        public float Y => y;
        public bool Hidden => hidden;
        
        public int enemyId;
        public float x;
        public float y;
        public bool hidden;
    }

    [Serializable]
    public class DataTroopPage
    {
        public DataTroopConditions Conditions => conditions;
        public List<DataEventCommand> List => list;
        public int Span => span;
        
        public DataTroopConditions conditions;
        public List<DataEventCommand> list;
        public int span;
    }

    [Serializable]
    public class DataTroopConditions
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
        
        public int actorHp;
        public int actorId;
        public bool actorValid;
        public int enemyHp;
        public int enemyIndex;
        public bool enemyValid;
        public int switchId;
        public bool switchValid;
        public int turnA;
        public int turnB;
        public bool turnEnding;
        public bool turnValid;
    }
}