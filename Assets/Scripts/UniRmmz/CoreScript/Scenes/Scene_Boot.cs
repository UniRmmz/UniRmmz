using System;
using System.Data;

namespace UniRmmz
{
    public partial class Scene_Boot //: Scene_Base
    {
        protected bool _databaseLoaded = false;

        public override void Initialize()
        {
            base.Initialize();
            _databaseLoaded = false;
        }

        public override void Create()
        {
            base.Create();
            Rmmz.DataManager.LoadDatabase();
            Rmmz.StorageManager.UpdateForageKeys();
        }

        public override bool IsReady()
        {
            if (!_databaseLoaded)
            {
                if (Rmmz.DataManager.IsDatabaseLoaded() &&
                    Rmmz.StorageManager.ForageKeysUpdated())
                {
                    _databaseLoaded = true;
                    OnDatabaseLoaded();
                }

                return false;
            }

            return base.IsReady() && IsPlayerDataLoaded();
        }

        protected virtual void OnDatabaseLoaded()
        {
            SetEncryptionInfo();
            LoadSystemImages();
            LoadPlayerData();
            LoadGameFonts();
        }

        protected virtual  void SetEncryptionInfo()
        {
            bool hasImages = Rmmz.dataSystem.HasEncryptedImages;
            bool hasAudio = Rmmz.dataSystem.HasEncryptedAudio;
            string key = Rmmz.dataSystem.EncryptionKey;
            Utils.SetEncryptionInfo(hasImages, hasAudio, key);
        }

        protected virtual  void LoadSystemImages()
        {
            Rmmz.ColorManager.LoadWindowskin();
            Rmmz.ImageManager.LoadSystem("Window");
            Rmmz.ImageManager.LoadSystem("IconSet");
        }

        protected virtual  void LoadPlayerData()
        {
            Rmmz.DataManager.LoadGlobalInfo();
            Rmmz.ConfigManager.Load();
        }

        protected virtual  void LoadGameFonts()
        {
            var advanced = Rmmz.dataSystem.Advanced;
            Rmmz.FontManager.Load("rmmz-mainfont", advanced.MainFontFilename);
            Rmmz.FontManager.Load("rmmz-numberfont", advanced.NumberFontFilename);
        }

        protected virtual  bool IsPlayerDataLoaded()
        {
            return Rmmz.DataManager.IsGlobalInfoLoaded() && Rmmz.ConfigManager.IsLoaded();
        }

        public override void StartScene()
        {
            base.StartScene();
            Rmmz.SoundManager.PreloadImportantSounds();
            if (Rmmz.DataManager.IsBattleTest())
            {
                Rmmz.DataManager.SetupBattleTest();
                Scene_Battle.Goto();
            }
            else if (Rmmz.DataManager.IsEventTest())
            {
                Rmmz.DataManager.SetupEventTest();
                Scene_Map.Goto();
            }
            else if (Rmmz.DataManager.IsTitleSkip())
            {
                CheckPlayerLocation();
                Rmmz.DataManager.SetupNewGame();
                Scene_Map.Goto();
            }
            else
            {
                StartNormalGame();
            }

            ResizeScreen();
            UpdateDocumentTitle();
        }

        protected virtual  void StartNormalGame()
        {
            CheckPlayerLocation();
            Rmmz.DataManager.SetupNewGame();
            Scene_Title.Goto();
            Window_TitleCommand.InitCommandPosition();
        }

        protected virtual  void ResizeScreen()
        {
            int screenWidth = Rmmz.dataSystem.Advanced.ScreenWidth;
            int screenHeight = Rmmz.dataSystem.Advanced.ScreenHeight;
            Graphics.Resize(screenWidth, screenHeight);
            Graphics.DefaultScale = ScreenScale();
            AdjustBoxSize();
            AdjustWindow();
        }

        protected virtual  void AdjustBoxSize()
        {
            int uiAreaWidth = Rmmz.dataSystem.Advanced.UiAreaWidth;
            int uiAreaHeight = Rmmz.dataSystem.Advanced.UiAreaHeight;
            int boxMargin = 4;
            Graphics.BoxWidth = uiAreaWidth - boxMargin * 2;
            Graphics.BoxHeight = uiAreaHeight - boxMargin * 2;
        }

        protected virtual  void AdjustWindow()
        {
            /* TODO
            float scale = ScreenScale();
            int xDelta = Graphics.Width * scale - window.innerWidth;
            int yDelta = Graphics.Height * scale - window.innerHeight;
            window.moveBy(-xDelta / 2, -yDelta / 2);
            window.resizeBy(xDelta, yDelta);
            */
        }

        protected virtual  float ScreenScale()
        {
            if (Rmmz.dataSystem.Advanced.ScreenScale > 0)
            {
                return Rmmz.dataSystem.Advanced.ScreenScale;
            }
            else
            {
                return 1;
            }
        }

        protected virtual  void UpdateDocumentTitle()
        {
            // UnityではPlayerSettingsで指定したものが反映される
        }

        protected virtual  void CheckPlayerLocation()
        {
            if (Rmmz.dataSystem.StartMapId == 0) 
            {
                throw new Exception("Player's starting position is not set");
            }
        }
    }

}