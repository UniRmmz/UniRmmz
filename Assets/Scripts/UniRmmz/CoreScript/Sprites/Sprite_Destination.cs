using UnityEngine;
using UnityEngine.Rendering;


namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying the destination place of the touch input.
    /// </summary>
    public partial class Sprite_Destination //: Sprite
    {
        protected int _frameCount;
        
        protected override void Awake()
        {
            base.Awake();
            CreateBitmap();
            _frameCount = 0;
        }

        protected override void OnDestroy()
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
            }
            base.OnDestroy();
        }
        
        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            if (Rmmz.gameTemp.IsDestinationValid())
            {
                UpdatePosition();
                UpdateAnimation();
                Visible = true;
            }
            else
            {
                _frameCount = 0;
                Visible = false;
            }
        }

        protected virtual void CreateBitmap()
        {
            int tileWidth = Rmmz.gameMap.TileWidth();
            int tileHeight = Rmmz.gameMap.TileHeight();
            Bitmap = new Bitmap(tileWidth, tileHeight);
            Bitmap.FillAll(UnityEngine.Color.white);
            Anchor = new Vector2(0.5f, 0.5f);
            BlendMode = BlendModes.Add;
        }

        protected virtual void UpdatePosition()
        {
            int tileWidth = Rmmz.gameMap.TileWidth();
            int tileHeight = Rmmz.gameMap.TileHeight();
            int x = Rmmz.gameTemp.DestinationX();
            int y = Rmmz.gameTemp.DestinationY();
            X = (Rmmz.gameMap.AdjustX(x) + 0.5f) * tileWidth; 
            Y = (Rmmz.gameMap.AdjustY(y) + 0.5f) * tileHeight;
        }

        protected virtual void UpdateAnimation()
        {
            _frameCount++;
            _frameCount %= 20;
            Opacity = (20 - _frameCount) * 6;
            float s = 1 + (float)_frameCount / 20;
            Scale = new Vector2(s, s);
        }
    }
}