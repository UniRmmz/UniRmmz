using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Color = System.Drawing.Color;

namespace UniRmmz
{
    /// <summary>
    /// The basic object that is rendered to the game screen.
    /// </summary>
    public partial class Sprite : RawImage, IRmmzDrawable2d
    {
        private static int s_counter = 0;
        
        private int _spriteId = 0;
        private Rect _frame;
        private Bitmap _bitmap;
        private int _hue;
        private UnityEngine.Color _blendColor = new UnityEngine.Color(0, 0, 0, 0);
        private Vector4 _colorTone = Vector4.zero;
        private ColorFilter _colorFilter;
        private BlendModes _blendMode;
        private bool _hidden;
        private bool _refreshFrame;
        private CanvasGroup _canvasGroup;
        private float _alpha = 1f;
        private List<IRmmzFilter> _filters = new();
        private RmmzFilterCanvas _filterCanvas;
        private bool _preventRecursive;
        
        /// <summary>
        /// PIXI.jsのブレンドモードに対応するC#のenum
        /// </summary>
        public enum BlendModes
        {
            /// <summary>
            /// 通常のブレンドモード
            /// </summary>
            Normal = 0,
            
            /// <summary>
            /// 加算ブレンドモード
            /// </summary>
            Add = 1,
            
            /// <summary>
            /// 乗算ブレンドモード
            /// </summary>
            Multiply = 2,
            
            /// <summary>
            /// スクリーンブレンドモード
            /// </summary>
            Screen = 3,
            
            /// <summary>
            /// オーバーレイブレンドモード
            /// </summary>
            Overlay = 4,
            
            /// <summary>
            /// 比較（暗）ブレンドモード
            /// </summary>
            //Darken = 5,
            
            /// <summary>
            /// 比較（明）ブレンドモード
            /// </summary>
            //Lighten = 6,
            
            /// <summary>
            /// カラー覆い焼きブレンドモード
            /// </summary>
            //ColorDodge = 7,
            
            /// <summary>
            /// カラー焼き込みブレンドモード
            /// </summary>
            //ColorBurn = 8,
            
            /// <summary>
            /// ハードライトブレンドモード
            /// </summary>
            //HardLight = 9,
            
            /// <summary>
            /// ソフトライトブレンドモード
            /// </summary>
            //SoftLight = 10,
            
            /// <summary>
            /// 差の絶対値ブレンドモード
            /// </summary>
            //Difference = 11,
            
            /// <summary>
            /// 除外ブレンドモード
            /// </summary>
            //Exclusion = 12,
            
            /// <summary>
            /// 色相ブレンドモード
            /// </summary>
            //Hue = 13,
            
            /// <summary>
            /// 彩度ブレンドモード
            /// </summary>
            //Saturation = 14,
            
            /// <summary>
            /// カラーブレンドモード
            /// </summary>
            //Color = 15,
            
            /// <summary>
            /// 輝度ブレンドモード
            /// </summary>
            //Luminosity = 16,
            
            /// <summary>
            /// 通常のブレンドモード（NPM版）
            /// </summary>
            //NormalNpm = 17,
            
            /// <summary>
            /// 加算ブレンドモード（NPM版）
            /// </summary>
            //AddNpm = 18,
            
            /// <summary>
            /// スクリーンブレンドモード（NPM版）
            /// </summary>
            //ScreenNpm = 19,
            
            /// <summary>
            /// なしブレンドモード
            /// </summary>
            //None = 20,
            
            /// <summary>
            /// 減算ブレンドモード
            /// </summary>
            Subtract = 21,
            
            /// <summary>
            /// 除算ブレンドモード
            /// </summary>
            //Divide = 22,
            
            /// <summary>
            /// 透明色をクリアするブレンドモード
            /// </summary>
            //Clear = 23,
            
            /// <summary>
            /// 元の色を保持するブレンドモード
            /// </summary>
            //Src = 24,
            
            /// <summary>
            /// 上書きブレンドモード
            /// </summary>
            //SrcOver = 25,
            
            /// <summary>
            /// 入力元のブレンドモード
            /// </summary>
            //SrcIn = 26,
            
