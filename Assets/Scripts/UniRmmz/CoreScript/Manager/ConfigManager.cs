using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The static class that manages the configuration data.
    /// </summary>
    public partial class ConfigManager
    {
        [Serializable]
        protected class ConfigData
        {
            public bool alwaysDash = false;
            public bool commandRemember = false;
            public bool touchUI = true;
            public int bgmVolume = 100;
            public int bgsVolume = 100;
            public int meVolume = 100;
            public int seVolume = 100;
        }
        
        protected bool _isLoaded;

        public virtual bool AlwaysDash { get; set; }
        public virtual bool CommandRemember { get; set; }
        public virtual bool TouchUI { get; set; }
        public virtual int BgmVolume
        {
            get => Rmmz.AudioManager.BgmVolume;
            set => Rmmz.AudioManager.BgmVolume = value;
        }
        
        public virtual int BgsVolume
        {
            get => Rmmz.AudioManager.BgsVolume;
            set => Rmmz.AudioManager.BgsVolume = value;
        }
        
        public virtual int MeVolume
        {
            get => Rmmz.AudioManager.MeVolume;
            set => Rmmz.AudioManager.MeVolume = value;
        }
        
        public virtual int SeVolume
        {
            get => Rmmz.AudioManager.SeVolume;
            set => Rmmz.AudioManager.SeVolume = value;
        }

        public virtual void Load()
        {
            Rmmz.StorageManager.LoadObject<ConfigData>("config",
                (config) =>
                {
                    try
                    {
                        ApplyData(config ?? new ConfigData());
                        _isLoaded = true;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                },
                () =>
                {
                    _isLoaded = true;
                });
        }

        public virtual void Save()
        {
            Rmmz.StorageManager.SaveObject("config", MakeData());        
        }

        public virtual bool IsLoaded()
        {
            return _isLoaded;
        }

        protected ConfigData MakeData()
        {
            var config = new ConfigData
            {
                alwaysDash = AlwaysDash,
                commandRemember = CommandRemember,
                touchUI = TouchUI,
                bgmVolume = BgmVolume,
                bgsVolume = BgsVolume,
                meVolume = MeVolume,
                seVolume = SeVolume
            };
            return config;
        }

        protected void ApplyData(ConfigData configData)
        {
            AlwaysDash = configData.alwaysDash;
            CommandRemember = configData.commandRemember;
            TouchUI = configData.touchUI;
            BgmVolume = Math.Clamp(configData.bgmVolume, 0, 100);
            BgsVolume = Math.Clamp(configData.bgsVolume, 0, 100);
            MeVolume = Math.Clamp(configData.meVolume, 0, 100);
            SeVolume = Math.Clamp(configData.seVolume, 0, 100);
        }
    }
}