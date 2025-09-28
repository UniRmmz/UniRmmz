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
            
            private int gameId;
            private int screenWidth;
            private int screenHeight;
            private int uiAreaWidth;
            private int uiAreaHeight;
            private string numberFontFilename;
            private string fallbackFonts;
            private int fontSize;
            private string mainFontFilename;
            private int windowOpacity;
            private float screenScale; 
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
            
            private DataSound bgm;
            private int characterIndex;
            private string characterName;
            private int startMapId;
            private int startX;
            private int startY;
        }
        
        [Serializable]
        public class DataSound
        {
            public string Name { get => name; set => name = value; }
            public int Pan { get => pan; set => pan = value; }
            public int Pitch { get => pitch; set => pitch = value; }
            public int Volume { get => volume; set => volume = value; }
            
            private string name;
            private int pan;
            private int pitch;
            private int volume;
        }

        [Serializable]
        public class DataAttackMotion
        {
            public int Type => type;
            public int WeaponImageId => weaponImageId;
            
            private int type;
            private int weaponImageId;
        }
        
        [Serializable]
        public class DataTerms
        {
            public string[] Basic => basic;
            public string[] Commands => commands;
            public string[] Params => @params;
            public DataMessages Messages => messages;
            
            private string[] basic;
            private string[] commands;
            private string[] @params;
            private DataMessages messages;
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
            
            private string alwaysDash;
            private string commandRemember;
            private string touchUI;
            private string bgmVolume;
            private string bgsVolume;
            private string meVolume;
            private string seVolume;
            private string possession;
            private string expTotal;
            private string expNext;
            private string saveMessage;
            private string loadMessage;
            private string file;
            private string autosave;
            private string partyName;
            private string emerge;
            private string preemptive;
            private string surprise;
            private string escapeStart;
            private string escapeFailure;
            private string victory;
            private string defeat;
            private string obtainExp;
            private string obtainGold;
            private string obtainItem;
            private string levelUp;
            private string obtainSkill;
            private string useItem;
            private string criticalToEnemy;
            private string criticalToActor;
            private string actorDamage;
            private string actorRecovery;
            private string actorGain;
            private string actorLoss;
            private string actorDrain;
            private string actorNoDamage;
            private string actorNoHit;
            private string enemyDamage;
            private string enemyRecovery;
            private string enemyGain;
            private string enemyLoss;
            private string enemyDrain;
            private string enemyNoDamage;
            private string enemyNoHit;
            private string evasion;
            private string magicEvasion;
            private string magicReflection;
            private string counterAttack;
            private string substitute;
            private string buffAdd;
            private string debuffAdd;
            private string buffRemove;
            private string actionFailure;
        }
        
        [Serializable]
        public class DataTestBattler
        {
            public int ActorId => actorId;
            public int Level => level;
            public int[] Equips => equips;
            
            private int actorId;
            private int level;
            private int[] equips;
        }
        
        [Serializable]
        public class DataTitleCommandWindow
        {
            public int Background => background;
            public int OffsetX => offsetX;
            public int OffsetY => offsetY;
            
            private int background;
            private int offsetX;
            private int offsetY;
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
        
        private DataAdvanced advanced;
        private DataVehicle airship;
        private string[] armorTypes;
        private DataAttackMotion[] attackMotions;
        private DataSound battleBgm;
        private string battleback1Name;
        private string battleback2Name;
        private int battlerHue;
        private string battlerName;
        private int battleSystem;
        private DataVehicle boat;
        private string currencyUnit;
        private DataSound defeatMe;
        private int editMapId;
        private string[] elements;
        private string[] equipTypes;
        private string gameTitle;
        private DataSound gameoverMe;
        private bool[] itemCategories;
        private string locale;
        private int[] magicSkills;
        private bool[] menuCommands;
        private bool optAutosave;
        private bool optDisplayTp;
        private bool optDrawTitle;
        private bool optExtraExp;
        private bool optFloorDeath;
        private bool optFollowers;
        private bool optKeyItemsNumber;
        private bool optSideView;
        private bool optSlipDeath;
        private bool optTransparent;
        private int[] partyMembers;
        private DataVehicle ship;
        private string[] skillTypes;
        private DataSound[] sounds;
        private int startMapId;
        private int startX;
        private int startY;
        private string[] switches;
        private DataTerms terms;
        private DataTestBattler[] testBattlers;
        private int testTroopId;
        private string title1Name;
        private string title2Name;
        private DataSound titleBgm;
        private DataTitleCommandWindow titleCommandWindow;
        private string[] variables;
        private int versionId;
        private DataSound victoryMe;
        private string[] weaponTypes;
        private int[] windowTone;
        private int tileSize;
        private bool hasEncryptedImages;
        private bool hasEncryptedAudio;
        private string encryptionKey;
    }
}