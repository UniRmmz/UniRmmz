using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of Scene_Save and Scene_Load.
    /// </summary>
    public abstract partial class Scene_File : Scene_MenuBase
    {
        protected Window_SavefileList _listWindow;

        public override void Create()
        {
            base.Create();
            Rmmz.DataManager.LoadAllSavefileImages();
            CreateHelpWindow();
            CreateListWindow();
            _helpWindow.SetText(HelpWindowText());
        }

        protected override float HelpAreaHeight()
        {
            return 0;
        }

        public override void StartScene()
        {
            base.StartScene();
            _listWindow.Refresh();
        }

        protected virtual int SavefileId()
        {
            return _listWindow.SavefileId();
        }

        protected virtual bool IsSavefileEnabled(int savefileId)
        {
            return _listWindow.IsEnabled(savefileId);
        }

        protected override Rect HelpWindowRect()
        {
            float wx = 0;
            float wy = MainAreaTop();
            float ww = Graphics.BoxWidth;
            float wh = CalcWindowHeight(1, false);
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateListWindow()
        {
            Rect rect = ListWindowRect();
            _listWindow = Window_SavefileList.Create(rect, "savefileList");
            _listWindow.SetHandler("ok", OnSavefileOk);
            _listWindow.SetHandler("cancel", PopScene);
            _listWindow.SetMode(Mode(), NeedsAutosave());
            _listWindow.SelectSavefile(FirstSavefileId());
            _listWindow.Refresh();
            AddWindow(_listWindow);
        }

        protected virtual Rect ListWindowRect()
        {
            float wx = 0;
            float wy = MainAreaTop() + _helpWindow.Height;
            float ww = Graphics.BoxWidth;
            float wh = MainAreaHeight() - _helpWindow.Height;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual string Mode()
        {
            return null;
        }

        protected virtual bool NeedsAutosave()
        {
            return Rmmz.gameSystem.IsAutosaveEnabled();
        }

        protected virtual void ActivateListWindow()
        {
            _listWindow.Activate();
        }

        protected virtual string HelpWindowText()
        {
            return "";
        }

        protected virtual int FirstSavefileId()
        {
            return 0;
        }

        protected virtual void OnSavefileOk()
        {
            // Override in derived classes
        }
    }
}