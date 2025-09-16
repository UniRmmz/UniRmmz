using UnityEngine;

namespace UniRmmz
{
    public static class RectExtensions
    {
        public static void Pad(this ref Rect rect, float x, float y)
        {
            rect.x -= x;
            rect.y -= y;
            rect.width += x * 2;
            rect.height += y * 2;
        }

        public static bool Contains(this Rect rect, float x, float y)
        {
            return rect.Contains(new Vector2(x, y));
        }
        
        public static void Enlarge(this ref Rect a, Rect b)
        {
            float minX = Mathf.Min(a.xMin, b.xMin);
            float minY = Mathf.Min(a.yMin, b.yMin);
            float maxX = Mathf.Max(a.xMax, b.xMax);
            float maxY = Mathf.Max(a.yMax, b.yMax);
    
            a.x = minX;
            a.y = minY;
            a.width = maxX - minX;
            a.height = maxY - minY;
        }
    }
}