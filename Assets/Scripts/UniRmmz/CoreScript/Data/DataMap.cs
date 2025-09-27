using System;
using System.Collections.Generic;

namespace UniRmmz
{
    [Serializable]
    public class DataMap : IMetadataContainer
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
        
        private bool autoplayBgm;
        private bool autoplayBgs;
        private string battleback1Name;
        private string battleback2Name;
        private DataSystem.DataSound bgm;
        private DataSystem.DataSound bgs;
        private bool disableDashing;
        private string displayName;
        private List<Encounter> encounterList;
        private int encounterStep;
        private int height;
        private string note;
        private bool parallaxLoopX;
        private bool parallaxLoopY;
        private string parallaxName;
        private bool parallaxShow;
        private int parallaxSx;
        private int parallaxSy;
        private int scrollType;
        private bool specifyBattleback;
        private int tilesetId;
        private int width;
        private int[] data;
        private List<DataEvent> events;
    }

    [Serializable]
    public class Encounter
    {
        public int TroopId => troopId;
        public int Weight => weight;
        public List<int> RegionSet => regionSet;
        
        private int troopId;
        private int weight;
        private List<int> regionSet;
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
        
        private int id;
        private string name;
        private string note;
        private List<EventPage> pages;
        private int x;
        private int y;
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
        
        private EventConditions conditions;
        private bool directionFix;
        private EventImage image;
        private List<DataEventCommand> list;
        private int moveFrequency;
        private MoveRoute moveRoute;
        private int moveSpeed;
        private int moveType;
        private int priorityType;
        private bool stepAnime;
        private bool through;
        private int trigger;
        private bool walkAnime;
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
        
        private int actorId;
        private bool actorValid;
        private int itemId;
        private bool itemValid;
        private string selfSwitchCh;
        private bool selfSwitchValid;
        private int switch1Id;
        private bool switch1Valid;
        private int switch2Id;
        private bool switch2Valid;
        private int variableId;
        private bool variableValid;
        private int variableValue;
    }

    [Serializable]
    public class EventImage
    {
        public int TileId => tileId;
        public string CharacterName => characterName;
        public int Direction => direction;
        public int Pattern => pattern;
        public int CharacterIndex => characterIndex;
        
        private int tileId;
        private string characterName;
        private int direction;
        private int pattern;
        private int characterIndex;
    }

    [Serializable]
    public class MoveRoute
    {
        public List<MoveCommand> List { get => list; set => list = value; }
        public bool Repeat { get => repeat; set => repeat = value; }
        public bool Skippable { get => skippable; set => skippable = value; }
        public bool Wait { get => wait; set => wait = value; }
        
        private List<MoveCommand> list;
        private bool repeat;
        private bool skippable;
        private bool wait;
    }

    [Serializable]
    public class MoveCommand
    {
        public int Code { get => code; set => code = value; }
        public List<object> Parameters { get => parameters; set => parameters = value; }
        
        private int code;
        private List<object> parameters;
    }
}