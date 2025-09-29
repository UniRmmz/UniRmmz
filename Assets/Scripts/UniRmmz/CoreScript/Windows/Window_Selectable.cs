using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UniRmmz
{
    /// <summary>
    /// The window class with cursor movement functions.
    /// </summary>
    public abstract partial class Window_Selectable //: Window_Scrollable
    {
        protected int _index;
        protected bool _cursorFixed;
        protected bool _cursorAll;
        protected Window_Help _helpWindow;
        protected Dictionary<string, Action> _handlers;
        protected bool _doubleTouch;
        protected bool _canRepeat;
        
        public new static class Prototype
        {
            public static int ItemHeight => 44;
        
            public static int FittingHeight(int numLines)
            {
                return numLines * ItemHeight + Rmmz.gameSystem.WindowPadding() * 2;
            }
        }
        
        public override void Initialize(Rect rect) 
        {
            base.Initialize(rect);
            _index = -1;
            _cursorFixed = false;
            _cursorAll = false;
            _helpWindow = null;
            _handlers = new();
            _doubleTouch = false;
            _canRepeat = true;
            Deactivate();
        }
        
        public virtual int Index() => _index;

        public virtual bool CursorFixed() => _cursorFixed;
        

        public virtual void SetCursorFixed(bool value)
        {
            _cursorFixed = value;
        }
        
        public virtual bool CursorAll() => _cursorAll;

        public virtual void SetCursorAll(bool cursorAll)
        {
            _cursorAll = cursorAll;
        }
        
        protected virtual int MaxCols() => 1;
        protected virtual int MaxItems() => 0;
        protected virtual int ColSpacing() => 8;
        protected virtual int RowSpacing() => 4;
        public override int ItemWidth() => (int)(InnerWidth / MaxCols());
        public override int ItemHeight() => Prototype.ItemHeight;
        protected override int ContentsHeight() => (int)InnerHeight + ItemHeight();
        public virtual int MaxRows() => (int)Mathf.Max(Mathf.Ceil((float)MaxItems() / MaxCols()), 1f);
        public override float OverallHeight() => MaxRows() * ItemHeight();


        public override void Activate()
        {
            base.Activate();
            Reselect();
        }

        public override void Deactivate()
        {
            base.Deactivate();
            Reselect();
        }

        public virtual void Select(int index)
        {
            _index = index;
            RefreshCursor();
            CallUpdateHelp();
        }
        
        public virtual void ForceSelect(int index)
        {
            Select(index);
            EnsureCursorVisible(false);
        }
        
        public virtual void SmoothSelect(int index)
        {
            Select(index);
            EnsureCursorVisible(true);
        }
        
        public virtual void Deselect()
        {
            Select(-1);
        }

        public virtual void Reselect()
        {
            Select(_index);
            EnsureCursorVisible(true);
            CursorVisible = true;
        }

        public virtual int Row() => (int)(Index() / MaxCols());
        public virtual int TopRow() => (int)(ScrollY() / ItemHeight());
        public virtual int MaxTopRow() => (int)Mathf.Max(0, MaxRows() - MaxPageRows());
        public virtual void SetTopRow(int row) => ScrollTo(ScrollX(), (float)(row * ItemHeight()));
        public virtual int MaxPageRows() => (int)(InnerHeight / ItemHeight());
        public virtual int MaxPageItems() => MaxPageRows() * MaxCols();
        
        public virtual int MaxVisibleItems()
        {
            int visibleRows = Mathf.CeilToInt((float)ContentsHeight() / ItemHeight());
            return visibleRows * MaxCols();
        }

        public virtual bool IsHorizontal()
        {
            return MaxPageRows() == 1;
        }

        public override void UpdateRmmz()
        {
            ProcessCursorMove();
            ProcessHandling();
            ProcessTouch();
            base.UpdateRmmz();
        }

        protected virtual void ProcessCursorMove()
        {
            if (IsCursorMovable())
            {
                int lastIndex = Index();
                if (Input.IsRepeated("down"))
                {
                    CursorDown(Input.IsTriggered("down"));
                }
                if (Input.IsRepeated("up"))
                {
                    CursorUp(Input.IsTriggered("up"));
                }
                if (Input.IsRepeated("right"))
                {
                    CursorRight(Input.IsTriggered("right"));
                }
                if (Input.IsRepeated("left"))
                {
                    CursorLeft(Input.IsTriggered("left"));
                }
                if (IsHandled("pagedown") && Input.IsTriggered("pagedown"))
                {
                    CursorPagedown();
                }
                if (IsHandled("pageup") && Input.IsTriggered("pageup"))
                {
                    CursorPageup();
                }
                if (Index() != lastIndex)
                {
                    PlayCursorSound();
                }
            }
        }

        protected virtual void ProcessHandling()
        {
            if (IsOpenAndActive())
            {
                if (IsOkEnabled() && IsOkTriggered())
                {
                    ProcessOk();
                }
                if (IsCancelEnabled() && IsCancelTriggered())
                {
                    ProcessCancel();
                }
                if (IsHandled("pagedown") && Input.IsTriggered("pagedown"))
                {
                    ProcessPagedown();
                }
                if (IsHandled("pageup") && Input.IsTriggered("pageup"))
                {
                    ProcessPageup();
                }
            }
        }

        protected virtual void ProcessTouch()
        {
            if (IsOpenAndActive())
            {
                if (IsHoverEnabled() && TouchInput.IsHovered())
                {
                    OnTouchSelect(false);
                }
                else if (TouchInput.IsTriggered())
                {
                    OnTouchSelect(true);
                }
                if (TouchInput.IsClicked())
                {
                    OnTouchOk();
                }
                else if (TouchInput.IsCancelled())
                {
                    OnTouchCancel();
                }
            }
        }

        protected virtual bool IsHoverEnabled() => true;
        
        protected virtual void OnTouchSelect(bool trigger)
        {
            _doubleTouch = false;
            if (IsCursorMovable())
            {
                int lastIndex = Index();
                int hitIndex = HitIndex();
                if (hitIndex >= 0)
                {
                    if (hitIndex == Index())
                    {
                        _doubleTouch = true;
                    }
                    Select(hitIndex);
                }
                if (trigger && Index() != lastIndex)
                {
                    PlayCursorSound();
                }
            }
        }

        protected virtual void OnTouchOk()
        {
            if (IsTouchOkEnabled())
            {
                int hitIndex = HitIndex();
                if (_cursorFixed)
                {
                    if (hitIndex == Index())
                    {
                        ProcessOk();
                    }
                }
                else if (hitIndex >= 0)
                {
                    ProcessOk();
                }
            }
        }

        protected virtual void OnTouchCancel()
        {
            if (IsCancelEnabled())
            {
                ProcessCancel();
            }
        }
        
        protected virtual int HitIndex()
        {
            var localPoint = Vector2.zero;
            RmmzRoot.Instance.Canvas.RmmzScreenPointToLocalPointInRectangle(rectTransform, TouchInput.X, TouchInput.Y, out localPoint);
            return HitTest(localPoint.x, localPoint.y);
        }
        
        protected virtual int HitTest(float x, float y)
        {
            if (InnerRect.Contains(x, y))
            {
                float cx = Origin.x + x - Padding;
                float cy = Origin.y + y - Padding;
                int topIndex = TopIndex();
                for (int i = 0; i < MaxVisibleItems(); i++)
                {
                    int index = topIndex + i;
                    if (index < MaxItems())
                    {
                        Rect rect = ItemRect(index);
                        if (rect.Contains(cx, cy))
                        {
                            return index;
                        }
                    }
                }
            }
            return -1;
        }

        protected virtual void CursorDown(bool wrap)
        {
            int index = Index();
            int maxItems = MaxItems();
            int maxCols = MaxCols();
            if (index < maxItems - maxCols || (wrap && maxCols == 1))
            {
                SmoothSelect((_index + maxCols) % maxItems);
            }
        }

        protected virtual void CursorUp(bool wrap)
        {
            int index = Math.Max(0, Index());
            int maxItems = MaxItems();
            int maxCols = MaxCols();
            if (_index >= maxCols || (wrap && maxCols == 1))
            {
                SmoothSelect((_index - maxCols + maxItems) % maxItems);
            }
        }

        protected virtual void CursorRight(bool wrap)
        {
            int index = Index();
            int maxItems = MaxItems();
            int maxCols = MaxCols();
            if (maxCols >= 2 && (index < maxItems - 1 || (wrap && IsHorizontal()))) 
            {
                SmoothSelect((index + 1) % maxItems);
            }
        }

        protected virtual void CursorLeft(bool wrap)
        {
            int index = Index();
            int maxItems = MaxItems();
            int maxCols = MaxCols();
            if (maxCols >= 2 && (index > 0 || (wrap && IsHorizontal())))
            {
                SmoothSelect((_index - 1 + maxItems) % maxItems);
            }
        }

        protected virtual void CursorPagedown()
        {
            int index = Index();
            int maxItems = MaxItems();
            if (TopRow() + MaxPageRows() < MaxRows()) 
            {
                SmoothScrollDown(MaxPageRows());
                Select(Math.Min(index + MaxPageItems(), maxItems - 1));
            }
        }
        
        protected virtual void CursorPageup()
        {
            int index = Index();
            if (TopRow() > 0) 
            {
                SmoothScrollUp(MaxPageRows());
                Select(Math.Max(index - MaxPageItems(), 0));
            }
        }

        protected virtual bool IsCursorMovable()
        {
            return IsOpenAndActive() && !_cursorFixed && !_cursorAll && MaxItems() > 0;
        }

        protected virtual bool IsOpenAndActive()
        {
            return IsOpen() && Visible && Active;
        }
        
        public virtual bool IsTouchOkEnabled()
        {
            return (
                IsOkEnabled() &&
                (_cursorFixed || _cursorAll || _doubleTouch)
            );
        }

        protected virtual bool IsOkEnabled() => IsHandled("ok");
        protected virtual bool IsCancelEnabled() => IsHandled("cancel");

        protected virtual bool IsOkTriggered() =>
            _canRepeat ? Input.IsRepeated("ok") : Input.IsTriggered("ok");

        protected virtual bool IsCancelTriggered() =>
            Input.IsRepeated("cancel");
        
        protected virtual void ProcessOk()
        {
            if (IsCurrentItemEnabled()) 
            {
                PlayOkSound();
                UpdateInputData();
                Deactivate();
                CallOkHandler();
            } 
            else
            {
                PlayBuzzerSound();
            }
        }
        
        protected virtual void CallOkHandler()
        {
            CallHandler("ok");
        }

        protected virtual void ProcessCancel()
        {
            Rmmz.SoundManager.PlayCancel();
            UpdateInputData();
            Deactivate();
            CallCancelHandler();
        }
        
        protected virtual void CallCancelHandler()
        {
            CallHandler("cancel");
        }

        protected virtual void ProcessPageup()
        {
            UpdateInputData();
            Deactivate();
            CallHandler("pageup");
        }
        
        protected virtual void ProcessPagedown()
        {
            UpdateInputData();
            Deactivate();
            CallHandler("pagedown");
        }
        
        protected virtual void UpdateInputData()
        {
            Input.Update();
            TouchInput.Update();
            ClearScrollStatus();
        }
        protected virtual void EnsureCursorVisible(bool smooth)
        {
            if (_cursorAll)
            {
                ScrollTo(0, 0);
            }
            else if (InnerHeight > 0 && Row() >= 0)
            {
                int scrollY = (int)ScrollY();
                int itemTop = Row() * ItemHeight();
                int itemBottom = itemTop + ItemHeight();
                int scrollMin = itemBottom - InnerHeight;
                if (scrollY > itemTop)
                {
                    if (smooth)
                    {
                        SmoothScrollTo(0, itemTop);
                    }
                    else
                    {
                        ScrollTo(0, itemTop);
                    }
                }
                else if (scrollY < scrollMin)
                {
                    if (smooth)
                    {
                        SmoothScrollTo(0, scrollMin);
                    }
                    else
                    {
                        ScrollTo(0, scrollMin);
                    }
                }    
            }
        }
        
        public virtual int TopIndex()
        {
            return TopRow() * MaxCols();
        }

        protected virtual Rect ItemRect(int index)
        {
            int cols = MaxCols();
            int iw = ItemWidth();
            int ih = ItemHeight();
            int colSpacing = ColSpacing(); 
            int rowSpacing = RowSpacing();
            int col = index % cols;
            int row = index / cols;
            int x = col * iw + colSpacing / 2 -  (int)ScrollBaseX();
            int y = row * ih + rowSpacing / 2 - (int)ScrollBaseY();
            int width = ItemWidth() - colSpacing;
            int height = ItemHeight() - rowSpacing;
            return new Rect(x, y, width, height);
        }
        
        protected virtual Rect ItemRectWithPadding(int index)
        {
            var rect = ItemRect(index);
            var padding = ItemPadding();
            rect.x += padding;
            rect.width -= padding * 2;
            return rect;
        }
        
        protected virtual Rect ItemLineRect(int index) 
        {
            var rect = ItemRectWithPadding(index);
            var padding = (rect.height - LineHeight()) / 2;
            rect.y += padding;
            rect.height -= padding * 2;
            return rect;
        }

        public void SetHelpWindow(Window_Help helpWindow)
        {
            _helpWindow = helpWindow;
            CallUpdateHelp();
        }

        protected void ShowHelpWindow()
        {
            _helpWindow?.Show();
        }
        
        protected void HideHelpWindow()
        {
            _helpWindow?.Hide();
        }
        

        protected virtual void CallUpdateHelp()
        {
            if (Active && _helpWindow != null)
            {
                UpdateHelp();
            }
        }
        
        protected virtual void UpdateHelp()
        {
            _helpWindow?.Clear();
        }

        public virtual void SetHelpWindowItem(DataCommonItem item)
        {
            _helpWindow?.SetItem(item);
        }

        public virtual void SetHandler(string symbol, Action method)
        {
            _handlers[symbol] = method;
        }
        
        public virtual bool IsHandled(string symbol)
        {
            return _handlers.TryGetValue(symbol, out _);
        }
        
        public virtual void CallHandler(string symbol)
        {
            if (IsHandled(symbol))
            {
                _handlers[symbol]();
            }
        }
        
        public virtual bool IsCurrentItemEnabled()
        {
            return true;
        }

        public virtual void DrawAllItems()
        {
            int topIndex = TopIndex();
            for (int i = 0; i < MaxVisibleItems(); i++)
            {
                int index = topIndex + i;
                if (index < MaxItems())
                {
                    DrawItemBackground(index);
                    DrawItem(index);
                }
            }
        }

        public virtual void DrawItem(int index)
        {
            // Override in derived class
        }

        public virtual void ClearItem(int index)
        {
            var rect = ItemRect(index);
            Contents.ClearRect(rect.x, rect.y, rect.width, rect.height);
            ContentsBack.ClearRect(rect.x, rect.y, rect.width, rect.height);
        }

        public virtual void DrawItemBackground(int index)
        {
            var rect = ItemRect(index);
            DrawBackgroundRect(rect);
        }

        protected virtual void DrawBackgroundRect(Rect rect)
        {
            var c1 = Rmmz.ColorManager.ItemBackColor1();
            var c2 = Rmmz.ColorManager.ItemBackColor2();
            float x = rect.x;
            float y = rect.y;
            float w = rect.width;
            float h = rect.height;
            ContentsBack.GradientFillRect(x, y, w, h, c1, c2, true);
            //ContentsBack.StrokeRect(x, y, w, h, c1);
        }

        public virtual void RedrawItem(int index)
        {
            if (index >= 0)
            {
                ClearItem(index);
                DrawItemBackground(index);
                DrawItem(index);
            }
        }

        public virtual void RedrawCurrentItem()
        {
            RedrawItem(Index());
        }
        
        public virtual void Refresh()
        {
            Paint();
        }

        protected override void Paint()
        {
            if (Contents != null)
            {
                Contents.Clear();
                ContentsBack.Clear();
                DrawAllItems();
            }
        }
 
        protected virtual void RefreshCursor()
        {
            if (_cursorAll)
            {
                RefreshCursorForAll();
            }
            else if (_index >= 0)
            {
                var rect = ItemRect(_index);
                SetCursorRect((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            }
            else
            {
                SetCursorRect(0, 0, 0, 0);
            }
        }

        protected virtual void RefreshCursorForAll()
        {
            int maxItems = MaxItems();
            if (maxItems > 0)
            {
                var rect = ItemRect(0);
                rect.Enlarge(ItemRect(maxItems - 1));
                SetCursorRect(rect.x, rect.y, rect.width, rect.height);
            }
            else
            {
                SetCursorRect(0, 0, 0, 0);
            }
        }
      
    }
}