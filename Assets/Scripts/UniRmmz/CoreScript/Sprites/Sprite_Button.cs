using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying a button.
    /// </summary>
    public partial class Sprite_Button : Sprite_Clickable
    {
        protected Input.ButtonTypes _buttonType;
        protected Rect _coldFrame;
        protected Rect _hotFrame;
        protected bool _isPressed = false;
        protected System.Action _clickHandler;

        public virtual void Initialize(Input.ButtonTypes buttonType)
        {
            _buttonType = buttonType;
            SetupFrames();
        }

        protected virtual void SetupFrames()
        {
            var data = ButtonData();
            float x = data.x * BlockWidth();
            float width = data.y * BlockWidth();
            float height = BlockHeight();
            LoadButtonImage();
            SetColdFrame(x, 0, width, height);
            SetHotFrame(x, height, width, height);
            UpdateFrame();
            UpdateOpacity();
        }

        protected virtual int BlockWidth() => 48;
        protected virtual int BlockHeight() => 48;

        protected virtual void LoadButtonImage()
        {
            Bitmap = Rmmz.ImageManager.LoadSystem("ButtonSet");
        }
        protected virtual Vector2 ButtonData()
        {
            return _buttonType switch
            {
                Input.ButtonTypes.Cancel => new Vector2(0, 2),
                Input.ButtonTypes.PageUp => new Vector2(2, 1),
                Input.ButtonTypes.PageDown => new Vector2(3, 1),
                Input.ButtonTypes.Down => new Vector2(4, 1),
                Input.ButtonTypes.Up => new Vector2(5, 1),
                Input.ButtonTypes.Down2 => new Vector2(6, 1),
                Input.ButtonTypes.Up2 => new Vector2(7, 1),
                Input.ButtonTypes.Ok => new Vector2(8, 2),
                Input.ButtonTypes.Menu => new Vector2(10, 1),
                _ => new Vector2(0, 1)
            };
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            CheckBitmap();
            UpdateFrame();
            UpdateOpacity();
            ProcessTouch();
        }

        public virtual void CheckBitmap()
        {
            if (Bitmap.IsReady() && Bitmap.Width < BlockWidth() * 11)
            {
                // Probably MV image is used
                throw new Exception("ButtonSet image is too small");
            }
        }

        protected virtual void UpdateFrame()
        {
            var frame = IsPressed() ? _hotFrame : _coldFrame;
            if (frame != null)
            {
                SetFrame(frame.x, frame.y, frame.width, frame.height);
            }
        }

        public virtual void UpdateOpacity()
        {
            Opacity = _isPressed ? 255 : 192;
        }

        protected virtual void SetColdFrame(float x, float y, float width, float height)
        {
            _coldFrame = new Rect(x, y, width, height);
        }
        
        protected virtual void SetHotFrame(float x, float y, float width, float height)
        {
            _hotFrame = new Rect(x, y, width, height);
        }

        public virtual void SetClickHandler(System.Action method)
        {
            _clickHandler = method;
        }

        protected override void OnClick()
        {
            if (_clickHandler != null)
            {
                _clickHandler();
            }
            else
            {
                UniRmmz.Input.VirtualClick(_buttonType);
            }
        }
    }
}