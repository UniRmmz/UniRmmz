using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

namespace UniRmmz
{
    /// <summary>
    /// The class that manages the database and game objects.
    /// </summary>
    public partial class DataManager : MonoBehaviour
    {
       [Serializable]
        public class GlobalInfoElement
        {
            public string title;
            public List<string[]> characters;
            public List<string[]> faces;
            public string playtime;
            public long timestamp;    
        }
        
        [System.Serializable]
        private class SaveContents
        {
            public Game_System system;
            public Game_Screen screen;
            public Game_Timer timer;
            public Game_Switches switches;
            public Game_Variables variables;
            public Game_SelfSwitches selfSwitches;
            public Game_Actors actors;
            public Game_Party party;
            public Game_Map map;
            public Game_Player player;
        }

        private List<GlobalInfoElement> _globalInfo;
        private SynchronizationContext _mainThreadContext;
        private Dictionary<Type, (Func<object> getter, Action<object> setter, string src)> _databaseFiles = null;
        private (Type, (Func<object> getter, Action<object> setter, string src)) GetEntry<T>(Func<T> getter, Action<T> setter, string filename)
        {
            return new (typeof(T), (() => getter(), o => setter((T)o), filename));
        }
        
        DataManager()
        {
            _databaseFiles = new[]
            {
                GetEntry( () => Rmmz.dataActors, o => Rmmz.dataActors = o, "Actors.json"),
                GetEntry( () => Rmmz.dataClasses, o => Rmmz.dataClasses = o, "Classes.json"),
                GetEntry( () => Rmmz.dataSkills, o => Rmmz.dataSkills = o, "Skills.json"),
                GetEntry( () => Rmmz.dataItems, o => Rmmz.dataItems = o, "Items.json"),
                GetEntry( () => Rmmz.dataWeapons, o => Rmmz.dataWeapons = o, "Weapons.json"),
                GetEntry( () => Rmmz.dataArmors, o => Rmmz.dataArmors = o, "Armors.json"),
                GetEntry( () => Rmmz.dataEnemies, o => Rmmz.dataEnemies = o, "Enemies.json"),
                GetEntry( () => Rmmz.dataTroops, o => Rmmz.dataTroops = o, "Troops.json"),
                GetEntry( () => Rmmz.dataStates, o => Rmmz.dataStates = o, "States.json"),
                GetEntry( () => Rmmz.dataAnimations, o => Rmmz.dataAnimations = o, "Animations.json"),
                GetEntry( () => Rmmz.dataTilesets, o => Rmmz.dataTilesets = o, "Tilesets.json"),
                GetEntry( () => Rmmz.dataCommonEvents, o => Rmmz.dataCommonEvents = o, "CommonEvents.json"),
                GetEntry( () => Rmmz.DataSystem, o => Rmmz.DataSystem = o, "System.json"),
                GetEntry( () => Rmmz.dataMapInfos, o => Rmmz.dataMapInfos = o, "MapInfos.json"),
            }.ToDictionary(pair => pair.Item1, pair => pair.Item2);
            _mainThreadContext = SynchronizationContext.Current;
        }

        public void LoadGlobalInfo()
        {
            Rmmz.StorageManager.LoadObject<List<GlobalInfoElement>>("global", 
                (globalInfo) => { _globalInfo = globalInfo; RemoveInvalidGlobalInfo(); },
                () => { _globalInfo = new ();;});
        }

        private void RemoveInvalidGlobalInfo()
        {
            for (int savefileId = 0; savefileId < _globalInfo.Count; ++savefileId) 
            {
                if (!SavefileExists(savefileId))
                {
                    _globalInfo[savefileId] = null;
                }
            }
        }

        public void SaveGlobalInfo(Action resolve = null, Action reject = null)
        {
            Rmmz.StorageManager.SaveObject("global", _globalInfo, resolve, reject);
        }

        public bool IsGlobalInfoLoaded()
        {
            return _globalInfo != null;
        }

