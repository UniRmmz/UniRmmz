
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying parameter changes on the equipment screen.
    /// </summary>
    public partial class Window_EquipStatus //: Window_StatusBase
    {
        protected Game_Actor _actor = null;
        protected Game_Actor _tempActor = null;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _actor = null;
            _tempActor = null;
            Refresh();
        }

        public virtual void SetActor(Game_Actor actor)
        {
            if (_actor != actor)
            {
                _actor = actor;
                Refresh();
            }
        }

        protected override int ColSpacing() => 0;

        public override void Refresh()
        {
            Contents.Clear();
            if (_actor != null)
            {
                var nameRect = ItemLineRect(0);
                DrawActorName(_actor, (int)nameRect.x, 0, (int)nameRect.width);
                DrawActorFace(_actor, (int)nameRect.x, (int)nameRect.height);
                DrawAllParams();
            }
        }

        public virtual void SetTempActor(Game_Actor tempActor)
        {
            if (_tempActor != tempActor)
            {
                _tempActor = tempActor;
                Refresh();
            }
        }

        protected virtual void DrawAllParams()
        {
            for (int i = 0; i < 6; i++)
            {
                int x = ItemPadding();
                int y = ParamY(i);
                DrawItem(x, y, 2 + i);
            }
        }

        protected virtual void DrawItem(int x, int y, int paramId)
        {
            int paramX = ParamX();
            int paramWidth = ParamWidth();
            int rightArrowWidth = RightArrowWidth();
            DrawParamName(x, y, paramId);
            if (_actor != null)
            {
                DrawCurrentParam(paramX, y, paramId);
            }

            DrawRightArrow(paramX + paramWidth, y);
            if (_tempActor != null)
            {
                DrawNewParam(paramX + paramWidth + rightArrowWidth, y, paramId);
            }
        }

        protected virtual void DrawParamName(int x, int y, int paramId)
        {
            int width = ParamX() - ItemPadding() * 2;
            ChangeTextColor(Rmmz.ColorManager.SystemColor());
            DrawText(Rmmz.TextManager.Param(paramId), x, y, width);
        }

        protected virtual void DrawCurrentParam(int x, int y, int paramId)
        {
            int paramWidth = ParamWidth();
            ResetTextColor();
            DrawText(_actor.Param(paramId).ToString(), x, y, paramWidth, Bitmap.TextAlign.Right);
        }

        protected virtual void DrawRightArrow(int x, int y)
        {
            int rightArrowWidth = RightArrowWidth();
            ChangeTextColor(Rmmz.ColorManager.SystemColor());
            DrawText("→", x, y, rightArrowWidth, Bitmap.TextAlign.Center);
        }

        protected virtual void DrawNewParam(int x, int y, int paramId)
        {
            int paramWidth = ParamWidth();
            int newValue = _tempActor.Param(paramId);
            int diffValue = newValue - _actor.Param(paramId);
            ChangeTextColor(Rmmz.ColorManager.ParamChangeTextColor(diffValue));
            DrawText(newValue.ToString(), x, y, paramWidth, Bitmap.TextAlign.Right);
        }

        protected virtual int RightArrowWidth() => 32;

        protected virtual int ParamWidth() => 48;

        protected virtual int ParamX()
        {
            int itemPadding = ItemPadding();
            int rightArrowWidth = RightArrowWidth();
            int paramWidth = ParamWidth();
            return (int)InnerWidth - itemPadding - paramWidth * 2 - rightArrowWidth;
        }

        protected virtual int ParamY(int index)
        {
            int faceHeight = Rmmz.ImageManager.FaceHeight;
            return faceHeight + Mathf.FloorToInt(LineHeight() * (index + 1.5f));
        }
    }
}