            /// <summary>
            /// 入力元の外側のブレンドモード
            /// </summary>
            //SrcOut = 27,
            
            /// <summary>
            /// 入力元の上のブレンドモード
            /// </summary>
            //SrcAtop = 28,
            
            /// <summary>
            /// 対象色を保持するブレンドモード
            /// </summary>
            //Dst = 29,
            
            /// <summary>
            /// 対象色の上のブレンドモード
            /// </summary>
            //DstOver = 30,
            
            /// <summary>
            /// 対象色の内側のブレンドモード
            /// </summary>
            DstIn = 31,
            
            /// <summary>
            /// 対象色の外側のブレンドモード
            /// </summary>
            //DstOut = 32,
            
            /// <summary>
            /// 対象色の上のブレンドモード
            /// </summary>
            //DstAtop = 33,
            
            /// <summary>
            /// XORブレンドモード
            /// </summary>
            //Xor = 34,
        }

        /// <summary>
        /// The image for the sprite.
        /// </summary>
        public Bitmap Bitmap
        {
            get => _bitmap;
            set
            {
                if (_bitmap != value)
                {
                    _bitmap = value;
                    this.OnBitmapChange();
                }
            }
        }

        /// <summary>
        /// The width of the sprite without the scale.
        /// </summary>
        public float Width
        {
            get => _frame.width;
            set
            {
                _frame.width = value;
                this.Refresh();
            }
        }
        
        /// <summary>
        /// The height of the sprite without the scale.
        /// </summary>
        public float Height
        {
            get => _frame.height;
            set
            {
                _frame.height = value;
                this.Refresh();
            }
        }
        
        public virtual bool Visible
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        /// <summary>
        /// The origin point of the window for scrolling.
        /// </summary>
        public virtual Vector2 Origin
        {
            get => new Vector2(rectTransform.localPosition.x, -rectTransform.localPosition.y);
            set => rectTransform.localPosition = new Vector2(value.x, -value.y);
        }
        
        public virtual float X
        {
            get => rectTransform.localPosition.x;
            set
            {
                var point = Origin;
                rectTransform.localPosition = new Vector2(value, point.y);
            }
        }
        
        public virtual float Y
        {
            get => -rectTransform.localPosition.y;
            set
            {
                var point = Origin;
                rectTransform.localPosition = new Vector2(point.x, -value);
            }
        }
        
        public virtual float Z { get; set; }
        
        public float Ax
        {
            get;
            set;
        }
        
        public float Ay
        {
            get;
            set;
        }
        
        public float Rx
        {
            get;
            set;
        }
        
        public float Ry
        {
            get;
            set;
        }
        
        public float Dx
        {
            get;
            set;
        }
        
        public float Dy
        {
            get;
            set;
        }

        public int SpriteId => _spriteId;
        
        public Vector2 Pivot
        {
            get => new Vector2(rectTransform.pivot.x * Width, 1 - rectTransform.pivot.y * Height);
            set
            {
                rectTransform.pivot = new Vector2(value.x / Width, 1 - value.y / Height);
            }
        }
        
        public Vector2 Anchor
        {
            get => new Vector2(rectTransform.pivot.x, 1 - rectTransform.pivot.y);
            set
            {
                rectTransform.pivot = new Vector2(value.x, 1 - value.y);
                rectTransform.anchorMin = rectTransform.pivot;
                rectTransform.anchorMax = rectTransform.pivot;
            }
        }

        // radian
        public virtual float Rotation
        {
            get => -rectTransform.rotation.eulerAngles.z / 180f * Mathf.PI;
            set => rectTransform.rotation = Quaternion.Euler(0, 0, -value * 180f / Mathf.PI);
        }
        
        public virtual Vector2 Scale
        {
            get => rectTransform.localScale;
            set => rectTransform.localScale = value;
        }
        
        public virtual float Alpha
        {
            get
            {
                return _alpha;
            }
            set
            {
                if (_alpha != value)
                {
                    _alpha = value;
                    if (_canvasGroup == null)
                    {
                        _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                    }
                    _canvasGroup.alpha = value;                    
                }
            }
        }

