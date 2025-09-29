using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting an item on the item screen.
    /// </summary>
    public partial class Window_ItemList //: Window_Selectable
    {
        protected string _category = "none";
        protected List<DataCommonItem> _data;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _category = "none";
            _data = new List<DataCommonItem>();
        }

        public virtual void SetCategory(string category)
        {
            if (_category != category)
            {
                _category = category;
                Refresh();
                ScrollTo(0, 0);
            }
        }

        protected override int MaxCols()
        {
            return 2;
        }

        protected override int ColSpacing()
        {
            return 16;
        }

        protected override int MaxItems()
        {
            return _data != null ? _data.Count : 1;
        }

        public virtual DataCommonItem Item()
        {
            return ItemAt(Index());
        }

        protected virtual DataCommonItem ItemAt(int index)
        {
            return _data != null && index >= 0 && index < _data.Count ? _data[index] : null;
        }

        public override bool IsCurrentItemEnabled()
        {
            return IsEnabled(Item());
        }

        protected virtual bool Includes(DataCommonItem item)
        {
            switch (_category)
            {
                case "item":
                    return Rmmz.DataManager.IsItem(item) && ((DataItem)item).ItypeId == 1;
                case "weapon":
                    return Rmmz.DataManager.IsWeapon(item);
                case "armor":
                    return Rmmz.DataManager.IsArmor(item);
                case "keyItem":
                    return Rmmz.DataManager.IsItem(item) && ((DataItem)item).ItypeId == 2;
                default:
                    return false;
            }
        }

        protected virtual bool NeedsNumber()
        {
            if (_category == "keyItem")
            {
                return Rmmz.dataSystem.OptKeyItemsNumber;
            }
            else
            {
                return true;
            }
        }

        protected virtual bool IsEnabled(DataCommonItem item)
        {
            return Rmmz.gameParty.CanUse(item);
        }

        protected virtual void MakeItemList()
        {
            _data = Rmmz.gameParty.AllItems().Where(item => Includes(item)).ToList();
            if (Includes(null))
            {
                _data.Add(null);
            }
        }

        public virtual void SelectLast()
        {
            var lastItem = Rmmz.gameParty.LastItem();
            int index = _data.IndexOf(lastItem);
            ForceSelect(index >= 0 ? index : 0);
        }

        public override void DrawItem(int index)
        {
            var item = ItemAt(index);
            if (item != null)
            {
                int numberWidth = NumberWidth();
                Rect rect = ItemLineRect(index);
                ChangePaintOpacity(IsEnabled(item));
                DrawItemName(item, (int)rect.x, (int)rect.y, (int)rect.width - numberWidth);
                DrawItemNumber(item, (int)rect.x, (int)rect.y, (int)rect.width);
                ChangePaintOpacity(true);
            }
        }

        protected virtual int NumberWidth()
        {
            return TextWidth("000");
        }

        protected virtual void DrawItemNumber(DataCommonItem item, int x, int y, int width)
        {
            if (NeedsNumber())
            {
                DrawText(":", x, y, width - TextWidth("00"), Bitmap.TextAlign.Right);
                DrawText(Rmmz.gameParty.NumItems(item).ToString(), x, y, width, Bitmap.TextAlign.Right);
            }
        }

        protected override void UpdateHelp()
        {
            SetHelpWindowItem(Item());
        }

        public override void Refresh()
        {
            MakeItemList();
            base.Refresh();
        }
    }
}