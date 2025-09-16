using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The scene class of the name input screen.
    /// </summary>
    public partial class Scene_Shop : Scene_MenuBase
    {
        protected List<ShopGoods> _goods;
        protected bool _purchaseOnly;
        protected DataCommonItem _item;
        protected Window_Gold _goldWindow;
        protected Window_ShopCommand _commandWindow;
        protected Window_Base _dummyWindow;
        protected Window_ShopNumber _numberWindow;
        protected Window_ShopStatus _statusWindow;
        protected Window_ShopBuy _buyWindow;
        protected Window_ItemCategory _categoryWindow;
        protected Window_ShopSell _sellWindow;

        public override void Prepare(params object[] args)
        {
            _goods = args[0] as List<ShopGoods>;
            _purchaseOnly = Convert.ToBoolean(args[1]);
            _item = null;
        }

        public override void Create()
        {
            base.Create();
            CreateHelpWindow();
            CreateGoldWindow();
            CreateCommandWindow();
            CreateDummyWindow();
            CreateNumberWindow();
            CreateStatusWindow();
            CreateBuyWindow();
            CreateCategoryWindow();
            CreateSellWindow();
        }

        protected virtual void CreateGoldWindow()
        {
            Rect rect = GoldWindowRect();
            _goldWindow = Window_Gold.Create(rect);
            AddWindow(_goldWindow);
        }

        protected virtual Rect GoldWindowRect()
        {
            float ww = MainCommandWidth();
            float wh = CalcWindowHeight(1, true);
            float wx = Graphics.BoxWidth - ww;
            float wy = MainAreaTop();
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateCommandWindow()
        {
            Rect rect = CommandWindowRect();
            _commandWindow = Window_ShopCommand.Create(rect);
            _commandWindow.SetPurchaseOnly(_purchaseOnly);
            _commandWindow.Y = MainAreaTop();
            _commandWindow.SetHandler("buy", CommandBuy);
            _commandWindow.SetHandler("sell", CommandSell);
            _commandWindow.SetHandler("cancel", PopScene);
            AddWindow(_commandWindow);
        }

        protected virtual Rect CommandWindowRect()
        {
            float wx = 0;
            float wy = MainAreaTop();
            float ww = _goldWindow.X;
            float wh = CalcWindowHeight(1, true);
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateDummyWindow()
        {
            Rect rect = DummyWindowRect();
            _dummyWindow = Window_Base.Create(rect);
            AddWindow(_dummyWindow);
        }

        protected virtual Rect DummyWindowRect()
        {
            float wx = 0;
            float wy = _commandWindow.Y + _commandWindow.Height;
            float ww = Graphics.BoxWidth;
            float wh = MainAreaHeight() - _commandWindow.Height;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateNumberWindow()
        {
            Rect rect = NumberWindowRect();
            _numberWindow = Window_ShopNumber.Create(rect);
            _numberWindow.Hide();
            _numberWindow.SetHandler("ok", OnNumberOk);
            _numberWindow.SetHandler("cancel", OnNumberCancel);
            AddWindow(_numberWindow);
        }

        protected virtual Rect NumberWindowRect()
        {
            float wx = 0;
            float wy = _dummyWindow.Y;
            float ww = Graphics.BoxWidth - StatusWidth();
            float wh = _dummyWindow.Height;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateStatusWindow()
        {
            Rect rect = StatusWindowRect();
            _statusWindow = Window_ShopStatus.Create(rect);
            _statusWindow.Hide();
            AddWindow(_statusWindow);
        }

        protected virtual Rect StatusWindowRect()
        {
            float ww = StatusWidth();
            float wh = _dummyWindow.Height;
            float wx = Graphics.BoxWidth - ww;
            float wy = _dummyWindow.Y;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateBuyWindow()
        {
            Rect rect = BuyWindowRect();
            _buyWindow = Window_ShopBuy.Create(rect);
            _buyWindow.SetupGoods(_goods);
            _buyWindow.SetHelpWindow(_helpWindow);
            _buyWindow.SetStatusWindow(_statusWindow);
            _buyWindow.Hide();
            _buyWindow.SetHandler("ok", OnBuyOk);
            _buyWindow.SetHandler("cancel", OnBuyCancel);
            AddWindow(_buyWindow);
        }

        protected virtual Rect BuyWindowRect()
        {
            float wx = 0;
            float wy = _dummyWindow.Y;
            float ww = Graphics.BoxWidth - StatusWidth();
            float wh = _dummyWindow.Height;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateCategoryWindow()
        {
            Rect rect = CategoryWindowRect();
            _categoryWindow = Window_ItemCategory.Create(rect);
            _categoryWindow.SetHelpWindow(_helpWindow);
            _categoryWindow.Hide();
            _categoryWindow.Deactivate();
            _categoryWindow.SetHandler("ok", OnCategoryOk);
            _categoryWindow.SetHandler("cancel", OnCategoryCancel);
            AddWindow(_categoryWindow);
        }

        protected virtual Rect CategoryWindowRect()
        {
            float wx = 0;
            float wy = _dummyWindow.Y;
            float ww = Graphics.BoxWidth;
            float wh = CalcWindowHeight(1, true);
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual void CreateSellWindow()
        {
            Rect rect = SellWindowRect();
            _sellWindow = Window_ShopSell.Create(rect);
            _sellWindow.SetHelpWindow(_helpWindow);
            _sellWindow.Hide();
            _sellWindow.SetHandler("ok", OnSellOk);
            _sellWindow.SetHandler("cancel", OnSellCancel);
            _categoryWindow.SetItemWindow(_sellWindow);
            AddWindow(_sellWindow);
            
            if (!_categoryWindow.NeedsSelection())
            {
                _sellWindow.Y -= _categoryWindow.Height;
                _sellWindow.Height += _categoryWindow.Height;
            }
        }

        protected virtual Rect SellWindowRect()
        {
            float wx = 0;
            float wy = _categoryWindow.Y + _categoryWindow.Height;
            float ww = Graphics.BoxWidth;
            float wh = MainAreaHeight() - _commandWindow.Height - _categoryWindow.Height;
            return new Rect(wx, wy, ww, wh);
        }

        protected virtual int StatusWidth() => 352;

        protected virtual void ActivateBuyWindow()
        {
            _buyWindow.SetMoney(Money());
            _buyWindow.Show();
            _buyWindow.Activate();
            _statusWindow.Show();
        }

        protected virtual void ActivateSellWindow()
        {
            if (_categoryWindow.NeedsSelection())
            {
                _categoryWindow.Show();
            }
            _sellWindow.Refresh();
            _sellWindow.Show();
            _sellWindow.Activate();
            _statusWindow.Hide();
        }

        protected virtual void CommandBuy()
        {
            _dummyWindow.Hide();
            ActivateBuyWindow();
        }

        protected virtual void CommandSell()
        {
            _dummyWindow.Hide();
            _sellWindow.Show();
            _sellWindow.Deselect();
            _sellWindow.Refresh();
            
            if (_categoryWindow.NeedsSelection())
            {
                _categoryWindow.Show();
                _categoryWindow.Activate();
            }
            else
            {
                OnCategoryOk();
            }
        }

        protected virtual void OnBuyOk()
        {
            _item = _buyWindow.Item();
            _buyWindow.Hide();
            _numberWindow.Setup(_item, MaxBuy(), BuyingPrice());
            _numberWindow.SetCurrencyUnit(CurrencyUnit());
            _numberWindow.Show();
            _numberWindow.Activate();
        }

        protected virtual void OnBuyCancel()
        {
            _commandWindow.Activate();
            _dummyWindow.Show();
            _buyWindow.Hide();
            _statusWindow.Hide();
            _statusWindow.SetItem(null);
            _helpWindow.Clear();
        }

        protected virtual void OnCategoryOk()
        {
            ActivateSellWindow();
            _sellWindow.Select(0);
        }

        protected virtual void OnCategoryCancel()
        {
            _commandWindow.Activate();
            _dummyWindow.Show();
            _categoryWindow.Hide();
            _sellWindow.Hide();
        }

        protected virtual void OnSellOk()
        {
            _item = _sellWindow.Item();
            _categoryWindow.Hide();
            _sellWindow.Hide();
            _numberWindow.Setup(_item, MaxSell(), SellingPrice());
            _numberWindow.SetCurrencyUnit(CurrencyUnit());
            _numberWindow.Show();
            _numberWindow.Activate();
            _statusWindow.SetItem(_item);
            _statusWindow.Show();
        }

        protected virtual void OnSellCancel()
        {
            _sellWindow.Deselect();
            _statusWindow.SetItem(null);
            _helpWindow.Clear();
            
            if (_categoryWindow.NeedsSelection())
            {
                _categoryWindow.Activate();
            }
            else
            {
                OnCategoryCancel();
            }
        }

        protected virtual void OnNumberOk()
        {
            Rmmz.SoundManager.PlayShop();
            switch (_commandWindow.CurrentSymbol())
            {
                case "buy":
                    DoBuy(_numberWindow.Number());
                    break;
                case "sell":
                    DoSell(_numberWindow.Number());
                    break;
            }
            
            EndNumberInput();
            _goldWindow.Refresh();
            _statusWindow.Refresh();
        }

        protected virtual void OnNumberCancel()
        {
            Rmmz.SoundManager.PlayCancel();
            EndNumberInput();
        }

        protected virtual void DoBuy(int number)
        {
            Rmmz.gameParty.LoseGold(number * BuyingPrice());
            Rmmz.gameParty.GainItem(_item, number);
        }

        protected virtual void DoSell(int number)
        {
            Rmmz.gameParty.GainGold(number * SellingPrice());
            Rmmz.gameParty.LoseItem(_item, number);
        }

        protected virtual void EndNumberInput()
        {
            _numberWindow.Hide();
            switch (_commandWindow.CurrentSymbol())
            {
                case "buy":
                    ActivateBuyWindow();
                    break;
                case "sell":
                    ActivateSellWindow();
                    break;
            }
        }

        protected virtual int MaxBuy()
        {
            int num = Rmmz.gameParty.NumItems(_item);
            int max = Rmmz.gameParty.MaxItems(_item) - num;
            int price = BuyingPrice();
            
            return price > 0 ? Mathf.Min(max, Money() / price) : max;
        }

        protected virtual int MaxSell()
        {
            return Rmmz.gameParty.NumItems(_item);
        }

        protected virtual int Money() => _goldWindow.Value();
        protected virtual string CurrencyUnit() => _goldWindow.CurrencyUnit();
        protected virtual int BuyingPrice() => _buyWindow.Price(_item);
        protected virtual int SellingPrice()
        {
            return Mathf.FloorToInt((float)_item.Price / 2);
        }
    }
}