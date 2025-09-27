using System;
using System.Collections.Generic;

namespace UniRmmz
{
    /// <summary>
    // The game object class for the player. It contains event starting
    // determinants and map scrolling functions.
    /// </summary>
    [Serializable]
    public partial class Game_Player : Game_Character
    {
        protected Game_Vehicle.VehicleTypes _vehicleType = Game_Vehicle.VehicleTypes.Walk;
        protected bool _vehicleGettingOn = false;
        protected bool _vehicleGettingOff = false;
        protected bool _dashing = false;
        protected bool _needsMapReload = false;

        protected bool _transferring = false;
        protected int _newMapId = 0;
        protected int _newX = 0;
        protected int _newY = 0;
        protected int _newDirection = 0;
        protected int _fadeType = 0;
        protected Game_Followers _followers = Game_Followers.Create();
        protected float _encounterCount = 0;

        protected Game_Player()
        {
            SetTransparent(Rmmz.dataSystem.OptTransparent);
        }

        public virtual void ClearTransferInfo()
        {
            _transferring = false;
            _newMapId = 0;
            _newX = 0;
            _newY = 0;
            _newDirection = 0;
        }

        public virtual Game_Followers Followers() => _followers;

        public virtual void Refresh()
        {
            var actor = Rmmz.gameParty.Leader();
            var characterName = actor?.CharacterName() ?? string.Empty;
            var characterIndex = actor?.CharacterIndex() ?? 0;
            SetImage(characterName, characterIndex);
            _followers.Refresh();
        }

        public override bool IsStopping()
        {
            if (_vehicleGettingOn || _vehicleGettingOff)
            {
                return false;
            }

            return base.IsStopping();
        }

        public virtual void ReserveTransfer(int mapId, int x, int y, int d, int fadeType)
        {
            _transferring = true;
            _newMapId = mapId;
            _newX = x;
            _newY = y;
            _newDirection = d;
            _fadeType = fadeType;
        }
        
        public virtual void SetupForNewGame()
        {
            ReserveTransfer(Rmmz.dataSystem.StartMapId, Rmmz.dataSystem.StartX, Rmmz.dataSystem.StartY, 2, 0);
        }

        public virtual void RequestMapReload() => _needsMapReload = true;

        public virtual bool IsTransferring() => _transferring;

        public virtual int NewMapId() => _newMapId;

        public virtual int FadeType() => _fadeType;

        public virtual void PerformTransfer()
        {
            if (IsTransferring())
            {
                SetDirection(_newDirection);
                if (_newMapId != Rmmz.gameMap.MapId() || _needsMapReload)
                {
                    Rmmz.gameMap.Setup(_newMapId);
                    _needsMapReload = false;
                }
                Locate(_newX, _newY);
                Refresh();
                ClearTransferInfo();
            }
        }

        public override bool IsMapPassable(int x, int y, int d)
        {
            var vehicle = Vehicle();
            if (vehicle != null)
            {
                return vehicle.IsMapPassable(x, y, d);
            }
            else
            {
                return base.IsMapPassable(x, y, d);    
            }
        }

        public virtual Game_Vehicle Vehicle()
        {
            return Rmmz.gameMap.Vehicle(_vehicleType);
        }

        public virtual bool IsInBoat()
        {
            return _vehicleType == Game_Vehicle.VehicleTypes.Boat;
        }

        public virtual bool IsInShip()
        {
            return _vehicleType == Game_Vehicle.VehicleTypes.Ship;
        }

        public virtual bool IsInAirship()
        {
            return _vehicleType == Game_Vehicle.VehicleTypes.Airship;
        }

        public virtual bool IsInVehicle()
        {
            return IsInBoat() || IsInShip() || IsInAirship();
        }

        public virtual bool IsNormal()
        {
            return _vehicleType == Game_Vehicle.VehicleTypes.Walk && !IsMoveRouteForcing();
        }

        public override bool IsDashing()
        {
            return _dashing;
        }

        public override bool IsDebugThrough()
        {
            return Input.IsPressed("control") && Rmmz.gameTemp.IsPlaytest();
        }

