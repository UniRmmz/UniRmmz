using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying parameters on the status screen.
    /// </summary>
    public partial class Window_StatusParams //: Window_StatusBase
    {
        protected Game_Actor _actor = null;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _actor = null;
        }

        public virtual void SetActor(Game_Actor actor)
        {
            if (_actor != actor)
            {
                _actor = actor;
                Refresh();
            }
        }

        protected override int MaxItems() => 6;

        public override int ItemHeight() => LineHeight();

        public override void DrawItem(int index)
        {
            Rect rect = ItemLineRect(index);
            int paramId = index + 2;
            string name = Rmmz.TextManager.Param(paramId);
            int value = _actor.Param(paramId);
            ChangeTextColor(Rmmz.ColorManager.SystemColor());
            DrawText(name, (int)rect.x, (int)rect.y, 160);
            ResetTextColor();
            DrawText(value.ToString(), (int)rect.x + 160, (int)rect.y, 60, Bitmap.TextAlign.Right);
        }

        public override void DrawItemBackground(int _)
        {
            // Empty implementation
        }
    }
}