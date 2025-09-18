using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public class DataMap
    {
        public bool AutoplayBgm => autoplayBgm;
        public bool AutoplayBgs => autoplayBgs;
        public string Battleback1Name => battleback1Name;
        public string Battleback2Name => battleback2Name;
        public DataSystem.DataSound Bgm => bgm;
        public DataSystem.DataSound Bgs => bgs;
        public bool DisableDashing => disableDashing;
        public string DisplayName => displayName;
        public List<Encounter> EncounterList => encounterList;
        public int EncounterStep => encounterStep;
        public int Height => height;
        public string Note => note;
        public bool ParallaxLoopX => parallaxLoopX;
        public bool ParallaxLoopY => parallaxLoopY;
        public string ParallaxName => parallaxName;
        public bool ParallaxShow => parallaxShow;
        public int ParallaxSx => parallaxSx;
        public int ParallaxSy => parallaxSy;
        public int ScrollType => scrollType;
        public bool SpecifyBattleback => specifyBattleback;
        public int TilesetId => tilesetId;
        public int Width => width;
        public int[] Data => data;
        public List<DataEvent> Events => events;
        
        public bool autoplayBgm;
        public bool autoplayBgs;
        public string battleback1Name;
        public string battleback2Name;
        public DataSystem.DataSound bgm;
        public DataSystem.DataSound bgs;
        public bool disableDashing;
        public string displayName;
        public List<Encounter> encounterList;
        public int encounterStep;
        public int height;
        public string note;
        public bool parallaxLoopX;
        public bool parallaxLoopY;
        public string parallaxName;
        public bool parallaxShow;
        public int parallaxSx;
        public int parallaxSy;
        public int scrollType;
        public bool specifyBattleback;
        public int tilesetId;
        public int width;
        public int[] data;
        public List<DataEvent> events;
    }

    [Serializable]
    public class Encounter
    {
        public int TroopId => troopId;
        public int Weight => weight;
        public List<int> RegionSet => regionSet;
        
        public int troopId;
        public int weight;
        public List<int> regionSet;
    }

    [Serializable]
    public class DataEvent : IMetadataContainer
    {
        public int Id => id;
        public string Name => name;
        public string Note => note;
        public List<EventPage> Pages => pages;
        public int X => x;
        public int Y => y;
        public RmmzMetadata Meta { get; set; }
        
        public int id;
        public string name;
        public string note;
        public List<EventPage> pages;
        public int x;
        public int y;
    }

    [Serializable]
    public class EventPage
    {
        public EventConditions Conditions => conditions;
        public bool DirectionFix => directionFix;
        public EventImage Image => image;
        public List<DataEventCommand> List => list;
        public int MoveFrequency => moveFrequency;
        public MoveRoute MoveRoute => moveRoute;
        public int MoveSpeed => moveSpeed;
        public int MoveType => moveType;
        public int PriorityType => priorityType;
        public bool StepAnime => stepAnime;
        public bool Through => through;
        public int Trigger => trigger;
        public bool WalkAnime => walkAnime;
        
        public EventConditions conditions;
        public bool directionFix;
        public EventImage image;
        public List<DataEventCommand> list;
        public int moveFrequency;
        public MoveRoute moveRoute;
        public int moveSpeed;
        public int moveType;
        public int priorityType;
        public bool stepAnime;
        public bool through;
        public int trigger;
        public bool walkAnime;
    }

    [Serializable]
    public class EventConditions
    {
        public int ActorId => actorId;
        public bool ActorValid => actorValid;
        public int ItemId => itemId;
        public bool ItemValid => itemValid;
        public string SelfSwitchCh => selfSwitchCh;
        public bool SelfSwitchValid => selfSwitchValid;
        public int Switch1Id => switch1Id;
        public bool Switch1Valid => switch1Valid;
        public int Switch2Id => switch2Id;
        public bool Switch2Valid => switch2Valid;
        public int VariableId => variableId;
        public bool VariableValid => variableValid;
        public int VariableValue => variableValue;
        
        public int actorId;
        public bool actorValid;
        public int itemId;
        public bool itemValid;
        public string selfSwitchCh;
        public bool selfSwitchValid;
        public int switch1Id;
        public bool switch1Valid;
        public int switch2Id;
        public bool switch2Valid;
        public int variableId;
        public bool variableValid;
        public int variableValue;
    }

    [Serializable]
    public class EventImage
    {
        public int TileId => tileId;
        public string CharacterName => characterName;
        public int Direction => direction;
        public int Pattern => pattern;
        public int CharacterIndex => characterIndex;
        
        public int tileId;
        public string characterName;
        public int direction;
        public int pattern;
        public int characterIndex;
    }

    [Serializable]
    public class MoveRoute
    {
        public List<MoveCommand> List => list;
        public bool Repeat => repeat;
        public bool Skippable => skippable;
        public bool Wait => wait;
        
        public List<MoveCommand> list;
        public bool repeat;
        public bool skippable;
        public bool wait;
    }

    [Serializable]
    public class MoveCommand
    {
        public int Code => code;
        public List<object> Parameters => parameters;
        
        public int code;
        public List<object> parameters;
    }
}