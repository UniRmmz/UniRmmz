using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The weather effect which displays rain, storm, or snow.
    /// </summary>
    public partial class Weather : RmmzContainer
    {
        private List<Sprite> _sprites = new List<Sprite>();
        private ScreenSprite _dimmerSprite;
        
        private Bitmap _rainBitmap;
        private Bitmap _stormBitmap;
        private Bitmap _snowBitmap;
        
        private WeatherTypes _type;
        private float _power = 0;

        public enum WeatherTypes
        {
            None,
            Rain,
            Storm,
            Snow
        }
        
        /// <summary>
        /// The type of the weather in ["none", "rain", "storm", "snow"].
        /// </summary>
        public WeatherTypes Type
        {
            get => _type;
            set => _type = value;
        }
        
        /// <summary>
        /// The power of the weather in the range (0, 9).
        /// </summary>
        public float Power
        {
            get => _power;
            set => _power = Mathf.Clamp(value, 0f, 9f);
        }
        
        public override Vector2 Origin { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            Width = Graphics.Width;
            Height = Graphics.Height;
            _sprites = new List<Sprite>();
            
            CreateBitmaps();
            CreateDimmer();
            
            _type = WeatherTypes.None;
            _power = 0;
            Origin = Vector2.zero;
        }
        
        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateDimmer();
            UpdateAllSprites();
        }
        
        protected override void OnDestroy()
        {
            _rainBitmap?.Dispose();
            _stormBitmap?.Dispose();
            _snowBitmap?.Dispose();
            base.OnDestroy();
        }
        
        protected void CreateBitmaps()
        {
            _rainBitmap = new Bitmap(1, 60);
            _rainBitmap.FillAll(Color.white);
            _stormBitmap = new Bitmap(2, 100);
            _stormBitmap.FillAll(Color.white);
            _snowBitmap = new Bitmap(9, 9);
            _snowBitmap.DrawCircle(4, 4, 4, Color.white);
        }
        protected void CreateDimmer()
        {
            _dimmerSprite = ScreenSprite.Create("dimer");
            _dimmerSprite.SetColor(80, 80, 80);
            this.AddChild(_dimmerSprite);
        }
        
        protected void UpdateDimmer()
        {
            _dimmerSprite.Opacity = Mathf.FloorToInt(_power * 6);
        }
        
        protected void UpdateAllSprites()
        {
            int maxSprites = Mathf.FloorToInt(_power * 10);
            while (_sprites.Count < maxSprites)
            {
                AddSprite();
            }
            while (_sprites.Count > maxSprites)
            {
                RemoveSprite();
            }
            foreach (var sprite in _sprites)
            {
                if (sprite != null)
                {
                    UpdateSprite(sprite);
                    sprite.X = sprite.Ax - Origin.x;
                    sprite.Y = sprite.Ay - Origin.y;
                }
            }
        }
        
        protected void AddSprite()
        {
            var sprite = Sprite.Create(/*viewport*/);
            sprite.Opacity = 0;
            _sprites.Add(sprite);
            this.AddChild(sprite);
        }
        
        protected void RemoveSprite()
        {
            if (_sprites.Count > 0)
            {
                var sprite = _sprites[_sprites.Count - 1];
                _sprites.RemoveAt(_sprites.Count - 1);
                GameObject.Destroy(sprite);
            }
        }
        
        protected void UpdateSprite(Sprite sprite)
        {
            switch (_type)
            {
                case WeatherTypes.Rain:
                    UpdateRainSprite(sprite);
                    break;
                case WeatherTypes.Storm:
                    UpdateStormSprite(sprite);
                    break;
                case WeatherTypes.Snow:
                    UpdateSnowSprite(sprite);
                    break;
            }
            
            if (sprite.Opacity < 40)
            {
                RebornSprite(sprite);
            }
        }
        
        protected void UpdateRainSprite(Sprite sprite)
        {
            sprite.Bitmap = _rainBitmap;
            sprite.Rotation = Mathf.PI / 16f;
            sprite.Ax -= 6 * Mathf.Sin(sprite.Rotation);
            sprite.Ay += 6 * Mathf.Cos(sprite.Rotation);
            sprite.Opacity -= 6;
        }
        
        private void UpdateStormSprite(Sprite sprite)
        {
            sprite.Bitmap = _stormBitmap;
            sprite.Rotation = Mathf.PI / 8f;
            sprite.Ax -= 8 * Mathf.Sin(sprite.Rotation);
            sprite.Ay += 8 * Mathf.Cos(sprite.Rotation);
            sprite.Opacity -= 8;
        }
        
        private void UpdateSnowSprite(Sprite sprite)
        {
            sprite.Bitmap = _snowBitmap;
            sprite.Rotation = Mathf.PI / 16f;
            sprite.Ax -= 3 * Mathf.Sin(sprite.Rotation);
            sprite.Ay += 3 * Mathf.Cos(sprite.Rotation);
            sprite.Opacity -= 3;
        }
        
        private void RebornSprite(Sprite sprite)
        {
            sprite.Ax = RmmzMath.RandomInt((int)Width + 100) - 100 + Origin.x;
            sprite.Ay = RmmzMath.RandomInt((int)Height + 200) - 200 + Origin.y;
            sprite.Opacity = 160 + RmmzMath.RandomInt(60);
        }
    }
    
}