using UnityEngine;
using UnityEngine.EventSystems;


namespace UniRmmz
{
    public abstract partial class Sprite_Clickable //: Sprite
    {
        protected bool _pressed = false;
        protected bool _hovered = false;

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            ProcessTouch();
        }

        protected virtual void ProcessTouch()
        {
            if (IsClickEnabled())
            {
                if (IsBeingTouched())
                {
                    if (!_hovered && TouchInput.IsHovered())
                    {
                        _hovered = true;
                        OnMouseEnter();
                    }
                    if (TouchInput.IsTriggered())
                    {
                        _pressed = true;
                        OnPress();
                    }
                }
                else
                {
                    if (_hovered)
                    {
                        OnMouseExit();
                    }
                    _pressed = false;
                    _hovered = false;
                }
                if (_pressed && TouchInput.IsReleased())
                {
                    _pressed = false;
                    OnClick();
                }
            }
            else
            {
                _pressed = false;
                _hovered = false;
            }
        }

        public virtual bool IsPressed() => _pressed;

        public virtual bool IsClickEnabled() => gameObject.activeInHierarchy;

        public virtual bool IsBeingTouched()
        {
            return RmmzRoot.Instance.Canvas.RectangleContainsRmmzScreenPoint(rectTransform, TouchInput.X, TouchInput.Y);
        }

        protected virtual void OnMouseEnter() { }
        protected virtual void OnMouseExit() { }
        protected virtual void OnPress() { }
        protected virtual void OnClick() { }
    }

}


