using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of Game_Player, Game_Follower, GameVehicle, and Game_Event.
    /// </summary>
    [Serializable]
    public abstract partial class Game_Character : Game_CharacterBase
    {
        public enum RouteCodes
        {
            End = 0,
            MoveDown = 1,
            MoveLeft = 2,
            MoveRight = 3,
            MoveUp = 4,
            MoveLowerLeft = 5,
            MoveLowerRight = 6,
            MoveUpperLeft = 7,
            MoveUpperRight = 8,
            MoveRandom = 9,
            MoveToward = 10,
            MoveAway = 11,
            MoveForward = 12,
            MoveBackward = 13,
            Jump = 14,
            Wait = 15,
            TurnDown = 16,
            TurnLeft = 17,
            TurnRight = 18,
            TurnUp = 19,
            Turn90Right = 20,
            Turn90Left = 21,
            Turn180 = 22,
            Turn90RightOrLeft = 23,
            TurnRandom = 24,
            TurnToward = 25,
            TurnAway = 26,
            SwitchOn = 27,
            SwitchOff = 28,
            ChangeSpeed = 29,
            ChangeFrequency = 30,
            WalkAnimeOn = 31,
            WalkAnimeOff = 32,
            StepAnimeOn = 33,
            StepAnimeOff = 34,
            DirectionFixOn = 35,
            DirectionFixOff = 36,
            ThroughOn = 37,
            ThroughOff = 38,
            TransparentOn = 39,
            TransparentOff = 40,
            ChangeImage = 41,
            ChangeOpacity = 42,
            ChangeBlendMode = 43,
            PlaySe = 44,
            Script = 45
        }

        protected bool _moveRouteForcing;
        protected MoveRoute _moveRoute;
        protected int _moveRouteIndex;
        protected MoveRoute _originalMoveRoute;
        protected int _originalMoveRouteIndex;
        protected int _waitCount;

        private class Node
        {
            public Node Parent;
            public int X;
            public int Y;
            public int G;
            public int F;

            public Node()
            {
            }

            public Node(Node parent, int x, int y, int g, int f)
            {
                Parent = parent;
                X = x;
                Y = y;
                G = g;
                F = f;
            }
        }

        public override void InitMembers()
        {
            base.InitMembers();
            _moveRouteForcing = false;
            _moveRoute = null;
            _moveRouteIndex = 0;
            _originalMoveRoute = null;
            _originalMoveRouteIndex = 0;
            _waitCount = 0;
        }

        public virtual void MemorizeMoveRoute()
        {
            _originalMoveRoute = _moveRoute;
            _originalMoveRouteIndex = _moveRouteIndex;
        }

        public virtual void RestoreMoveRoute()
        {
            _moveRoute = _originalMoveRoute;
            _moveRouteIndex = _originalMoveRouteIndex;
            _originalMoveRoute = null;
        }

        public virtual bool IsMoveRouteForcing() => _moveRouteForcing;

        public virtual void SetMoveRoute(MoveRoute moveRoute)
        {
            if (_moveRouteForcing)
            {
                _originalMoveRoute = moveRoute;
                _originalMoveRouteIndex = 0;
            }
            else
            {
                _moveRoute = moveRoute;
                _moveRouteIndex = 0;
            }
        }

        public virtual void ForceMoveRoute(MoveRoute moveRoute)
        {
            if (_originalMoveRoute == null)
            {
                MemorizeMoveRoute();
            }

            _moveRoute = moveRoute;
            _moveRouteIndex = 0;
            _moveRouteForcing = true;
            _waitCount = 0;
        }

        public override void UpdateStop()
        {
            base.UpdateStop();
            if (_moveRouteForcing)
            {
                UpdateRoutineMove();
            }
        }

        public virtual void UpdateRoutineMove()
        {
            if (_waitCount > 0)
            {
                _waitCount--;
            }
            else
            {
                SetMovementSuccess(true);
                var command = _moveRoute.List[_moveRouteIndex];
                if (command != null)
                {
                    ProcessMoveCommand(command);
                    AdvanceMoveRouteIndex();
                }    
            }
        }

        public void ProcessMoveCommand(MoveCommand command)
        {
            var p = command.Parameters;
            switch ((RouteCodes)command.Code)
            {
                case RouteCodes.End:
                    ProcessRouteEnd();
                    break;
                case RouteCodes.MoveDown:
                    MoveStraight(2);
                    break;
                case RouteCodes.MoveLeft:
                    MoveStraight(4);
                    break;
                case RouteCodes.MoveRight:
                    MoveStraight(6);
                    break;
                case RouteCodes.MoveUp:
                    MoveStraight(8);
                    break;
                case RouteCodes.MoveLowerLeft:
                    MoveDiagonally(4, 2);
                    break;
                case RouteCodes.MoveLowerRight:
                    MoveDiagonally(6, 2);
                    break;
                case RouteCodes.MoveUpperLeft:
                    MoveDiagonally(4, 8);
                    break;
                case RouteCodes.MoveUpperRight:
                    MoveDiagonally(6, 8);
                    break;
                case RouteCodes.MoveRandom:
                    MoveRandom();
                    break;
                case RouteCodes.MoveToward:
                    MoveTowardPlayer();
                    break;
                case RouteCodes.MoveAway:
                    MoveAwayFromPlayer();
                    break;
                case RouteCodes.MoveForward:
                    MoveForward();
                    break;
                case RouteCodes.MoveBackward:
                    MoveBackward();
                    break;
                case RouteCodes.Jump:
                    Jump((int)p[0], (int)p[1]);
                    break;
                case RouteCodes.Wait:
                    _waitCount = (int)p[0] - 1;
                    break;
                case RouteCodes.TurnDown:
                    SetDirection(2);
                    break;
                case RouteCodes.TurnLeft:
                    SetDirection(4);
                    break;
                case RouteCodes.TurnRight:
                    SetDirection(6);
                    break;
                case RouteCodes.TurnUp:
                    SetDirection(8);
                    break;
                case RouteCodes.Turn90Right:
                    TurnRight90();
                    break;
                case RouteCodes.Turn90Left:
                    TurnLeft90();
                    break;
                case RouteCodes.Turn180:
                    Turn180();
                    break;
                case RouteCodes.Turn90RightOrLeft:
                    TurnRightOrLeft90();
                    break;
                case RouteCodes.TurnRandom:
                    TurnRandom();
                    break;
                case RouteCodes.TurnToward:
                    TurnTowardPlayer();
                    break;
                case RouteCodes.TurnAway:
                    TurnAwayFromPlayer();
                    break;
                case RouteCodes.SwitchOn:
                    Rmmz.gameSwitches.SetValue((int)p[0], true);
                    break;
                case RouteCodes.SwitchOff:
                    Rmmz.gameSwitches.SetValue((int)p[0], false);
                    break;
                case RouteCodes.ChangeSpeed:
                    SetMoveSpeed((int)p[0]);
                    break;
                case RouteCodes.ChangeFrequency:
                    SetMoveFrequency((int)p[0]);
                    break;
                case RouteCodes.WalkAnimeOn:
                    SetWalkAnime(true);
                    break;
                case RouteCodes.WalkAnimeOff:
                    SetWalkAnime(false);
                    break;
                case RouteCodes.StepAnimeOn:
                    SetStepAnime(true);
                    break;
                case RouteCodes.StepAnimeOff:
                    SetStepAnime(false);
                    break;
                case RouteCodes.DirectionFixOn:
                    SetDirectionFix(true);
                    break;
                case RouteCodes.DirectionFixOff:
                    SetDirectionFix(false);
                    break;
                case RouteCodes.ThroughOn:
                    SetThrough(true);
                    break;
                case RouteCodes.ThroughOff:
                    SetThrough(false);
                    break;
                case RouteCodes.TransparentOn:
                    SetTransparent(true);
                    break;
                case RouteCodes.TransparentOff:
                    SetTransparent(false);
                    break;
                case RouteCodes.ChangeImage:
                    SetImage((string)p[0], (int)p[1]);
                    break;
                case RouteCodes.ChangeOpacity:
                    SetOpacity((int)p[0]);
                    break;
                case RouteCodes.ChangeBlendMode:
                    SetBlendMode((int)p[0]);
                    break;
                case RouteCodes.PlaySe:
                    Rmmz.AudioManager.PlaySe(ConvertEx.ToSoundData(p[0]));
                    break;
                case RouteCodes.Script:
                    //GameInterpreter.Eval((string)p[0]);
                    break;
            }
        }

        public virtual int DeltaXFrom(int x) => Rmmz.gameMap.DeltaX(this.X, x);

        public virtual int DeltaYFrom(int y) => Rmmz.gameMap.DeltaY(this.Y, y);

        public virtual void MoveRandom()
        {
            int d = 2 + RmmzMath.RandomInt(4) * 2;
            if (CanPass(X, Y, d))
            {
                MoveStraight(d);
            }
        }

        public virtual void MoveTowardCharacter(Game_CharacterBase character)
        {
            int sx = DeltaXFrom(character.X);
            int sy = DeltaYFrom(character.Y);
            if (Mathf.Abs(sx) > Mathf.Abs(sy))
            {
                MoveStraight(sx > 0 ? 4 : 6);
                if (!IsMovementSucceeded() && sy != 0)
                {
                    MoveStraight(sy > 0 ? 8 : 2);
                }
            }
            else if (sy != 0)
            {
                MoveStraight(sy > 0 ? 8 : 2);
                if (!IsMovementSucceeded() && sx != 0)
                {
                    MoveStraight(sx > 0 ? 4 : 6);
                }
            }
        }

        public virtual void MoveAwayFromCharacter(Game_CharacterBase character)
        {
            int sx = DeltaXFrom(character.X);
            int sy = DeltaYFrom(character.Y);
            if (Mathf.Abs(sx) > Mathf.Abs(sy))
            {
                MoveStraight(sx > 0 ? 6 : 4);
                if (!IsMovementSucceeded() && sy != 0)
                {
                    MoveStraight(sy > 0 ? 2 : 8);
                }
            }
            else if (sy != 0)
            {
                MoveStraight(sy > 0 ? 2 : 8);
                if (!IsMovementSucceeded() && sx != 0)
                {
                    MoveStraight(sx > 0 ? 6 : 4);
                }
            }
        }

        public virtual void TurnTowardCharacter(Game_CharacterBase character)
        {
            int sx = DeltaXFrom(character.X);
            int sy = DeltaYFrom(character.Y);
            if (Mathf.Abs(sx) > Mathf.Abs(sy))
            {
                SetDirection(sx > 0 ? 4 : 6);
            }
            else if (sy != 0)
            {
                SetDirection(sy > 0 ? 8 : 2);
            }
        }

        public virtual void TurnAwayFromCharacter(Game_CharacterBase character)
        {
            int sx = DeltaXFrom(character.X);
            int sy = DeltaYFrom(character.Y);
            if (Mathf.Abs(sx) > Mathf.Abs(sy))
            {
                SetDirection(sx > 0 ? 6 : 4);
            }
            else if (sy != 0)
            {
                SetDirection(sy > 0 ? 2 : 8);
            }
        }

        public virtual void TurnTowardPlayer() => TurnTowardCharacter(Rmmz.gamePlayer);

        public virtual void TurnAwayFromPlayer() => TurnAwayFromCharacter(Rmmz.gamePlayer);

        public virtual void MoveTowardPlayer() => MoveTowardCharacter(Rmmz.gamePlayer);

        public virtual void MoveAwayFromPlayer() => MoveAwayFromCharacter(Rmmz.gamePlayer);

        public virtual void MoveForward() => MoveStraight(Direction());

        public virtual void MoveBackward()
        {
            bool lastDirectionFix = IsDirectionFixed();
            SetDirectionFix(true);
            MoveStraight(ReverseDir(Direction()));
            SetDirectionFix(lastDirectionFix);
        }
        
        public virtual void ProcessRouteEnd()
        {
            if (_moveRoute.Repeat)
            {
                _moveRouteIndex = -1;
            }
            else if (_moveRouteForcing)
            {
                _moveRouteForcing = false;
                RestoreMoveRoute();
                SetMovementSuccess(false);
            }
        }

        public virtual void AdvanceMoveRouteIndex()
        {
            if (_moveRoute != null && (IsMovementSucceeded() || _moveRoute.Skippable))
            {
                int numCommands = _moveRoute.List.Count - 1;
                _moveRouteIndex++;
                if (_moveRoute.Repeat && _moveRouteIndex >= numCommands)
                {
                    _moveRouteIndex = 0;
                }
            }
        }

        public virtual void TurnRight90()
        {
            switch (Direction())
            {
                case 2:
                    SetDirection(4);
                    break;
                case 4:
                    SetDirection(8);
                    break;
                case 6:
                    SetDirection(2);
                    break;
                case 8:
                    SetDirection(6);
                    break;
            }
        }

        public virtual void TurnLeft90()
        {
            switch (Direction())
            {
                case 2:
                    SetDirection(6);
                    break;
                case 4:
                    SetDirection(2);
                    break;
                case 6:
                    SetDirection(8);
                    break;
                case 8:
                    SetDirection(4);
                    break;
            }
        }

        public virtual void Turn180() => SetDirection(ReverseDir(Direction()));

        public virtual void TurnRightOrLeft90()
        {
            if (RmmzMath.RandomInt(2) == 0)
                TurnRight90();
            else
                TurnLeft90();
        }

        public virtual void TurnRandom() => SetDirection(2 + RmmzMath.RandomInt(4) * 2);

        public virtual void Swap(Game_Character character)
        {
            int newX = character.X;
            int newY = character.Y;
            character.Locate(X, Y);
            Locate(newX, newY);
        }

        public virtual int FindDirectionTo(int goalX, int goalY)
        {
            int searchLimit = SearchLimit();
            int mapWidth = Rmmz.gameMap.Width();
            var nodeList = new List<Node>();
            var openList = new HashSet<int>();
            var closedList = new HashSet<int>();
            var start = new Node(null, this.X, this.Y, 0, Rmmz.gameMap.Distance(this.X, this.Y, goalX, goalY));
            Node best = start;

            if (this.X == goalX && this.Y == goalY)
            {
                return 0;
            }

            nodeList.Add(start);
            openList.Add(start.Y * mapWidth + start.X);

            while (nodeList.Count > 0)
            {
                int bestIndex = 0;
                for (int i = 1; i < nodeList.Count; i++)
                {
                    if (nodeList[i].F < nodeList[bestIndex].F)
                    {
                        bestIndex = i;
                    }
                }

                var current = nodeList[bestIndex];
                int x1 = current.X;
                int y1 = current.Y;
                int pos1 = y1 * mapWidth + x1;
                int g1 = current.G;

                nodeList.RemoveAt(bestIndex);
                openList.Remove(pos1);
                closedList.Add(pos1);

                if (x1 == goalX && y1 == goalY)
                {
                    best = current;
                    break;
                }

                if (g1 >= searchLimit)
                {
                    continue;
                }

                for (int j = 0; j < 4; j++)
                {
                    int direction = 2 + j * 2;
                    int x2 = Rmmz.gameMap.RoundXWithDirection(x1, direction);
                    int y2 = Rmmz.gameMap.RoundYWithDirection(y1, direction);
                    int pos2 = y2 * mapWidth + x2;

                    if (closedList.Contains(pos2)) continue;
                    if (!this.CanPass(x1, y1, direction)) continue;

                    int g2 = g1 + 1;
                    Node neighbor = nodeList.Find(n => n.X == x2 && n.Y == y2);
                    if (neighbor == null || g2 < neighbor.G)
                    {
                        if (neighbor == null)
                        {
                            neighbor = new Node();
                            nodeList.Add(neighbor);
                            openList.Add(pos2);
                        }

                        neighbor.Parent = current;
                        neighbor.X = x2;
                        neighbor.Y = y2;
                        neighbor.G = g2;
                        neighbor.F = g2 + Rmmz.gameMap.Distance(x2, y2, goalX, goalY);

                        if (best == null || neighbor.F - neighbor.G < best.F - best.G)
                        {
                            best = neighbor;
                        }
                    }
                }
            }

            Node nodePath = best;
            while (nodePath.Parent != null && nodePath.Parent != start)
            {
                nodePath = nodePath.Parent;
            }

            int deltaX1 = Rmmz.gameMap.DeltaX(nodePath.X, start.X);
            int deltaY1 = Rmmz.gameMap.DeltaY(nodePath.Y, start.Y);
            if (deltaY1 > 0) return 2;
            if (deltaX1 < 0) return 4;
            if (deltaX1 > 0) return 6;
            if (deltaY1 < 0) return 8;

            int deltaX2 = this.DeltaXFrom(goalX);
            int deltaY2 = this.DeltaYFrom(goalY);
            if (Mathf.Abs(deltaX2) > Mathf.Abs(deltaY2)) return deltaX2 > 0 ? 4 : 6;
            if (deltaY2 != 0) return deltaY2 > 0 ? 8 : 2;

            return 0;
        }

        public virtual int SearchLimit() => 12;
    }
}