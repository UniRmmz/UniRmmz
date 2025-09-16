using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting an item to sell on the shop screen.
    /// </summary>
    public partial class Window_ShopSell : Window_ItemList
    {
        protected override bool IsEnabled(DataCommonItem item)
        {
            return item != null && item.Price > 0;
        }
    }
}