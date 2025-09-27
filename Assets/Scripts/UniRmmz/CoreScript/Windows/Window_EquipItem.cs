using Newtonsoft.Json;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting an equipment item on the equipment screen.
    /// </summary>
    public partial class Window_EquipItem : Window_ItemList
    {
        protected Game_Actor _actor = null;
        protected int _slotId = 0;
        protected Window_EquipStatus _statusWindow;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _actor = null;
            _slotId = 0;
        }

        protected override int MaxCols() => 1;

        protected override int ColSpacing() => 8;

        public virtual void SetActor(Game_Actor actor)
        {
            if (_actor != actor)
            {
                _actor = actor;
                Refresh();
                ScrollTo(0, 0);
            }
        }

        public virtual void SetSlotId(int slotId)
        {
            if (_slotId != slotId)
            {
                _slotId = slotId;
                Refresh();
                ScrollTo(0, 0);
            }
        }

        protected override bool Includes(DataCommonItem item)
        {
            if (item == null)
            {
                return true;
            }
            return _actor != null &&
                   _actor.CanEquip(item) &&
                   ((EquipableItem)item).EtypeId == EtypeId();
        }

        protected virtual int EtypeId()
        {
            if (_actor != null && _slotId >= 0)
            {
                return _actor.EquipSlots()[_slotId];
            }
            else
            {
                return 0;
            }
        }

        protected override bool IsEnabled(DataCommonItem _)
        {
            return true;
        }

        public override void SelectLast()
        {
            // Empty implementation
        }

        public virtual void SetStatusWindow(Window_EquipStatus statusWindow)
        {
            _statusWindow = statusWindow;
            CallUpdateHelp();
        }
        
        protected override void UpdateHelp()
        {
            base.UpdateHelp();
            if (_actor != null && _statusWindow != null && _slotId >= 0)
            {
                var actor = JsonEx.MakeDeepCopy(_actor);
                actor.ForceChangeEquip(_slotId, Item() as EquipableItem);
                _statusWindow.SetTempActor(actor);
            }
        }

        protected override void PlayOkSound()
        {
            // Empty implementation
        }
    }

}