using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying an overlay image for a state.
    /// </summary>
    public partial class Sprite_StateOverlay : Sprite
    {
        protected Game_Battler _battler;
        protected int _overlayIndex;
        protected int _animationCount;
        protected int _pattern;

        protected override void Awake()
        {
            base.Awake();
            InitMembers();
            LoadBitmap();
        }

        protected virtual void InitMembers()
        {
            _battler = null;
            _overlayIndex = 0;
            _animationCount = 0;
            _pattern = 0;
            Anchor = new Vector2(0.5f, 1f);
        }

        protected virtual void LoadBitmap()
        {
            Bitmap = Rmmz.ImageManager.LoadSystem("States");
            SetFrame(0, 0, 0, 0);
        }

        public virtual void Setup(Game_Battler battler)
        {
            _battler = battler;
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            _animationCount++;
            if (_animationCount >= AnimationWait())
            {
                UpdatePattern();
                UpdateFrame();
                _animationCount = 0;
            }
        }

        protected virtual int AnimationWait()
        {
            return 8;
        }

        protected virtual void UpdatePattern()
        {
            _pattern++;
            _pattern %= 8;
            if (_battler != null)
            {
                _overlayIndex = _battler.StateOverlayIndex();
            }
            else
            {
                _overlayIndex = 0;
            }
        }

        protected virtual void UpdateFrame()
        {
            if (_overlayIndex > 0)
            {
                const int w = 96;
                const int h = 96;
                int sx = _pattern * w;
                int sy = (_overlayIndex - 1) * h;
                SetFrame(sx, sy, w, h);
            }
            else
            {
                SetFrame(0, 0, 0, 0);
            }
        }
    }
}