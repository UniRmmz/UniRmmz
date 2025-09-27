using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying equipment items on the status screen.
    /// </summary>
    public partial class Window_StatusEquip : Window_StatusBase
    {
        protected Game_Actor _actor = null;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _actor = null;
        }

        public virtual void SetActor(Game_Actor actor)
        {
            if (_actor != actor)
            {
                _actor = actor;
                Refresh();
            }
        }

        protected override int MaxItems()
        {
            return _actor != null ? _actor.EquipSlots().Count : 0;
        }

        public override int ItemHeight() => LineHeight();

        public override void DrawItem(int index)
        {
            Rect rect = ItemLineRect(index);
            var equips = _actor.Equips();
            var item = equips.ElementAt(index);
            string slotName = ActorSlotName(_actor, index);
            int sw = 138;
            ChangeTextColor(Rmmz.ColorManager.SystemColor());
            DrawText(slotName, (int)rect.x, (int)rect.y, sw, Bitmap.TextAlign.Left);
            DrawItemName(item, (int)rect.x + sw, (int)rect.y, (int)rect.width - sw);
        }

        public override void DrawItemBackground(int _)
        {
            // Empty implementation
        }
    }
}