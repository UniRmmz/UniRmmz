using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting a category of items on the item and shop screens.
    /// </summary>
    public partial class Window_ItemCategory : Window_HorzCommand
    {
        protected Window_ItemList _itemWindow;

        protected override int MaxCols() => 4;

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            if (_itemWindow != null)
            {
                _itemWindow.SetCategory(CurrentSymbol());
            }
        }

        protected override void MakeCommandList()
        {
            if (NeedsCommand("item"))
            {
                AddCommand(Rmmz.TextManager.Item, "item");
            }
            if (NeedsCommand("weapon"))
            {
                AddCommand(Rmmz.TextManager.Weapon, "weapon");
            }
            if (NeedsCommand("armor"))
            {
                AddCommand(Rmmz.TextManager.Armor, "armor");
            }
            if (NeedsCommand("keyItem"))
            {
                AddCommand(Rmmz.TextManager.KeyItem, "keyItem");
            }
        }

        protected virtual bool NeedsCommand(string name)
        {
            string[] table = { "item", "weapon", "armor", "keyItem" };
            int index = System.Array.IndexOf(table, name);
            if (index >= 0)
            {
                return Rmmz.dataSystem.ItemCategories[index];
            }
            return true;
        }

        public virtual void SetItemWindow(Window_ItemList itemWindow)
        {
            _itemWindow = itemWindow;
        }

        public virtual bool NeedsSelection() => MaxItems() >= 2;
    }

}