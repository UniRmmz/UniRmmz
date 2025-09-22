using UnityEngine;
using System.IO;

namespace UniRmmz
{
    public static partial class Rmmz
    {
        public static readonly string RootPath = Path.Combine(Application.streamingAssetsPath, "project");
        public static readonly int RmmzDefaultLayer = LayerMask.NameToLayer("RmmzDefault");
        public static readonly int RmmzOffscreenLayer = LayerMask.NameToLayer("RmmzOffscreen");
        public static readonly int RmmzDontRenderLayer = LayerMask.NameToLayer("RmmzDontRender");
        public static readonly int RmmzRmmzEffekseer1 = LayerMask.NameToLayer("RmmzEffekseer1");
        public static readonly int RmmzRmmzEffekseer2 = LayerMask.NameToLayer("RmmzEffekseer2");
        public static readonly int RmmzRmmzEffekseer3 = LayerMask.NameToLayer("RmmzEffekseer3");
        public static readonly int RmmzRmmzEffekseer4 = LayerMask.NameToLayer("RmmzEffekseer4");
        public static readonly int RmmzRmmzEffekseer5 = LayerMask.NameToLayer("RmmzEffekseer5");
        public static readonly int RmmzRmmzEffekseer6 = LayerMask.NameToLayer("RmmzEffekseer6");
        public static readonly int RmmzRmmzEffekseer7 = LayerMask.NameToLayer("RmmzEffekseer7");
        public static readonly int RmmzRmmzEffekseer8 = LayerMask.NameToLayer("RmmzEffekseer8");

        public static DataActor[] dataActors;
        public static DataClass[] dataClasses;
        public static DataSkill[] dataSkills;
        public static DataItem[] dataItems;
        public static DataWeapon[] dataWeapons;
        public static DataArmor[] dataArmors;
        public static DataEnemy[] dataEnemies;
        public static DataTroop[] dataTroops;
        public static DataState[] dataStates;
        public static DataAnimation[] dataAnimations;
        public static DataTileset[] dataTilesets;
        public static DataCommonEvent[] dataCommonEvents;
        public static DataSystem dataSystem;
        public static DataMapInfo[] dataMapInfos;
        public static DataMap dataMap;

        public static Game_Temp gameTemp;
        public static Game_System gameSystem;
        public static Game_Screen gameScreen;
        public static Game_Timer gameTimer;
        public static Game_Message gameMessage;
        public static Game_Switches gameSwitches;
        public static Game_Variables gameVariables;
        public static Game_SelfSwitches gameSelfSwitches;
        public static Game_Actors gameActors;
        public static Game_Party gameParty;
        public static Game_Troop gameTroop;
        public static Game_Map gameMap;
        public static Game_Player gamePlayer;

        public static EffectManager EffectManager;
        public static AudioManager AudioManager;
        public static BattleManager BattleManager;
        public static ColorManager ColorManager;
        public static ConfigManager ConfigManager;
        public static DataManager DataManager;
        public static FontManager FontManager;
        public static ImageManager ImageManager;
        public static SceneManager SceneManager;
        public static SoundManager SoundManager;
        public static StorageManager StorageManager;
        public static TextManager TextManager;
        public static PluginManager PluginManager;

        public static void InitializeManager()
        {
            RmmzRoot.Initialize();
            AudioManager.Create();
            BattleManager.Create();
            ColorManager.Create();
            ConfigManager.Create();
            DataManager.Create();
            FontManager.Create();
            ImageManager.Create();
            EffectManager.Create();
            SceneManager.Create();
            SoundManager.Create();
            StorageManager.Create();
            TextManager.Create();
            PluginManager.Create();
        }

        public static void Clear()
        {
            Rmmz.dataActors = null;
            Rmmz.dataClasses = null;
            Rmmz.dataSkills = null;
            Rmmz.dataItems = null;
            Rmmz.dataWeapons = null;
            Rmmz.dataArmors = null;
            Rmmz.dataEnemies = null;
            Rmmz.dataTroops = null;
            Rmmz.dataStates = null;
            Rmmz.dataAnimations = null;
            Rmmz.dataTilesets = null;
            Rmmz.dataCommonEvents = null;
            Rmmz.dataSystem = null;
            Rmmz.dataMapInfos = null;
            Rmmz.dataMap = null;

            Rmmz.gameTemp = null;
            Rmmz.gameSystem = null;
            Rmmz.gameScreen = null;
            Rmmz.gameTimer = null;
            Rmmz.gameMessage = null;
            Rmmz.gameSwitches = null;
            Rmmz.gameVariables = null;
            Rmmz.gameSelfSwitches = null;
            Rmmz.gameActors = null;
            Rmmz.gameParty = null;
            Rmmz.gameTroop = null;
            Rmmz.gameMap = null;
            Rmmz.gamePlayer = null;

            EffectManager = null;
        }
    }
}