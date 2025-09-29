using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of Sprite_Actor and Sprite_Enemy.
    /// </summary>
    public partial class Sprite_Battler //: Sprite_Clickable
    {
        protected Game_Battler _battler;
        protected List<Sprite_Damage> _damages;
        protected float _homeX;
        protected float _homeY;
        protected float _offsetX;
        protected float _offsetY;
        protected float _targetOffsetX;
        protected float _targetOffsetY;
        protected int _movementDuration;
        protected int _selectionEffectCount;

        protected override void Awake()
        {
            base.Awake();
            InitMembers();
        }

        public virtual void Initialize(Game_Battler battler)
        {
            SetBattler(battler);
        }

        protected virtual void InitMembers()
        {
            Anchor = new Vector2(0.5f, 1f);
            _battler = null;
            _damages = new List<Sprite_Damage>();
            _homeX = 0;
            _homeY = 0;
            _offsetX = 0;
            _offsetY = 0;
            _targetOffsetX = float.NaN;
            _targetOffsetY = float.NaN;
            _movementDuration = 0;
            _selectionEffectCount = 0;
        }

        public virtual void SetBattler(Game_Battler battler)
        {
            _battler = battler;
        }

        public virtual bool CheckBattler(object battler)
        {
            return _battler == battler;
        }

        public virtual Sprite_Battler MainSprite()
        {
            return this;
        }

        public virtual void SetHome(float x, float y)
        {
            _homeX = x;
            _homeY = y;
            UpdatePosition();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            if (_battler != null)
            {
                UpdateMain();
                UpdateDamagePopup();
                UpdateSelectionEffect();
                UpdateVisibility();
            }
            else
            {
                Bitmap = null;
            }
        }

        protected override void UpdateVisibility()
        {
            base.UpdateVisibility();
            if (_battler == null || !_battler.IsSpriteVisible())
            {
                Visible = false;
            }
        }

        protected virtual void UpdateMain()
        {
            if (_battler.IsSpriteVisible())
            {
                UpdateBitmap();
                UpdateFrame();
            }
            UpdateMove();
            UpdatePosition();
        }

        protected virtual void UpdateBitmap()
        {
            // Override in subclasses
        }

        protected virtual void UpdateFrame()
        {
            // Override in subclasses
        }

        protected virtual void UpdateMove()
        {
            if (_movementDuration > 0)
            {
                float d = _movementDuration;
                _offsetX = (_offsetX * (d - 1) + _targetOffsetX) / d;
                _offsetY = (_offsetY * (d - 1) + _targetOffsetY) / d;
                _movementDuration--;
                if (_movementDuration == 0)
                {
                    OnMoveEnd();
                }
            }
        }

        protected virtual void UpdatePosition()
        {
            X = _homeX + _offsetX;
            Y = _homeY + _offsetY;
        }

        protected virtual void UpdateDamagePopup()
        {
            SetupDamagePopup();
            if (_damages.Count > 0)
            {
                foreach (var damage in _damages)
                {
                    damage.UpdateRmmz();
                }
                if (!_damages[0].IsPlaying())
                {
                    DestroyDamageSprite(_damages[0]);
                }
            }
        }

        protected virtual void UpdateSelectionEffect()
        {
            var target = MainSprite();
            if (_battler.IsSelected())
            {
                _selectionEffectCount++;
                if (_selectionEffectCount % 30 < 15)
                {
                    target.SetBlendColor(new Color(1f, 1f, 1f, 64f / 255f));
                }
                else
                {
                    target.SetBlendColor(new Color(0, 0, 0, 0));
                }
            }
            else if (_selectionEffectCount > 0)
            {
                _selectionEffectCount = 0;
                target.SetBlendColor(new Color(0, 0, 0, 0));
            }
        }

        protected virtual void SetupDamagePopup()
        {
            if (_battler.IsDamagePopupRequested())
            {
                if (_battler.IsSpriteVisible())
                {
                    CreateDamageSprite();
                }
                _battler.ClearDamagePopup();
                _battler.ClearResult();
            }
        }

        protected virtual void CreateDamageSprite()
        {
            var last = _damages.Count > 0 ? _damages[_damages.Count - 1] : null;
            var sprite = Sprite_Damage.Create("damage");
            
            if (last != null)
            {
                sprite.X = last.X + 8;
                sprite.Y = last.Y - 16;
            }
            else
            {
                sprite.X = X + DamageOffsetX();
                sprite.Y = Y + DamageOffsetY();
            }
            
            sprite.Setup(_battler);
            _damages.Add(sprite);
            this.AddSibling(sprite.gameObject);
        }

        protected virtual void DestroyDamageSprite(Sprite_Damage sprite)
        {
            this.RemoveSibling(sprite);
            _damages.Remove(sprite);
            DestroyImmediate(sprite.gameObject);
        }

        protected virtual float DamageOffsetX()
        {
            return 0;
        }

        protected virtual float DamageOffsetY()
        {
            return 0;
        }

        public virtual void StartMove(float x, float y, int duration)
        {
            if (_targetOffsetX != x || _targetOffsetY != y)
            {
                _targetOffsetX = x;
                _targetOffsetY = y;
                _movementDuration = duration;
                if (duration == 0)
                {
                    _offsetX = x;
                    _offsetY = y;
                }
            }
        }

        protected virtual void OnMoveEnd()
        {
            // Override in subclasses
        }

        public virtual bool IsEffecting()
        {
            return false;
        }

        public virtual bool IsMoving()
        {
            return _movementDuration > 0;
        }

        public virtual bool InHomePosition()
        {
            return _offsetX == 0 && _offsetY == 0;
        }

        protected override void OnMouseEnter()
        {
            Rmmz.gameTemp.SetTouchState(_battler, "select");
        }

        protected override void OnPress()
        {
            Rmmz.gameTemp.SetTouchState(_battler, "select");
        }

        protected override void OnClick()
        {
            Rmmz.gameTemp.SetTouchState(_battler, "click");
        }
    }
}