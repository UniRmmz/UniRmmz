using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting an item to buy on the shop screen.
    /// </summary>
   public partial class Window_ShopBuy : Window_Selectable
   {
       protected List<ShopGoods> _shopGoods;
       protected List<DataCommonItem> _data = new List<DataCommonItem>();
       protected List<int> _price = new List<int>();
       protected int _money;
       protected Window_ShopStatus _statusWindow;

       public override void Initialize(Rect rect)
       {
           base.Initialize(rect);
           _money = 0;
       }

       public virtual void SetupGoods(List<ShopGoods> shopGoods)
       {
           _shopGoods = shopGoods;
           Refresh();
           Select(0);
       }

       protected override int MaxItems()
       {
           return _data?.Count ?? 1;
       }

       public virtual DataCommonItem Item()
       {
           return ItemAt(Index());
       }

       public virtual DataCommonItem ItemAt(int index)
       {
           return _data != null && index >= 0 && index < _data.Count ? _data[index] : null;
       }

       public virtual void SetMoney(int money)
       {
           _money = money;
           Refresh();
       }

       public override bool IsCurrentItemEnabled()
       {
           return IsEnabled(_data[Index()]);
       }

       public virtual int Price(DataCommonItem item)
       {
           return _price.ElementAtOrDefault(_data.IndexOf(item));
       }

       public virtual bool IsEnabled(DataCommonItem item)
       {
           return item != null && Price(item) <= _money && !Rmmz.gameParty.HasMaxItems(item);
       }

       public override void Refresh()
       {
           MakeItemList();
           base.Refresh();
       }

       protected virtual void MakeItemList()
       {
           _data.Clear();
           _price.Clear();
           
           if (_shopGoods != null)
           {
               foreach (var goods in _shopGoods)
               {
                   var item = GoodsToItem(goods);
                   if (item != null)
                   {
                       _data.Add(item);
                       int priceValue = goods.IsOverwritePrice == false ? item.Price : goods.Price;
                       _price.Add(priceValue);
                   }
               }
           }
       }

       protected virtual DataCommonItem GoodsToItem(ShopGoods shopGoods)
       {
           switch (shopGoods.ItemType)
           {
               case 0:
                   return Rmmz.dataItems[shopGoods.ItemId];
               case 1:
                   return Rmmz.dataWeapons[shopGoods.ItemId];
               case 2:
                   return Rmmz.dataArmors[shopGoods.ItemId];
               default:
                   return null;
           }
       }

       public override void DrawItem(int index)
       {
           var item = ItemAt(index);
           int price = Price(item);
           Rect rect = ItemLineRect(index);
           int priceWidth = PriceWidth();
           int priceX = (int)rect.x + (int)rect.width - priceWidth;
           int nameWidth = (int)rect.width - priceWidth;
           
           ChangePaintOpacity(IsEnabled(item));
           DrawItemName(item, (int)rect.x, (int)rect.y, nameWidth);
           DrawText(price.ToString(), priceX, (int)rect.y, priceWidth, Bitmap.TextAlign.Right);
           ChangePaintOpacity(true);
       }

       protected virtual int PriceWidth()
       {
           return 96;
       }

       public virtual void SetStatusWindow(Window_ShopStatus statusWindow)
       {
           _statusWindow = statusWindow;
           CallUpdateHelp();
       }

       protected override void UpdateHelp()
       {
           SetHelpWindowItem(Item());
           if (_statusWindow != null)
           {
               _statusWindow.SetItem(Item());
           }
       }
   }

}