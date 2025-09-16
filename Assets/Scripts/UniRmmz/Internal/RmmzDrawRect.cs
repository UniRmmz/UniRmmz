using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class RmmzDrawRect : Image
    {
        private Color[] _colors = new Color[4];
        private Rect _uvRect = new Rect(0, 0, 1, 1);
        private Texture _texture;

        public void Dispose()
        {
            if (material != null)
            {
                Destroy(material);
            }
        }

        public void Clear()
        {
            SetColor(Color.white);
            SetTexture(null, Rect.zero);
        }

        public void SetTexture(Texture texture, Rect uvRect)
        {
            material.mainTexture = texture;
            _texture = texture;
            _uvRect = uvRect;
            
            SetAllDirty();
        }
        
        public void SetRect(Rect rect)
        {
            rectTransform.anchoredPosition = new Vector2(rect.x, -rect.y);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
        }
        
        public void SetColor(Color color)
        {
            SetColor(color, color, color, color);
        }

        public void SetColor(Color color1, Color color2, Color color3, Color color4)
        {
            _colors[0] = color1;
            _colors[1] = color2;
            _colors[2] = color3;
            _colors[3] = color4;
            
            SetVerticesDirty();
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect rect = rectTransform.rect;

            Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);
            Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);
            Vector2 topRight = new Vector2(rect.xMax, rect.yMax);
            Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);

            var tmp = new Rect(0, 0, 1, 1);
            if (_texture != null)
            {
                tmp.xMin = _uvRect.xMin / _texture.width;
                tmp.xMax = _uvRect.xMax / _texture.width;
                tmp.yMin = 1f - _uvRect.yMin / _texture.height;
                tmp.yMax = 1f - _uvRect.yMax / _texture.height;
            }
            Vector2 uvBottomLeft = new Vector2(tmp.xMin, tmp.yMax);
            Vector2 uvTopLeft = new Vector2(tmp.xMin, tmp.yMin);
            Vector2 uvTopRight = new Vector2(tmp.xMax, tmp.yMin);
            Vector2 uvBottomRight = new Vector2(tmp.xMax, tmp.yMax);

            vh.AddVert(bottomLeft, _colors[0], uvBottomLeft);
            vh.AddVert(topLeft, _colors[1], uvTopLeft);
            vh.AddVert(topRight, _colors[2], uvTopRight);
            vh.AddVert(bottomRight, _colors[3], uvBottomRight);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(0, 2, 3);
        }
    }
}