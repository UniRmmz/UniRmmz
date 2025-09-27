using System;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for the system data.
    /// </summary>
    [Serializable]
    public partial class Game_System
    {
        protected bool _saveEnabled = true;
        protected bool _menuEnabled = true;
        protected bool _encounterEnabled = true;
        protected bool _formationEnabled = true;

        protected int _battleCount = 0;
        protected int _winCount = 0;
        protected int _escapeCount = 0;
        protected int _saveCount = 0;

        protected int _versionId = 0;
        protected int _savefileId = 0;
        protected int _framesOnSave = 0;

        protected AudioManager.Sound _bgmOnSave = null;
        protected AudioManager.Sound _bgsOnSave = null;
        protected int[] _windowTone = null;
        protected DataSystem.DataSound _battleBgm = null;
        protected DataSystem.DataSound _victoryMe = null;
        protected DataSystem.DataSound _defeatMe = null;
        protected AudioManager.Sound _savedBgm = null;
        protected AudioManager.Sound _walkingBgm = null;
        
        public virtual bool IsJapanese() => Rmmz.dataSystem.Locale.StartsWith("ja");
        public virtual bool IsChinese() => Rmmz.dataSystem.Locale.StartsWith("zh");
        public virtual bool IsKorean() => Rmmz.dataSystem.Locale.StartsWith("ko");
        public virtual bool IsCJK() => IsJapanese() || IsChinese() || IsKorean();
        public virtual bool IsRussian() => Rmmz.dataSystem.Locale.StartsWith("ru");
        public virtual bool IsSideView() => Rmmz.dataSystem.OptSideView;
        public virtual bool IsAutosaveEnabled() => Rmmz.dataSystem.OptAutosave;

        public virtual bool IsSaveEnabled() => _saveEnabled;
        public virtual void DisableSave() => _saveEnabled = false;
        public virtual void EnableSave() => _saveEnabled = true;

        public virtual bool IsMenuEnabled() => _menuEnabled;
        public virtual void DisableMenu() => _menuEnabled = false;
        public virtual void EnableMenu() => _menuEnabled = true;

        public virtual bool IsEncounterEnabled() => _encounterEnabled;
        public virtual void DisableEncounter() => _encounterEnabled = false;
        public virtual void EnableEncounter() => _encounterEnabled = true;

        public virtual bool IsFormationEnabled() => _formationEnabled;
        public virtual void DisableFormation() => _formationEnabled = false;
        public virtual void EnableFormation() => _formationEnabled = true;

        public virtual int BattleCount => _battleCount;
        public virtual int WinCount => _winCount;
        public virtual int EscapeCount => _escapeCount;
        public virtual int SaveCount => _saveCount;
        public virtual int VersionId() => _versionId;
        public virtual int SavefileId() => _savefileId;
        public virtual void SetSavefileId(int id) => _savefileId = id;

        public virtual int[] WindowTone() => _windowTone ?? Rmmz.dataSystem.WindowTone;
        public virtual void SetWindowTone(int[] tone) => _windowTone = tone;

        public virtual DataSystem.DataSound BattleBgm() => _battleBgm ?? Rmmz.dataSystem.BattleBgm;
        public virtual void SetBattleBgm(DataSystem.DataSound bgm) => _battleBgm = bgm;

        public virtual DataSystem.DataSound VictoryMe() => _victoryMe ?? Rmmz.dataSystem.VictoryMe;
        public virtual void SetVictoryMe(DataSystem.DataSound me) => _victoryMe = me;

        public virtual DataSystem.DataSound DefeatMe() => _defeatMe ?? Rmmz.dataSystem.DefeatMe;
        public virtual void SetDefeatMe(DataSystem.DataSound me) => _defeatMe = me;

        public virtual void OnBattleStart() => _battleCount++;
        public virtual void OnBattleWin() => _winCount++;
        public virtual void OnBattleEscape() => _escapeCount++;
        
        protected Game_System() {}

        public virtual void OnBeforeSave()
        {
            _saveCount++;
            _versionId = Rmmz.dataSystem.VersionId;
            _framesOnSave = Graphics.FrameCount;
            _bgmOnSave = Rmmz.AudioManager.SaveBgm();
            _bgsOnSave = Rmmz.AudioManager.SaveBgs();
        }

        public virtual void OnAfterLoad()
        {
            Graphics.FrameCount = _framesOnSave;
            Rmmz.AudioManager.PlayBgm(_bgmOnSave);
            Rmmz.AudioManager.PlayBgs(_bgsOnSave);
        }

        public virtual int Playtime()
        {
            return Graphics.FrameCount / 60;    
        }

        public virtual string PlaytimeText()
        {
            int hour = Playtime() / 60 / 60;
            int min = (Playtime() / 60) % 60;
            int sec = Playtime() % 60;
            return $"{hour:D2}:{min:D2}:{sec:D2}";
        }

        public virtual void SaveBgm()
        {
            _savedBgm = Rmmz.AudioManager.SaveBgm();
        }

        public virtual void ReplayBgm()
        {
            if (_savedBgm != null)
            {
                Rmmz.AudioManager.ReplayBgm(_savedBgm);
            }
        }

        public virtual void SaveWalkingBgm()
        {
            _walkingBgm = Rmmz.AudioManager.SaveBgm();
        }
        
        public virtual void ReplayWalkingBgm()
        {
            if (_walkingBgm != null)
            {
                Rmmz.AudioManager.PlayBgm(_walkingBgm);
            }
        }

        public virtual void SaveWalkingBgm2()
        {
            _walkingBgm = Rmmz.dataMap.Bgm;
        }
        
        public virtual string MainFontFace()
        {
            return $"rmmz-mainfont, {Rmmz.dataSystem.Advanced.FallbackFonts}";
        }

        public virtual string NumberFontFace()
        {
            return $"rmmz-numberfont, {MainFontFace()}";
        }

        public virtual int MainFontSize()
        {
            return Rmmz.dataSystem.Advanced.FontSize;
        }

        public virtual int WindowPadding() => 12;

        public virtual int WindowOpacity()
        {
            return Rmmz.dataSystem.Advanced.WindowOpacity;
        }

    }
}