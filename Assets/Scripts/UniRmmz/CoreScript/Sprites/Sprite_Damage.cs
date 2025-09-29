using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying a popup damage.
    /// </summary>
    public partial class Sprite_Damage //: Sprite
    {
        protected int _duration;
        protected Color _flashColor;
        protected int _flashDuration;
        protected int _colorType;
        protected List<Sprite> _childSprites;

        protected override void Awake()
        {
            base.Awake();
            _duration = 90;
            _flashColor = new Color(0, 0, 0, 0);
            _flashDuration = 0;
            _colorType = 0;
            _childSprites = new List<Sprite>();
        }

        protected override void OnDestroy()
        {
            // 子スプライトのビットマップを破棄
            foreach (var child in _childSprites)
            {
                if (child != null && child.Bitmap != null)
                {
                    child.Bitmap.Dispose();
                }
                if (child != null && child.gameObject != null)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            _childSprites.Clear();
            base.OnDestroy();
        }

        public virtual void Setup(Game_Battler target)
        {
            var result = target.Result();
            
            if (result.missed || result.evaded)
            {
                _colorType = 0;
                CreateMiss();
            }
            else if (result.hpAffected)
            {
                _colorType = result.hpDamage >= 0 ? 0 : 1;
                CreateDigits(result.hpDamage);
            }
            else if (target.IsAlive() && result.mpDamage != 0)
            {
                _colorType = result.mpDamage >= 0 ? 2 : 3;
                CreateDigits(result.mpDamage);
            }
            
            if (result.critical)
            {
                SetupCriticalEffect();
            }
        }

        protected virtual void SetupCriticalEffect()
        {
            _flashColor = new Color(1f, 0f, 0f, 160f / 255f);
            _flashDuration = 60;
        }

        protected virtual string FontFace()
        {
            return Rmmz.gameSystem.NumberFontFace();
        }

        protected virtual int FontSize()
        {
            return Rmmz.gameSystem.MainFontSize() + 4;
        }

        protected virtual Color DamageColor()
        {
            return Rmmz.ColorManager.DamageColor(_colorType);
        }

        protected virtual Color OutlineColor()
        {
            return new Color(0, 0, 0, 0.7f);
        }

        protected virtual int OutlineWidth()
        {
            return 4;
        }

        protected virtual void CreateMiss()
        {
            int h = FontSize();
            int w = Mathf.FloorToInt(h * 3.0f);
            var sprite = CreateChildSprite(w, h);
            sprite.Bitmap.DrawText("Miss", 0, 0, w, h, Bitmap.TextAlign.Center);
            sprite.Dy = 0;
        }

        protected virtual void CreateDigits(int value)
        {
            string valueString = Mathf.Abs(value).ToString();
            int h = FontSize();
            int w = Mathf.FloorToInt(h * 0.75f);
            
            for (int i = 0; i < valueString.Length; i++)
            {
                var sprite = CreateChildSprite(w, h);
                sprite.Bitmap.DrawText(valueString[i].ToString(), 0, 0, w, h, Bitmap.TextAlign.Center);
                sprite.X = (i - (valueString.Length - 1) / 2f) * w;
                sprite.Dy = -i;
            }
        }

        protected virtual Sprite CreateChildSprite(int width, int height)
        {
            var sprite = Sprite.Create("damage");
            sprite.Bitmap = CreateBitmap(width, height);
            sprite.Anchor = new Vector2(0.5f, 1f);
            sprite.Y = -40;
            sprite.Ry = sprite.Y;
            this.AddChild(sprite);
            _childSprites.Add(sprite);
            return sprite;
        }

        protected virtual Bitmap CreateBitmap(int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            bitmap.FontFace = FontFace();
            bitmap.FontSize = FontSize();
            bitmap.TextColor = DamageColor();
            bitmap.OutlineColor = OutlineColor();
            bitmap.OutlineWidth = OutlineWidth();
            return bitmap;
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            
            if (_duration > 0)
            {
                _duration--;
                foreach (var child in _childSprites)
                {
                    UpdateChild(child);
                }
            }
            
            UpdateFlash();
            UpdateOpacity();
        }

        protected virtual void UpdateChild(Sprite sprite)
        {
            sprite.Dy += 0.5f;
            sprite.Ry += sprite.Dy;
            
            if (sprite.Ry >= 0)
            {
                sprite.Ry = 0;
                sprite.Dy *= -0.6f;
            }
            
            sprite.Y = Mathf.Round(sprite.Ry);
            sprite.SetBlendColor(_flashColor);
        }

        protected virtual void UpdateFlash()
        {
            if (_flashDuration > 0)
            {
                float d = _flashDuration--;
                _flashColor.a *= (d - 1) / d;
            }
        }

        protected virtual void UpdateOpacity()
        {
            if (_duration < 10)
            {
                Opacity = (255 * _duration) / 10;
            }
        }

        public virtual bool IsPlaying()
        {
            return _duration > 0;
        }
    }
}