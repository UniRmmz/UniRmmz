using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying the map name on the map screen.
    /// </summary>
    public partial class Window_MapName : Window_Base
    {
        protected int _showCount;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Opacity = 0;
            ContentsOpacity = 0;
            _showCount = 0;
            Refresh();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            if (_showCount > 0 && Rmmz.gameMap.IsNameDisplayEnabled())
            {
                UpdateFadeIn();
                _showCount--;
            }
            else
            {
                UpdateFadeOut();
            }
        }

        protected virtual void UpdateFadeIn()
        {
            ContentsOpacity += 16;
        }

        protected virtual void UpdateFadeOut()
        {
            ContentsOpacity -= 16;
        }

        public override void Open()
        {
            Refresh();
            _showCount = 150;
        }

        public override void Close()
        {
            _showCount = 0;
        }

        public virtual void Refresh()
        {
            Contents.Clear();
            string name = Rmmz.gameMap.DisplayName();
            if (!string.IsNullOrEmpty(name))
            {
                float width = InnerWidth;
                DrawBackgroundRect(0, 0, width, LineHeight());
                DrawText(name, 0, 0, (int)width, Bitmap.TextAlign.Center);
            }
        }

        protected virtual void DrawBackgroundRect(float x, float y, float width, float height)
        {
            Color color1 = Rmmz.ColorManager.DimColor1();
            Color color2 = Rmmz.ColorManager.DimColor2();
            float half = width / 2f;
            Contents.GradientFillRect(x, y, half, height, color2, color1, false);
            Contents.GradientFillRect(x + half, y, half, height, color1, color2, false);
        }
    }
}