        /// <summary>
        /// The opacity of the sprite (0 to 255).
        /// </summary>
        public int Opacity
        {
            get => (int)(Alpha * 255);
            set => Alpha = (float)Mathf.Clamp(value, 0, 255) / 255;
        }

        /// <summary>
        /// The blend mode to be applied to the sprite.
        /// </summary>
        public BlendModes BlendMode
        {
            get
            {
                if (_colorFilter != null)
                {
                    return _colorFilter.BlendMode;
                }
                else
                {
                    return _blendMode;
                }
            }

            set
            {
                _blendMode = value;
                if (_colorFilter == null)
                {
                    CreateColorFilter();// TODO Default shader
                }
                if (_colorFilter != null)
                {
                    _colorFilter.BlendMode = value;
                }
            }
        }

        public IEnumerable<IRmmzFilter> Filters
        {
            get => _filters;
        }

        protected override void Awake()
        {
            base.Awake();
            _spriteId = s_counter++;
            Anchor = new Vector2(0f, 0f);
            rectTransform.sizeDelta = Vector2.zero;
        }

        protected override void OnDestroy()
        {
            foreach (var filter in _filters)
            {
                filter.Dispose();
            }
            _filters.Clear();
#region UniRmmz
            GameObject.Destroy(_maskMaterial);
#endregion
        }

        public virtual void UpdateRmmz()
        {
            foreach (var child in this.Children<IRmmzDrawable2d>())
            {
                child.UpdateRmmz();
            }
        }
        
        public virtual void Hide()
        {
            _hidden = true;
            UpdateVisibility();
        }
        
        public virtual void Show()
        {
            _hidden = false;
            UpdateVisibility();
        }

        protected virtual void UpdateVisibility()
        {
            Visible = !_hidden;
        }

