using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The scene class of the game end screen.
    /// </summary>
    public partial class Scene_GameEnd //: Scene_MenuBase
    {
        protected Window_GameEnd _commandWindow;

        public override void Create()
        {
            base.Create();
            CreateCommandWindow();
        }

        public override void StopScene()
        {
            base.StopScene();
            _commandWindow.Close();
        }

        protected override void CreateBackground()
        {
            base.CreateBackground();
            SetBackgroundOpacity(128);
        }

        protected virtual void CreateCommandWindow()
        {
            Rect rect = CommandWindowRect();
            _commandWindow = Window_GameEnd.Create(rect, "gameEnd");
            _commandWindow.SetHandler("toTitle", CommandToTitle);
            _commandWindow.SetHandler("cancel", PopScene);
            AddWindow(_commandWindow);
        }

        protected virtual Rect CommandWindowRect()
        {
            float ww = MainCommandWidth();
            float wh = CalcWindowHeight(2, true);
            float wx = (Graphics.BoxWidth - ww) / 2;
            float wy = (Graphics.BoxHeight - wh) / 2;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CommandToTitle()
        {
            FadeOutAll();
            Scene_Title.Goto();
            Window_TitleCommand.InitCommandPosition();
        }
    }
}