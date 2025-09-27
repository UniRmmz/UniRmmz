using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The scene class of the equipment screen.
    /// </summary>
    public partial class Scene_Equip : Scene_MenuBase
    {
        protected Window_EquipStatus _statusWindow;
        protected Window_EquipCommand _commandWindow;
        protected Window_EquipSlot _slotWindow;
        protected Window_EquipItem _itemWindow;

        public override void Create()
        {
            base.Create();
            CreateHelpWindow();
            CreateStatusWindow();
            CreateCommandWindow();
            CreateSlotWindow();
            CreateItemWindow();
            RefreshActor();
        }

        protected virtual void CreateStatusWindow()
        {
            Rect rect = StatusWindowRect();
            _statusWindow = Window_EquipStatus.Create(rect, "equipStatus");
            AddWindow(_statusWindow);
        }

        protected virtual Rect StatusWindowRect()
        {
            float wx = 0;
            float wy = MainAreaTop();
            float ww = StatusWidth();
            float wh = MainAreaHeight();
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateCommandWindow()
        {
            Rect rect = CommandWindowRect();
            _commandWindow = Window_EquipCommand.Create(rect, "equipCommand");
            _commandWindow.SetHelpWindow(_helpWindow);
            _commandWindow.SetHandler("equip", CommandEquip);
            _commandWindow.SetHandler("optimize", CommandOptimize);
            _commandWindow.SetHandler("clear", CommandClear);
            _commandWindow.SetHandler("cancel", PopScene);
            _commandWindow.SetHandler("pagedown", NextActor);
            _commandWindow.SetHandler("pageup", PreviousActor);
            AddWindow(_commandWindow);
        }

        protected virtual Rect CommandWindowRect()
        {
            float wx = StatusWidth();
            float wy = MainAreaTop();
            float ww = Graphics.BoxWidth - StatusWidth();
            float wh = CalcWindowHeight(1, true);
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateSlotWindow()
        {
            Rect rect = SlotWindowRect();
            _slotWindow = Window_EquipSlot.Create(rect, "equipSlot");
            _slotWindow.SetHelpWindow(_helpWindow);
            _slotWindow.SetStatusWindow(_statusWindow);
            _slotWindow.SetHandler("ok", OnSlotOk);
            _slotWindow.SetHandler("cancel", OnSlotCancel);
            AddWindow(_slotWindow);
        }

        protected virtual Rect SlotWindowRect()
        {
            Rect commandWindowRect = CommandWindowRect();
            float wx = StatusWidth();
            float wy = commandWindowRect.y + commandWindowRect.height;
            float ww = Graphics.BoxWidth - StatusWidth();
            float wh = MainAreaHeight() - commandWindowRect.height;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateItemWindow()
        {
            Rect rect = ItemWindowRect();
            _itemWindow = Window_EquipItem.Create(rect, "equipItem");
            _itemWindow.SetHelpWindow(_helpWindow);
            _itemWindow.SetStatusWindow(_statusWindow);
            _itemWindow.SetHandler("ok", OnItemOk);
            _itemWindow.SetHandler("cancel", OnItemCancel);
            _itemWindow.Hide();
            _slotWindow.SetItemWindow(_itemWindow);
            AddWindow(_itemWindow);
        }

        protected virtual Rect ItemWindowRect()
        {
            return SlotWindowRect();
        }

        protected virtual float StatusWidth()
        {
            return 312f;
        }

        protected override bool NeedsPageButtons()
        {
            return true;
        }

        protected override bool ArePageButtonsEnabled()
        {
            return !(_itemWindow != null && _itemWindow.Active);
        }

        protected virtual void RefreshActor()
        {
            Game_Actor actor = Actor();
            _statusWindow.SetActor(actor);
            _slotWindow.SetActor(actor);
            _itemWindow.SetActor(actor);
        }

        protected virtual void CommandEquip()
        {
            _slotWindow.Activate();
            _slotWindow.Select(0);
        }

        protected virtual void CommandOptimize()
        {
            Rmmz.SoundManager.PlayEquip();
            Actor().OptimizeEquipments();
            _statusWindow.Refresh();
            _slotWindow.Refresh();
            _commandWindow.Activate();
        }

        protected virtual void CommandClear()
        {
            Rmmz.SoundManager.PlayEquip();
            Actor().ClearEquipments();
            _statusWindow.Refresh();
            _slotWindow.Refresh();
            _commandWindow.Activate();
        }

        protected virtual void OnSlotOk()
        {
            _slotWindow.Hide();
            _itemWindow.Show();
            _itemWindow.Activate();
            _itemWindow.Select(0);
        }

        protected virtual void OnSlotCancel()
        {
            _slotWindow.Deselect();
            _commandWindow.Activate();
        }

        protected virtual void OnItemOk()
        {
            Rmmz.SoundManager.PlayEquip();
            ExecuteEquipChange();
            HideItemWindow();
            _slotWindow.Refresh();
            _itemWindow.Refresh();
            _statusWindow.Refresh();
        }

        protected virtual void ExecuteEquipChange()
        {
            Game_Actor actor = Actor();
            int slotId = _slotWindow.Index();
            var item = _itemWindow.Item();
            actor.ChangeEquip(slotId, item as EquipableItem);
        }

        protected virtual void OnItemCancel()
        {
            HideItemWindow();
        }

        protected override void OnActorChange()
        {
            base.OnActorChange();
            RefreshActor();
            HideItemWindow();
            _slotWindow.Deselect();
            _slotWindow.Deactivate();
            _commandWindow.Activate();
        }

        protected virtual void HideItemWindow()
        {
            _slotWindow.Show();
            _slotWindow.Activate();
            _itemWindow.Hide();
            _itemWindow.Deselect();
        }
    }
}