        public virtual bool IsCollided(int x, int y)
        {
            if (IsThrough())
            {
                return false;
            }
            return Pos(x, y) || _followers.IsSomeoneCollided(x, y);
        }

        public virtual float CenterX()
        {
            return (Rmmz.gameMap.ScreenTileX() - 1) / 2;
        }

        public virtual float CenterY()
        {
            return (Rmmz.gameMap.ScreenTileY() - 1) / 2f;
        }

        public virtual void Center(float x, float y)
        {
            Rmmz.gameMap.SetDisplayPos(x - CenterX(), y - CenterY());
        }

        public override void Locate(int x, int y)
        {
            base.Locate(x, y);
            Center(x, y);
            MakeEncounterCount();
            if (IsInVehicle())
            {
                Vehicle().Refresh();
            }
            _followers.Synchronize(x, y, Direction());
        }

        public override void IncreaseSteps()
        {
            base.IncreaseSteps();
            if (IsNormal())
            {
                Rmmz.gameParty.IncreaseSteps();
            }
        }

        public virtual void MakeEncounterCount()
        {
            int n = Rmmz.gameMap.EncounterStep();
            _encounterCount = RmmzMath.RandomInt(n) + RmmzMath.RandomInt(n) + 1;
        }

        public virtual int MakeEncounterTroopId()
        {
            var encounterList = new List<Encounter>();
            int weightSum = 0;
            foreach (var encounter in Rmmz.gameMap.EncounterList())
            {
                if (MeetsEncounterConditions(encounter))
                {
                    encounterList.Add(encounter);
                    weightSum += encounter.Weight;
                }
            }
            if (weightSum > 0)
            {
                int value = RmmzMath.RandomInt(weightSum);
                foreach (var encounter in encounterList)
                {
                    if (MeetsEncounterConditions(encounter))
                    {
                        value -= encounter.Weight;
                        if (value < 0)
                        {
                            return encounter.TroopId;
                        }
                    }
                }
            }
            return 0;
        }

        public virtual bool MeetsEncounterConditions(Encounter encounter)
        {
            if (encounter.RegionSet.Count == 0)
            {
                return true;
            }
            return encounter.RegionSet.Contains(RegionId());
        }
        
