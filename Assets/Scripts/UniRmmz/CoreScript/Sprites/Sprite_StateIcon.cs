using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying state icons.
    /// </summary>
    public partial class Sprite_StateIcon : Sprite
    {
        protected Game_Battler _battler;
        protected int _iconIndex;
        protected int _animationCount;
        protected int _animationIndex;

        protected override void Awake()
        {
            base.Awake();
            InitMembers();
            LoadBitmap();
        }

        protected virtual void InitMembers()
        {
            _battler = null;
            _iconIndex = 0;
            _animationCount = 0;
            _animationIndex = 0;
            Anchor = new Vector2(0.5f, 0.5f);
        }

        protected virtual void LoadBitmap()
        {
            Bitmap = Rmmz.ImageManager.LoadSystem("IconSet");
            SetFrame(0, 0, 0, 0);
        }

        public virtual void Setup(Game_Battler battler)
        {
            if (_battler != battler)
            {
                _battler = battler;
                _animationCount = AnimationWait();
            }
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            _animationCount++;
            if (_animationCount >= AnimationWait())
            {
                UpdateIcon();
                UpdateFrame();
                _animationCount = 0;
            }
        }

        protected virtual int AnimationWait()
        {
            return 40;
        }

        protected virtual void UpdateIcon()
        {
            var icons = new List<int>();
            if (ShouldDisplay())
            {
                icons.AddRange(_battler.AllIcons());
            }
            
            if (icons.Count > 0)
            {
                _animationIndex++;
                if (_animationIndex >= icons.Count)
                {
                    _animationIndex = 0;
                }
                _iconIndex = icons[_animationIndex];
            }
            else
            {
                _animationIndex = 0;
                _iconIndex = 0;
            }
        }

        protected virtual bool ShouldDisplay()
        {
            var battler = _battler;
            return battler != null && (battler.IsActor() || battler.IsAlive());
        }

        protected virtual void UpdateFrame()
        {
            int pw = Rmmz.ImageManager.IconWidth;
            int ph = Rmmz.ImageManager.IconHeight;
            int sx = (_iconIndex % 16) * pw;
            int sy = Mathf.FloorToInt(_iconIndex / 16) * ph;
            SetFrame(sx, sy, pw, ph);
        }
    }
}