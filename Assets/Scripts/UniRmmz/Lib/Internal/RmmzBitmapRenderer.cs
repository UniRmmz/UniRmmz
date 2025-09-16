using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UniRmmz
{
    public class RmmzBitmapRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private RmmzDrawRect _drawRect;
        [SerializeField] private RmmzDrawCircle _drawCircle;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Camera _camera;

        private RmmzTextRenderer _textRenderer;

        protected void Awake()
        {
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.worldCamera = _camera;
            _textRenderer = new RmmzTextRenderer(_camera, _text);
        }

        public void DrawText(string content, int x, int y, int maxWidth, int lineHeight, Bitmap.TextAlign align, RmmzTextRenderer.Context context, RenderTexture texture)
        {
            _textRenderer.DrawText(content, x, y, maxWidth, lineHeight, align, context, texture);
        }

        public int MeasureTextWidth(string content, RmmzTextRenderer.Context context)
        {
            return _textRenderer.MeasureTextWidth(content, context);
        }
        
        public void FillRect(Rect rect, Color color, RenderTexture texture)
        {
            _drawRect.Clear();
            _drawRect.SetRect(rect);
            _drawRect.SetColor(color);
            _drawRect.SetVerticesDirty();
            _drawRect.gameObject.SetActive(true);
            
            if (texture != null)
            {
                _camera.rect = new Rect(0, 0, 1, 1);
                _camera.clearFlags = CameraClearFlags.Nothing;
                _camera.cullingMask = 1 << Rmmz.RmmzOffscreenLayer;
                _camera.targetTexture = texture;
                _camera.Render();
                _camera.targetTexture = null;
            }
            
            _drawRect.gameObject.SetActive(false);
        }
        
        public void Blt(Texture source, Rect sourceRect, Rect destRect, float alpha, RenderTexture texture)
        {
            _drawRect.Clear();
            _drawRect.SetRect(destRect);
            _drawRect.SetTexture(source, sourceRect);
            _drawRect.SetColor(new Color(1, 1, 1, alpha));
            _drawRect.SetVerticesDirty();
            _drawRect.gameObject.SetActive(true);
            
            if (texture != null)
            {
                _camera.rect = new Rect(0, 0, 1, 1);
                _camera.clearFlags = CameraClearFlags.Nothing;
                _camera.cullingMask = 1 << Rmmz.RmmzOffscreenLayer;
                _camera.targetTexture = texture;
                _camera.Render();
                _camera.targetTexture = null;
            }
            
            _drawRect.gameObject.SetActive(false);
        }
        
        public void GradientFillRect(Rect rect, Color color1, Color color2, bool vertical, RenderTexture texture)
        {
            _drawRect.Clear();
            _drawRect.SetRect(rect);
            if (vertical)
            {
                _drawRect.SetColor(color2, color1, color1, color2);
            }
            else
            {
                _drawRect.SetColor(color1, color1, color2, color2);    
            }
            _drawRect.SetVerticesDirty();
            _drawRect.gameObject.SetActive(true);
            
            if (texture != null)
            {
                _camera.rect = new Rect(0, 0, 1, 1);
                _camera.clearFlags = CameraClearFlags.Nothing;
                _camera.cullingMask = 1 << Rmmz.RmmzOffscreenLayer;
                _camera.targetTexture = texture;
                _camera.Render();
                _camera.targetTexture = null;
            }
            
            _drawRect.gameObject.SetActive(false);
        }
        
        public void ClearRect(Rect rect, Color color, RenderTexture texture)
        {
            _drawRect.Clear();
            if (texture != null)
            {
                _camera.rect = new Rect(rect.x / texture.width, 1f - (rect.y / texture.height) - (rect.height / texture.height), rect.width / texture.width, rect.height / texture.height);
                _camera.clearFlags = CameraClearFlags.Color;
                _camera.cullingMask = 1 << Rmmz.RmmzOffscreenLayer;
                _camera.backgroundColor = color;
                _camera.targetTexture = texture;
                _camera.Render();
                _camera.targetTexture = null;
                _camera.clearFlags = CameraClearFlags.Nothing;
            }
        }

        public void DrawCircle(int x, int y, int radius, Color color, RenderTexture texture)
        {
            _drawCircle.SetRect(new Rect(x + 0.5f, y + 0.5f, texture.width, texture.height));
            _drawCircle.SetCircle(radius, color);
            _drawCircle.SetVerticesDirty();
            _drawCircle.gameObject.SetActive(true);
            
            if (texture != null)
            {
                _camera.rect = new Rect(0, 0, 1, 1);
                _camera.clearFlags = CameraClearFlags.Nothing;
                _camera.cullingMask = 1 << Rmmz.RmmzOffscreenLayer;
                _camera.targetTexture = texture;
                _camera.Render();
                _camera.targetTexture = null;
            }
            
            _drawCircle.gameObject.SetActive(false);
        }
    }
}