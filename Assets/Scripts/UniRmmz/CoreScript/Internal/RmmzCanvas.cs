using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    public class RmmzCanvas : MonoBehaviour
    {
        [SerializeField] public Canvas sceneCanvas;
        [SerializeField] public Camera sceneCamera = null;
        [SerializeField] public Canvas renderCanvas;
        [SerializeField] private RawImage renderImage = null;
        
        private RenderTexture _sceneRenderTarget;
        private CanvasScaler _sceneCanvasScaler;
        private CanvasScaler _renderCanvasScaler;
        private BlurFilter _blurFilter;
        
        public void Awake()
        {
            _sceneCanvasScaler = sceneCanvas.GetComponent<CanvasScaler>();
            _renderCanvasScaler = renderCanvas.GetComponent<CanvasScaler>();

            _sceneRenderTarget = new RenderTexture(GetRenderTextureDescriptor(1, 1));
            _sceneRenderTarget.Create();
            
            sceneCanvas.worldCamera = sceneCamera;
            _blurFilter = new BlurFilter();
        }

        public void OnDestroy()
        {
            _blurFilter.Dispose();
            if (_sceneRenderTarget != null)
            {
                _sceneRenderTarget.Release();
                GameObject.Destroy(_sceneRenderTarget);
            }
        }


        public void UpdateCanvas(int width, int height)
        {
            Debug.Log($"Update Canvas {width}x{height}");
            ResizeRenderTexture(sceneCamera, renderImage, _sceneRenderTarget, GetRenderTextureDescriptor(width, height));
            
            _renderCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _renderCanvasScaler.referenceResolution = new Vector2(width, height);
            _renderCanvasScaler.matchWidthOrHeight = 1;// match Height 
        }

        public Vector2 UnityScreenPointToRmmzScreenPoint(Vector2 screenPoint)
        {
            Vector2 mousePos = UnityEngine.Input.mousePosition;
            var rectTransform = renderImage.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, null, out var tmpPoint))
            {
                var rect = rectTransform.rect;
                
                // RawImage 内の UV 座標 (0~1)
                float uvX = (tmpPoint.x - rect.x) / rect.width;
                float uvY = (tmpPoint.y - rect.y) / rect.height;

                // Y反転 (UnityとツクールMZの違い)
                uvY = 1f - uvY;

                // RenderTexture内のピクセル座標
                float x = uvX * sceneCamera.targetTexture.width;
                float y = uvY * sceneCamera.targetTexture.height;

                return new Vector2(x, y);
            }

            return Vector2.zero;
        }

        public void Snap(RenderTexture output)
        {
            // 最後の描画結果そのまま使うだけなら、これが効率的。ただしウィンドウとか映りこんだりする
            // UnityEngine.Graphics.Blit(_sceneRenderTarget, output);
            sceneCamera.targetTexture = output;
            RmmzRoot.Instance.Camera.Render();
            sceneCamera.targetTexture = _sceneRenderTarget;
        }

        public bool RectangleContainsRmmzScreenPoint(RectTransform rectTransform, float x, float y)
        {
            var pos = new Vector2(x, Graphics.Height - y);
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, pos, RmmzRoot.Instance.Canvas.sceneCamera);    
        }
        
        public bool RmmzScreenPointToLocalPointInRectangle(RectTransform rectTransform, float x, float y, out Vector2 localPoint)
        {
            var pos = new Vector2(x, Graphics.Height - y);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, pos,
                    RmmzRoot.Instance.Canvas.sceneCamera, out var tmp))
            {
                localPoint = new Vector2(tmp.x, -tmp.y);
                return true;
            }
            else
            {
                localPoint = Vector2.zero;
                return false;
            }
        }

        public void RmmzLocalPointInRectangleToScreenPoint(RectTransform rectTransform, float x, float y, out Vector2 screenPoint)
        {
            var adjustedLocalPoint = new Vector2(x, -y);
            Vector3 worldPoint = rectTransform.TransformPoint(adjustedLocalPoint);
            Vector3 screenPos = RmmzRoot.Instance.Canvas.sceneCamera.WorldToScreenPoint(worldPoint);
            screenPoint = new Vector2(screenPos.x, Graphics.Height - screenPos.y);
        }

        public void Show()
        {
            sceneCanvas.gameObject.SetActive(true);
            renderImage.gameObject.SetActive(true);
        }

        public void Hide()
        {
            sceneCanvas.gameObject.SetActive(false);
            renderImage.gameObject.SetActive(false);
        }

        public void SetEnableBlurFilter(bool enable)
        {
            if (enable)
            {
                _blurFilter.Bind(renderImage);
                _blurFilter.SetBlendColor(new Color32(0, 0, 0, 127));
            }
            else
            {
                renderImage.material = null;
            }
        }
            
        private static RenderTextureDescriptor GetRenderTextureDescriptor(int width, int height)
        {
            var desc = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32);
            desc.depthBufferBits = 24;
            return desc;
        }
        
        /// <summary>
        /// CameraとRawImageに参照されているRenderTextureを正しくリサイズする
        /// </summary>
        private static void ResizeRenderTexture(Camera camera, RawImage image, RenderTexture renderTexture, RenderTextureDescriptor newDesc)
        {
            // 一度参照を外さないと、正しく反映されない
            camera.targetTexture = null;
            image.texture = null;
            
            // RenderTextureを作り直す
            renderTexture.Release();
            renderTexture.descriptor = newDesc; 
            renderTexture.Create();
            
            // 変更後のサイズ反映
            image.rectTransform.sizeDelta = new Vector2(renderTexture.width, renderTexture.height);
            
            // 参照を付けなおす
            camera.targetTexture = renderTexture;
            image.texture = renderTexture;
        }
    }
}