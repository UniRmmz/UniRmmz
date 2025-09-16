using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    /// <summary>
    /// The sprite which covers the entire game screen.
    /// </summary>
    public partial class ScreenSprite : RmmzContainer
    {
        protected byte _red;
        protected byte _green;
        protected byte _blue;
        
        protected override void Awake()
        {
            base.Awake();
            Opacity = 0;
            SetBlack();
        }
        
        /// <summary>
        /// Sets black to the color of the screen sprite.
        /// </summary>
        public void SetBlack()
        {
            SetColor(0, 0, 0);
        }
        
        /// <summary>
        /// Sets white to the color of the screen sprite.
        /// </summary>
        public void SetWhite()
        {
            SetColor(255, 255, 255);
        }

        /// <summary>
        /// Sets the color of the screen sprite by values.
        /// </summary>
        /// <param name="r">The red value in the range (0, 255).</param>
        /// <param name="g">The green value in the range (0, 255).</param>
        /// <param name="b">The blue value in the range (0, 255).</param>
        public void SetColor(byte r, byte g, byte b)
        {
            if (_red != r || _blue != g || _red != b)
            {
                _red = r;
                _green = g;
                _blue = b;
                SetVerticesDirty();
            }
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect rect = rectTransform.rect;

            Vector2 bottomLeft = new Vector2(-50000, -50000);
            Vector2 topLeft = new Vector2(-50000, 50000);
            Vector2 topRight = new Vector2(50000, 50000);
            Vector2 bottomRight = new Vector2(50000, -50000);

            var color = new Color32(_red, _green, _blue, (byte)Opacity);
            vh.AddVert(bottomLeft, color, Vector4.zero);
            vh.AddVert(topLeft, color, Vector4.zero);
            vh.AddVert(topRight, color, Vector4.zero);
            vh.AddVert(bottomRight, color, Vector4.zero);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(0, 2, 3);
        }
    }
}