using UnityEngine;
using Color = UnityEngine.Color;
using ColorUtility = UnityEngine.ColorUtility;

namespace UniRmmz
{
    public partial class Scene_Title : Scene_Base
    {
        private Sprite _gameTitleSprite;
        private Sprite _backSprite1;
        private Sprite _backSprite2;
        private Window_Base _commandWindow;
        
        public override void Create()
        {
            base.Create();
            CreateBackground();
            CreateForeground();
            CreateWindowLayer();
            CreateCommandWindow();
        }

        public override void StartScene()
        {
            base.StartScene();
            Rmmz.SceneManager.ClearStack();
            AdjustBackgrond();
            PlayTitleMusic();
            StartFadeIn(FadeSpeed(), false);
        }

        public override void UpdateRmmz()
        {
            if (!IsBusy())
            {
                _commandWindow.Open();
            }
            base.UpdateRmmz();
        }

        public override bool IsBusy()
        {
            return _commandWindow.IsClosing() || base.IsBusy();
        }

        public override void Terminate()
        {
            base.Terminate();
            Rmmz.SceneManager.SnapForBackground();
            if (_gameTitleSprite != null)
            {
                _gameTitleSprite.Bitmap.Dispose();
            }
        }

        private void CreateBackground()
        {
            _backSprite1 = Sprite.Create("backSprite1");
            _backSprite1.Bitmap = Rmmz.ImageManager.LoadTitle1(Rmmz.DataSystem.Title1Name);
            
            _backSprite2 = Sprite.Create("backSprite2");
            _backSprite2.Bitmap = Rmmz.ImageManager.LoadTitle2(Rmmz.DataSystem.Title2Name);

            this.AddChild(_backSprite1);
            this.AddChild(_backSprite2);
        }
        
        private void CreateForeground()
        {
            _gameTitleSprite = Sprite.Create("gameTitleSprite");
            _gameTitleSprite.Bitmap = new Bitmap(Graphics.Width, Graphics.Height);
            this.AddChild(_gameTitleSprite);
            if (Rmmz.DataSystem.OptDrawTitle)
            {
                DrawGameTitle();
            }
        }

        private void DrawGameTitle()
        {
            int x = 20;
            int y = Graphics.Height / 4;
            int maxWidth = Graphics.Width - x * 2;
            var text = Rmmz.DataSystem.GameTitle;
            var bitmap = _gameTitleSprite.Bitmap;
            
            bitmap.FontFace = Rmmz.gameSystem.MainFontFace();
            bitmap.OutlineColor = Color.black;
            bitmap.OutlineWidth = 8;
            bitmap.FontSize = 72;
            bitmap.DrawText(text, x, y, maxWidth, 48, Bitmap.TextAlign.Center);
        }
        
        private void AdjustBackgrond()
        {
            ScaleSprite(_backSprite1);
            ScaleSprite(_backSprite2);
            CenterSprite(_backSprite1);
            CenterSprite(_backSprite2);
        }

        private void CreateCommandWindow()
        {
            var background = Rmmz.DataSystem.TitleCommandWindow.Background;
            var rect = CommandWindowRect();
            var window = Window_TitleCommand.Create(rect, "titleCommand");
            window.SetBackgroundType(background);
            window.SetHandler("newGame", CommandNewGame);
            window.SetHandler("continue", CommandContinue);
            window.SetHandler("options", CommandOptions);
            _commandWindow = window;
            AddWindow(_commandWindow);
        }

        private Rect CommandWindowRect()
        {
            int offsetX = Rmmz.DataSystem.TitleCommandWindow.OffsetX;
            int offsetY = Rmmz.DataSystem.TitleCommandWindow.OffsetY;
            int ww = MainCommandWidth();
            int wh = CalcWindowHeight(3, true);
            int wx = (UniRmmz.Graphics.BoxWidth - ww) / 2 + offsetX;
            int wy = UniRmmz.Graphics.BoxHeight - wh - 96 + offsetY;
            return new Rect(wx, wy, ww, wh);
        }

        private void CommandNewGame()
        {
            Rmmz.DataManager.SetupNewGame();
            _commandWindow.Close();
            FadeOutAll();
            Scene_Map.Goto();
        }

        private void CommandContinue()
        {
            _commandWindow.Close();
            Scene_Load.Goto();
        }

        private void CommandOptions()
        {
            _commandWindow.Close();
            Scene_Optiions.Push();
        }

        private void PlayTitleMusic()
        {
            Rmmz.AudioManager.PlayBgm(Rmmz.DataSystem.TitleBgm);
            Rmmz.AudioManager.StopBgs();
            Rmmz.AudioManager.StopMe();
        }
    }
}