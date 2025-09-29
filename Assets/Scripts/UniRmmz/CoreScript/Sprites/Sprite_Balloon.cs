using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying a balloon icon.
    /// </summary>
    public partial class Sprite_Balloon //: Sprite
    {
        protected Sprite _target;
        protected int _balloonId;
        protected int _duration;

        public IBalloonTarget TargetObject { get; set; }

        protected override void Awake()
        {
            base.Awake();
            InitMembers();
            LoadBitmap();
        }

        protected virtual void InitMembers()
        {
            _target = null;
            _balloonId = 0;
            _duration = 0;
            Anchor = new Vector2(0.5f, 1f);
            Z = 7;
        }

        protected virtual void LoadBitmap()
        {
            Bitmap = Rmmz.ImageManager.LoadSystem("Balloon");
            SetFrame(0, 0, 0, 0);
        }

        public virtual void Setup(Sprite targetSprite, int balloonId)
        {
            _target = targetSprite;
            _balloonId = balloonId;
            _duration = 8 * Speed() + WaitTime();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            if (_duration > 0)
            {
                _duration--;
                if (_duration > 0)
                {
                    UpdatePosition();
                    UpdateFrame();
                }
            }
        }

        protected virtual void UpdatePosition()
        {
            X = _target.X;
            Y = _target.Y - _target.Height;
        }

        protected virtual void UpdateFrame()
        {
            const int w = 48;
            const int h = 48;
            int sx = FrameIndex() * w;
            int sy = (_balloonId - 1) * h;
            SetFrame(sx, sy, w, h);
        }

        protected virtual int Speed()
        {
            return 8;
        }

        protected virtual int WaitTime()
        {
            return 12;
        }

        protected virtual int FrameIndex()
        {
            int index = (_duration - WaitTime()) / Speed();
            return 7 - Math.Max(index, 0);
        }

        public virtual bool IsPlaying()
        {
            return _duration > 0;
        }
    }
}