using System;
using UnityEngine.Serialization;

namespace UniRmmz
{
    [Serializable]
    public class DataSystem
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
            
            public int gameId;
            public int screenWidth;
            public int screenHeight;
            public int uiAreaWidth;
            public int uiAreaHeight;
            public string numberFontFilename;
            public string fallbackFonts;
            public int fontSize;
            public string mainFontFilename;
            public int windowOpacity;
            public float screenScale; 
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
            
            public DataSound bgm;
            public int characterIndex;
            public string characterName;
            public int startMapId;
            public int startX;
            public int startY;
        }
        
        [Serializable]
        public class DataSound
        {
            public string Name => name;
            public int Pan => pan;
            public int Pitch => pitch;
            public int Volume => volume;
            
            public string name;
            public int pan;
            public int pitch;
            public int volume;
        }

        [Serializable]
        public class DataAttackMotion
        {
            public int Type => type;
            public int WeaponImageId => weaponImageId;
            
            public int type;
            public int weaponImageId;
        }
        
        [Serializable]
        public class DataTerms
        {
            public string[] Basic => basic;
            public string[] Commands => commands;
            public string[] Params => @params;
            public DataMessages Messages => messages;
            
            public string[] basic;
            public string[] commands;
            public string[] @params;
            public DataMessages messages;
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
            
            public string alwaysDash;
            public string commandRemember;
            public string touchUI;
            public string bgmVolume;
            public string bgsVolume;
            public string meVolume;
            public string seVolume;
            public string possession;
            public string expTotal;
            public string expNext;
            public string saveMessage;
            public string loadMessage;
            public string file;
            public string autosave;
            public string partyName;
            public string emerge;
            public string preemptive;
            public string surprise;
            public string escapeStart;
            public string escapeFailure;
            public string victory;
            public string defeat;
            public string obtainExp;
            public string obtainGold;
            public string obtainItem;
            public string levelUp;
            public string obtainSkill;
            public string useItem;
            public string criticalToEnemy;
            public string criticalToActor;
            public string actorDamage;
            public string actorRecovery;
            public string actorGain;
            public string actorLoss;
            public string actorDrain;
            public string actorNoDamage;
            public string actorNoHit;
            public string enemyDamage;
            public string enemyRecovery;
            public string enemyGain;
            public string enemyLoss;
            public string enemyDrain;
            public string enemyNoDamage;
            public string enemyNoHit;
            public string evasion;
            public string magicEvasion;
            public string magicReflection;
            public string counterAttack;
            public string substitute;
            public string buffAdd;
            public string debuffAdd;
            public string buffRemove;
            public string actionFailure;
        }
        
        [Serializable]
        public class DataTestBattler
        {
            public int ActorId => actorId;
            public int Level => level;
            public int[] Equips => equips;
            
            public int actorId;
            public int level;
            public int[] equips;
        }
        
        [Serializable]
        public class DataTitleCommandWindow
        {
            public int Background => background;
            public int OffsetX => offsetX;
            public int OffsetY => offsetY;
            
            public int background;
            public int offsetX;
            public int offsetY;
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
        
        public DataAdvanced advanced;
        public DataVehicle airship;
        public string[] armorTypes;
        public DataAttackMotion[] attackMotions;
        public DataSound battleBgm;
        public string battleback1Name;
        public string battleback2Name;
        public int battlerHue;
        public string battlerName;
        public int battleSystem;
        public DataVehicle boat;
        public string currencyUnit;
        public DataSound defeatMe;
        public int editMapId;
        public string[] elements;
        public string[] equipTypes;
        public string gameTitle;
        public DataSound gameoverMe;
        public bool[] itemCategories;
        public string locale;
        public int[] magicSkills;
        public bool[] menuCommands;
        public bool optAutosave;
        public bool optDisplayTp;
        public bool optDrawTitle;
        public bool optExtraExp;
        public bool optFloorDeath;
        public bool optFollowers;
        public bool optKeyItemsNumber;
        public bool optSideView;
        public bool optSlipDeath;
        public bool optTransparent;
        public int[] partyMembers;
        public DataVehicle ship;
        public string[] skillTypes;
        public DataSound[] sounds;
        public int startMapId;
        public int startX;
        public int startY;
        public string[] switches;
        public DataTerms terms;
        public DataTestBattler[] testBattlers;
        public int testTroopId;
        public string title1Name;
        public string title2Name;
        public DataSound titleBgm;
        public DataTitleCommandWindow titleCommandWindow;
        public string[] variables;
        public int versionId;
        public DataSound victoryMe;
        public string[] weaponTypes;
        public int[] windowTone;
        public int tileSize;
        public bool hasEncryptedImages;
        public bool hasEncryptedAudio;
        public string encryptionKey;
    }
}