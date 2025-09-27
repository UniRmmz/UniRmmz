using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying an actor.
    /// </summary>
    public partial class Sprite_Actor : Sprite_Battler
    {
        [Serializable]
        public class MotionData
        {
            public int index;
            public bool loop;

            public MotionData(int index, bool loop)
            {
                this.index = index;
                this.loop = loop;
            }
        }

        public static readonly Dictionary<string, MotionData> MOTIONS = new Dictionary<string, MotionData>
        {
            { "walk", new MotionData(0, true) },
            { "wait", new MotionData(1, true) },
            { "chant", new MotionData(2, true) },
            { "guard", new MotionData(3, true) },
            { "damage", new MotionData(4, false) },
            { "evade", new MotionData(5, false) },
            { "thrust", new MotionData(6, false) },
            { "swing", new MotionData(7, false) },
            { "missile", new MotionData(8, false) },
            { "skill", new MotionData(9, false) },
            { "spell", new MotionData(10, false) },
            { "item", new MotionData(11, false) },
            { "escape", new MotionData(12, true) },
            { "victory", new MotionData(13, true) },
            { "dying", new MotionData(14, true) },
            { "abnormal", new MotionData(15, true) },
            { "sleep", new MotionData(16, true) },
            { "dead", new MotionData(17, true) }
        };

        protected Game_Actor _actor;
        protected string _battlerName;
        protected MotionData _motion;
        protected int _motionCount;
        protected int _pattern;
        protected Sprite _mainSprite;
        protected Sprite _shadowSprite;
        protected Sprite_Weapon _weaponSprite;
        protected Sprite_StateOverlay _stateSprite;

        public override void Initialize(Game_Battler battler)
        {
            base.Initialize(battler);
            MoveToStartPosition();
        }

        protected override void InitMembers()
        {
            base.InitMembers();
            _battlerName = "";
            _motion = null;
            _motionCount = 0;
            _pattern = 0;
            CreateShadowSprite();
            CreateWeaponSprite();
            CreateMainSprite();
            CreateStateSprite();
        }

        public override Sprite_Battler MainSprite()
        {
            return this;
        }

        protected virtual void CreateMainSprite()
        {
            _mainSprite = Sprite.Create("mainSprite");
            _mainSprite.Anchor = new Vector2(0.5f, 1f);
            this.AddChild(_mainSprite);
        }

        protected virtual void CreateShadowSprite()
        {
            _shadowSprite = Sprite.Create("shadowSprite");
            _shadowSprite.Bitmap = Rmmz.ImageManager.LoadSystem("Shadow2");
            _shadowSprite.Anchor = new Vector2(0.5f, 0.5f);
            _shadowSprite.Y = -2;
            this.AddChild(_shadowSprite);
        }

        protected virtual void CreateWeaponSprite()
        {
            _weaponSprite = Sprite_Weapon.Create("weaponSprite");
            this.AddChild(_weaponSprite);
        }

        protected virtual void CreateStateSprite()
        {
            _stateSprite = Sprite_StateOverlay.Create("stateSprite");
            this.AddChild(_stateSprite);
        }

        public override void SetBattler(Game_Battler battler)
        {
            base.SetBattler(battler);
            if (battler != _actor)
            {
                _actor = battler as Game_Actor;
                if (battler != null)
                {
                    SetActorHome(_actor.Index());
                }
                else
                {
                    _mainSprite.Bitmap = null;
                    _battlerName = "";
                }
                StartEntryMotion();
                _stateSprite.Setup(battler);
            }
        }

        protected virtual void MoveToStartPosition()
        {
            StartMove(300, 0, 0);
        }

        protected virtual void SetActorHome(int index)
        {
            SetHome(600 + index * 32, 280 + index * 48);
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateShadow();
            if (_actor != null)
            {
                UpdateMotion();
            }
        }

        protected virtual void UpdateShadow()
        {
            _shadowSprite.Visible = _actor != null;
        }

        protected override void UpdateMain()
        {
            base.UpdateMain();
            if (_actor.IsSpriteVisible() && !IsMoving())
            {
                UpdateTargetPosition();
            }
        }

        protected virtual void SetupMotion()
        {
            if (_actor.IsMotionRequested())
            {
                StartMotion(_actor.MotionType());
                _actor.ClearMotion();
            }
        }

        protected virtual void SetupWeaponAnimation()
        {
            if (_actor.IsWeaponAnimationRequested())
            {
                _weaponSprite.Setup(_actor.WeaponImageId());
                _actor.ClearWeaponAnimation();
            }
        }

        public virtual void StartMotion(string motionType)
        {
            if (MOTIONS.TryGetValue(motionType, out var newMotion))
            {
                if (_motion != newMotion)
                {
                    _motion = newMotion;
                    _motionCount = 0;
                    _pattern = 0;
                }
            }
        }

        protected virtual void UpdateTargetPosition()
        {
            if (_actor.CanMove() && Rmmz.BattleManager.IsEscaped())
            {
                Retreat();
            }
            else if (ShouldStepForward())
            {
                StepForward();
            }
            else if (!InHomePosition())
            {
                StepBack();
            }
        }

        protected virtual bool ShouldStepForward()
        {
            return _actor.IsInputting() || _actor.IsActing();
        }

        protected override void UpdateBitmap()
        {
            base.UpdateBitmap();
            string name = _actor.BattlerName();
            if (_battlerName != name)
            {
                _battlerName = name;
                _mainSprite.Bitmap = Rmmz.ImageManager.LoadSvActor(name);
            }
        }

        protected override void UpdateFrame()
        {
            base.UpdateFrame();
            var bitmap = _mainSprite.Bitmap;
            if (bitmap != null)
            {
                int motionIndex = _motion != null ? _motion.index : 0;
                int pattern = _pattern < 3 ? _pattern : 1;
                int cw = bitmap.Width / 9;
                int ch = bitmap.Height / 6;
                int cx = Mathf.FloorToInt(motionIndex / 6) * 3 + pattern;
                int cy = motionIndex % 6;
                _mainSprite.SetFrame(cx * cw, cy * ch, cw, ch);
                SetFrame(0, 0, cw, ch);
            }
        }

        protected override void UpdateMove()
        {
            var bitmap = _mainSprite.Bitmap;
            if (bitmap == null || bitmap.IsReady())
            {
                base.UpdateMove();
            }
        }

        protected virtual void UpdateMotion()
        {
            SetupMotion();
            SetupWeaponAnimation();
            if (_actor.IsMotionRefreshRequested())
            {
                RefreshMotion();
                _actor.ClearMotion();
            }
            UpdateMotionCount();
        }

        protected virtual void UpdateMotionCount()
        {
            if (_motion != null && ++_motionCount >= MotionSpeed())
            {
                if (_motion.loop)
                {
                    _pattern = (_pattern + 1) % 4;
                }
                else if (_pattern < 2)
                {
                    _pattern++;
                }
                else
                {
                    RefreshMotion();
                }
                _motionCount = 0;
            }
        }

        protected virtual int MotionSpeed()
        {
            return 12;
        }

        protected virtual void RefreshMotion()
        {
            var actor = _actor;
            if (actor != null)
            {
                int stateMotion = actor.StateMotionIndex();
                if (actor.IsInputting() || actor.IsActing())
                {
                    StartMotion("walk");
                }
                else if (stateMotion == 3)
                {
                    StartMotion("dead");
                }
                else if (stateMotion == 2)
                {
                    StartMotion("sleep");
                }
                else if (actor.IsChanting())
                {
                    StartMotion("chant");
                }
                else if (actor.IsGuard() || actor.IsGuardWaiting())
                {
                    StartMotion("guard");
                }
                else if (stateMotion == 1)
                {
                    StartMotion("abnormal");
                }
                else if (actor.IsDying())
                {
                    StartMotion("dying");
                }
                else if (actor.IsUndecided())
                {
                    StartMotion("walk");
                }
                else
                {
                    StartMotion("wait");
                }
            }
        }

        protected virtual void StartEntryMotion()
        {
            if (_actor != null && _actor.CanMove())
            {
                StartMotion("walk");
                StartMove(0, 0, 30);
            }
            else if (!IsMoving())
            {
                RefreshMotion();
                StartMove(0, 0, 0);
            }
        }

        protected virtual void StepForward()
        {
            StartMove(-48, 0, 12);
        }

        protected virtual void StepBack()
        {
            StartMove(0, 0, 12);
        }

        protected virtual void Retreat()
        {
            StartMove(300, 0, 30);
        }

        protected override void OnMoveEnd()
        {
            base.OnMoveEnd();
            if (!Rmmz.BattleManager.IsBattleEnd())
            {
                RefreshMotion();
            }
        }

        protected override float DamageOffsetX()
        {
            return base.DamageOffsetX() - 32;
        }

        protected override float DamageOffsetY()
        {
            return base.DamageOffsetY();
        }
    }
}