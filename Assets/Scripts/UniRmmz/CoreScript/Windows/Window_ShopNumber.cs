using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for inputting quantity of items to buy or sell on the shop
    /// screen.
    /// </summary>
   public partial class Window_ShopNumber //: Window_Selectable
   {
       protected DataCommonItem _item;
       protected int _max = 1;
       protected int _price;
       protected int _number = 1;
       protected string _currencyUnit;
       protected List<Sprite_Button> _buttons;

       public override void Initialize(Rect rect)
       {
           base.Initialize(rect);
           _item = null;
           _max = 1;
           _price = 0;
           _number = 1;
           _currencyUnit = Rmmz.TextManager.CurrencyUnit;
           CreateButtons();
           Select(0);
           _canRepeat = false;
       }

       protected override bool IsScrollEnabled()
       {
           return false;
       }

       public virtual int Number()
       {
           return _number;
       }

       public virtual void Setup(DataCommonItem item, int max, int price)
       {
           _item = item;
           _max = Mathf.FloorToInt(max);
           _price = price;
           _number = 1;
           PlaceButtons();
           Refresh();
       }

       public virtual void SetCurrencyUnit(string currencyUnit)
       {
           _currencyUnit = currencyUnit;
           Refresh();
       }

       protected virtual void CreateButtons()
       {
           _buttons = new List<Sprite_Button>();
           if (Rmmz.ConfigManager.TouchUI)
           {
               Input.ButtonTypes[] types = { Input.ButtonTypes.Down2, Input.ButtonTypes.Down, Input.ButtonTypes.Up, Input.ButtonTypes.Up2, Input.ButtonTypes.Ok };
               foreach (var type in types)
               {
                   var button = Sprite_Button.Create($"button {type}");
                   button.Initialize(type);
                   _buttons.Add(button);
                   AddInnerChild(button);
               }
               
               _buttons[0].SetClickHandler(OnButtonDown2);
               _buttons[1].SetClickHandler(OnButtonDown);
               _buttons[2].SetClickHandler(OnButtonUp);
               _buttons[3].SetClickHandler(OnButtonUp2);
               _buttons[4].SetClickHandler(OnButtonOk);
           }
       }

       protected virtual void PlaceButtons()
       {
           float sp = ButtonSpacing();
           float totalWidth = TotalButtonWidth();
           float x = (InnerWidth - totalWidth) / 2f;
           
           foreach (var button in _buttons)
           {
               button.X = x;
               button.Y = ButtonY();
               x += button.Width + sp;
           }
       }

       protected virtual int TotalButtonWidth()
       {
           int sp = ButtonSpacing();
           return (int)_buttons.Aggregate(0f, (total, button) => total + button.Width + sp) - sp;
       }

       protected virtual int ButtonSpacing()
       {
           return 8;
       }

       public override void Refresh()
       {
           base.Refresh();
           DrawItemBackground(0);
           DrawCurrentItemName();
           DrawMultiplicationSign();
           DrawNumber();
           DrawHorzLine();
           DrawTotalPrice();
       }

       protected virtual void DrawCurrentItemName()
       {
           int padding = ItemPadding();
           int x = padding * 2;
           int y = ItemNameY();
           int width = MultiplicationSignX() - padding * 3;
           DrawItemName(_item, x, y, width);
       }

       protected virtual void DrawMultiplicationSign()
       {
           string sign = MultiplicationSign();
           int width = TextWidth(sign);
           int x = MultiplicationSignX();
           int y = ItemNameY();
           ResetTextColor();
           DrawText(sign, x, y, width);
       }

       protected virtual string MultiplicationSign()
       {
           return "×";
       }

       protected virtual int MultiplicationSignX()
       {
           string sign = MultiplicationSign();
           int width = TextWidth(sign);
           return CursorX() - width * 2;
       }

       protected virtual void DrawNumber()
       {
           int x = CursorX();
           int y = ItemNameY();
           int width = CursorWidth() - ItemPadding();
           ResetTextColor();
           DrawText(_number.ToString(), x, y, width, Bitmap.TextAlign.Right);
       }

       protected virtual void DrawHorzLine()
       {
           int padding = ItemPadding();
           int lineHeight = LineHeight();
           int itemY = ItemNameY();
           int totalY = TotalPriceY();
           int x = padding;
           int y = (itemY + totalY + lineHeight) / 2;
           int width = InnerWidth - padding * 2;
           DrawRect(x, y, width, 5);
       }

       protected virtual void DrawTotalPrice()
       {
           int padding = ItemPadding();
           int total = _price * _number;
           int width = InnerWidth - padding * 2;
           int y = TotalPriceY();
           DrawCurrencyValue(total, _currencyUnit, 0, y, width);
       }

       protected virtual int ItemNameY()
       {
           return Mathf.FloorToInt(InnerHeight / 2f - LineHeight() * 1.5f);
       }

       protected virtual int TotalPriceY()
       {
           return Mathf.FloorToInt(ItemNameY() + LineHeight() * 2f);
       }

       protected virtual int ButtonY()
       {
           return Mathf.FloorToInt(TotalPriceY() + LineHeight() * 2f);
       }

       protected virtual int CursorWidth()
       {
           int padding = ItemPadding();
           int digitWidth = TextWidth("0");
           return MaxDigits() * digitWidth + padding * 2;
       }

       protected virtual int CursorX()
       {
           int padding = ItemPadding();
           return InnerWidth - CursorWidth() - padding * 2;
       }

       protected virtual int MaxDigits()
       {
           return 2;
       }

       public override void UpdateRmmz()
       {
           base.UpdateRmmz();
           ProcessNumberChange();
       }

       protected override void PlayOkSound()
       {
       }

       protected virtual void ProcessNumberChange()
       {
           if (IsOpenAndActive())
           {
               if (Input.IsRepeated("right"))
               {
                   ChangeNumber(1);
               }
               if (Input.IsRepeated("left"))
               {
                   ChangeNumber(-1);
               }
               if (Input.IsRepeated("up"))
               {
                   ChangeNumber(10);
               }
               if (Input.IsRepeated("down"))
               {
                   ChangeNumber(-10);
               }
           }
       }

       protected virtual void ChangeNumber(int amount)
       {
           int lastNumber = _number;
           _number = Mathf.Clamp(_number + amount, 1, _max);
           if (_number != lastNumber)
           {
               PlayCursorSound();
               Refresh();
           }
       }

       protected override Rect ItemRect(int _)
       {
           return new Rect(CursorX(), ItemNameY(), CursorWidth(), LineHeight());
       }

       public override bool IsTouchOkEnabled()
       {
           return false;
       }

       protected virtual void OnButtonUp()
       {
           ChangeNumber(1);
       }

       protected virtual void OnButtonUp2()
       {
           ChangeNumber(10);
       }

       protected virtual void OnButtonDown()
       {
           ChangeNumber(-1);
       }

       protected virtual void OnButtonDown2()
       {
           ChangeNumber(-10);
       }

       protected virtual void OnButtonOk()
       {
           ProcessOk();
       }
   }

}