        /// <summary>
        /// Loads all database files into memory.
        /// </summary>
        public void LoadDatabase()
        {
            bool test = IsBattleTest() || IsEventTest();
            string prefix = test ? "Test_" : "";
            
            LoadDataFile<DataActor[]>();
            LoadDataFile<DataClass[]>();
            LoadDataFile<DataSkill[]>();
            LoadDataFile<DataItem[]>();
            LoadDataFile<DataWeapon[]>();
            LoadDataFile<DataArmor[]>();
            LoadDataFile<DataEnemy[]>();
            LoadDataFile<DataTroop[]>();
            LoadDataFile<DataState[]>();
            LoadDataFile<DataAnimation[]>();
            LoadDataFile<DataTileset[]>();
            LoadDataFile<DataCommonEvent[]>();
            LoadDataFile<DataSystem>();
            LoadDataFile<DataMapInfo[]>();
            
            if (IsEventTest()) 
            {
                //LoadDataFile("$testEvent", prefix + "Event.json");
            }
        }

        /// <summary>
        /// Loads a specific data file. (wrapper)
        /// </summary>
        private void LoadDataFile<T>() where T : class
        {
            var data = _databaseFiles[typeof(T)];
            data.setter(null);
            LoadDataFile<T>((obj) => data.setter(OnLoad(obj)), data.src);
        }

        /// <summary>
        /// Loads a specific data file.
        /// </summary>
        /// <param name="onLoaded">result callback</param>
        /// <param name="fileName">The file name of the data file.</param>
        private void LoadDataFile<T>(Action<object> onLoaded, string fileName) where T : class
        {
            Debug.Log($"loading {fileName}");
            System.Collections.IEnumerator LoadCoroutine(Action<object> onLoaded, string fileName)
            {
                string filePath = Path.Combine(Rmmz.RootPath, "data", fileName);
                using (UnityWebRequest uwr = UnityWebRequest.Get(filePath))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.result == UnityWebRequest.Result.Success)
                    {
                        string json = RemoveBom(uwr.downloadHandler.text);
                        var data = JsonEx.Parse<T>(json);
                        onLoaded.Invoke(data);
                    }
                    else
                    {
                        throw new RmmzError($"File load error: {fileName}, {uwr.error}");
                    }
                }
            }

