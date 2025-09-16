using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting a target actor on the battle screen.
    /// </summary>
    public partial class Window_BattleActor : Window_BattleStatus
    {
        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Openness = 255;
            Hide();
        }

        public override void Show()
        {
            ForceSelect(0);
            Rmmz.gameTemp.ClearTouchState();
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
            Rmmz.gameParty.Select(null);
        }

        public override void Select(int index)
        {
            base.Select(index);
            Rmmz.gameParty.Select(Actor(index));
        }

        protected override void ProcessTouch()
        {
            base.ProcessTouch();
            if (IsOpenAndActive())
            {
                var target = Rmmz.gameTemp.TouchTarget() as Game_Actor;
                if (target != null)
                {
                    var members = Rmmz.gameParty.BattleMembers().ToList();
                    if (members.Contains(target))
                    {
                        Select(members.IndexOf(target));
                        if (Rmmz.gameTemp.TouchState() == "click")
                        {
                            ProcessOk();
                        }
                    }
                    Rmmz.gameTemp.ClearTouchState();
                }
            }
        }
    }
}