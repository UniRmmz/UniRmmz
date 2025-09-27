using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying a weapon image for attacking.
    /// </summary>
    public partial class Sprite_Weapon : Sprite
    {
        protected int _weaponImageId;
        protected int _animationCount;
        protected int _pattern;

        protected override void Awake()
        {
            base.Awake();
            InitMembers();
        }

        protected virtual void InitMembers()
        {
            _weaponImageId = 0;
            _animationCount = 0;
            _pattern = 0;
            Anchor = new Vector2(0.5f, 1f);
            X = -16;
        }

        public virtual void Setup(int weaponImageId)
        {
            _weaponImageId = weaponImageId;
            _animationCount = 0;
            _pattern = 0;
            LoadBitmap();
            UpdateFrame();
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
            return 12;
        }

        protected virtual void UpdatePattern()
        {
            _pattern++;
            if (_pattern >= 3)
            {
                _weaponImageId = 0;
            }
        }

        protected virtual void LoadBitmap()
        {
            int pageId = Mathf.FloorToInt((_weaponImageId - 1) / 12) + 1;
            if (pageId >= 1)
            {
                Bitmap = Rmmz.ImageManager.LoadSystem("Weapons" + pageId);
            }
            else
            {
                Bitmap = Rmmz.ImageManager.LoadSystem("");
            }
        }

        protected virtual void UpdateFrame()
        {
            if (_weaponImageId > 0)
            {
                int index = (_weaponImageId - 1) % 12;
                const int w = 96;
                const int h = 64;
                int sx = (Mathf.FloorToInt(index / 6) * 3 + _pattern) * w;
                int sy = Mathf.FloorToInt(index % 6) * h;
                SetFrame(sx, sy, w, h);
            }
            else
            {
                SetFrame(0, 0, 0, 0);
            }
        }

        public virtual bool IsPlaying()
        {
            return _weaponImageId > 0;
        }
    }
}