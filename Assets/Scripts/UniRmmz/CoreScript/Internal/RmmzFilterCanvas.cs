using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UniRmmz
{
    public class RmmzFilterCanvas : RmmzContainer, IMaterialModifier
    {
        private Rect _screenRect;
        private Rect _uvRect;
        private Vector2 _oldLocalPosition;
        private Vector2 _oldSizeDelta;
        private Texture _texture;
        private GameObject _filterContainerObject;
        
        public override Texture mainTexture
        {
            get
            {
                return _texture;
            }
        }
        
        public Texture LastRenderedTexture { get; private set; }
        public IRmmzDrawable2d FilterContainer { get; private set; }
        public Canvas HelperCanvas { get; private set; }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameObject.Destroy(_maskMaterial);
        }

        public void Initialize(IRmmzDrawable2d filterContainer, Canvas helperCanvas)
        {
            FilterContainer = filterContainer;
            _filterContainerObject = (filterContainer as MonoBehaviour).gameObject;
            HelperCanvas = helperCanvas;
        }

        public static RmmzFilterCanvas Create(IRmmzDrawable2d filterContainer, bool worldPositionStays = true)
        {
            var mainDrawable = filterContainer as MonoBehaviour;
                
            var canvasObject = new GameObject(mainDrawable.gameObject.name + " canvas");
            canvasObject.layer = Rmmz.RmmzDontRenderLayer;
            canvasObject.AddComponent<RmmzContainer>();
            var helperCanvas = canvasObject.gameObject.AddComponent<Canvas>();
                
            var filterObject = new GameObject(mainDrawable.gameObject.name + " sprite");
            filterObject.layer = Rmmz.RmmzDontRenderLayer;
            var filterCanvas = filterObject.AddComponent<RmmzFilterCanvas>();
            
            filterCanvas.Initialize(filterContainer, helperCanvas);
            RefreshParent(filterContainer, helperCanvas, filterCanvas, worldPositionStays);
            return filterCanvas;
        }
        
        public static void RefreshParent(IRmmzDrawable2d filterContainer, Canvas helperCanvas, RmmzFilterCanvas filterCanvas, bool worldPositionStays = true)
        {
            filterContainer.FilterCanvasTransform = filterCanvas.transform;
            var mainDrawable = filterContainer as MonoBehaviour;
            var canvasObject = helperCanvas.gameObject;
            
            canvasObject.transform.SetParent(filterCanvas.transform);
                
            // 生成したオブジェクトを親との間に挿入する
            var parent = mainDrawable.transform.parent;
            int childIndex = mainDrawable.transform.GetSiblingIndex();

            filterCanvas.transform.SetParent(parent, worldPositionStays);
            filterCanvas.transform.SetSiblingIndex(childIndex);

            mainDrawable.transform.SetParent(canvasObject.transform);
        }
        
        public static void Destroy(RmmzFilterCanvas canvas)
        {
            if (canvas != null)
            {
                GameObject.Destroy(canvas.gameObject);    
            }
        }

        public void Update()
        {
            if (_filterContainerObject == null || HelperCanvas == null)
            {
                GameObject.Destroy(gameObject);
            }
        }
        
        public void BeginParentRender(RmmzCamera.RenderingPass pass)
        {
            foreach (var filter in FilterContainer.Filters)
            {
                filter.Bind(this);
            }
            _texture = pass.RenderTexture;
            int w = Graphics.Width;
            int h = Graphics.Height;
            _oldLocalPosition = rectTransform.localPosition;
            _oldSizeDelta = rectTransform.sizeDelta;
            var boundingBox = pass.BoundingBox;
            var rect = FilterContainer.FilterArea;
            if (FilterContainer.FilterArea == Rect.zero)
            {
                rect = new Rect(0, 0, w, h);
            }
            
            var tmp = new Rect(rect.x / w, (h - rect.height - rect.y) / h, rect.width / w, rect.height / h);
            //rectTransform.localPosition = new Vector2(boundingBox.x / w, boundingBox.y / h);
            //rectTransform.sizeDelta = new Vector2(boundingBox.width / w, boundingBox.height / h);
            //rectTransform.localPosition = new Vector2(0.3f, 0);
            rectTransform.sizeDelta = new Vector2(1, 1);
            //rectTransform.localPosition = new Vector2(rect.x, h - rect.height - rect.y);
            //rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
            //_screenRect = rectTransform.rect;
            _uvRect = tmp;
            _screenRect = new Rect(rect.x / w, (rect.y - h) / h, rect.width / w, rect.height / h);
            //rectTransform.localPosition = new Vector2(100, 0);
            //rectTransform.sizeDelta = new Vector2(1, 1);
            
            //_screenRect.x = 0.1f;
            //_screenRect.y = -0.9f;
            /*
            _screenRect.x = tmp.x;
            _screenRect.y = tmp.y - 1f;
            _screenRect.width = tmp.width;
            _screenRect.height = tmp.height;
            */
            
            LastRenderedTexture = _texture;
        }

        public void EndParentRender()
        {
            this.material = null;
            rectTransform.localPosition = _oldLocalPosition;
            rectTransform.sizeDelta = _oldSizeDelta;
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect rect = rectTransform.rect;

            Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);
            Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);
            Vector2 topRight = new Vector2(rect.xMax, rect.yMax);
            Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);


            Vector2 screenBottomLeft = new Vector2(_screenRect.xMin, _screenRect.yMin);
            Vector2 screenTopLeft = new Vector2(_screenRect.xMin, _screenRect.yMax);
            Vector2 screenTopRight = new Vector2(_screenRect.xMax, _screenRect.yMax);
            Vector2 screenBottomRight = new Vector2(_screenRect.xMax, _screenRect.yMin);

            Vector2 uvBottomLeft = new Vector2(_uvRect.xMin, _uvRect.yMax);
            Vector2 uvTopLeft = new Vector2(_uvRect.xMin, _uvRect.yMin);
            Vector2 uvTopRight = new Vector2(_uvRect.xMax, _uvRect.yMin);
            Vector2 uvBottomRight = new Vector2(_uvRect.xMax, _uvRect.yMax);

            vh.AddVert(bottomLeft, Color.white, new Vector4(screenBottomLeft.x, screenBottomLeft.y, uvBottomLeft.x, uvBottomLeft.y));
            vh.AddVert(topLeft, Color.white, new Vector4(screenTopLeft.x, screenTopLeft.y, uvTopLeft.x, uvTopLeft.y));
            vh.AddVert(topRight, Color.white, new Vector4(screenTopRight.x, screenTopRight.y, uvTopRight.x, uvTopRight.y));
            vh.AddVert(bottomRight, Color.white, new Vector4(screenBottomRight.x, screenBottomRight.y, uvBottomRight.x, uvBottomRight.y));

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(0, 2, 3);
        }

        public override IEnumerable<IRmmzFilter> Filters => FilterContainer.Filters;

        public override Rect FilterArea
        {
            get => FilterContainer.FilterArea;
            set
            {
                FilterContainer.FilterArea = value;
            }
            
        }
        
        private Material _maskMaterial;
        public Material GetModifiedMaterial(Material baseMaterial)
        {
            Material tmp = null;
            if (!Filters.Any())
            {
                _maskMaterial ??= new Material(Shader.Find("UI/Default"));
                tmp = _maskMaterial;
                tmp.CopyPropertiesFromMaterial(baseMaterial);
            }
            else
            {
                tmp = baseMaterial;
            }
            tmp.SetInt("_Stencil", 0);
            tmp.SetInt("_StencilComp", (int)CompareFunction.Equal);
            tmp.SetInt("_StencilOp", (int)StencilOp.Keep);
            tmp.SetInt("_StencilReadMask", 255);
            tmp.SetInt("_StencilWriteMask", 255);
            
            return tmp;
        }
    }
}