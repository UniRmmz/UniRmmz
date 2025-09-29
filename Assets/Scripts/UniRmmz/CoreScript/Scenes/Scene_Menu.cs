using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The scene class of the menu screen.
    /// </summary>
    public partial class Scene_Menu //: Scene_MenuBase
    {
        protected Window_MenuCommand _commandWindow;
        protected Window_Gold _goldWindow;
        protected Window_MenuStatus _statusWindow;

        protected override float HelpAreaHeight()
        {
            return 0;
        }

        public override void Create()
        {
            base.Create();
            CreateCommandWindow();
            CreateGoldWindow();
            CreateStatusWindow();
        }

        public override void StartScene()
        {
            base.StartScene();
            _statusWindow.Refresh();
        }

        protected virtual void CreateCommandWindow()
        {
            Rect rect = CommandWindowRect();
            _commandWindow = Window_MenuCommand.Create(rect, "menuCommand");
            _commandWindow.SetHandler("item", CommandItem);
            _commandWindow.SetHandler("skill", CommandPersonal);
            _commandWindow.SetHandler("equip", CommandPersonal);
            _commandWindow.SetHandler("status", CommandPersonal);
            _commandWindow.SetHandler("formation", CommandFormation);
            _commandWindow.SetHandler("options", CommandOptions);
            _commandWindow.SetHandler("save", CommandSave);
            _commandWindow.SetHandler("gameEnd", CommandGameEnd);
            _commandWindow.SetHandler("cancel", PopScene);
            AddWindow(_commandWindow);
        }

        protected virtual Rect CommandWindowRect()
        {
            float ww = MainCommandWidth();
            float wh = MainAreaHeight() - GoldWindowRect().height;
            float wx = IsRightInputMode() ? Graphics.BoxWidth - ww : 0;
            float wy = MainAreaTop();
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateGoldWindow()
        {
            Rect rect = GoldWindowRect();
            _goldWindow = Window_Gold.Create(rect, "gold");
            AddWindow(_goldWindow);
        }

        protected virtual Rect GoldWindowRect()
        {
            float ww = MainCommandWidth();
            float wh = CalcWindowHeight(1, true);
            float wx = IsRightInputMode() ? Graphics.BoxWidth - ww : 0;
            float wy = MainAreaBottom() - wh;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateStatusWindow()
        {
            Rect rect = StatusWindowRect();
            _statusWindow = Window_MenuStatus.Create(rect, "menuStatus");
            AddWindow(_statusWindow);
        }

        protected virtual Rect StatusWindowRect()
        {
            float ww = Graphics.BoxWidth - MainCommandWidth();
            float wh = MainAreaHeight();
            float wx = IsRightInputMode() ? 0 : Graphics.BoxWidth - ww;
            float wy = MainAreaTop();
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CommandItem()
        {
            Scene_Item.Push();
        }

        protected virtual void CommandPersonal()
        {
            _statusWindow.SetFormationMode(false);
            _statusWindow.SelectLast();
            _statusWindow.Activate();
            _statusWindow.SetHandler("ok", OnPersonalOk);
            _statusWindow.SetHandler("cancel", OnPersonalCancel);
        }

        protected virtual void CommandFormation()
        {
            _statusWindow.SetFormationMode(true);
            _statusWindow.SelectLast();
            _statusWindow.Activate();
            _statusWindow.SetHandler("ok", OnFormationOk);
            _statusWindow.SetHandler("cancel", OnFormationCancel);
        }

        protected virtual void CommandOptions()
        {
            Scene_Optiions.Push();
        }

        protected virtual void CommandSave()
        { 
            Scene_Save.Push();
        }

        protected virtual void CommandGameEnd()
        {
            Scene_GameEnd.Push();
        }

        protected virtual void OnPersonalOk()
        {
            switch (_commandWindow.CurrentSymbol())
            {
                case "skill":
                    Scene_Skill.Push();
                    break;
                case "equip":
                    Scene_Equip.Push();
                    break;
                case "status":
                    Scene_Status.Push();
                    break;
            }
        }

        protected virtual void OnPersonalCancel()
        {
            _statusWindow.Deselect();
            _commandWindow.Activate();
        }

        protected virtual void OnFormationOk()
        {
            int index = _statusWindow.Index();
            int pendingIndex = _statusWindow.PendingIndex();
            if (pendingIndex >= 0)
            {
                Rmmz.gameParty.SwapOrder(index, pendingIndex);
                _statusWindow.SetPendingIndex(-1);
                _statusWindow.RedrawItem(index);
            }
            else
            {
                _statusWindow.SetPendingIndex(index);
            }
            _statusWindow.Activate();
        }

        protected virtual void OnFormationCancel()
        {
            if (_statusWindow.PendingIndex() >= 0)
            {
                _statusWindow.SetPendingIndex(-1);
                _statusWindow.Activate();
            }
            else
            {
                _statusWindow.Deselect();
                _commandWindow.Activate();
            }
        }
    }
}