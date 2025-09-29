using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying a battler name.
    /// </summary>
    public partial class Sprite_Name //: Sprite
    {
        protected Game_Actor _battler;
        protected string _name;
        protected Color _textColor;

        protected override void Awake()
        {
            base.Awake();
            InitMembers();
            CreateBitmap();
        }

        protected virtual void InitMembers()
        {
            _battler = null;
            _name = "";
            _textColor = Color.white;
        }

        protected override void OnDestroy()
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
            }
            base.OnDestroy();
        }

        protected virtual void CreateBitmap()
        {
            int width = BitmapWidth();
            int height = BitmapHeight();
            Bitmap = new Bitmap(width, height);
        }

        protected virtual int BitmapWidth()
        {
            return 128;
        }

        protected virtual int BitmapHeight()
        {
            return 24;
        }

        protected virtual string FontFace()
        {
            return Rmmz.gameSystem.MainFontFace();
        }

        protected virtual int FontSize()
        {
            return Rmmz.gameSystem.MainFontSize();
        }

        public virtual void Setup(Game_Actor battler)
        {
            _battler = battler;
            UpdateBitmap();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateBitmap();
        }

        protected virtual void UpdateBitmap()
        {
            string name = Name();
            Color color = TextColor();
            
            if (name != _name || color != _textColor)
            {
                _name = name;
                _textColor = color;
                Redraw();
            }
        }

        protected virtual string Name()
        {
            return _battler?.Name() ?? "";
        }

        protected virtual Color TextColor()
        {
            return Rmmz.ColorManager.HpColor(_battler);
        }

        protected virtual Color OutlineColor()
        {
            return Rmmz.ColorManager.OutlineColor();
        }

        protected virtual int OutlineWidth()
        {
            return 3;
        }

        protected virtual void Redraw()
        {
            string name = Name();
            int width = BitmapWidth();
            int height = BitmapHeight();
            
            SetupFont();
            Bitmap.Clear();
            Bitmap.DrawText(name, 0, 0, width, height, Bitmap.TextAlign.Left);
        }

        protected virtual void SetupFont()
        {
            Bitmap.FontFace = FontFace();
            Bitmap.FontSize = FontSize();
            Bitmap.TextColor = TextColor();
            Bitmap.OutlineColor = OutlineColor();
            Bitmap.OutlineWidth = OutlineWidth();
        }

    }
}