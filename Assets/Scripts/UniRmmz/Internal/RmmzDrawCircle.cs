using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(RectTransform))]
    public class RmmzDrawCircle : Image
    {
        private Color _color;
        private int _radius;
        private const int Division = 12;

        public void SetCircle(int radius, Color color)
        {
            _radius = radius;
            _color = color;
        }
        
        public void SetRect(Rect rect)
        {
            rectTransform.anchoredPosition = new Vector2(rect.x, -rect.y);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            vh.AddVert(new Vector3(0, 0, 0), _color, Vector4.zero);
            
            float d = 2 * Mathf.PI / Division;
            for (int i = 0; i <= Division; ++i)
            {
                float x = Mathf.Cos(d * i) * _radius;
                float y = Mathf.Sin(d * i) * _radius;
                vh.AddVert(new Vector3(x, y, 0), _color, Vector4.zero);
            }

            for (int i = 1; i < Division; ++i)
            {
                vh.AddTriangle(0, i, i + 1);    
            }
            vh.AddTriangle(0, Division, 1);
        }
    }
}