using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    /// <summary>
    /// The scene class of the options screen.
    /// </summary>
    public partial class Scene_Options : Scene_MenuBase
    {
        private Window_Base _optionsWindow;

        public override void Create()
        {
            base.Create();
            CreateOptionsWindow();
        }

        public override void Terminate()
        {
            base.Terminate();
            Rmmz.ConfigManager.Save();
        }

        private void CreateOptionsWindow()
        {
            Rect rect = OptionsWindowRect();
            var window = Window_Options.Create(rect, "options");
            window.SetHandler("cancel", PopScene);
            _optionsWindow = window;
            AddWindow(_optionsWindow);
        }

        private Rect OptionsWindowRect()
        {
            int n = Mathf.Min(MaxCommands(), MaxVisibleCommands());
            int ww = 400;
            int wh = CalcWindowHeight(n, true);
            int wx = (Graphics.BoxWidth - ww) / 2;
            int wy = (Graphics.BoxHeight - wh) / 2;
            return new Rect(wx, wy, ww, wh);
        }

        private int MaxCommands()
        {
            // Increase this value when adding option items.
            return 7;
        }

        private int MaxVisibleCommands()
        {
            return 12;
        }
    }
}