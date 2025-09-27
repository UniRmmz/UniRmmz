using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying an enemy.
    /// </summary>
    public partial class Sprite_Enemy : Sprite_Battler
    {
        protected Game_Enemy _enemy;
        protected bool _appeared;
        protected string _battlerName;
        protected int _battlerHue;
        protected string _effectType;
        protected int _effectDuration;
        protected float _shake;
        protected Sprite_StateIcon _stateIconSprite;

        public override void Initialize(Game_Battler battler)
        {
            base.Initialize(battler);
        }

        protected override void InitMembers()
        {
            base.InitMembers();
            _enemy = null;
            _appeared = false;
            _battlerName = null;
            _battlerHue = 0;
            _effectType = null;
            _effectDuration = 0;
            _shake = 0;
            CreateStateIconSprite();
        }

        protected virtual void CreateStateIconSprite()
        {
            _stateIconSprite = Sprite_StateIcon.Create("stateIconSprite");
            this.AddChild(_stateIconSprite);
        }

        public override void SetBattler(Game_Battler battler)
        {
            base.SetBattler(battler);
            _enemy = battler as Game_Enemy;
            SetHome(_enemy.ScreenX(), -_enemy.ScreenY());
            _stateIconSprite.Setup(battler);
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            if (_enemy != null)
            {
                UpdateEffect();
                UpdateStateSprite();
            }
        }

        protected override void UpdateBitmap()
        {
            base.UpdateBitmap();
            string name = _enemy.BattlerName();
            int hue = _enemy.BattlerHue();
            if (_battlerName != name || _battlerHue != hue)
            {
                _battlerName = name;
                _battlerHue = hue;
                LoadBitmap(name);
                SetHue(hue);
                InitVisibility();
            }
        }

        protected virtual void LoadBitmap(string name)
        {
            if (Rmmz.gameSystem.IsSideView())
            {
                Bitmap = Rmmz.ImageManager.LoadSvEnemy(name);
            }
            else
            {
                Bitmap = Rmmz.ImageManager.LoadEnemy(name);
            }
        }

        public override void SetHue(int hue)
        {
            base.SetHue(hue);
            foreach (var sprite in this.Children<Sprite>())
            {
                sprite.SetHue(-hue);
            }
        }

        protected override void UpdateFrame()
        {
            base.UpdateFrame();
            if (_effectType == "bossCollapse")
            {
                SetFrame(0, 0, Bitmap.Width, _effectDuration);
            }
            else
            {
                SetFrame(0, 0, Bitmap.Width, Bitmap.Height);
            }
        }

        protected override void UpdatePosition()
        {
            base.UpdatePosition();
            X += _shake;
        }

        protected virtual void UpdateStateSprite()
        {
            _stateIconSprite.Y = -Mathf.Round((Bitmap.Height + 40) * 0.9f);
            if (_stateIconSprite.Y < 20 - Y)
            {
                _stateIconSprite.Y = 20 - Y;
            }
        }

        protected virtual void InitVisibility()
        {
            _appeared = _enemy.IsAlive();
            if (!_appeared)
            {
                Opacity = 0;
            }
        }

        protected virtual void SetupEffect()
        {
            if (_appeared && _enemy.IsEffectRequested())
            {
                StartEffect(_enemy.EffectType());
                _enemy.ClearEffect();
            }
            if (!_appeared && _enemy.IsAlive())
            {
                StartEffect("appear");
            }
            else if (_appeared && _enemy.IsHidden())
            {
                StartEffect("disappear");
            }
        }

        protected virtual void StartEffect(string effectType)
        {
            _effectType = effectType;
            switch (_effectType)
            {
                case "appear":
                    StartAppear();
                    break;
                case "disappear":
                    StartDisappear();
                    break;
                case "whiten":
                    StartWhiten();
                    break;
                case "blink":
                    StartBlink();
                    break;
                case "collapse":
                    StartCollapse();
                    break;
                case "bossCollapse":
                    StartBossCollapse();
                    break;
                case "instantCollapse":
                    StartInstantCollapse();
                    break;
            }
            RevertToNormal();
        }

        protected virtual void StartAppear()
        {
            _effectDuration = 16;
            _appeared = true;
        }

        protected virtual void StartDisappear()
        {
            _effectDuration = 32;
            _appeared = false;
        }

        protected virtual void StartWhiten()
        {
            _effectDuration = 16;
        }

        protected virtual void StartBlink()
        {
            _effectDuration = 20;
        }

        protected virtual void StartCollapse()
        {
            _effectDuration = 32;
            _appeared = false;
        }

        protected virtual void StartBossCollapse()
        {
            _effectDuration = Bitmap.Height;
            _appeared = false;
        }

        protected virtual void StartInstantCollapse()
        {
            _effectDuration = 16;
            _appeared = false;
        }

        protected virtual void UpdateEffect()
        {
            SetupEffect();
            if (_effectDuration > 0)
            {
                _effectDuration--;
                switch (_effectType)
                {
                    case "whiten":
                        UpdateWhiten();
                        break;
                    case "blink":
                        UpdateBlink();
                        break;
                    case "appear":
                        UpdateAppear();
                        break;
                    case "disappear":
                        UpdateDisappear();
                        break;
                    case "collapse":
                        UpdateCollapse();
                        break;
                    case "bossCollapse":
                        UpdateBossCollapse();
                        break;
                    case "instantCollapse":
                        UpdateInstantCollapse();
                        break;
                }
                if (_effectDuration == 0)
                {
                    _effectType = null;
                }
            }
        }

        public override bool IsEffecting()
        {
            return _effectType != null;
        }

        protected virtual void RevertToNormal()
        {
            _shake = 0;
            BlendMode = 0;
            Opacity = 255;
            SetBlendColor(new Color(0, 0, 0, 0));
        }

        protected virtual void UpdateWhiten()
        {
            float alpha = (128 - (16 - _effectDuration) * 8) / 255f;
            SetBlendColor(new Color(1f, 1f, 1f, alpha));
        }

        protected virtual void UpdateBlink()
        {
            Opacity = _effectDuration % 10 < 5 ? 255 : 0;
        }

        protected virtual void UpdateAppear()
        {
            Opacity = (16 - _effectDuration) * 16;
        }

        protected virtual void UpdateDisappear()
        {
            Opacity = 256 - (32 - _effectDuration) * 10;
        }

        protected virtual void UpdateCollapse()
        {
            BlendMode = Sprite.BlendModes.Add;
            SetBlendColor(new Color(1f, 0.5f, 0.5f, 0.5f));
            Opacity = Mathf.RoundToInt(Opacity * _effectDuration / (_effectDuration + 1f));
        }

        protected virtual void UpdateBossCollapse()
        {
            _shake = (_effectDuration % 2) * 4 - 2;
            BlendMode = Sprite.BlendModes.Add;
            Opacity = Mathf.RoundToInt(Opacity * _effectDuration / (_effectDuration + 1f));
            SetBlendColor(new Color(1f, 1f, 1f, (255 - Opacity) / 255f));
            if (_effectDuration % 20 == 19)
            {
                Rmmz.SoundManager.PlayBossCollapse2();
            }
        }

        protected virtual void UpdateInstantCollapse()
        {
            Opacity = 0;
        }

        protected override float DamageOffsetX()
        {
            return base.DamageOffsetX();
        }

        protected override float DamageOffsetY()
        {
            return base.DamageOffsetY() - 8;
        }
    }
}