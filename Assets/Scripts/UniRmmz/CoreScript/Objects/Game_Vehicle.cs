using System;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for a vehicle.
    /// </summary>
    [Serializable]
    public partial class Game_Vehicle : Game_Character
    {
       
        public enum VehicleTypes
        {
            Boat = 0,
            Ship = 1,
            Airship = 2,
            Walk,
        }
        
        protected VehicleTypes _type;
        protected int _mapId;
        protected int _altitude;
        protected bool _driving;
        protected DataSystem.DataSound _bgm;

        protected Game_Vehicle(VehicleTypes type) : base()
        {
            _type = type;
            ResetDirection();
            InitMoveSpeed();
            LoadSystemSettings();
        }

        public override void InitMembers()
        {
            base.InitMembers();
            _type = VehicleTypes.Walk;
            _mapId = 0;
            _altitude = 0;
            _driving = false;
            _bgm = null;
        }

        public virtual bool IsBoat()
        {
            return _type == VehicleTypes.Boat;
        }

        public virtual bool IsShip()
        {
            return _type == VehicleTypes.Ship;
        }

        public virtual bool IsAirship()
        {
            return _type == VehicleTypes.Airship;
        }

        public virtual void ResetDirection()
        {
            SetDirection(4);
        }

        public virtual void InitMoveSpeed()
        {
            if (IsBoat())
            {
                SetMoveSpeed(4);
            }
            else if (IsShip())
            {
                SetMoveSpeed(5);
            }
            else if (IsAirship())
            {
                SetMoveSpeed(6);
            }
        }

        public virtual DataSystem.DataVehicle Vehicle()
        {
            if (IsBoat())
            {
                return Rmmz.dataSystem.Boat;
            }
            else if (IsShip())
            {
                return Rmmz.dataSystem.Ship;
            }
            else if (IsAirship())
            {
                return Rmmz.dataSystem.Airship;
            }
            else
            {
                return null;
            }
        }

        public virtual void LoadSystemSettings()
        {
            var vehicle = Vehicle();
            _mapId = vehicle.StartMapId;
            SetPosition(vehicle.StartX, vehicle.StartY);
            SetImage(vehicle.CharacterName, vehicle.CharacterIndex);
        }

        public virtual void Refresh()
        {
            if (_driving)
            {
                _mapId = Rmmz.gameMap.MapId();
                SyncWithPlayer();
            }
            else if (_mapId == Rmmz.gameMap.MapId())
            {
                Locate(X, Y);
            }

            if (IsAirship())
            {
                SetPriorityType(_driving ? 2 : 0);
            }
            else
            {
                SetPriorityType(1);
            }

            SetWalkAnime(_driving);
            SetStepAnime(_driving);
            SetTransparent(_mapId != Rmmz.gameMap.MapId());
        }

        public virtual void SetLocation(int mapId, int x, int y)
        {
            _mapId = mapId;
            SetPosition(x, y);
            Refresh();
        }

        public override bool Pos(int x, int y)
        {
            if (_mapId == Rmmz.gameMap.MapId())
            {
                return base.Pos(x, y);
            }
            else
            {
                return false;
            }
        }

        public override bool IsMapPassable(int x, int y, int d)
        {
            var x2 = Rmmz.gameMap.RoundXWithDirection(x, d);
            var y2 = Rmmz.gameMap.RoundYWithDirection(y, d);
            
            if (IsBoat())
            {
                return Rmmz.gameMap.IsBoatPassable(x2, y2);
            }
            else if (IsShip())
            {
                return Rmmz.gameMap.IsShipPassable(x2, y2);
            }
            else if (IsAirship())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void GetOn()
        {
            _driving = true;
            SetWalkAnime(true);
            SetStepAnime(true);
            Rmmz.gameSystem.SaveWalkingBgm();
            PlayBgm();
        }

        public virtual void GetOff()
        {
            _driving = false;
            SetWalkAnime(false);
            SetStepAnime(false);
            ResetDirection();
            Rmmz.gameSystem.ReplayWalkingBgm();
        }

        public virtual void SetBgm(DataSystem.DataSound bgm)
        {
            _bgm = bgm;
        }

        public virtual void PlayBgm()
        {
            Rmmz.AudioManager.PlayBgm(_bgm ?? Vehicle().Bgm);
        }

        public virtual void SyncWithPlayer()
        {
            CopyPosition(Rmmz.gamePlayer);
            RefreshBushDepth();
        }

        public override int ScreenY()
        {
            return base.ScreenY() - _altitude;
        }

        public virtual int ShadowX()
        {
            return ScreenX();
        }

        public virtual int ShadowY()
        {
            return ScreenY() + _altitude;
        }

        public virtual int ShadowOpacity()
        {
            return (255 * _altitude) / MaxAltitude();
        }

        public virtual bool CanMove()
        {
            if (IsAirship())
            {
                return IsHighest();
            }
            else
            {
                return true;
            }
        }

        public override void Update()
        {
            base.Update();
            if (IsAirship())
            {
                UpdateAirship();
            }
        }

        public virtual void UpdateAirship()
        {
            UpdateAirshipAltitude();
            SetStepAnime(IsHighest());
            SetPriorityType(IsLowest() ? 0 : 2);
        }

        public virtual void UpdateAirshipAltitude()
        {
            if (_driving && !IsHighest())
            {
                _altitude++;
            }
            if (!_driving && !IsLowest())
            {
                _altitude--;
            }
        }

        public virtual int MaxAltitude()
        {
            return 48;
        }

        public virtual bool IsLowest()
        {
            return _altitude <= 0;
        }

        public virtual bool IsHighest()
        {
            return _altitude >= MaxAltitude();
        }

        public virtual bool IsTakeoffOk()
        {
            return Rmmz.gamePlayer.AreFollowersGathered();
        }

        public virtual bool IsLandOk(int x, int y, int d)
        {
            if (IsAirship())
            {
                if (!Rmmz.gameMap.IsAirshipLandOk(x, y))
                {
                    return false;
                }
                if (Rmmz.gameMap.EventsXy(x, y).Count > 0)
                {
                    return false;
                }
            }
            else
            {
                var x2 = Rmmz.gameMap.RoundXWithDirection(x, d);
                var y2 = Rmmz.gameMap.RoundYWithDirection(y, d);
                if (!Rmmz.gameMap.IsValid(x2, y2))
                {
                    return false;
                }
                if (!Rmmz.gameMap.IsPassable(x2, y2, ReverseDir(d)))
                {
                    return false;
                }
                if (IsCollidedWithCharacters(x2, y2))
                {
                    return false;
                }
            }
            return true;
        }
    }
}