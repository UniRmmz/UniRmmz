using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of Scene_Item and Scene_Skill.
    /// </summary>
    public abstract partial class Scene_ItemBase<T> //: Scene_MenuBase where T : Window_Selectable 
    {
        protected Window_MenuActor _actorWindow;
        protected T _itemWindow;

        protected virtual void CreateActorWindow()
        {
            Rect rect = ActorWindowRect();
            _actorWindow = Window_MenuActor.Create(rect, "menuActor");
            _actorWindow.SetHandler("ok", OnActorOk);
            _actorWindow.SetHandler("cancel", OnActorCancel);
            AddWindow(_actorWindow);
        }

        protected virtual Rect ActorWindowRect()
        {
            float wx = 0;
            float wy = Mathf.Min(MainAreaTop(), HelpAreaTop());
            float ww = Graphics.BoxWidth - MainCommandWidth();
            float wh = MainAreaHeight() + HelpAreaHeight();
            return new Rect(wx, wy, ww, wh);
        }

        protected abstract UsableItem Item();

        protected virtual Game_Battler User()
        {
            return null;
        }

        protected virtual bool IsCursorLeft()
        {
            return _itemWindow.Index() % 2 == 0;
        }

        protected virtual void ShowActorWindow()
        {
            if (IsCursorLeft())
            {
                _actorWindow.X = Graphics.BoxWidth - _actorWindow.Width;
            }
            else
            {
                _actorWindow.X = 0;
            }
            _actorWindow.Show();
            _actorWindow.Activate();
        }

        protected virtual void HideActorWindow()
        {
            _actorWindow.Hide();
            _actorWindow.Deactivate();
        }

        protected virtual bool IsActorWindowActive()
        {
            return _actorWindow != null && _actorWindow.Active;
        }

        protected virtual void OnActorOk()
        {
            if (CanUse())
            {
                UseItem();
            }
            else
            {
                Rmmz.SoundManager.PlayBuzzer();
            }
        }

        protected virtual void OnActorCancel()
        {
            HideActorWindow();
            ActivateItemWindow();
        }

        protected virtual void DetermineItem()
        {
            var action = Game_Action.Create(User());
            var item = Item();
            action.SetItemObject(item as UsableItem);
            if (action.IsForFriend())
            {
                ShowActorWindow();
                _actorWindow.SelectForItem(Item());
            }
            else
            {
                UseItem();
                ActivateItemWindow();
            }
        }

        protected virtual void UseItem()
        {
            PlaySeForItem();
            User().UseItem(Item());
            ApplyItem();
            CheckCommonEvent();
            CheckGameover();
            _actorWindow.Refresh();
        }

        protected virtual void ActivateItemWindow()
        {
            _itemWindow.Refresh();
            _itemWindow.Activate();
        }

        protected virtual List<Game_Actor> ItemTargetActors()
        {
            var action = Game_Action.Create(User());
            action.SetItemObject(Item());
            if (!action.IsForFriend())
            {
                return new List<Game_Actor>();
            }
            else if (action.IsForAll())
            {
                return Rmmz.gameParty.Members().Cast<Game_Actor>().ToList();
            }
            else
            {
                return new List<Game_Actor> { Rmmz.gameParty.Members().ElementAt(_actorWindow.Index()) as Game_Actor };
            }
        }

        protected virtual bool CanUse()
        {
            Game_Battler user = User();
            return user != null && user.CanUse(Item()) && IsItemEffectsValid();
        }

        protected virtual bool IsItemEffectsValid()
        {
            var action = Game_Action.Create(User());
            action.SetItemObject(Item());
            return ItemTargetActors().Any(target => action.TestApply(target));
        }

        protected virtual void ApplyItem()
        {
            var action = Game_Action.Create(User());
            action.SetItemObject(Item());
            foreach (var target in ItemTargetActors())
            {
                for (int i = 0; i < action.NumRepeats(); i++)
                {
                    action.Apply(target);
                }
            }
            action.ApplyGlobal();
        }

        protected virtual void CheckCommonEvent()
        {
            if (Rmmz.gameTemp.IsCommonEventReserved())
            {
                Scene_Map.Goto();
            }
        }

        protected virtual void PlaySeForItem()
        {
            // Override in derived classes
        }

    }
}