        public virtual bool ExecuteEncounter()
        {
            if (!Rmmz.gameMap.IsEventRunning() && _encounterCount <= 0)
            {
                MakeEncounterCount();
                int troopId = MakeEncounterTroopId();
                if (Rmmz.dataTroops[troopId] != null)
                {
                    Rmmz.BattleManager.Setup(troopId, true, false);
                    Rmmz.BattleManager.OnEncounter();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        protected virtual void StartMapEvent(int x, int y, int[] triggeres, bool normal)
        {
            if (!Rmmz.gameMap.IsEventRunning())
            {
                foreach (var ev in Rmmz.gameMap.EventsXy(x, y))
                {
                    if (ev.IsTriggerIn(triggeres) && ev.IsNormalPriority() == normal)
                    {
                        ev.Start();
                    }
                }
            }
        }
        
        private void MoveByInput()
        {
            if (!IsMoving() && CanMove())
            {
                int direction = Input.Dir4;
                if (direction > 0)
                {
                    Rmmz.gameTemp.ClearDestination();
                }
                else if (Rmmz.gameTemp.IsDestinationValid())
                {
                    int x = Rmmz.gameTemp.DestinationX();
                    int y = Rmmz.gameTemp.DestinationY();
                    direction = FindDirectionTo(x, y);
                }
                if (direction > 0)
                {
                    ExecuteMove(direction);
                }
            }
        }
        
        public virtual bool CanMove()
        {
            if (Rmmz.gameMap.IsEventRunning() || Rmmz.gameMessage.IsBusy())
            {
                return false;
            }
            if (IsMoveRouteForcing() || Followers().AreGathering())
            {
                return false;
            }
            if (_vehicleGettingOn || _vehicleGettingOff)
            {
                return false;
            }
            if (IsInVehicle() && !Vehicle().CanMove())
            {
                return false;
            }
            return true;
        }

        protected virtual int GetInputDirection() => Input.Dir4;

        protected virtual void ExecuteMove(int direction)
        {
            MoveStraight(direction);
        }
        
        public virtual void Update(bool sceneActive)
        {
            var lastScrolledX = ScrolledX();
            var lastScrolledY = ScrolledY();
            var wasMoving = IsMoving();
            UpdateDashing();
            if (sceneActive)
            {
                MoveByInput();
            }
            base.Update();
            UpdateScroll(lastScrolledX, lastScrolledY);
            UpdateVehicle();
            if (!IsMoving())
            {
                UpdateNonmoving(wasMoving, sceneActive);
            }
            _followers.Update();
        }

        protected virtual void UpdateDashing()
        {
            if (IsMoving())
            {
                return;
            }
            if (CanMove() && !IsInVehicle() && !Rmmz.gameMap.IsDashDisabled())
            {
                _dashing = IsDashButtonPressed() || Rmmz.gameTemp.IsDestinationValid();
            }
            else
            {
                _dashing = false;
            }
        }

        protected virtual bool IsDashButtonPressed()
        {
            bool shift = Input.IsPressed("shift");
            return Rmmz.ConfigManager.AlwaysDash ? !shift : shift;
        }

        protected virtual void UpdateScroll(float lastScrolledX, float lastScrolledY)
        {
            float x1 = lastScrolledX;
            float y1 = lastScrolledY;
            float x2 = ScrolledX();
            float y2 = ScrolledY();
            if (y2 > y1 && y2 > CenterY())
            {
                Rmmz.gameMap.ScrollDown(y2 - y1);
            }
            if (x2 < x1 && x2 < CenterX())
            {
                Rmmz.gameMap.ScrollLeft(x1 - x2);
            }
            if (x2 > x1 && x2 > CenterX())
            {
                Rmmz.gameMap.ScrollRight(x2 - x1);
            }
            if (y2 < y1 && y2 < CenterY())
            {
                Rmmz.gameMap.ScrollUp(y1 - y2);
            }
        }

        protected virtual void UpdateVehicle()
        {
            if (IsInVehicle() && !AreFollowersGathering())
            {
                if (_vehicleGettingOn)
                {
                    UpdateVehicleGetOn();
                }
                else if (_vehicleGettingOff)
                {
                    UpdateVehicleGetOff();
                }
                else
                {
                    Vehicle().SyncWithPlayer();
                }
            }
        }

        protected virtual void UpdateVehicleGetOn()
        {
            if (!AreFollowersGathering() && !IsMoving())
            {
                SetDirection(Vehicle().Direction());
                SetMoveSpeed(Vehicle().MoveSpeed());
                _vehicleGettingOn = false;
                SetTransparent(true);
                if (IsInAirship())
                {
                    SetThrough(true);
                }
                Vehicle().GetOn();
            }
        }

        protected virtual void UpdateVehicleGetOff()
        {
            if (!AreFollowersGathering() && Vehicle().IsLowest())
            {
                _vehicleGettingOff = false;
                _vehicleType = Game_Vehicle.VehicleTypes.Walk;
                SetTransparent(false);
            }
        }

        protected virtual void UpdateNonmoving(bool wasMoving, bool sceneActive)
        {
            if (!Rmmz.gameMap.IsEventRunning())
            {
                if (wasMoving)
                {
                    Rmmz.gameParty.OnPlayerWalk();
                    CheckEventTriggerHere(new int[] { 1, 2 });
                    if (Rmmz.gameMap.SetupStartingEvent())
                    {
                        return;
                    }
                }
                if (sceneActive && TriggerAction())
                {
                    return;
                }
                if (wasMoving)
                {
                    UpdateEncounterCount();
                }
                else
                {
                    Rmmz.gameTemp.ClearDestination();
                }
            }
        }

        protected virtual bool TriggerAction()
        {
            if (CanMove())
            {
                if (TriggerButtonAction())
                {
                    return true;
                }

                if (TriggerTouchAction())
                {
                    return true;
                }
            }
            return false;
        }
        
        protected virtual bool TriggerButtonAction()
        {
            if (Input.IsTriggered("ok"))
            {
                if (GetOnOffVehicle())
                {
                    return true;
                }
                CheckEventTriggerHere(new int[] { 0 });
                if (Rmmz.gameMap.SetupStartingEvent())
                {
                    return true;
                }
                CheckEventTriggerThere(new int[] { 0, 1, 2 });
                if (Rmmz.gameMap.SetupStartingEvent())
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual  bool TriggerTouchAction()
        {
            if (Rmmz.gameTemp.IsDestinationValid())
            {
                int direction = Direction();
                int x1 = X;
                int y1 = Y;
                int x2 = Rmmz.gameMap.RoundXWithDirection(x1, direction);
                int y2 = Rmmz.gameMap.RoundYWithDirection(y1, direction);
                int x3 = Rmmz.gameMap.RoundXWithDirection(x2, direction);
                int y3 = Rmmz.gameMap.RoundYWithDirection(y2, direction);
                int destX = Rmmz.gameTemp.DestinationX();
                int destY = Rmmz.gameTemp.DestinationY();
                if (destX == x1 && destY == y1)
                {
                    return TriggerTouchActionD1(x1, y1);
                }
                else if (destX == x2 && destY == y2)
                {
                    return TriggerTouchActionD2(x2, y2);
                }
                else if (destX == x3 && destY == y3)
                {
                    return TriggerTouchActionD3(x2, y2);
                }
            }
            return false;
        }

        protected virtual  bool TriggerTouchActionD1(int x1, int y1)
        {
            if (Rmmz.gameMap.Airship().Pos(x1, y1))
            {
                if (TouchInput.IsTriggered() && GetOnOffVehicle())
                {
                    return true;
                }
            }
            CheckEventTriggerHere(new int[] { 0 });
            return Rmmz.gameMap.SetupStartingEvent();
        }

        protected virtual  bool TriggerTouchActionD2(int x2, int y2)
        {
            if (Rmmz.gameMap.Boat().Pos(x2, y2) || Rmmz.gameMap.Ship().Pos(x2, y2))
            {
                if (TouchInput.IsTriggered() && GetOnVehicle())
                {
                    return true;
                }
            }
            if (IsInBoat() || IsInShip())
            {
                if (TouchInput.IsTriggered() && GetOffVehicle())
                {
                    return true;
                }
            }
            CheckEventTriggerThere(new int[] { 0, 1, 2 });
            return Rmmz.gameMap.SetupStartingEvent();
        }

        protected virtual  bool TriggerTouchActionD3(int x2, int y2)
        {
            if (Rmmz.gameMap.IsCounter(x2, y2))
            {
                CheckEventTriggerThere(new int[] { 0, 1, 2 });
            }
            return Rmmz.gameMap.SetupStartingEvent();
        }

        public virtual  void UpdateEncounterCount()
        {
            if (CanEncounter())
            {
                _encounterCount -= EncounterProgressValue();
            }
        }

        public virtual  bool CanEncounter()
        {
            return (
                !Rmmz.gameParty.HasEncounterNone() &&
                Rmmz.gameSystem.IsEncounterEnabled() &&
                !IsInAirship() &&
                !IsMoveRouteForcing() &&
                !IsDebugThrough()
            );
        }

        public virtual float EncounterProgressValue()
        {
            float value = Rmmz.gameMap.IsBush(X, Y) ? 2 : 1;
            if (Rmmz.gameParty.HasEncounterHalf())
            {
                value *= 0.5f;
            }
            if (IsInShip())
            {
                value *= 0.5f;
            }
            return value;
        }

        public virtual  void CheckEventTriggerHere(int[] triggers)
        {
            if (CanStartLocalEvents())
            {
                StartMapEvent(X, Y, triggers, false);
            }
        }

        public virtual  void CheckEventTriggerThere(int[] triggers)
        {
            if (CanStartLocalEvents())
            {
                int direction = Direction();
                int x1 = X;
                int y1 = Y;
                int x2 = Rmmz.gameMap.RoundXWithDirection(x1, direction);
                int y2 = Rmmz.gameMap.RoundYWithDirection(y1, direction);
                StartMapEvent(x2, y2, triggers, true);
                if (!Rmmz.gameMap.IsAnyEventStarting() && Rmmz.gameMap.IsCounter(x2, y2))
                {
                    int x3 = Rmmz.gameMap.RoundXWithDirection(x2, direction);
                    int y3 = Rmmz.gameMap.RoundYWithDirection(y2, direction);
                    StartMapEvent(x3, y3, triggers, true);
                }
            }
        }

        public override void CheckEventTriggerTouch(int x, int y)
        {
            if (CanStartLocalEvents())
            {
                StartMapEvent(x, y, new int[] { 1, 2 }, true);
            }
        }

        public virtual  bool CanStartLocalEvents()
        {
            return !IsInAirship();
        }

        public virtual  bool GetOnOffVehicle()
        {
            if (IsInVehicle())
            {
                return GetOffVehicle();
            }
            else
            {
                return GetOnVehicle();
            }
        }

        public virtual  bool GetOnVehicle()
        {
            int direction = Direction();
            int x1 = X;
            int y1 = Y;
            int x2 = Rmmz.gameMap.RoundXWithDirection(x1, direction);
            int y2 = Rmmz.gameMap.RoundYWithDirection(y1, direction);
            if (Rmmz.gameMap.Airship().Pos(x1, y1))
            {
                _vehicleType = Game_Vehicle.VehicleTypes.Airship;
            }
            else if (Rmmz.gameMap.Ship().Pos(x2, y2))
            {
                _vehicleType = Game_Vehicle.VehicleTypes.Ship;
            }
            else if (Rmmz.gameMap.Boat().Pos(x2, y2))
            {
                _vehicleType = Game_Vehicle.VehicleTypes.Boat;
            }
            if (IsInVehicle())
            {
                _vehicleGettingOn = true;
                if (!IsInAirship())
                {
                    ForceMoveForward();
                }
                GatherFollowers();
            }
            return _vehicleGettingOn;
        }

        public virtual  bool GetOffVehicle()
        {
            if (Vehicle().IsLandOk(X, Y, Direction()))
            {
                if (IsInAirship())
                {
                    SetDirection(2);
                }
                _followers.Synchronize(X, Y, Direction());
                Vehicle().GetOff();
                if (!IsInAirship())
                {
                    ForceMoveForward();
                    SetTransparent(false);
                }
                _vehicleGettingOff = true;
                SetMoveSpeed(4);
                SetThrough(false);
                MakeEncounterCount();
                GatherFollowers();
            }
            return _vehicleGettingOff;
        }

        public virtual  void ForceMoveForward()
        {
            SetThrough(true);
            MoveForward();
            SetThrough(false);
        }

        public virtual  bool IsOnDamageFloor()
        {
            return Rmmz.gameMap.IsDamageFloor(X, Y) && !IsInAirship();
        }

        public override void MoveStraight(int d)
        {
            if (CanPass(X, Y, d))
            {
                _followers.UpdateMove();
            }
            base.MoveStraight(d);
        }

        public override void MoveDiagonally(int horz, int vert)
        {
            if (CanPassDiagonally(X, Y, horz, vert))
            {
                _followers.UpdateMove();
            }
            base.MoveDiagonally(horz, vert);
        }

        public override void Jump(int xPlus, int yPlus)
        {
            base.Jump(xPlus, yPlus);
            _followers.JumpAll();
        }

        public virtual  void ShowFollowers()
        {
            _followers.Show();
        }

        public virtual void HideFollowers()
        {
            _followers.Hide();
        }

        public virtual void GatherFollowers()
        {
            _followers.Gather();
        }

        public virtual bool AreFollowersGathering()
        {
            return _followers.AreGathering();
        }

        public virtual bool AreFollowersGathered()
        {
            return _followers.AreGathered();
        }
    }
}