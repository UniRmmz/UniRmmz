using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting buy/sell on the shop screen.
    /// </summary>
   public partial class Window_ShopCommand //: Window_HorzCommand
   {
       protected bool _purchaseOnly;

       public virtual void SetPurchaseOnly(bool purchaseOnly)
       {
           _purchaseOnly = purchaseOnly;
           Refresh();
       }

       protected override int MaxCols()
       {
           return 3;
       }

       protected override void MakeCommandList()
       {
           AddCommand(Rmmz.TextManager.Buy, "buy");
           AddCommand(Rmmz.TextManager.Sell, "sell", !_purchaseOnly);
           AddCommand(Rmmz.TextManager.Cancel, "cancel");
       }
   }
}