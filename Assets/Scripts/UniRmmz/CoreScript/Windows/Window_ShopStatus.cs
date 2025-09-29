using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{

    /// <summary>
    /// The window for displaying number of items in possession and the actor's
    /// equipment on the shop screen.
    /// </summary>
    public partial class Window_ShopStatus //: Window_StatusBase
    {
       protected DataCommonItem _item;
       protected int _pageIndex;

       public override void Initialize(Rect rect)
       {
           base.Initialize(rect);
           _item = null;
           _pageIndex = 0;
           Refresh();
       }

       public override void Refresh()
       {
           Contents.Clear();
           if (_item != null)
           {
               int x = ItemPadding();
               DrawPossession(x, 0);
               if (IsEquipItem())
               {
                   int y = Mathf.FloorToInt(LineHeight() * 1.5f);
                   DrawEquipInfo(x, y);
               }
           }
       }

       public virtual void SetItem(DataCommonItem item)
       {
           _item = item;
           Refresh();
       }

       protected virtual bool IsEquipItem()
       {
           return Rmmz.DataManager.IsWeapon(_item) || Rmmz.DataManager.IsArmor(_item);
       }

       protected virtual void DrawPossession(int x, int y)
       {
           int width = InnerWidth - ItemPadding() - x;
           int possessionWidth = TextWidth("0000");
           ChangeTextColor(Rmmz.ColorManager.SystemColor());
           DrawText(Rmmz.TextManager.Possession, x, y, width - possessionWidth);
           ResetTextColor();
           DrawText(Rmmz.gameParty.NumItems(_item).ToString(), x, y, width, Bitmap.TextAlign.Right);
       }

       protected virtual void DrawEquipInfo(int x, int y)
       {
           var members = StatusMembers();
           for (int i = 0; i < members.Count; i++)
           {
               int actorY = y + Mathf.FloorToInt(LineHeight() * i * 2.2f);
               DrawActorEquipInfo(x, actorY, members[i]);
           }
       }

       protected virtual List<Game_Actor> StatusMembers()
       {
           int start = _pageIndex * PageSize();
           int end = start + PageSize();
           return Rmmz.gameParty.Members().Cast<Game_Actor>().ToList().Slice(start, end);
       }

       protected virtual int PageSize()
       {
           return 4;
       }

       protected virtual int MaxPages()
       {
           return Mathf.FloorToInt((Rmmz.gameParty.Size() + PageSize() - 1f) / PageSize());
       }

       protected virtual void DrawActorEquipInfo(int x, int y, Game_Actor actor)
       {
           var item = _item as EquipableItem;
           var item1 = CurrentEquippedItem(actor, item.EtypeId);
           int width = InnerWidth - x - ItemPadding();
           bool enabled = actor.CanEquip(_item);
           
           ChangePaintOpacity(enabled);
           ResetTextColor();
           DrawText(actor.Name(), x, y, width);
           
           if (enabled)
           {
               DrawActorParamChange(x, y, actor, item1);
           }
           
           DrawItemName(item1, x, y + LineHeight(), width);
           ChangePaintOpacity(true);
       }

       protected virtual void DrawActorParamChange(int x, int y, Game_Actor actor, EquipableItem item1)
       {
           int width = InnerWidth - ItemPadding() - x;
           int paramId = ParamId();
           var currentItem = _item as EquipableItem;
           var oldItem = item1 as EquipableItem;
           
           int change = currentItem.Params[paramId] - (oldItem?.Params[paramId] ?? 0);
           ChangeTextColor(Rmmz.ColorManager.ParamChangeTextColor(change));
           string changeText = (change > 0 ? "+" : "") + change;
           DrawText(changeText, x, y, width, Bitmap.TextAlign.Right);
       }

       protected virtual int ParamId()
       {
           return Rmmz.DataManager.IsWeapon(_item) ? 2 : 3;
       }

       protected virtual EquipableItem CurrentEquippedItem(Game_Actor actor, int etypeId)
       {
           var list = new List<EquipableItem>();
           var equips = actor.Equips().ToList();
           var slots = actor.EquipSlots();
           
           for (int i = 0; i < slots.Count; i++)
           {
               if (slots[i] == etypeId)
               {
                   list.Add(equips[i]);
               }
           }
           
           int paramId = ParamId();
           int worstParam = int.MaxValue;
           EquipableItem worstItem = null;
           
           foreach (var item in list)
           {
               if (item != null)
               {
                   if (item.Params[paramId] < worstParam)
                   {
                       worstParam = item.Params[paramId];
                       worstItem = item;
                   }
               }
           }
           
           return worstItem;
       }

       public override void UpdateRmmz()
       {
           base.UpdateRmmz();
           UpdatePage();
       }

       protected virtual void UpdatePage()
       {
           if (IsPageChangeEnabled() && IsPageChangeRequested())
           {
               ChangePage();
           }
       }

       protected virtual bool IsPageChangeEnabled()
       {
           return Visible && MaxPages() >= 2;
       }

       protected virtual bool IsPageChangeRequested()
       {
           if (Input.IsTriggered("shift"))
           {
               return true;
           }
           if (TouchInput.IsTriggered() && IsTouchedInsideFrame())
           {
               return true;
           }
           return false;
       }

       protected virtual void ChangePage()
       {
           _pageIndex = (_pageIndex + 1) % MaxPages();
           Refresh();
           PlayCursorSound();
       }
    }
}