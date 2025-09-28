using System;
using UnityEngine.Serialization;

namespace UniRmmz
{
    [Serializable]
    public partial class DataSystem
    {
        [Serializable]
        public class DataAdvanced
        {
            public int GameId => gameId;
            public int ScreenWidth => screenWidth;
            public int ScreenHeight => screenHeight;
            public int UiAreaWidth => uiAreaWidth;
            public int UiAreaHeight => uiAreaHeight;
            public string NumberFontFilename => numberFontFilename;
            public string FallbackFonts => fallbackFonts;
            public int FontSize => fontSize;
            public string MainFontFilename => mainFontFilename;
            public int WindowOpacity => windowOpacity;
            public float ScreenScale => screenScale;
            
            protected int gameId;
            protected int screenWidth;
            protected int screenHeight;
            protected int uiAreaWidth;
            protected int uiAreaHeight;
            protected string numberFontFilename;
            protected string fallbackFonts;
            protected int fontSize;
            protected string mainFontFilename;
            protected int windowOpacity;
            protected float screenScale; 
        }

        [Serializable]
        public class DataVehicle
        {
            public DataSound Bgm => bgm;
            public int CharacterIndex => characterIndex;
            public string CharacterName => characterName;
            public int StartMapId => startMapId;
            public int StartX => startX;
            public int StartY => startY;
            
            protected DataSound bgm;
            protected int characterIndex;
            protected string characterName;
            protected int startMapId;
            protected int startX;
            protected int startY;
        }
        
        [Serializable]
        public class DataSound
        {
            public string Name { get => name; set => name = value; }
            public int Pan { get => pan; set => pan = value; }
            public int Pitch { get => pitch; set => pitch = value; }
            public int Volume { get => volume; set => volume = value; }
            
            protected string name;
            protected int pan;
            protected int pitch;
            protected int volume;
        }

        [Serializable]
        public class DataAttackMotion
        {
            public int Type => type;
            public int WeaponImageId => weaponImageId;
            
            protected int type;
            protected int weaponImageId;
        }
        
        [Serializable]
        public class DataTerms
        {
            public string[] Basic => basic;
            public string[] Commands => commands;
            public string[] Params => @params;
            public DataMessages Messages => messages;
            
            protected string[] basic;
            protected string[] commands;
            protected string[] @params;
            protected DataMessages messages;
        }
        
        [Serializable]
        public class DataMessages
        {
            public string AlwaysDash => alwaysDash;
            public string CommandRemember => commandRemember;
            public string TouchUI => touchUI;
            public string BgmVolume => bgmVolume;
            public string BgsVolume => bgsVolume;
            public string MeVolume => meVolume;
            public string SeVolume => seVolume;
            public string Possession => possession;
            public string ExpTotal => expTotal;
            public string ExpNext => expNext;
            public string SaveMessage => saveMessage;
            public string LoadMessage => loadMessage;
            public string File => file;
            public string Autosave => autosave;
            public string PartyName => partyName;
            public string Emerge => emerge;
            public string Preemptive => preemptive;
            public string Surprise => surprise;
            public string EscapeStart => escapeStart;
            public string EscapeFailure => escapeFailure;
            public string Victory => victory;
            public string Defeat => defeat;
            public string ObtainExp => obtainExp;
            public string ObtainGold => obtainGold;
            public string ObtainItem => obtainItem;
            public string LevelUp => levelUp;
            public string ObtainSkill => obtainSkill;
            public string UseItem => useItem;
            public string CriticalToEnemy => criticalToEnemy;
            public string CriticalToActor => criticalToActor;
            public string ActorDamage => actorDamage;
            public string ActorRecovery => actorRecovery;
            public string ActorGain => actorGain;
            public string ActorLoss => actorLoss;
            public string ActorDrain => actorDrain;
            public string ActorNoDamage => actorNoDamage;
            public string ActorNoHit => actorNoHit;
            public string EnemyDamage => enemyDamage;
            public string EnemyRecovery => enemyRecovery;
            public string EnemyGain => enemyGain;
            public string EnemyLoss => enemyLoss;
            public string EnemyDrain => enemyDrain;
            public string EnemyNoDamage => enemyNoDamage;
            public string EnemyNoHit => enemyNoHit;
            public string Evasion => evasion;
            public string MagicEvasion => magicEvasion;
            public string MagicReflection => magicReflection;
            public string CounterAttack => counterAttack;
            public string Substitute => substitute;
            public string BuffAdd => buffAdd;
            public string DebuffAdd => debuffAdd;
            public string BuffRemove => buffRemove;
            public string ActionFailure => actionFailure;
            
            protected string alwaysDash;
            protected string commandRemember;
            protected string touchUI;
            protected string bgmVolume;
            protected string bgsVolume;
            protected string meVolume;
            protected string seVolume;
            protected string possession;
            protected string expTotal;
            protected string expNext;
            protected string saveMessage;
            protected string loadMessage;
            protected string file;
            protected string autosave;
            protected string partyName;
            protected string emerge;
            protected string preemptive;
            protected string surprise;
            protected string escapeStart;
            protected string escapeFailure;
            protected string victory;
            protected string defeat;
            protected string obtainExp;
            protected string obtainGold;
            protected string obtainItem;
            protected string levelUp;
            protected string obtainSkill;
            protected string useItem;
            protected string criticalToEnemy;
            protected string criticalToActor;
            protected string actorDamage;
            protected string actorRecovery;
            protected string actorGain;
            protected string actorLoss;
            protected string actorDrain;
            protected string actorNoDamage;
            protected string actorNoHit;
            protected string enemyDamage;
            protected string enemyRecovery;
            protected string enemyGain;
            protected string enemyLoss;
            protected string enemyDrain;
            protected string enemyNoDamage;
            protected string enemyNoHit;
            protected string evasion;
            protected string magicEvasion;
            protected string magicReflection;
            protected string counterAttack;
            protected string substitute;
            protected string buffAdd;
            protected string debuffAdd;
            protected string buffRemove;
            protected string actionFailure;
        }
        
