using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying a balloon icon.
    /// </summary>
    public partial class Sprite_Balloon : Sprite
    {
        private Sprite _target;
        private int _balloonId;
        private int _duration;

        public IBalloonTarget TargetObject { get; set; }

        protected override void Awake()
        {
            base.Awake();
            InitMembers();
            LoadBitmap();
        }

        private void InitMembers()
        {
            _target = null;
            _balloonId = 0;
            _duration = 0;
            Anchor = new Vector2(0.5f, 1f);
            Z = 7;
        }

        private void LoadBitmap()
        {
            Bitmap = Rmmz.ImageManager.LoadSystem("Balloon");
            SetFrame(0, 0, 0, 0);
        }

        public void Setup(Sprite targetSprite, int balloonId)
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

        private void UpdatePosition()
        {
            X = _target.X;
            Y = _target.Y - _target.Height;
        }

        private void UpdateFrame()
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

        private int FrameIndex()
        {
            int index = (_duration - WaitTime()) / Speed();
            return 7 - Math.Max(index, 0);
        }

        public bool IsPlaying()
        {
            return _duration > 0;
        }
    }
}