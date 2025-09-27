using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying the timer.
    /// </summary>
    public partial class Sprite_Timer : Sprite
    {
        private int _seconds;

        protected override void Awake()
        {
            base.Awake();
            _seconds = 0;
            CreateBitmap();
            UpdateRmmz();
        }

        protected override void OnDestroy()
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
            }
        }

        protected virtual void CreateBitmap()
        {
            Bitmap = new Bitmap(96, 48);
            Bitmap.FontFace = FontFace();
            Bitmap.FontSize = FontSize();
            Bitmap.OutlineColor = Rmmz.ColorManager.OutlineColor();
        }

        protected virtual string FontFace()
        {
            return Rmmz.gameSystem.NumberFontFace();
        }
        
        protected virtual int FontSize()
        {
            return Rmmz.gameSystem.MainFontSize() + 8;
        }
        
        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateBitmap();
            UpdatePosition();
            UpdateVisibility();
        }

        protected virtual void UpdateBitmap()
        {
            if (_seconds != Rmmz.gameTimer.Seconds())
            {
                _seconds = Rmmz.gameTimer.Seconds();
                Redraw();
            }
        }

        protected virtual void Redraw()
        {
            var text = TimerText();
            var width = Bitmap.Width;
            var height = Bitmap.Height;
            Bitmap.Clear();
            Bitmap.DrawText(text, 0, 0, width, height, Bitmap.TextAlign.Center);
        }

        protected virtual string TimerText()
        {
            var min = Mathf.FloorToInt(_seconds / 60f) % 60;
            var sec = _seconds % 60;
            return min.ToString("00") + ":" + sec.ToString("00");
        }

        protected virtual void UpdatePosition()
        {
            X = (Graphics.Width - Bitmap.Width) / 2;
            Y = 0;
        }

        protected override void UpdateVisibility()
        {
            Visible = Rmmz.gameTimer.IsWorking();
        }
    }
}