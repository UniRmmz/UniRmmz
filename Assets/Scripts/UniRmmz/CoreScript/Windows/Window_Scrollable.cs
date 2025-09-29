using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window class with scroll functions.
    /// </summary>
    public abstract partial class Window_Scrollable //: Window_Base
    {
        protected float _scrollX = 0;
        protected float _scrollY = 0;
        protected float _scrollBaseX = 0;
        protected float _scrollBaseY = 0;

        protected float _scrollTargetX = 0;
        protected float _scrollTargetY = 0;
        protected int _scrollDuration = 0;

        protected float _scrollAccelX = 0;
        protected float _scrollAccelY = 0;

        protected bool _scrollTouching = false;
        protected float _scrollLastTouchX = 0;
        protected float _scrollLastTouchY = 0;
        protected bool _scrollLastCursorVisible = false;

        public override void Initialize(Rect rect) 
        {
            base.Initialize(rect);
            ClearScrollStatus();
        }

        public virtual void ClearScrollStatus()
        {
            _scrollTargetX = 0;
            _scrollTargetY = 0;
            _scrollDuration = 0;
            _scrollAccelX = 0;
            _scrollAccelY = 0;
            _scrollTouching = false;
            _scrollLastTouchX = 0;
            _scrollLastTouchY = 0;
            _scrollLastCursorVisible = false;
        }

        public virtual float ScrollX() => _scrollX;
        public virtual float ScrollY() => _scrollY;
        public virtual float ScrollBaseX() => _scrollBaseX;
        public virtual float ScrollBaseY() => _scrollBaseY;

        public virtual void ScrollTo(float x, float y)
        {
            float scrollX = Mathf.Clamp(x, 0, MaxScrollX());
            float scrollY = Mathf.Clamp(y, 0, MaxScrollY());
            if (_scrollX != scrollX || _scrollY != scrollY)
            {
                _scrollX = scrollX;
                _scrollY = scrollY;
                UpdateOrigin();
            }
        }

        public virtual void ScrollBy(float dx, float dy)
        {
            ScrollTo(_scrollX + dx, _scrollY + dy);
        }

        public virtual void SmoothScrollTo(float x, float y)
        {
            _scrollTargetX = Mathf.Clamp(x, 0, MaxScrollX());
            _scrollTargetY = Mathf.Clamp(y, 0, MaxScrollY());
            _scrollDuration = Input.KeyRepeatInterval;
        }

        public virtual void SmoothScrollBy(float dx, float dy)
        {
            if (_scrollDuration == 0)
            {
                _scrollTargetX = _scrollX;
                _scrollTargetY = _scrollY;
            }
            SmoothScrollTo(_scrollTargetX + dx, _scrollTargetY + dy);
        }

        public virtual void SetScrollAccel(float x, float y)
        {
            _scrollAccelX = x;
            _scrollAccelY = y;
        }

        public virtual float OverallWidth() => InnerWidth;
        public virtual float OverallHeight() => InnerHeight;
        public virtual float MaxScrollX() => Mathf.Max(0, OverallWidth() - InnerWidth);
        public virtual float MaxScrollY() => Mathf.Max(0, OverallHeight() - InnerHeight);

        public virtual float ScrollBlockWidth() => ItemWidth();
        public virtual float ScrollBlockHeight() => ItemHeight();

        public virtual void SmoothScrollDown(int lines) => SmoothScrollBy(0, ItemHeight() * lines);
        public virtual void SmoothScrollUp(int lines) => SmoothScrollBy(0, -ItemHeight() * lines);

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            ProcessWheelScroll();
            ProcessTouchScroll();
            UpdateSmoothScroll();
            UpdateScrollAccel();
            UpdateArrows();
            UpdateOrigin();
        }

        protected virtual void ProcessWheelScroll()
        {
            if (IsWheelScrollEnabled() && IsTouchedInsideFrame())
            {
                float threshold = 20f;
                if (TouchInput.WheelY >= threshold)
                {
                    SmoothScrollDown(1);
                }
                if (TouchInput.WheelY <= -threshold)
                {
                    SmoothScrollUp(1);
                }
            }
        }

        protected virtual void ProcessTouchScroll()
        {
            if (IsTouchScrollEnabled())
            {
                if (TouchInput.IsTriggered() && IsTouchedInsideFrame())
                {
                    OnTouchScrollStart();
                }
                if (_scrollTouching)
                {
                    if (TouchInput.IsReleased())
                    {
                        OnTouchScrollEnd();
                    }
                    else if (TouchInput.IsMoved())
                    {
                        OnTouchScroll();
                    }
                }
            }
        }

        protected virtual bool IsWheelScrollEnabled() => IsScrollEnabled();
        protected virtual bool IsTouchScrollEnabled() => IsScrollEnabled();
        protected virtual bool IsScrollEnabled() => true;

        protected virtual bool IsTouchedInsideFrame()
        {
            Vector2 touchPos = new Vector2(TouchInput.X, TouchInput.Y);
            //Vector2 local = transform.InverseTransformPoint(touchPos);
            //return InnerRect.Contains(new PointF(local.x, local.y));
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, touchPos,
                    RmmzRoot.Instance.Canvas.sceneCamera, new Vector2(0, 0));
        }

        protected virtual void OnTouchScrollStart()
        {
            _scrollTouching = true;
            _scrollLastTouchX = TouchInput.X;
            _scrollLastTouchY = TouchInput.Y;
            _scrollLastCursorVisible = CursorVisible;
            SetScrollAccel(0, 0);
        }

        protected virtual void OnTouchScroll()
        {
            float accelX = _scrollLastTouchX - TouchInput.X;
            float accelY = _scrollLastTouchY - TouchInput.Y;
            SetScrollAccel(accelX, accelY);
            _scrollLastTouchX = TouchInput.X;
            _scrollLastTouchY = TouchInput.Y;
            CursorVisible = false;
        }

        protected virtual void OnTouchScrollEnd()
        {
            _scrollTouching = false;
            CursorVisible = _scrollLastCursorVisible;
        }

        protected virtual void UpdateSmoothScroll()
        {
            if (_scrollDuration > 0)
            {
                int d = _scrollDuration;
                float deltaX = (_scrollTargetX - _scrollX) / d;
                float deltaY = (_scrollTargetY - _scrollY) / d;
                ScrollBy(deltaX, deltaY);
                _scrollDuration--;
            }
        }

        protected virtual void UpdateScrollAccel()
        {
            if (_scrollAccelX != 0 || _scrollAccelY != 0)
            {
                ScrollBy(_scrollAccelX, _scrollAccelY);
                _scrollAccelX *= 0.92f;
                _scrollAccelY *= 0.92f;
                if (Mathf.Abs(_scrollAccelX) < 1)
                {
                    _scrollAccelX = 0;
                }
                if (Mathf.Abs(_scrollAccelY) < 1)
                {
                    _scrollAccelY = 0;
                }
            }
        }

        protected virtual void UpdateArrows()
        {
            DownArrowVisible = _scrollY < MaxScrollY();
            UpArrowVisible = _scrollY > 0;
        }

        protected virtual void UpdateOrigin()
        {
            float bw = Math.Max(ScrollBlockWidth(), 1);
            float bh = Math.Max(ScrollBlockHeight(), 1);
            float baseX = _scrollX - (_scrollX % bw);
            float baseY = _scrollY - (_scrollY % bh);
            if (baseX != _scrollBaseX || baseY != _scrollBaseY)
            {
                UpdateScrollBase(baseX, baseY);
                Paint();
            }
            Origin = new Vector2(_scrollX % bw, _scrollY % bh);
        }

        protected virtual void UpdateScrollBase(float baseX, float baseY)
        {
            float dx = baseX - _scrollBaseX;
            float dy = baseY - _scrollBaseY;
            _scrollBaseX = baseX;
            _scrollBaseY = baseY;
            MoveCursorBy((int)-dx, (int)-dy);
            MoveInnerChildrenBy((int)-dx, (int)-dy);
        }

        protected virtual void Paint()
        {
            // override to redraw contents
        }
    }
}