        [Serializable]
        public class DataTestBattler
        {
            public int ActorId => actorId;
            public int Level => level;
            public int[] Equips => equips;
            
            protected int actorId;
            protected int level;
            protected int[] equips;
        }
        
        [Serializable]
        public class DataTitleCommandWindow
        {
            public int Background => background;
            public int OffsetX => offsetX;
            public int OffsetY => offsetY;
            
            protected int background;
            protected int offsetX;
            protected int offsetY;
        }
        
        public DataAdvanced Advanced => advanced;
        public DataVehicle Airship => airship;
        public string[] ArmorTypes => armorTypes;
        public DataAttackMotion[] AttackMotions => attackMotions;
        public DataSound BattleBgm => battleBgm;
        public string Battleback1Name => battleback1Name;
        public string Battleback2Name => battleback2Name;
        public int BattlerHue => battlerHue;
        public string BattlerName => battlerName;
        public int BattleSystem => battleSystem;
        public DataVehicle Boat => boat;
        public string CurrencyUnit => currencyUnit;
        public DataSound DefeatMe => defeatMe;
        public int EditMapId => editMapId;
        public string[] Elements => elements;
        public string[] EquipTypes => equipTypes;
        public string GameTitle => gameTitle;
        public DataSound GameoverMe => gameoverMe;
        public bool[] ItemCategories => itemCategories;
        public string Locale => locale;
        public int[] MagicSkills => magicSkills;
        public bool[] MenuCommands => menuCommands;
        public bool OptAutosave => optAutosave;
        public bool OptDisplayTp => optDisplayTp;
        public bool OptDrawTitle => optDrawTitle;
        public bool OptExtraExp => optExtraExp;
        public bool OptFloorDeath => optFloorDeath;
        public bool OptFollowers => optFollowers;
        public bool OptKeyItemsNumber => optKeyItemsNumber;
        public bool OptSideView => optSideView;
        public bool OptSlipDeath => optSlipDeath;
        public bool OptTransparent => optTransparent;
        public int[] PartyMembers => partyMembers;
        public DataVehicle Ship => ship;
        public string[] SkillTypes => skillTypes;
        public DataSound[] Sounds => sounds;
        public int StartMapId => startMapId;
        public int StartX => startX;
        public int StartY => startY;
        public string[] Switches => switches;
        public DataTerms Terms => terms;
        public DataTestBattler[] TestBattlers => testBattlers;
        public int TestTroopId => testTroopId;
        public string Title1Name => title1Name;
        public string Title2Name => title2Name;
        public DataSound TitleBgm => titleBgm;
        public DataTitleCommandWindow TitleCommandWindow => titleCommandWindow;
        public string[] Variables => variables;
        public int VersionId => versionId;
        public DataSound VictoryMe => victoryMe;
        public string[] WeaponTypes => weaponTypes;
        public int[] WindowTone => windowTone;
        public int TileSize => tileSize;
        public bool HasEncryptedImages => hasEncryptedImages;
        public bool HasEncryptedAudio => hasEncryptedAudio;
        public string EncryptionKey => encryptionKey;
        
        protected DataAdvanced advanced;
        protected DataVehicle airship;
        protected string[] armorTypes;
        protected DataAttackMotion[] attackMotions;
        protected DataSound battleBgm;
        protected string battleback1Name;
        protected string battleback2Name;
        protected int battlerHue;
        protected string battlerName;
        protected int battleSystem;
        protected DataVehicle boat;
        protected string currencyUnit;
        protected DataSound defeatMe;
        protected int editMapId;
        protected string[] elements;
        protected string[] equipTypes;
        protected string gameTitle;
        protected DataSound gameoverMe;
        protected bool[] itemCategories;
        protected string locale;
        protected int[] magicSkills;
        protected bool[] menuCommands;
        protected bool optAutosave;
        protected bool optDisplayTp;
        protected bool optDrawTitle;
        protected bool optExtraExp;
        protected bool optFloorDeath;
        protected bool optFollowers;
        protected bool optKeyItemsNumber;
        protected bool optSideView;
        protected bool optSlipDeath;
        protected bool optTransparent;
        protected int[] partyMembers;
        protected DataVehicle ship;
        protected string[] skillTypes;
        protected DataSound[] sounds;
        protected int startMapId;
        protected int startX;
        protected int startY;
        protected string[] switches;
        protected DataTerms terms;
        protected DataTestBattler[] testBattlers;
        protected int testTroopId;
        protected string title1Name;
        protected string title2Name;
        protected DataSound titleBgm;
        protected DataTitleCommandWindow titleCommandWindow;
        protected string[] variables;
        protected int versionId;
        protected DataSound victoryMe;
        protected string[] weaponTypes;
        protected int[] windowTone;
        protected int tileSize;
        protected bool hasEncryptedImages;
        protected bool hasEncryptedAudio;
        protected string encryptionKey;
    }
}