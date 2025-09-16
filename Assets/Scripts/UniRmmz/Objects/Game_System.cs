using System;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for the system data.
    /// </summary>
    [Serializable]
    public partial class Game_System
    {
        private bool _saveEnabled = true;
        private bool _menuEnabled = true;
        private bool _encounterEnabled = true;
        private bool _formationEnabled = true;

        private int _battleCount = 0;
        private int _winCount = 0;
        private int _escapeCount = 0;
        private int _saveCount = 0;

        private int _versionId = 0;
        private int _savefileId = 0;
        private int _framesOnSave = 0;

        private AudioManager.Sound _bgmOnSave = null;
        private AudioManager.Sound _bgsOnSave = null;
        private int[] _windowTone = null;
        private DataSystem.DataSound _battleBgm = null;
        private DataSystem.DataSound _victoryMe = null;
        private DataSystem.DataSound _defeatMe = null;
        private AudioManager.Sound _savedBgm = null;
        private AudioManager.Sound _walkingBgm = null;
        
        public bool IsJapanese() => Rmmz.DataSystem.Locale.StartsWith("ja");
        public bool IsChinese() => Rmmz.DataSystem.Locale.StartsWith("zh");
        public bool IsKorean() => Rmmz.DataSystem.Locale.StartsWith("ko");
        public bool IsCJK() => IsJapanese() || IsChinese() || IsKorean();
        public bool IsRussian() => Rmmz.DataSystem.Locale.StartsWith("ru");
        public bool IsSideView() => Rmmz.DataSystem.OptSideView;
        public bool IsAutosaveEnabled() => Rmmz.DataSystem.OptAutosave;

        public bool IsSaveEnabled() => _saveEnabled;
        public void DisableSave() => _saveEnabled = false;
        public void EnableSave() => _saveEnabled = true;

        public bool IsMenuEnabled() => _menuEnabled;
        public void DisableMenu() => _menuEnabled = false;
        public void EnableMenu() => _menuEnabled = true;

        public bool IsEncounterEnabled() => _encounterEnabled;
        public void DisableEncounter() => _encounterEnabled = false;
        public void EnableEncounter() => _encounterEnabled = true;

        public bool IsFormationEnabled() => _formationEnabled;
        public void DisableFormation() => _formationEnabled = false;
        public void EnableFormation() => _formationEnabled = true;

        public int BattleCount => _battleCount;
        public int WinCount => _winCount;
        public int EscapeCount => _escapeCount;
        public int SaveCount => _saveCount;
        public int VersionId() => _versionId;
        public int SavefileId() => _savefileId;
        public void SetSavefileId(int id) => _savefileId = id;

        public int[] WindowTone() => _windowTone ?? Rmmz.DataSystem.WindowTone;
        public void SetWindowTone(int[] tone) => _windowTone = tone;

        public DataSystem.DataSound BattleBgm() => _battleBgm ?? Rmmz.DataSystem.BattleBgm;
        public void SetBattleBgm(DataSystem.DataSound bgm) => _battleBgm = bgm;

        public DataSystem.DataSound VictoryMe() => _victoryMe ?? Rmmz.DataSystem.VictoryMe;
        public void SetVictoryMe(DataSystem.DataSound me) => _victoryMe = me;

        public DataSystem.DataSound DefeatMe() => _defeatMe ?? Rmmz.DataSystem.DefeatMe;
        public void SetDefeatMe(DataSystem.DataSound me) => _defeatMe = me;

        public void OnBattleStart() => _battleCount++;
        public void OnBattleWin() => _winCount++;
        public void OnBattleEscape() => _escapeCount++;
        
        protected Game_System() {}

        public void OnBeforeSave()
        {
            _saveCount++;
            _versionId = Rmmz.DataSystem.VersionId;
            _framesOnSave = Graphics.FrameCount;
            _bgmOnSave = Rmmz.AudioManager.SaveBgm();
            _bgsOnSave = Rmmz.AudioManager.SaveBgs();
        }

        public void OnAfterLoad()
        {
            Graphics.FrameCount = _framesOnSave;
            Rmmz.AudioManager.PlayBgm(_bgmOnSave);
            Rmmz.AudioManager.PlayBgs(_bgsOnSave);
        }

        public int Playtime()
        {
            return Graphics.FrameCount / 60;    
        }

        public string PlaytimeText()
        {
            int hour = Playtime() / 60 / 60;
            int min = (Playtime() / 60) % 60;
            int sec = Playtime() % 60;
            return $"{hour:D2}:{min:D2}:{sec:D2}";
        }

        public void SaveBgm()
        {
            _savedBgm = Rmmz.AudioManager.SaveBgm();
        }

        public void ReplayBgm()
        {
            if (_savedBgm != null)
            {
                Rmmz.AudioManager.ReplayBgm(_savedBgm);
            }
        }

        public void SaveWalkingBgm()
        {
            _walkingBgm = Rmmz.AudioManager.SaveBgm();
        }
        
        public void ReplayWalkingBgm()
        {
            if (_walkingBgm != null)
            {
                Rmmz.AudioManager.PlayBgm(_walkingBgm);
            }
        }

        public void SaveWalkingBgm2()
        {
            _walkingBgm = Rmmz.DataMap.Bgm;
        }
        
        public string MainFontFace()
        {
            return $"rmmz-mainfont, {Rmmz.DataSystem.advanced.FallbackFonts}";
        }

        public string NumberFontFace()
        {
            return $"rmmz-numberfont, {MainFontFace()}";
        }

        public int MainFontSize()
        {
            return Rmmz.DataSystem.advanced.FontSize;
        }

        public int WindowPadding() => 12;

        public int WindowOpacity()
        {
            return Rmmz.DataSystem.advanced.WindowOpacity;
        }

    }
}