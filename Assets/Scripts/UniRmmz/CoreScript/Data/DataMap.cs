using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public partial class DataMap : IMetadataContainer
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
        public int Height { get => height; set => height = value; }
        public string Note => note;
        public RmmzMetadata Meta { get; set; }
        public bool ParallaxLoopX => parallaxLoopX;
        public bool ParallaxLoopY => parallaxLoopY;
        public string ParallaxName => parallaxName;
        public bool ParallaxShow => parallaxShow;
        public int ParallaxSx => parallaxSx;
        public int ParallaxSy => parallaxSy;
        public int ScrollType { get => scrollType; set => scrollType = value; }
        public bool SpecifyBattleback => specifyBattleback;
        public int TilesetId => tilesetId;
        public int Width { get => width; set => width = value; }
        public int[] Data { get => data; set => data = value; }
        public List<DataEvent> Events { get => events; set => events = value; }
        
        protected bool autoplayBgm;
        protected bool autoplayBgs;
        protected string battleback1Name;
        protected string battleback2Name;
        protected DataSystem.DataSound bgm;
        protected DataSystem.DataSound bgs;
        protected bool disableDashing;
        protected string displayName;
        protected List<Encounter> encounterList;
        protected int encounterStep;
        protected int height;
        protected string note;
        protected bool parallaxLoopX;
        protected bool parallaxLoopY;
        protected string parallaxName;
        protected bool parallaxShow;
        protected int parallaxSx;
        protected int parallaxSy;
        protected int scrollType;
        protected bool specifyBattleback;
        protected int tilesetId;
        protected int width;
        protected int[] data;
        protected List<DataEvent> events;
    }

    [Serializable]
    public partial class Encounter
    {
        public int TroopId => troopId;
        public int Weight => weight;
        public List<int> RegionSet => regionSet;
        
        protected int troopId;
        protected int weight;
        protected List<int> regionSet;
    }

    [Serializable]
    public partial class DataEvent : IMetadataContainer
    {
        public int Id => id;
        public string Name => name;
        public string Note => note;
        public List<EventPage> Pages => pages;
        public int X => x;
        public int Y => y;
        public RmmzMetadata Meta { get; set; }
        
        protected int id;
        protected string name;
        protected string note;
        protected List<EventPage> pages;
        protected int x;
        protected int y;
    }

    [Serializable]
    public partial class EventPage
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
        
        protected EventConditions conditions;
        protected bool directionFix;
        protected EventImage image;
        protected List<DataEventCommand> list;
        protected int moveFrequency;
        protected MoveRoute moveRoute;
        protected int moveSpeed;
        protected int moveType;
        protected int priorityType;
        protected bool stepAnime;
        protected bool through;
        protected int trigger;
        protected bool walkAnime;
    }

    [Serializable]
    public partial class EventConditions
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
        
        protected int actorId;
        protected bool actorValid;
        protected int itemId;
        protected bool itemValid;
        protected string selfSwitchCh;
        protected bool selfSwitchValid;
        protected int switch1Id;
        protected bool switch1Valid;
        protected int switch2Id;
        protected bool switch2Valid;
        protected int variableId;
        protected bool variableValid;
        protected int variableValue;
    }

    [Serializable]
    public partial class EventImage
    {
        public int TileId => tileId;
        public string CharacterName => characterName;
        public int Direction => direction;
        public int Pattern => pattern;
        public int CharacterIndex => characterIndex;
        
        protected int tileId;
        protected string characterName;
        protected int direction;
        protected int pattern;
        protected int characterIndex;
    }

    [Serializable]
    public partial class MoveRoute
    {
        public List<MoveCommand> List { get => list; set => list = value; }
        public bool Repeat { get => repeat; set => repeat = value; }
        public bool Skippable { get => skippable; set => skippable = value; }
        public bool Wait { get => wait; set => wait = value; }
        
        protected List<MoveCommand> list;
        protected bool repeat;
        protected bool skippable;
        protected bool wait;
    }

    [Serializable]
    public partial class MoveCommand
    {
        public int Code { get => code; set => code = value; }
        public List<object> Parameters { get => parameters; set => parameters = value; }
        
        protected int code;
        protected List<object> parameters;
    }
}