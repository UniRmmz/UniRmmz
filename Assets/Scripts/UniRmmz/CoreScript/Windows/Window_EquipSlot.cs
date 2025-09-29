using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting an equipment slot on the equipment screen.
    /// </summary>
    public partial class Window_EquipSlot //: Window_StatusBase
    {
        protected Game_Actor _actor = null;
        protected Window_EquipStatus _statusWindow;
        protected Window_EquipItem _itemWindow;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _actor = null;
            Refresh();
        }

        public virtual void SetActor(Game_Actor actor)
        {
            if (_actor != actor)
            {
                _actor = actor;
                Refresh();
            }
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            if (_itemWindow != null)
            {
                _itemWindow.SetSlotId(Index());
            }
        }

        protected override int MaxItems()
        {
            return _actor != null ? _actor.EquipSlots().Count : 0;
        }

        public virtual DataCommonItem Item()
        {
            return ItemAt(Index());
        }

        protected virtual DataCommonItem ItemAt(int index)
        {
            return _actor?.Equips().ElementAtOrDefault(index) ?? null;
        }

        public override void DrawItem(int index)
        {
            if (_actor != null)
            {
                string slotName = ActorSlotName(_actor, index);
                var item = ItemAt(index);
                int slotNameWidth = SlotNameWidth();
                Rect rect = ItemLineRect(index);
                int itemWidth = (int)rect.width - slotNameWidth;
                ChangeTextColor(Rmmz.ColorManager.SystemColor());
                ChangePaintOpacity(IsEnabled(index));
                DrawText(slotName, (int)rect.x, (int)rect.y, slotNameWidth, Bitmap.TextAlign.Left);
                DrawItemName(item, (int)rect.x + slotNameWidth, (int)rect.y, itemWidth);
                ChangePaintOpacity(true);
            }
        }

        protected virtual int SlotNameWidth() => 138;

        protected virtual bool IsEnabled(int index)
        {
            return _actor != null && _actor.IsEquipChangeOk(index);
        }

        public override bool IsCurrentItemEnabled()
        {
            return IsEnabled(Index());
        }

        public virtual void SetStatusWindow(Window_EquipStatus statusWindow)
        {
            _statusWindow = statusWindow;
            CallUpdateHelp();
        }

        public virtual void SetItemWindow(Window_EquipItem itemWindow)
        {
            _itemWindow = itemWindow;
        }

        protected override void UpdateHelp()
        {
            base.UpdateHelp();
            SetHelpWindowItem(Item());
            if (_statusWindow != null)
            {
                _statusWindow.SetTempActor(null);
            }
        }
    }
}
