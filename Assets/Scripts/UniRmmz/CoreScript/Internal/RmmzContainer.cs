using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    
    [RequireComponent(typeof(CanvasRenderer))]
    public class RmmzContainer : UnityEngine.UI.Graphic, IRmmzDrawable2d
    {
        private RmmzFilterCanvas _filterCanvas;
        private bool _preventRecursive;
        
        protected override void Awake()
        {
            base.Awake();
            Anchor = new Vector2(0f, 0f);
            raycastTarget = false;
        }
        
        /// <summary>
        /// The width of the window in pixels.
        /// </summary>
        public virtual float Width
        {
            get => rectTransform.sizeDelta.x;
            set
            {
                var size = rectTransform.sizeDelta;
                size.x = value;
                rectTransform.sizeDelta = size;
            }
        }
        
        /// <summary>
        /// The height of the window in pixels.
        /// </summary>
        public virtual float Height
        {
            get => rectTransform.sizeDelta.y;
            set
            {
                var size = rectTransform.sizeDelta;
                size.y = value;
                rectTransform.sizeDelta = size;
            }
        }
            
        /// <summary>
        /// The opacity of the window without contents (0 to 255).
        /// </summary>
        public virtual int Opacity
        {
            get => (int)(Alpha * 255);
            set => Alpha = (float)Mathf.Clamp(value, 0, 255) / 255;
        }
            
        public virtual bool Visible
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }
        
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
                var point = rectTransform.localPosition;
                rectTransform.localPosition = new Vector2(value, point.y);
            }
        }
        
        public virtual float Y
        {
            get => -rectTransform.localPosition.y;
            set
            {
                var point = rectTransform.localPosition;
                rectTransform.localPosition = new Vector2(point.x, -value);
            }
        }
        
        public virtual float Z { get; set; }

        public int SpriteId => 0;
        
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
        
        public virtual Vector2 Scale
        {
            get => rectTransform.localScale;
            set => rectTransform.localScale = value;
        }
            
        public virtual float Alpha
        {
            get => color.a;
            set
            {
                var tmp = color;
                tmp.a = value;
                color = tmp;
            }
        }
        
        public virtual void UpdateRmmz()
        {
            foreach (var child in transform.GetChildren<UnityEngine.UI.Graphic>())
            {
                if (child is IRmmzDrawable2d drawable)
                {
                    drawable.UpdateRmmz();    
                }
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        private List<IRmmzFilter> _filters = new();
        public virtual IEnumerable<IRmmzFilter> Filters => _filters;
        
        public void AddFilter(IRmmzFilter filter)
        {
            if (!_filters.Any())
            {
                if (transform.parent != null)
                {
                    _filterCanvas = RmmzFilterCanvas.Create(this, true);    
                }
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
        
        public static T _Create<T>(string name = "") where T : RmmzContainer
        {
            var obj = new GameObject(name);
            obj.layer = Rmmz.RmmzDontRenderLayer;
            return obj.AddComponent<T>();
        }
        
        #endregion
    }
}