using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    /// <summary>
    /// The sprite object for a tiling image.
    /// </summary>
    public partial class TilingSprite : RmmzContainer
    {
        private static Texture2D _emptyTexture;
        
        protected Rect _frame;
        protected Bitmap _bitmap;
        protected Texture _texture;

        public override Texture mainTexture => _texture;
        
        /// <summary>
        /// The origin point of the tiling sprite for scrolling.
        /// </summary>
        public override Vector2 Origin { get; set; } = Vector2.zero;
        
        /// <summary>
        /// The offset of the image that is being tiled
        /// </summary>
        public Vector2 TilePosition { get; set; } = Vector2.zero;

        protected override void Awake()
        {
            base.Awake();
            material = new Material(Shader.Find("UniRmmz/Tiling"));
        }

        protected override void OnDestroy()
        {
            GameObject.Destroy(material);
            material = null;
        }
        
        /// <summary>
        /// The image for the tiling sprite.
        /// </summary>
        public virtual Bitmap Bitmap
        {
            get => _bitmap;
            set
            {
                if (_bitmap != value)
                {
                    _bitmap = value;
                    OnBitmapChange();
                }
            }
        }
        
        /// <summary>
        /// Sets the x, y, width, and height all at once.
        /// </summary>
        /// <param name="x">The x coordinate of the tiling sprite.</param>
        /// <param name="y">The y coordinate of the tiling sprite.</param>
        /// <param name="width">The width of the tiling sprite.</param>
        /// <param name="height">The height of the tiling sprite.</param>
        public virtual void Move(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            
            SetVerticesDirty();
        }

        /// <summary>
        /// Specifies the region of the image that the tiling sprite will use.
        /// </summary>
        /// <param name="x">The x coordinate of the frame.</param>
        /// <param name="y">The y coordinate of the frame.</param>
        /// <param name="width">The width of the frame.</param>
        /// <param name="height">The height of the frame.</param>
        public virtual void SetFrame(float x, float y, float width, float height)
        {
            _frame.x = x;
            _frame.y = y;
            _frame.width = width;
            _frame.height = height;
            _Refresh();
        }
        
        /// <summary>
        /// Updates the transform on all children of this container for rendering.
        /// </summary>
        public void LateUpdate()
        {
            TilePosition = new Vector2(Mathf.Round(Origin.x), Mathf.Round(Origin.y));
            SetVerticesDirty();
        }

        protected virtual void OnBitmapChange()
        {
            if (_bitmap != null)
            {
                _bitmap.AddLoadListener(OnBitmapLoad);
            }
            else
            {
                _frame = new Rect();
                _Refresh();
            }
        }

        protected virtual void OnBitmapLoad(Bitmap _)
        {
            _texture = _bitmap.BaseTexture;
            _Refresh();
        }

        protected virtual void _Refresh()
        {
            if (_frame.width == 0 && _frame.height == 0 && _bitmap != null)
            {
                _frame.width = _bitmap.Width;
                _frame.height = _bitmap.Height;
            }
            if (_texture)
            {
                float x = _frame.x / _texture.width;
                float y = _frame.y / _texture.height;
                float w = _frame.width / _texture.width;
                float h = _frame.height / _texture.height;
                material.SetVector("_FrameRect", new Vector4(x, y, w, h));
                
                float ox = TilePosition.x / _frame.width;
                float oy = TilePosition.y / _frame.height;
                float sx = Width / _frame.width;
                float sy = Height / _frame.height;
                material.SetVector("_Tiling", new Vector4(ox, oy, sx, sy));
                
                SetMaterialDirty();
            }
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect rect = rectTransform.rect;

            Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);
            Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);
            Vector2 topRight = new Vector2(rect.xMax, rect.yMax);
            Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);

            vh.AddVert(bottomLeft, color, new Vector2(0, 1));
            vh.AddVert(topLeft, color, new Vector4(0, 0));
            vh.AddVert(topRight, color, new Vector4(1, 0));
            vh.AddVert(bottomRight, color, new Vector4(1, 1));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(0, 2, 3);
            
            // Rectに依存したマテリアルパラメータを更新            
            {
                float ox = TilePosition.x / _frame.width;
                float oy = TilePosition.y / _frame.height;
                float sx = Width / _frame.width;
                float sy = Height / _frame.height;
                material.SetVector("_Tiling", new Vector4(ox, oy, sx, sy));    
            }
        }
    }
}