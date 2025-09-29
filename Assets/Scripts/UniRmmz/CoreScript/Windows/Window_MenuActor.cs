using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting a target actor on the item and skill screens.
    /// </summary>
    public partial class Window_MenuActor //: Window_MenuStatus
    {
        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Hide();
        }

        protected override void ProcessOk()
        {
            if (!CursorAll())
            {
                var actor = Rmmz.gameParty.Members().ElementAt(Index()) as Game_Actor;
                Rmmz.gameParty.SetTargetActor(actor);
            }
            CallOkHandler();
        }

        public override void SelectLast()
        {
            Game_Actor targetActor = Rmmz.gameParty.TargetActor();
            int index = targetActor != null ? targetActor.Index() : 0;
            ForceSelect(index);
        }

        public virtual void SelectForItem(UsableItem item)
        {
            Game_Actor actor = Rmmz.gameParty.MenuActor();
            Game_Action action = Game_Action.Create(actor);
            action.SetItemObject(item);
            SetCursorFixed(false);
            SetCursorAll(false);
            
            if (action.IsForUser())
            {
                if (Rmmz.DataManager.IsSkill(item))
                {
                    SetCursorFixed(true);
                    ForceSelect(actor.Index());
                }
                else
                {
                    SelectLast();
                }
            }
            else if (action.IsForAll())
            {
                SetCursorAll(true);
                ForceSelect(0);
            }
            else
            {
                SelectLast();
            }
        }
    }
}