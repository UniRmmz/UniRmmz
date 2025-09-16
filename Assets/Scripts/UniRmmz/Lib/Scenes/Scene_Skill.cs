using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The scene class of the skill screen.
    /// </summary>
    public partial class Scene_Skill : Scene_ItemBase<Window_SkillList>
    {
        protected Window_SkillType _skillTypeWindow;
        protected Window_SkillStatus _statusWindow;

        public override void Create()
        {
            base.Create();
            CreateHelpWindow();
            CreateSkillTypeWindow();
            CreateStatusWindow();
            CreateItemWindow();
            CreateActorWindow();
        }

        public override void StartScene()
        {
            base.StartScene();
            RefreshActor();
        }

        protected virtual void CreateSkillTypeWindow()
        {
            Rect rect = SkillTypeWindowRect();
            _skillTypeWindow = Window_SkillType.Create(rect, "skillType");
            _skillTypeWindow.SetHelpWindow(_helpWindow);
            _skillTypeWindow.SetHandler("skill", CommandSkill);
            _skillTypeWindow.SetHandler("cancel", PopScene);
            _skillTypeWindow.SetHandler("pagedown", NextActor);
            _skillTypeWindow.SetHandler("pageup", PreviousActor);
            AddWindow(_skillTypeWindow);
        }

        protected virtual Rect SkillTypeWindowRect()
        {
            float ww = MainCommandWidth();
            float wh = CalcWindowHeight(3, true);
            float wx = IsRightInputMode() ? Graphics.BoxWidth - ww : 0;
            float wy = MainAreaTop();
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateStatusWindow()
        {
            Rect rect = StatusWindowRect();
            _statusWindow = Window_SkillStatus.Create(rect, "skillStatus");
            AddWindow(_statusWindow);
        }

        protected virtual Rect StatusWindowRect()
        {
            float ww = Graphics.BoxWidth - MainCommandWidth();
            float wh = _skillTypeWindow.Height;
            float wx = IsRightInputMode() ? 0 : Graphics.BoxWidth - ww;
            float wy = MainAreaTop();
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateItemWindow()
        {
            Rect rect = ItemWindowRect();
            _itemWindow = Window_SkillList.Create(rect, "skillList");
            _itemWindow.SetHelpWindow(_helpWindow);
            _itemWindow.SetHandler("ok", OnItemOk);
            _itemWindow.SetHandler("cancel", OnItemCancel);
            _skillTypeWindow.SetSkillWindow(_itemWindow);
            AddWindow(_itemWindow);
        }

        protected virtual Rect ItemWindowRect()
        {
            float wx = 0;
            float wy = _statusWindow.Y + _statusWindow.Height;
            float ww = Graphics.BoxWidth;
            float wh = MainAreaHeight() - _statusWindow.Height;
            return new Rect(wx, wy, ww, wh);
        }

        protected override bool NeedsPageButtons()
        {
            return true;
        }

        protected override bool ArePageButtonsEnabled()
        {
            return !IsActorWindowActive();
        }

        protected virtual void RefreshActor()
        {
            Game_Actor actor = Actor();
            _skillTypeWindow.SetActor(actor);
            _statusWindow.SetActor(actor);
            _itemWindow.SetActor(actor);
        }

        protected override Game_Battler User()
        {
            return Actor();
        }

        protected virtual void CommandSkill()
        {
            _itemWindow.Activate();
            _itemWindow.SelectLast();
        }

        protected virtual void OnItemOk()
        {
            Actor().SetLastMenuSkill(Item() as DataSkill);
            DetermineItem();
        }

        protected virtual void OnItemCancel()
        {
            _itemWindow.Deselect();
            _skillTypeWindow.Activate();
        }

        protected override void PlaySeForItem()
        {
            Rmmz.SoundManager.PlayUseSkill();
        }

        protected override void UseItem()
        {
            base.UseItem();
            _statusWindow.Refresh();
            _itemWindow.Refresh();
        }

        protected override void OnActorChange()
        {
            base.OnActorChange();
            RefreshActor();
            _itemWindow.Deselect();
            _skillTypeWindow.Activate();
        }

        protected override UsableItem Item()
        {
            return _itemWindow.Item();
        }
    }
}