        public virtual void Move(float x, float y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Sets the rectagle of the bitmap that the sprite displays.
        /// </summary>
        /// <param name="x">The x coordinate of the frame.</param>
        /// <param name="y">The y coordinate of the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        public void SetFrame(float x, float y, float width, float height)
        {
            _refreshFrame = false;
            if (
                x != _frame.x || y != _frame.y || width != _frame.width || height != _frame.height)
            {
                _frame.x = x;
                _frame.y = y;
                _frame.width = width;
                _frame.height = height;
                Refresh();
            }
        }

        /// <summary>
        /// Sets the hue rotation value.
        /// </summary>
        /// <param name="hue">The hue value (-360, 360).</param>
        public virtual void SetHue(int hue)
        {
            if (_hue != hue)
            {
                _hue = hue;
                UpdateColorFilter();
            }
        }

        /// <summary>
        /// Gets the blend color for the sprite.
        /// </summary>
        /// <returns>The blend color</returns>
        public UnityEngine.Color GetBendColor()
        {
            return _blendColor;
        }

        /// <summary>
        /// Sets the blend color for the sprite.
        /// </summary>
        /// <param name="color">The blend color [r, g, b, a].</param>
        public void SetBlendColor(UnityEngine.Color color)
        {
            if (_blendColor != color)
            {
                _blendColor = color;
                UpdateColorFilter();
            }
        }

        /// <summary>
        /// Gets the color tone for the sprite.
        /// </summary>
        /// <returns>The color tone [r, g, b, gray].</returns>
        public Vector4 GetColorTone()
        {
            return _colorTone;
        }

        /// <summary>
        /// Sets the color tone for the sprite.
        /// </summary>
        /// <param name="tone">The color tone [r, g, b, gray].</param>
        public void SetColorTone(Vector4 tone)
        {
            if (_colorTone != tone)
            {
                _colorTone = tone;
                UpdateColorFilter();
            }
        }

        private void OnBitmapChange()
        {
            if (_bitmap != null)
            {
                _refreshFrame = true;
                _bitmap.AddLoadListener(OnBitmapLoad);
            } 
            else
            {
                _frame = new Rect();
            }
        }
        
        private void OnBitmapLoad(Bitmap bitmapLoaded)
        {
            if (bitmapLoaded == _bitmap) 
            {
                if (_refreshFrame && this._bitmap != null)
                {
                    _refreshFrame = false;
                    _frame.width = _bitmap.Width;
                    _frame.height = _bitmap.Height;
                }
            }
            this.Refresh();
        }

        private void Refresh()
        {
            float frameX = Mathf.Floor(this._frame.x);
            float frameY = Mathf.Floor(this._frame.y);
            float frameW = Mathf.Floor(this._frame.width);
            float frameH = Mathf.Floor(this._frame.height);
            var baseTexture = _bitmap?.BaseTexture;
            float baseTextureW = baseTexture?.width ?? 0;
            float baseTextureH = baseTexture?.height ?? 0;
            float realX = Mathf.Clamp(frameX, 0f, baseTextureW);
            float realY = Mathf.Clamp(frameY, 0f, baseTextureH);
            float realW = Mathf.Clamp(frameW - realX + frameX, 0f, baseTextureW - realX);
            float realH = Mathf.Clamp(frameH - realY + frameY, 0f, baseTextureH - realY);
            var frame = new Rect(realX, realY, realW, realH);
            //if (texture)
            {
                //Pivot = new Vector2(frameX - realX, frameY - realY);
                rectTransform.sizeDelta = new Vector2(realW, realH);
                if (baseTexture)
                {
                    texture = baseTexture;
                    uvRect = new Rect(frame.x / baseTextureW, 1f - (frame.y / baseTextureH) - (frame.height / baseTextureH), frame.width / baseTextureW, frame.height / baseTextureH);
                }
            }
        }

        private void CreateColorFilter()
        {
            _colorFilter = new ColorFilter();
            AddFilter(_colorFilter);
        }

        private void UpdateColorFilter()
        {
            if (_colorFilter == null)
            {
                CreateColorFilter();
            }
            _colorFilter.SetHue(_hue);
            _colorFilter.SetBlendColor(_blendColor);
            _colorFilter.SetColorTone(_colorTone);
        }

        public void AddFilter(IRmmzFilter filter)
        {
            if (!_filters.Any())
            {
                _filterCanvas = RmmzFilterCanvas.Create(this, false);
            }
            _filters.Add(filter);
        }

        public virtual Rect FilterArea { get; set; }

#region UniRmmz
        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            if (_preventRecursive || !_filters.Any())
            {
                return;
            }

            try
            {
                _preventRecursive = true;
                if (_filterCanvas == null)
                {
                    _filterCanvas = RmmzFilterCanvas.Create(this, true);    
                }
                else if (transform.parent != null && _filterCanvas.transform != transform.parent.parent)
                {
                    RmmzFilterCanvas.RefreshParent(_filterCanvas.FilterContainer, _filterCanvas.HelperCanvas, _filterCanvas, true);
                }
                else if (transform.parent == null)
                {
                    RmmzFilterCanvas.Destroy(_filterCanvas);
                }
            }
            finally
            {
                _preventRecursive = false;
            }
        }

        private Transform _filterCanvasTransform;
        public virtual Transform FilterCanvasTransform
        {
            get => _filterCanvasTransform ?? transform;
            set => _filterCanvasTransform = value;
        }
 
        private Material _maskMaterial;
        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            _maskMaterial ??= new Material(Shader.Find("UI/Default"));
            _maskMaterial.CopyPropertiesFromMaterial(baseMaterial);
            _maskMaterial.SetInt("_Stencil", 0);
            _maskMaterial.SetInt("_StencilComp", (int)CompareFunction.Equal);
            _maskMaterial.SetInt("_StencilOp", (int)StencilOp.Keep);
            _maskMaterial.SetInt("_StencilReadMask", 255);
            _maskMaterial.SetInt("_StencilWriteMask", 255);
            
            return _maskMaterial;
        }
        
        public static T _Create<T>(string name = "") where T : Sprite
        {
            var obj = new GameObject(name);
            obj.layer = Rmmz.RmmzDontRenderLayer;
            return obj.AddComponent<T>();
        }
#endregion
    }
}