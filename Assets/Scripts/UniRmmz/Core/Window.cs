using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UniRmmz
{
    public partial class Window : RmmzContainer
    {
        protected bool _isWindow;
        protected Bitmap _windowskin;
        protected Rect _cursorRect;
        protected int _openness = 255;
        protected int _animationCount;

        protected int _padding = 12;
        protected int _margin = 4;
        protected Vector4 _colorTone = Vector4.zero;
        protected List<Sprite> _innerChildren = new();

        protected Sprite _container;
        protected Sprite _backSprite;
        protected Sprite _frameSprite;
        protected Sprite _contentsBackSprite;
        protected Sprite _cursorSprite;
        protected Sprite _contentsSprite;
        protected Sprite _downArrowSprite;
        protected Sprite _upArrowSprite;
        protected Sprite _pauseSignSprite;
        protected Sprite _clientArea;

        /// <summary>
        /// The image used as a window skin.
        /// </summary>
        public virtual Bitmap WindowSkin
        {
            get => _windowskin;
            set
            {
                if (_windowskin != value)
                {
                    _windowskin = value;
                    _windowskin.AddLoadListener(OnWindowskinLoad);
                }
            }
        }

        /// <summary>
        /// The bitmap used for the window contents.
        /// </summary>
        public virtual Bitmap Contents
        {
            get => _contentsSprite.Bitmap;
            set => _contentsSprite.Bitmap = value;
        }

        /// <summary>
        /// The bitmap used for the window contents background.
        /// </summary>
        public virtual Bitmap ContentsBack
        {
            get => _contentsBackSprite.Bitmap;
            set => _contentsBackSprite.Bitmap = value;
        }

        /// <summary>
        /// The width of the window in pixels.
        /// </summary>
        public override float Width
        {
            get => rectTransform.sizeDelta.x;
            set
            {
                var size = rectTransform.sizeDelta;
                size.x = value;
                rectTransform.sizeDelta = size;
                _RefreshAllParts();
            }
        }
        
        /// <summary>
        /// The height of the window in pixels.
        /// </summary>
        public override float Height
        {
            get => rectTransform.sizeDelta.y;
            set
            {
                var size = rectTransform.sizeDelta;
                size.y = value;
                rectTransform.sizeDelta = size;
                _RefreshAllParts();
            }
        }
        
        /// <summary>
        /// The size of the padding between the frame and contents.
        /// </summary>
        public virtual int Padding
        {
            get => _padding;
            set
            {
                _padding = value;
                _RefreshAllParts();
            }
        }
        
        /// <summary>
        /// The size of the margin for the window background.
        /// </summary>
        public virtual int Margin
        {
            get => _margin;
            set
            {
                _margin = value;
                _RefreshAllParts();
            }
        }

        /// <summary>
        /// The opacity of the window without contents (0 to 255).
        /// </summary>
        public override int Opacity
        {
            get => (int)(_container.Alpha * 255);
            set => _container.Alpha = (float)Mathf.Clamp(value, 0, 255) / 255;
        }
        
        /// <summary>
        /// The opacity of the window background (0 to 255).
        /// </summary>
        public virtual int BackOpacity
        {
            get => (int)(_backSprite.Alpha * 255);
            set => _backSprite.Alpha = (float)Mathf.Clamp(value, 0, 255) / 255;
        }
        
        /// <summary>
        /// The opacity of the window contents (0 to 255).
        /// </summary>
        public virtual int ContentsOpacity
        {
            get => (int)(_contentsSprite.Alpha * 255);
            set => _contentsSprite.Alpha = (float)Mathf.Clamp(value, 0, 255) / 255;
        }

        /// <summary>
        /// The openness of the window (0 to 255).
        /// </summary>
        public virtual int Openness
        {
            get => _openness;
            set
            {
                if (_openness != value)
                {
                    _openness = Math.Clamp(value, 0, 255);
                    _container.Scale = new Vector2(_container.Scale.x, (float)_openness / 255);
                    _container.Y = ((float)Height / 2) * (1 - (float)_openness / 255);
                    #region UniRmmz
                    _mask.SetVerticesDirty();
                    #endregion
                }
            }
        }

        /// <summary>
        /// The width of the content area in pixels.
        /// </summary>
        public virtual int InnerWidth
        {
            get => Math.Max(0, (int)Width - _padding * 2);
        }
        
        /// <summary>
        /// The height of the content area in pixels.
        /// </summary>
        public virtual int InnerHeight
        {
            get => Math.Max(0, (int)Height - _padding * 2);
        }

        /// <summary>
        /// The rectangle of the content area.
        /// </summary>
        public virtual Rect InnerRect
        {
            get => new Rect(_padding, _padding, InnerWidth, InnerHeight);
        }
        
        /// <summary>
        /// The origin point of the window for scrolling.
        /// </summary>
        public override Vector2 Origin { get; set; } 

        /// <summary>
        /// The active state for the window.
        /// </summary>
        public virtual bool Active { get; set; } = true;

        /// <summary>
        /// The visibility of the frame.
        /// </summary>
        public virtual bool FrameVisible { get; set; } = true;

        /// <summary>
        /// The visibility of the cursor.
        /// </summary>
        public virtual bool CursorVisible { get; set; } = true;

        /// <summary>
        /// The visibility of the down scroll arrow.
        /// </summary>
        public virtual bool DownArrowVisible { get; set; } = false;

        /// <summary>
        /// The visibility of the up scroll arrow.
        /// </summary>
        public virtual bool UpArrowVisible { get; set; } = false;

        /// <summary>
        /// The visibility of the pause sign.
        /// </summary>
        public virtual bool Pause { get; set; } = false;

        public virtual void Initialize(Rect rect)
        {
            _CreateAllParts();
#region UniRmmz
            CreateWindowMask();
#endregion
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            if (Active)
            {
                _animationCount++;
            }
        }
        
        /// <summary>
        /// Sets the x, y, width, and height all at once.
        /// </summary>
        /// <param name="x">The x coordinate of the window.</param>
        /// <param name="y">The y coordinate of the window.</param>
        /// <param name="width">The width of the window.</param>
        /// <param name="height">The height of the window.</param>
        public virtual void Move(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            X = x;
            Y = y;
            if (Width != (int)width || Height != (int)height)
            {
                Width = (int)width;
                Height = (int)height;
                _RefreshAllParts();
            }
        }

        /// <summary>
        /// Checks whether the window is completely open (openness == 255).
        /// </summary>
        /// <returns>True if the window is open.</returns>
        public virtual bool IsOpen()
        {
            return _openness >= 255;
        }

        /// <summary>
        /// Checks whether the window is completely closed (openness == 0).
        /// </summary>
        /// <returns>True if the window is closed.</returns>
        public virtual bool IsClosed()
        {
            return _openness <= 0;
        }

        /// <summary>
        /// Sets the position of the command cursor.
        /// </summary>
        /// <param name="x">The x coordinate of the cursor.</param>
        /// <param name="y">The y coordinate of the cursor.</param>
        /// <param name="width">The width of the cursor.</param>
        /// <param name="height">The height of the cursor.</param>
        public virtual void SetCursorRect(float x = 0, float y = 0, float width = 0, float height = 0)
        {
            int cw = Mathf.FloorToInt(width);
            int ch = Mathf.FloorToInt(height);
            _cursorRect.x = Mathf.FloorToInt(x);
            _cursorRect.y = Mathf.FloorToInt(y);
            if (_cursorRect.width != cw || _cursorRect.height != ch)
            {
                _cursorRect.width = cw;
                _cursorRect.height = ch;
                _RefreshCursor();
            }
        }

        /// <summary>
        /// Moves the cursor position by the given amount.
        /// </summary>
        /// <param name="x">The amount of horizontal movement.</param>
        /// <param name="y">The amount of vertical movement.</param>
        public virtual void MoveCursorBy(int x, int y)
        {
            _cursorRect.x += x;
            _cursorRect.y += y;
        }

        /// <summary>
        /// Moves the inner children by the given amount.
        /// </summary>
        /// <param name="x">The amount of horizontal movement.</param>
        /// <param name="y">The amount of vertical movement.</param>
        public virtual void MoveInnerChildrenBy(int x, int y)
        {
            foreach (var child in _innerChildren)
            {
                child.X += x;
                child.Y += y;
            }
        }

        /// <summary>
        /// Changes the color of the background.
        /// </summary>
        /// <param name="r">The red value in the range (-255, 255).</param>
        /// <param name="g">The green value in the range (-255, 255).</param>
        /// <param name="b">The blue value in the range (-255, 255).</param>
        public virtual void SetTone(int r, int g, int b)
        {
            var tone = _colorTone;
            if (r != tone[0] || g != tone[1] || b != tone[2])
            {
                _colorTone = new Vector4(r, g, b, 0);
                _RefreshBack();
            }
        }

        /// <summary>
        /// Adds a child between the background and contents.
        /// </summary>
        /// <param name="child">The child to add.</param>
        /// <returns>The child that was added.</returns>
        public virtual void AddChildToBack(MonoBehaviour child)
        {
            this.AddChild(child);
            for (int childIndex = 0; childIndex < transform.childCount; ++childIndex)
            {
                if (transform.GetChild(childIndex) == _container.transform)
                {
                    child.transform.SetSiblingIndex(childIndex);
                    break;
                }
            }
        }

        /// <summary>
        /// Adds a child to the client area.
        /// </summary>
        /// <param name="child">child - The child to add.</param>
        /// <returns>The child that was added.</returns>
        public virtual Sprite AddInnerChild(Sprite child)
        {
            _innerChildren.Add(child);
            return _clientArea.AddChild(child);
        }

        public virtual void LateUpdate()
        {
            if (!Application.IsPlaying(gameObject))
            {
                return;
            }
            
            _UpdateClientArea();
            _UpdateFrame();
            _UpdateContentsBack();
            _UpdateCursor();
            _UpdateContents();
            _UpdateArrows();
            _UpdatePauseSign();
            _UpdateFilterArea();
        }

        protected virtual void _CreateAllParts()
        {
            if (!Application.IsPlaying(gameObject))
            {
                return;
            }
            
            _CreateContainer();
            _CreateBackSprite();
            _CreateFrameSprite();
            _CreateClientArea();
            _CreateContentsBackSprite();
            _CreateCursorSprite();
            _CreateContentsSprite();
            _CreateArrowSprites();
            _CreatePauseSignSprites();
        }

        protected virtual void _CreateContainer()
        {
            _container = Sprite.Create("container");
            this.AddChild(_container);
        }
        
        protected virtual void _CreateBackSprite()
        {
            _backSprite = Sprite.Create("backSprite");
            _backSprite.AddChild(TilingSprite.Create("tiling"));
            _container.AddChild(_backSprite);
        }
        
        protected virtual void _CreateFrameSprite()
        {
            _frameSprite = Sprite.Create("frameSprite");
            for (int i = 0; i < 8; ++i)
            {
                _frameSprite.AddChild(Sprite.Create());
            }
            _container.AddChild(_frameSprite);
        }
        
        protected virtual void _CreateClientArea()
        {
            _clientArea = Sprite.Create("clientArea");
            _clientArea.AddFilter(new AlphaFilter());
            _clientArea.FilterArea = new Rect(); 
            _clientArea.Move(_padding, _padding);
            this.AddChild(_clientArea);
        }
        
        protected virtual void _CreateContentsBackSprite()
        {
            _contentsBackSprite = Sprite.Create("contentsBackSprite");
            _clientArea.AddChild(_contentsBackSprite);
        }
        
        protected virtual void _CreateCursorSprite()
        {
            _cursorSprite = Sprite.Create("cursorSprite");
            for (int i = 0; i < 9; ++i)
            {
                _cursorSprite.AddChild(Sprite.Create());
            }
            _clientArea.AddChild(_cursorSprite);
        }
        
        protected virtual void _CreateContentsSprite()
        {
            _contentsSprite = Sprite.Create("contentsSprite");
            _clientArea.AddChild(_contentsSprite);
        }
        
        protected virtual void _CreateArrowSprites()
        {
            _downArrowSprite = Sprite.Create("downArrowSprite");
            this.AddChild(_downArrowSprite);
            _upArrowSprite = Sprite.Create("upArrowSprite");
            this.AddChild(_upArrowSprite);
        }
        
        protected virtual void _CreatePauseSignSprites()
        {
            _pauseSignSprite = Sprite.Create("pauseSignSprite");
            this.AddChild(_pauseSignSprite);
        }

        protected virtual void OnWindowskinLoad(Bitmap _)
        {
            _RefreshAllParts();
        }

        protected virtual void _RefreshAllParts()
        {
            _RefreshBack();
            _RefreshFrame();
            _RefreshCursor();
            _RefreshArrows();
            _RefreshPauseSign();
        }

        protected virtual void _RefreshBack()
        {
            float w = Math.Max(0, Width - _margin * 2);
            float h = Math.Max(0, Height - _margin * 2);
            var sprite = _backSprite;
            var tilingSprite = sprite.transform.GetChild(0).GetComponent<TilingSprite>();
            // [Note] We use 95 instead of 96 here to avoid blurring edges.
            sprite.Bitmap = _windowskin;
            sprite.SetFrame(0, 0, 95, 95);
            sprite.Move(_margin, _margin);
            sprite.Scale = new Vector2(w / 95, h / 95);
            tilingSprite.Bitmap = _windowskin;
            tilingSprite.SetFrame(0, 96, 96, 96);
            tilingSprite.Move(0, 0, w, h);
            if (sprite.Scale.x != 0 && sprite.Scale.y != 0)
            {
                tilingSprite.Scale = new Vector2(1f / sprite.Scale.x, 1f / sprite.Scale.y);    
            }
            sprite.SetColorTone(_colorTone);
        }

        protected virtual void _RefreshFrame()
        {
            var drect = new Rect(0, 0, Width, Height);
            var srect = new Rect(96, 0, 96, 96);
            int m = 24;
            foreach (var child in _frameSprite.transform.GetChildren<Sprite>())
            {
                child.Bitmap = _windowskin;
            }
            _SetRectPartsGeometry(_frameSprite, srect, drect, m);
        }

        protected virtual void _RefreshCursor()
        {
            var drect = _cursorRect;
            var srect = new Rect(96, 96, 48, 48);
            int m = 4;
            foreach (var child in _cursorSprite.transform.GetChildren<Sprite>())
            {
                child.Bitmap = _windowskin;
            }
            _SetRectPartsGeometry(_cursorSprite, srect, drect, m);
        }

        protected virtual void _SetRectPartsGeometry(Sprite sprite, Rect srect, Rect drect, int m)
        {
            float sx = srect.x;
            float sy = srect.y;
            float sw = srect.width;
            float sh = srect.height;
            float dx = drect.x;
            float dy = drect.y;
            float dw = drect.width;
            float dh = drect.height;
            float smw = sw - m * 2;
            float smh = sh - m * 2;
            float dmw = dw - m * 2;
            float dmh = dh - m * 2;
            var children = sprite.transform.GetChildren<Sprite>().ToArray();
            sprite.SetFrame(0, 0, dw, dh);
            sprite.Move(dx, dy);
            // corner
            children[0].SetFrame(sx, sy, m, m);
            children[1].SetFrame(sx + sw - m, sy, m, m);
            children[2].SetFrame(sx, sy + sw - m, m, m);
            children[3].SetFrame(sx + sw - m, sy + sw - m, m, m);
            children[0].Move(0, 0);
            children[1].Move(dw - m, 0);
            children[2].Move(0, dh - m);
            children[3].Move(dw - m, dh - m);
            // edge
            children[4].Move(m, 0);
            children[5].Move(m, dh - m);
            children[6].Move(0, m);
            children[7].Move(dw - m, m);
            children[4].SetFrame(sx + m, sy, smw, m);
            children[5].SetFrame(sx + m, sy + sw - m, smw, m);
            children[6].SetFrame(sx, sy + m, m, smh);
            children[7].SetFrame(sx + sw - m, sy + m, m, smh);
            children[4].Scale = new Vector2(dmw / smw, children[4].Scale.y);
            children[5].Scale = new Vector2(dmw / smw, children[5].Scale.y);
            children[6].Scale = new Vector2(children[6].Scale.x, dmh / smh);
            children[7].Scale = new Vector2(children[7].Scale.x, dmh / smh);
            // center
            if (children.Length > 8) 
            {
                children[8].SetFrame(sx + m, sy + m, smw, smh);
                children[8].Move(m, m);
                children[8].Scale = new Vector2(dmw / smw, dmh / smh);
            }
            foreach (var child in children) 
            {
                child.Visible = dw > 0 && dh > 0;
            }
        }

        protected virtual void _RefreshArrows()
        {
            float w = Width;
            float h = Height;
            int p = 24;
            int q = p / 2;
            int sx = 96 + p;
            int sy = 0 + p;
            _downArrowSprite.Bitmap = _windowskin;
            _downArrowSprite.rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
            _downArrowSprite.SetFrame(sx + q, sy + q + p, p, q);
            _downArrowSprite.Move(w / 2, h - q);
            _upArrowSprite.Bitmap = _windowskin;
            _upArrowSprite.rectTransform.anchoredPosition = new Vector2(0.5f, 0.5f);
            _upArrowSprite.SetFrame(sx + q, sy, p, q);
            _upArrowSprite.Move(w / 2, q);
        }

        protected virtual void _RefreshPauseSign()
        {
            int sx = 144;
            int sy = 96;
            int p = 24;
            _pauseSignSprite.Bitmap = _windowskin;
            _pauseSignSprite.Anchor = new Vector2(0.5f, 1f);
            _pauseSignSprite.Move(Width / 2, Height);
            _pauseSignSprite.SetFrame(sx, sy, p, p);
            _pauseSignSprite.Alpha = 0;
        }
        
        protected virtual void _UpdateClientArea()
        {
            int pad = this._padding;
            _clientArea.Move(pad, pad);
            _clientArea.X = pad - Origin.x;
            _clientArea.Y = pad - Origin.y;
            if (InnerWidth > 0 && InnerHeight > 0) 
            {
                _clientArea.Visible = IsOpen();
            }
            else 
            {
                _clientArea.Visible = false;
            }
        }
        
        protected virtual void _UpdateFrame()
        {
            _frameSprite.Visible = FrameVisible;
        }
        
        protected virtual void _UpdateContentsBack()
        {
            var bitmap = _contentsBackSprite.Bitmap;
            if (bitmap != null) 
            {
                _contentsBackSprite.SetFrame(0, 0, bitmap.Width, bitmap.Height);
            }
        }
        
        protected virtual void _UpdateCursor()
        {
            _cursorSprite.Alpha = _MakeCursorAlpha();
            _cursorSprite.Visible = IsOpen() && CursorVisible;
            _cursorSprite.X = _cursorRect.x;
            _cursorSprite.Y = _cursorRect.y;
        }
        
        protected virtual float _MakeCursorAlpha()
        {
            float blinkCount = _animationCount % 40;
            float baseAlpha = (float)ContentsOpacity / 255;
            if (Active)
            {
                if (blinkCount < 20)
                {
                    return baseAlpha - blinkCount / 32;
                }
                else 
                {
                    return baseAlpha - (40 - blinkCount) / 32;
                }
            }
            return baseAlpha;
        }
        
        protected virtual void _UpdateContents()
        {
            var bitmap = _contentsSprite.Bitmap;
            if (bitmap != null)
            {
                _contentsSprite.SetFrame(0, 0, bitmap.Width, bitmap.Height);
            }
        }
        
        protected virtual void _UpdateArrows()
        {
            _downArrowSprite.Visible = IsOpen() && DownArrowVisible;
            _upArrowSprite.Visible = IsOpen() && UpArrowVisible;
        }
        
        protected virtual void _UpdatePauseSign()
        {
            var sprite = _pauseSignSprite;
            int x = Mathf.FloorToInt(this._animationCount / 16) % 2;
            int y = Mathf.FloorToInt(this._animationCount / 16 / 2) % 2;
            int sx = 144;
            int sy = 96;
            int p = 24;
            if (!Pause) 
            {
                sprite.Alpha = 0;
            } 
            else if (sprite.Alpha < 1)
            {
                sprite.Alpha = Mathf.Min(sprite.Alpha + 0.1f, 1f);
            }
            sprite.SetFrame(sx + x * p, sy + y * p, p, p);
            sprite.Visible = IsOpen();
        }
        
        protected virtual void _UpdateFilterArea()
        {
            // ウィンドウはWindowLayer直下にある前提にして、計算を簡略化している
            var rect = InnerRect;
            var windowLayer = transform.parent.GetComponent<WindowLayer>();
            var screenPos = new Vector2(rect.x + X + windowLayer.X, rect.y + Y + windowLayer.Y);
            _clientArea.FilterArea = new Rect(screenPos.x, screenPos.y, rect.width, rect.height);
        }
        
        #region UniRmmz
        private RmmzWindowMask _mask;
        private void CreateWindowMask()
        {
            _mask = RmmzWindowMask.Create("_mask");
            this.AddChild(_mask);           
        }
        
        public static T _Create<T>(Rect rect, string name = "") where T : Window
        {
            var obj = new GameObject(name);
            obj.layer = Rmmz.RmmzDontRenderLayer;
            var window = obj.AddComponent<T>();
            window.Initialize(rect);
            return window;
        }
        
        #endregion
    }
}