using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying the skill user's status on the skill screen.
    /// </summary>
    public partial class Window_SkillStatus //: Window_StatusBase
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

        public override void Refresh()
        {
            base.Refresh();
            if (_actor != null)
            {
                int x = ColSpacing() / 2;
                int h = InnerHeight;
                int y = h / 2 - Mathf.FloorToInt(LineHeight() * 1.5f);
                DrawActorFace(_actor, x + 1, 0, 144, h);
                DrawActorSimpleStatus(_actor, x + 180, y);
            }
        }
    }
}

