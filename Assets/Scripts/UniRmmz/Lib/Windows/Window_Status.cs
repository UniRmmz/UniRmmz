using Unity.VisualScripting;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying full status on the status screen.
    /// </summary>
    public partial class Window_Status : Window_StatusBase
    {
        protected Game_Actor _actor = null;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _actor = null;
            Refresh();
            Activate();
        }

        public virtual void SetActor(Game_Actor actor)
        {
            if (_actor != actor)
            {
                _actor = actor;
                Refresh();
            }
        }

        public override void Refresh()
        {
            base.Refresh();
            if (_actor != null)
            {
                DrawBlock1();
                DrawBlock2();
            }
        }

        protected virtual void DrawBlock1()
        {
            int y = Block1Y();
            DrawActorName(_actor, 6, y, 168);
            DrawActorClass(_actor, 192, y, 168);
            DrawActorNickname(_actor, 432, y, 270);
        }

        protected virtual int Block1Y() => 0;

        protected virtual void DrawBlock2()
        {
            int y = Block2Y();
            DrawActorFace(_actor, 12, y);
            DrawBasicInfo(204, y);
            DrawExpInfo(456, y);
        }

        protected virtual int Block2Y()
        {
            int lineHeight = (int)LineHeight();
            int min = lineHeight;
            int max = (int)InnerHeight - lineHeight * 4;
            return Mathf.FloorToInt(Mathf.Clamp(lineHeight * 1.4f, min, max));
        }

        protected virtual void DrawBasicInfo(int x, int y)
        {
            int lineHeight = (int)LineHeight();
            DrawActorLevel(_actor, x, y + lineHeight * 0);
            DrawActorIcons(_actor, x, y + lineHeight * 1);
            PlaceBasicGauges(_actor, x, y + lineHeight * 2);
        }

        protected virtual void DrawExpInfo(int x, int y)
        {
            int lineHeight = (int)LineHeight();
            string expTotal = Rmmz.TextManager.ExpTotal.RmmzFormat(Rmmz.TextManager.Exp);
            string expNext = Rmmz.TextManager.ExpNext.RmmzFormat(Rmmz.TextManager.Level);
            ChangeTextColor(Rmmz.ColorManager.SystemColor());
            DrawText(expTotal, x, y + lineHeight * 0, 270);
            DrawText(expNext, x, y + lineHeight * 2, 270);
            ResetTextColor();
            DrawText(ExpTotalValue(), x, y + lineHeight * 1, 270, Bitmap.TextAlign.Right);
            DrawText(ExpNextValue(), x, y + lineHeight * 3, 270, Bitmap.TextAlign.Right);
        }

        protected virtual string ExpTotalValue()
        {
            if (_actor.IsMaxLevel())
            {
                return "-------";
            }
            else
            {
                return _actor.CurrentExp().ToString();
            }
        }

        protected virtual string ExpNextValue()
        {
            if (_actor.IsMaxLevel())
            {
                return "-------";
            }
            else
            {
                return _actor.NextRequiredExp().ToString();
            }
        }
    }
}