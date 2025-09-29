    using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The scene class of the status screen.
    /// </summary>
    public partial class Scene_Status //: Scene_MenuBase
    {
        protected Window_Status _statusWindow;
        protected Window_StatusParams _statusParamsWindow;
        protected Window_StatusEquip _statusEquipWindow;
        protected Window_Help _profileWindow;

        public override void Create()
        {
            base.Create();
            CreateProfileWindow();
            CreateStatusWindow();
            CreateStatusParamsWindow();
            CreateStatusEquipWindow();
        }

        protected override float HelpAreaHeight()
        {
            return 0;
        }

        public override void StartScene()
        {
            base.StartScene();
            RefreshActor();
        }

        protected virtual void CreateProfileWindow()
        {
            Rect rect = ProfileWindowRect();
            _profileWindow = Window_Help.Create(rect, "profile");
            AddWindow(_profileWindow);
        }

        protected virtual Rect ProfileWindowRect()
        {
            float ww = Graphics.BoxWidth;
            float wh = ProfileHeight();
            float wx = 0;
            float wy = MainAreaBottom() - wh;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateStatusWindow()
        {
            Rect rect = StatusWindowRect();
            _statusWindow = Window_Status.Create(rect, "status");
            _statusWindow.SetHandler("cancel", PopScene);
            _statusWindow.SetHandler("pagedown", NextActor);
            _statusWindow.SetHandler("pageup", PreviousActor);
            AddWindow(_statusWindow);
        }

        protected virtual Rect StatusWindowRect()
        {
            float wx = 0;
            float wy = MainAreaTop();
            float ww = Graphics.BoxWidth;
            float wh = StatusParamsWindowRect().y - wy;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateStatusParamsWindow()
        {
            Rect rect = StatusParamsWindowRect();
            _statusParamsWindow = Window_StatusParams.Create(rect, "statusParams");
            AddWindow(_statusParamsWindow);
        }

        protected virtual Rect StatusParamsWindowRect()
        {
            float ww = StatusParamsWidth();
            float wh = StatusParamsHeight();
            float wx = 0;
            float wy = MainAreaBottom() - ProfileHeight() - wh;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateStatusEquipWindow()
        {
            Rect rect = StatusEquipWindowRect();
            _statusEquipWindow = Window_StatusEquip.Create(rect, "statusEquip");
            AddWindow(_statusEquipWindow);
        }

        protected virtual Rect StatusEquipWindowRect()
        {
            float ww = Graphics.BoxWidth - StatusParamsWidth();
            float wh = StatusParamsHeight();
            float wx = StatusParamsWidth();
            float wy = MainAreaBottom() - ProfileHeight() - wh;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual float StatusParamsWidth()
        {
            return 300f;
        }

        protected virtual float StatusParamsHeight()
        {
            return CalcWindowHeight(6, false);
        }

        protected virtual float ProfileHeight()
        {
            return CalcWindowHeight(2, false);
        }

        protected override bool NeedsPageButtons()
        {
            return true;
        }

        protected virtual void RefreshActor()
        {
            Game_Actor actor = Actor();
            _profileWindow.SetText(actor.Profile());
            _statusWindow.SetActor(actor);
            _statusParamsWindow.SetActor(actor);
            _statusEquipWindow.SetActor(actor);
        }

        protected override void OnActorChange()
        {
            base.OnActorChange();
            RefreshActor();
            _statusWindow.Activate();
        }
    }
}