            RmmzRoot.RunCoroutine(LoadCoroutine(onLoaded, fileName));
        }

        /// <summary>
        /// Checks if all database files are loaded.
        /// </summary>
        /// <returns>Returns true if all database files are loaded, false otherwise.</returns>
        public bool IsDatabaseLoaded()
        {
            foreach (var pair in _databaseFiles)
            {
                if (pair.Value.getter() == null)
                {
                    return false;
                }
            }
            return true;
        }

        public void LoadMapData(int mapId)
        {
            if (mapId > 0) 
            {
                var filename = string.Format("Map{0:D3}.json", mapId);
                Rmmz.DataMap = null;
                LoadDataFile<DataMap>((o) => Rmmz.DataMap = (DataMap)OnLoad(o), filename);
            } 
            else 
            {
                MakeEmptyMap();
            }        
        }
        
        public void MakeEmptyMap()
        {
            Rmmz.DataMap = new DataMap();
            Rmmz.DataMap.data = new int[0];
            Rmmz.DataMap.events.Clear();
            Rmmz.DataMap.width = 100;
            Rmmz.DataMap.height = 100;
            Rmmz.DataMap.scrollType = 3;
        }

        public bool IsMapLoaded()
        {
            CheckError();
            return Rmmz.DataMap != null;        
        }

        private object OnLoad(object obj)
        {
            if (IsMapObject(obj))
            {
                var map = obj as DataMap;
                ExtractMetadata(map);
                ExtractArrayMetadata(map.events);
            }
            else
            {
                ExtractArrayMetadata(obj);
            }

            return obj;
        }

        private bool IsMapObject(object obj)
        {
            return obj is DataMap;
        }

        private void ExtractArrayMetadata(object obj)
        {
            if (obj is object[] array)
            {
                foreach (var data in array)
                {
                    if (data is IMetadataContainer metadataContainer)
                    {
                        ExtractMetadata(data);
                    }
                }
            }
        }
        
        private readonly Regex RegExp = new Regex(@"<([^<>:]+)(:?)([^>]*)>", RegexOptions.Compiled);
        
        private void ExtractMetadata(object obj)
        {
            if (obj is IMetadataContainer container)
            {
                container.Meta = new RmmzMetadata();
                var matches = RegExp.Matches(container.Note);
        
                foreach (Match match in matches)
                {
                    if (match.Success)
                    {
                        string key = match.Groups[1].Value;
                        string colon = match.Groups[2].Value;
                        string value = match.Groups[3].Value;
                
                        if (colon == ":")
                        {
                            container.Meta.Add(key, value);
                        }
                        else
                        {
                            container.Meta.Add(key);
                        }
                    }
                }
            }
        }

        private void CheckError()
        {
            /*
            if (_errors.Length > 0)
            {
                const error = this._errors.shift();
                const retry = () => {
                    this.loadDataFile(error.name, error.src);
                };
                throw ["LoadError", error.url, retry];
            }
            */
        }

        public bool IsBattleTest()
        {
            return Utils.IsOptionValid("btest");
        }

        public bool IsEventTest()
        {
            return Utils.IsOptionValid("etest");
        }
        
        public bool IsTitleSkip()
        {
            return Utils.IsOptionValid("tskip");
        }

        public bool IsSkill(DataCommonItem item)
        {
            return item != null && Rmmz.dataSkills.Contains(item);
        }

        public bool IsItem(DataCommonItem item)
        {
            return item != null && Rmmz.dataItems.Contains(item);
        }

        public bool IsWeapon(DataCommonItem item)
        {
            return item != null && Rmmz.dataWeapons.Contains(item);
        }

        public bool IsArmor(DataCommonItem item)
        {
            return item != null && Rmmz.dataArmors.Contains(item);
        }
        
        private void CreateGameObjects()
        {
            Rmmz.gameTemp = Game_Temp.Create();
            Rmmz.gameSystem = Game_System.Create();
            Rmmz.gameScreen = Game_Screen.Create();
            Rmmz.gameTimer = Game_Timer.Create();
            Rmmz.gameMessage = Game_Message.Create();
            Rmmz.gameSwitches = Game_Switches.Create();
            Rmmz.gameVariables = Game_Variables.Create();
            Rmmz.gameSelfSwitches = Game_SelfSwitches.Create();
            Rmmz.gameActors = Game_Actors.Create();
            Rmmz.gameParty = Game_Party.Create();
            Rmmz.gameTroop = Game_Troop.Create();
            Rmmz.gameMap = Game_Map.Create();
            Rmmz.gamePlayer = Game_Player.Create();
        }

        public void SetupNewGame()
        {
            CreateGameObjects();
            SelectSavefileForNewGame();
            Rmmz.gameParty.SetupStartingMembers();
            Rmmz.gamePlayer.SetupForNewGame();
            Graphics.FrameCount = 0;
        }

        public void SetupBattleTest()
        {
            throw new NotImplementedException();
        }
        
        public void SetupEventTest()
        {
            throw new NotImplementedException();
        }

