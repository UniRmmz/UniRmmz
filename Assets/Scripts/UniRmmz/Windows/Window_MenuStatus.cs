using System;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for displaying party member status on the menu screen.
    /// </summary>
    public partial class Window_MenuStatus : Window_StatusBase
    {
        protected bool _formationMode = false;
        protected int _pendingIndex = -1;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _formationMode = false;
            _pendingIndex = -1;
            Refresh();
        }

        protected override int MaxItems() => Rmmz.gameParty.Size();

        protected virtual int NumVisibleRows() => 4;

        public override int ItemHeight()
        {
            return InnerHeight / NumVisibleRows();
        }

        protected virtual Game_Actor Actor(int index)
        {
            return Rmmz.gameParty.Members().ElementAt(index) as Game_Actor;
        }

        public override void DrawItem(int index)
        {
            DrawPendingItemBackground(index);
            DrawItemImage(index);
            DrawItemStatus(index);
        }

        protected virtual void DrawPendingItemBackground(int index)
        {
            if (index == _pendingIndex)
            {
                Rect rect = ItemRect(index);
                Color color = Rmmz.ColorManager.PendingColor();
                ChangePaintOpacity(false);
                Contents.FillRect((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, color);
                ChangePaintOpacity(true);
            }
        }

        protected virtual void DrawItemImage(int index)
        {
            Game_Actor actor = Actor(index);
            Rect rect = ItemRect(index);
            int width = Rmmz.ImageManager.FaceWidth;
            int height = (int)rect.height - 2;
            ChangePaintOpacity(actor.IsBattleMember());
            DrawActorFace(actor, (int)rect.x + 1, (int)rect.y + 1, width, height);
            ChangePaintOpacity(true);
        }

        protected virtual void DrawItemStatus(int index)
        {
            var actor = Actor(index);
            var rect = ItemRect(index);
            int x = (int)rect.x + 180;
            int y = (int)rect.y + Mathf.FloorToInt(rect.height / 2 - LineHeight() * 1.5f);
            DrawActorSimpleStatus(actor, x, y);
        }

        protected override void ProcessOk()
        {
            base.ProcessOk();
            var actor = Actor(Index());
            Rmmz.gameParty.SetMenuActor(actor);
        }

        public override bool IsCurrentItemEnabled()
        {
            if (_formationMode)
            {
                var actor = Actor(Index());
                return actor != null && actor.IsFormationChangeOk();
            }
            else
            {
                return true;
            }
        }

        public virtual void SelectLast()
        {
            Game_Actor menuActor = Rmmz.gameParty.MenuActor();
            int index = menuActor != null ? menuActor.Index() : 0;
            SmoothSelect(index);
        }

        public virtual bool FormationMode()
        {
            return _formationMode;
        }

        public virtual void SetFormationMode(bool formationMode)
        {
            _formationMode = formationMode;
        }

        public virtual int PendingIndex()
        {
            return _pendingIndex;
        }

        public virtual void SetPendingIndex(int index)
        {
            int lastPendingIndex = _pendingIndex;
            _pendingIndex = index;
            RedrawItem(_pendingIndex);
            RedrawItem(lastPendingIndex);
        }
    }

}