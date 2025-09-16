using System;
using System.Data;

namespace UniRmmz
{
    public partial class Scene_Boot : Scene_Base
    {
        public bool _databaseLoaded = false;

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

        private void OnDatabaseLoaded()
        {
            SetEncryptionInfo();
            LoadSystemImages();
            LoadPlayerData();
            LoadGameFonts();
        }

        private void SetEncryptionInfo()
        {
            bool hasImages = Rmmz.DataSystem.HasEncryptedImages;
            bool hasAudio = Rmmz.DataSystem.HasEncryptedAudio;
            string key = Rmmz.DataSystem.EncryptionKey;
            Utils.SetEncryptionInfo(hasImages, hasAudio, key);
        }

        private void LoadSystemImages()
        {
            Rmmz.ColorManager.LoadWindowskin();
            Rmmz.ImageManager.LoadSystem("Window");
            Rmmz.ImageManager.LoadSystem("IconSet");
        }

        private void LoadPlayerData()
        {
            Rmmz.DataManager.LoadGlobalInfo();
            Rmmz.ConfigManager.Load();
        }

        private void LoadGameFonts()
        {
            var advanced = Rmmz.DataSystem.advanced;
            Rmmz.FontManager.Load("rmmz-mainfont", advanced.MainFontFilename);
            Rmmz.FontManager.Load("rmmz-numberfont", advanced.NumberFontFilename);
        }

        private bool IsPlayerDataLoaded()
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

        private void StartNormalGame()
        {
            CheckPlayerLocation();
            Rmmz.DataManager.SetupNewGame();
            Scene_Title.Goto();
            Window_TitleCommand.InitCommandPosition();
        }

        private void ResizeScreen()
        {
            int screenWidth = Rmmz.DataSystem.advanced.ScreenWidth;
            int screenHeight = Rmmz.DataSystem.advanced.ScreenHeight;
            Graphics.Resize(screenWidth, screenHeight);
            Graphics.DefaultScale = ScreenScale();
            AdjustBoxSize();
            AdjustWindow();
        }

        private void AdjustBoxSize()
        {
            int uiAreaWidth = Rmmz.DataSystem.advanced.UiAreaWidth;
            int uiAreaHeight = Rmmz.DataSystem.advanced.UiAreaHeight;
            int boxMargin = 4;
            Graphics.BoxWidth = uiAreaWidth - boxMargin * 2;
            Graphics.BoxHeight = uiAreaHeight - boxMargin * 2;
        }

        private void AdjustWindow()
        {
            /* TODO
            float scale = ScreenScale();
            int xDelta = Graphics.Width * scale - window.innerWidth;
            int yDelta = Graphics.Height * scale - window.innerHeight;
            window.moveBy(-xDelta / 2, -yDelta / 2);
            window.resizeBy(xDelta, yDelta);
            */
        }

        private float ScreenScale()
        {
            if (Rmmz.DataSystem.advanced.ScreenScale > 0)
            {
                return Rmmz.DataSystem.advanced.ScreenScale;
            }
            else
            {
                return 1;
            }
        }

        private void UpdateDocumentTitle()
        {
            // UnityではPlayerSettingsで指定したものが反映される
        }

        private void CheckPlayerLocation()
        {
            if (Rmmz.DataSystem.StartMapId == 0) 
            {
                throw new Exception("Player's starting position is not set");
            }
        }
    }

}