/*
DataManager.setupBattleTest = function() {
    this.createGameObjects();
    $gameParty.setupBattleTest();
    BattleManager.setup($dataSystem.testTroopId, true, false);
    BattleManager.setBattleTest(true);
    BattleManager.playBattleBgm();
};

DataManager.setupEventTest = function() {
    this.createGameObjects();
    this.selectSavefileForNewGame();
    $gameParty.setupStartingMembers();
    $gamePlayer.reserveTransfer(-1, 8, 6);
    $gamePlayer.setTransparent(false);
};
*/
        public bool IsAnySavefileExists()
        {
            return _globalInfo.Any(x => x != null);
        }

        public int LatestSavefileId()
        {
            var valid = _globalInfo.Skip(1).Where(x => x != null).ToList();
            if (valid.Count == 0)
            {
                return 0;
            }

            var latest = valid.Max(x => x!.timestamp);
            var index = _globalInfo.FindIndex(x => x != null && x!.timestamp == latest);
            return index > 0 ? index : 0;
        }

        public int EarliestSavefileId()
        {
            var valid = _globalInfo.Skip(1).Where(x => x != null).ToList();
            if (valid.Count == 0)
            {
                return 0;
            }

            var earliest = valid.Min(x => x!.timestamp);
            var index = _globalInfo.FindIndex(x => x != null && x!.timestamp == earliest);
            return index > 0 ? index : 0;
        }

        public int EmptySavefileId()
        {
            if (_globalInfo.Count < MaxSavefiles())
            {
                return Mathf.Max(1, _globalInfo.Count);
            }
            else
            {
                int index = _globalInfo.Skip(1).ToList().FindIndex(x => x == null);
                return index >= 0 ? index + 1 : -1;
            }
        }

        public void LoadAllSavefileImages()
        {
            foreach (var info in _globalInfo.Where(x => x != null))
            {
                LoadSavefileImages(info!);
            }
        }

        private void LoadSavefileImages(GlobalInfoElement info)
        {
            if (info.characters != null)
            {
                foreach (var character in info.characters)
                {
                    Rmmz.ImageManager.LoadCharacter(character[0]);
                }
            }

            if (info.faces != null)
            {
                foreach (var face in info.faces)
                {
                    Rmmz.ImageManager.LoadFace(face[0]);
                }
            }
        }

        public int MaxSavefiles() => 20;

        public GlobalInfoElement SavefileInfo(int savefileId)
        {
            return savefileId >= 0 && savefileId < _globalInfo.Count ? _globalInfo[savefileId] : null;
        }
        
        public void SaveGame(int savefileId, Action resolve = null, Action reject = null)
        {
            var contents = MakeSaveContents();
            var saveName = MakeSavename(savefileId);
            Rmmz.StorageManager.SaveObject(saveName, contents, () =>
            {
                _globalInfo.SetWithExpansion(savefileId, MakeSavefileInfo());
                SaveGlobalInfo(resolve, reject);
            }, reject);
        }

        public void LoadGame(int savefileId, Action resolve = null, Action reject = null)
        {
            var saveName = MakeSavename(savefileId);
            Rmmz.StorageManager.LoadObject<SaveContents>(saveName, (contents) =>
            {
                CreateGameObjects();
                ExtractSaveContents(contents);
                CorrectDataErrors();
                resolve?.Invoke();
            }, reject);
        }
        
        public bool SavefileExists(int savefileId) 
        {
            var saveName = MakeSavename(savefileId); 
            return Rmmz.StorageManager.Exists(saveName);
        }

        public string MakeSavename(int savefileId)
        {
            return $"file{savefileId}";
        }

        public void SelectSavefileForNewGame()
        {
            int id = EmptySavefileId();
            if (id > 0)
            {
                Rmmz.gameSystem.SetSavefileId(id);
            }
            else
            {
                Rmmz.gameSystem.SetSavefileId(EarliestSavefileId());
            }
        }

        private GlobalInfoElement MakeSavefileInfo()
        {
            return new GlobalInfoElement
            {
                title = Rmmz.DataSystem.GameTitle,
                characters = Rmmz.gameParty.CharactersForSavefile().ToList(),
                faces = Rmmz.gameParty.FacesForSavefile().ToList(),
                playtime = Rmmz.gameSystem.PlaytimeText(),
                timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };
        }

        private SaveContents MakeSaveContents()
        {
            return new SaveContents
            {
                system = Rmmz.gameSystem,
                screen = Rmmz.gameScreen,
                timer = Rmmz.gameTimer,
                switches = Rmmz.gameSwitches,
                variables = Rmmz.gameVariables,
                selfSwitches = Rmmz.gameSelfSwitches,
                actors = Rmmz.gameActors,
                party = Rmmz.gameParty,
                map = Rmmz.gameMap,
                player = Rmmz.gamePlayer
            };
        }

        private void ExtractSaveContents(SaveContents contents)
        {
            Rmmz.gameSystem = contents.system;
            Rmmz.gameScreen = contents.screen;
            Rmmz.gameTimer = contents.timer;
            Rmmz.gameSwitches = contents.switches;
            Rmmz.gameVariables = contents.variables;
            Rmmz.gameSelfSwitches = contents.selfSwitches;
            Rmmz.gameActors = contents.actors;
            Rmmz.gameParty = contents.party;
            Rmmz.gameMap = contents.map;
            Rmmz.gamePlayer = contents.player;
        }

        public void CorrectDataErrors()
        {
            Rmmz.gameParty.RemoveInvalidMembers();
        }
        
        #region UniRmmz
        
        private string RemoveBom(string input)
        {
            // UTF-8 BOM (\uFEFF) を除去
            if (input.Length > 0 && input[0] == '\uFEFF')
            {
                return input.Substring(1);
            }

            return input;
        }
        
        #endregion
    }
}