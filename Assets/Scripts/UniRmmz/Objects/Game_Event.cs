using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for an event. It contains functionality for event page
    /// switching and running parallel process events.
    /// </summary>
    [Serializable]
    public partial class Game_Event : Game_Character
    {
        private int _mapId;
        private int _eventId;
        private int _moveType;
        private int _trigger;
        private bool _starting;
        private bool _erased;
        private int _pageIndex;
        private int _originalPattern;
        private int _originalDirection;
        private int _prelockDirection;
        private bool _locked;
        private Game_Interpreter _interpreter;

        protected Game_Event(int mapId, int eventId)
        {
            _mapId = mapId;
            _eventId = eventId;
            Locate(Event().X, Event().Y);
            Refresh();
        }

        public override void InitMembers()
        {
            base.InitMembers();
            _moveType = 0;
            _trigger = 0;
            _starting = false;
            _erased = false;
            _pageIndex = -2;
            _originalPattern = 1;
            _originalDirection = 2;
            _prelockDirection = 0;
            _locked = false;
        }

        public virtual int EventId() => _eventId;

        public virtual DataEvent Event() => Rmmz.DataMap.Events[_eventId];

        public virtual EventPage Page() => Event().Pages[_pageIndex];

        public virtual List<DataEventCommand> List() => Page().List;

        public override bool IsCollidedWithCharacters(int x, int y)
        {
            return base.IsCollidedWithCharacters(x, y) || IsCollidedWithPlayerCharacters(x, y);
        }

        public override bool IsCollidedWithEvents(int x, int y)
        {
            return Rmmz.gameMap.EventsXyNt(x, y).Count > 0;
        }

        public virtual bool IsCollidedWithPlayerCharacters(int x, int y)
        {
            return IsNormalPriority() && Rmmz.gamePlayer.IsCollided(x, y);
        }

        public virtual void Lock()
        {
            if (!_locked)
            {
                _prelockDirection = Direction();
                TurnTowardPlayer();
                _locked = true;
            }
        }

        public virtual void Unlock()
        {
            if (_locked)
            {
                _locked = false;
                SetDirection(_prelockDirection);
            }
        }

        public override void UpdateStop()
        {
            if (_locked)
            {
                ResetStopCount();
            }
            base.UpdateStop();
            if (!IsMoveRouteForcing())
            {
                UpdateSelfMovement();
            }
        }

        protected virtual void UpdateSelfMovement()
        {
            if (!_locked && IsNearTheScreen() && CheckStop(StopCountThreshold()))
            {
                switch (_moveType)
                {
                    case 1: MoveTypeRandom(); break;
                    case 2: MoveTypeTowardPlayer(); break;
                    case 3: MoveTypeCustom(); break;
                }
            }
        }

        protected virtual int StopCountThreshold()
        {
            return 30 * (5 - MoveFrequency());
        }

        protected virtual void MoveTypeRandom()
        {
            switch (UnityEngine.Random.Range(0, 6))
            {
                case 0:
                case 1:
                    MoveRandom();
                    break;
                case 2:
                case 3:
                case 4:
                    MoveForward();
                    break;
                case 5:
                    ResetStopCount();
                    break;
            }
        }

        protected virtual void MoveTypeTowardPlayer()
        {
            if (IsNearThePlayer())
            {
                switch (UnityEngine.Random.Range(0, 6))
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        MoveTowardPlayer();
                        break;
                    case 4:
                        MoveRandom();
                        break;
                    case 5:
                        MoveForward();
                        break;
                }
            }
            else
            {
                MoveRandom();
            }
        }

        protected virtual bool IsNearThePlayer()
        {
            int sx = Mathf.Abs(DeltaXFrom(Rmmz.gamePlayer.X));
            int sy = Mathf.Abs(DeltaYFrom(Rmmz.gamePlayer.Y));
            return (sx + sy) < 20;
        }

        protected virtual void MoveTypeCustom()
        {
            UpdateRoutineMove();
        }

        public virtual bool IsStarting() => _starting;

        public virtual void ClearStartingFlag()
        {
            _starting = false;
        }

        public virtual bool IsTriggerIn(IEnumerable<int> triggers)
        {
            return triggers.Contains(_trigger);
        }

        public virtual void Start()
        {
            var list = List();
            if (list != null && list.Count > 1)
            {
                _starting = true;
                if (IsTriggerIn(new List<int> { 0, 1, 2 }))
                {
                    Lock();
                }
            }
        }

        public virtual void Erase()
        {
            _erased = true;
            Refresh();
        }

        public virtual void Refresh()
        {
            int newPageIndex = _erased ? -1 : FindProperPageIndex();
            if (_pageIndex != newPageIndex)
            {
                _pageIndex = newPageIndex;
                SetupPage();
            }
        }

        protected virtual int FindProperPageIndex()
        {
            var pages = Event().Pages;
            for (int i = pages.Count - 1; i >= 0; i--)
            {
                if (MeetsConditions(pages[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        protected virtual bool MeetsConditions(EventPage page)
        {
            var c = page.Conditions;
            if (c.Switch1Valid && !Rmmz.gameSwitches.Value(c.Switch1Id)) return false;
            if (c.Switch2Valid && !Rmmz.gameSwitches.Value(c.Switch2Id)) return false;
            if (c.VariableValid && Rmmz.gameVariables.Value(c.VariableId) < c.VariableValue) return false;
            if (c.SelfSwitchValid)
            {
                var key = new Game_SelfSwitches.Key(_mapId, _eventId, c.SelfSwitchCh);
                if (!Rmmz.gameSelfSwitches.Value(key))
                {
                    return false;
                }
            }
            if (c.ItemValid)
            {
                var item = Rmmz.dataItems[c.ItemId];
                if (!Rmmz.gameParty.HasItem(item))
                {
                    return false;
                }
            }
            if (c.ActorValid)
            {
                var actor = Rmmz.gameActors.Actor(c.ActorId);
                if (!Rmmz.gameParty.Members().Contains(actor))
                {
                    return false;
                }
            }
            return true;
        }

        protected virtual void SetupPage()
        {
            if (_pageIndex >= 0)
            {
                SetupPageSettings();
            }
            else
            {
                ClearPageSettings();
            }
            RefreshBushDepth();
            ClearStartingFlag();
            CheckEventTriggerAuto();
        }

        protected virtual void ClearPageSettings()
        {
            SetImage("", 0);
            _moveType = 0;
            _trigger = -1;
            _interpreter = null;
            SetThrough(true);
        }

        protected virtual void SetupPageSettings()
        {
            var page = Page();
            var image = page.Image;
            if (image.TileId > 0)
            {
                SetTileImage(image.TileId);
            }
            else
            {
                SetImage(image.CharacterName, image.CharacterIndex);
            }
            if (_originalDirection != image.Direction)
            {
                _originalDirection = image.Direction;
                _prelockDirection = 0;
                SetDirectionFix(false);
                SetDirection(image.Direction);
            }
            if (_originalPattern != image.Pattern)
            {
                _originalPattern = image.Pattern;
                SetPattern(image.Pattern);
            }
            SetMoveSpeed(page.MoveSpeed);
            SetMoveFrequency(page.MoveFrequency);
            SetPriorityType(page.PriorityType);
            SetWalkAnime(page.WalkAnime);
            SetStepAnime(page.StepAnime);
            SetDirectionFix(page.DirectionFix);
            SetThrough(page.Through);
            SetMoveRoute(page.MoveRoute);
            _moveType = page.MoveType;
            _trigger = page.Trigger;
            _interpreter = (_trigger == 4) ? Game_Interpreter.Create() : null;
        }

        public override bool IsOriginalPattern() => Pattern() == _originalPattern;

        public override void ResetPattern()
        {
            SetPattern(_originalPattern);
        }

        public override void CheckEventTriggerTouch(int x, int y)
        {
            if (!Rmmz.gameMap.IsEventRunning())
            {
                if (_trigger == 2 && Rmmz.gamePlayer.Pos(x, y))
                {
                    if (!IsJumping() && IsNormalPriority())
                    {
                        Start();
                    }
                }
            }
        }

        public virtual void CheckEventTriggerAuto()
        {
            if (_trigger == 3)
            {
                Start();
            }
        }

        public override void Update()
        {
            base.Update();
            CheckEventTriggerAuto();
            UpdateParallel();
        }

        protected virtual void UpdateParallel()
        {
            if (_interpreter != null)
            {
                if (!_interpreter.IsRunning())
                {
                    _interpreter.Setup(List(), _eventId);
                }
                _interpreter.Update();
            }
        }

        public override void Locate(int x, int y)
        {
            base.Locate(x, y);
            _prelockDirection = 0;
        }

        public override void ForceMoveRoute(MoveRoute moveRoute)
        {
            base.ForceMoveRoute(moveRoute);
            _prelockDirection = 0;
        }
    }
}