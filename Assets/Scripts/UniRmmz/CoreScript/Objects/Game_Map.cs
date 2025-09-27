using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for a map. It contains scrolling and passage
    /// determination functions.
    /// </summary>
    [Serializable]
    public partial class Game_Map
    {
        protected Game_Interpreter _interpreter;
        protected int _mapId;
        protected int _tilesetId;
        protected List<Game_Event> _events;
        protected List<Game_CommonEvent> _commonEvents;
        protected List<Game_Vehicle> _vehicles;
        protected float _displayX;
        protected float _displayY;
        protected bool _nameDisplay;
        protected int _scrollDirection;
        protected float _scrollRest;
        protected int _scrollSpeed;
        protected string _parallaxName;
        protected bool _parallaxZero;
        protected bool _parallaxLoopX;
        protected bool _parallaxLoopY;
        protected float _parallaxSx;
        protected float _parallaxSy;
        protected float _parallaxX;
        protected float _parallaxY;
        protected string _battleback1Name;
        protected string _battleback2Name;
        protected bool _needsRefresh;
        protected List<Game_Event> _tileEvents;

        protected Game_Map()
        {
            _interpreter = Game_Interpreter.Create();
            _mapId = 0;
            _tilesetId = 0;
            _events = new List<Game_Event>();
            _commonEvents = new List<Game_CommonEvent>();
            _vehicles = new List<Game_Vehicle>();
            _displayX = 0;
            _displayY = 0;
            _nameDisplay = true;
            _scrollDirection = 2;
            _scrollRest = 0;
            _scrollSpeed = 4;
            _parallaxName = "";
            _parallaxZero = false;
            _parallaxLoopX = false;
            _parallaxLoopY = false;
            _parallaxSx = 0;
            _parallaxSy = 0;
            _parallaxX = 0;
            _parallaxY = 0;
            _battleback1Name = null;
            _battleback2Name = null;
            CreateVehicles();
        }

        public virtual void Setup(int mapId)
        {
            if (Rmmz.dataMap == null)
            {
                throw new Exception("The map data is not available");
            }
            _mapId = mapId;
            _tilesetId = Rmmz.dataMap.TilesetId;
            _displayX = 0;
            _displayY = 0;
            RefreshVehicles();
            SetupEvents();
            SetupScroll();
            SetupParallax();
            SetupBattleback();
            _needsRefresh = false;
        }

        public virtual bool IsEventRunning()
        {
            return _interpreter.IsRunning() || IsAnyEventStarting();
        }

        public virtual int TileWidth()
        {
            if (Rmmz.dataSystem.TileSize > 0)
            {
                return Rmmz.dataSystem.TileSize;
            }
            return 48;
        }

        public virtual int TileHeight() => TileWidth();
        public virtual float BushDepth() => TileHeight() / 4f;
        public virtual int MapId() => _mapId;
        public virtual int TilesetId() => _tilesetId;
        public virtual float DisplayX() => _displayX;
        public virtual float DisplayY() => _displayY;
        public virtual string ParallaxName() => _parallaxName;
        public virtual string Battleback1Name() => _battleback1Name;
        public virtual string Battleback2Name() => _battleback2Name;
        public virtual void RequestRefresh() { _needsRefresh = true; }
        public virtual bool IsNameDisplayEnabled() => _nameDisplay;
        public virtual void DisableNameDisplay() { _nameDisplay = false; }
        public virtual void EnableNameDisplay() { _nameDisplay = true; }

        public virtual void CreateVehicles()
        {
            _vehicles.Add(Game_Vehicle.Create(Game_Vehicle.VehicleTypes.Boat));
            _vehicles.Add(Game_Vehicle.Create(Game_Vehicle.VehicleTypes.Ship));
            _vehicles.Add(Game_Vehicle.Create(Game_Vehicle.VehicleTypes.Airship));
        }

        public virtual void RefreshVehicles()
        {
            foreach (var v in _vehicles)
            {
                v.Refresh();
            }
        }

        public virtual IEnumerable<Game_Vehicle> Vehicles() => _vehicles;

        public virtual Game_Vehicle Vehicle(Game_Vehicle.VehicleTypes type)
        {
            return type switch
            {
                Game_Vehicle.VehicleTypes.Boat => Boat(),
                Game_Vehicle.VehicleTypes.Ship => Ship(),
                Game_Vehicle.VehicleTypes.Airship => Airship(),
                _ => null
            };
        }

        public virtual Game_Vehicle Boat() => _vehicles[0];
        public virtual Game_Vehicle Ship() => _vehicles[1];
        public virtual Game_Vehicle Airship() => _vehicles[2];

        public virtual void SetupEvents()
        {
            _events = new List<Game_Event>(Enumerable.Repeat<Game_Event>(null, Rmmz.dataMap.Events.Count));
            _commonEvents = new List<Game_CommonEvent>();

            for (int i = 0; i < Rmmz.dataMap.Events.Count; i++)
            {
                var ev = Rmmz.dataMap.Events[i];
                if (ev != null && ev.Id != 0)
                {
                    _events[ev.Id] = Game_Event.Create(_mapId, ev.Id);
                }
            }

            foreach (var ce in ParallelCommonEvents())
            {
                _commonEvents.Add(Game_CommonEvent.Create(ce.Id));
            }

            RefreshTileEvents();
        }

        public virtual List<Game_Event> Events()
        {
            return _events.Where(e => e != null).ToList();
        }

        public virtual Game_Event Event(int eventId)
        {
            return _events[eventId];
        }
        
        
        public virtual void EraseEvent(int eventId)
        {
            _events[eventId]?.Erase();
        }

        public virtual IEnumerable<DataCommonEvent> AutorunCommonEvents()
        {
            return Rmmz.dataCommonEvents.Where(e => e != null && e.Trigger == 1);
        }

        public virtual IEnumerable<DataCommonEvent> ParallelCommonEvents()
        {
            return Rmmz.dataCommonEvents.Where(e => e != null && e.Trigger == 2);
        }

        public virtual void SetupScroll()
        {
            _scrollDirection = 2;
            _scrollRest = 0;
            _scrollSpeed = 4;
        }

        public virtual void SetupParallax()
        {
            _parallaxName = Rmmz.dataMap.ParallaxName ?? "";
            _parallaxZero = Rmmz.ImageManager.IsZeroParallax(_parallaxName);
            _parallaxLoopX = Rmmz.dataMap.ParallaxLoopX;
            _parallaxLoopY = Rmmz.dataMap.ParallaxLoopY;
            _parallaxSx = Rmmz.dataMap.ParallaxSx;
            _parallaxSy = Rmmz.dataMap.ParallaxSy;
            _parallaxX = 0;
            _parallaxY = 0;
        }

        public virtual void SetupBattleback()
        {
            if (Rmmz.dataMap.SpecifyBattleback)
            {
                _battleback1Name = Rmmz.dataMap.Battleback1Name;
                _battleback2Name = Rmmz.dataMap.Battleback2Name;
            }
            else
            {
                _battleback1Name = null;
                _battleback2Name = null;
            }
        }

        public virtual void SetDisplayPos(float x, float y)
        {
            if (IsLoopHorizontal())
            {
                _displayX = (x % Width());
                _parallaxX = x;
            }
            else
            {
                var endX = Width() - ScreenTileX();
                _displayX = endX < 0 ? endX / 2 : Math.Clamp(x, 0, endX);
                _parallaxX = _displayX;
            }

            if (IsLoopVertical())
            {
                _displayY = (y % Height());
                _parallaxY = y;
            }
            else
            {
                var endY = Height() - ScreenTileY();
                _displayY = endY < 0 ? endY / 2 : Math.Clamp(y, 0, endY);
                _parallaxY = _displayY;
            }
        }

        public virtual float ParallaxOx() =>
            _parallaxZero 
                ? _parallaxX * TileWidth() 
                : (_parallaxLoopX ? (_parallaxX * TileWidth()) / 2 : 0);

        public virtual float ParallaxOy() =>
            _parallaxZero 
                ? _parallaxY * TileHeight() 
                : (_parallaxLoopY ? (_parallaxY * TileHeight()) / 2 : 0);

        public virtual DataTileset Tileset() => Rmmz.dataTilesets[_tilesetId];

        public virtual int[] TilesetFlags() => Tileset()?.Flags ?? Array.Empty<int>();
        
        public virtual string DisplayName() => Rmmz.dataMap.DisplayName;

        public virtual int Width() => Rmmz.dataMap.Width;

        public virtual int Height() => Rmmz.dataMap.Height;

        public virtual int[] Data() => Rmmz.dataMap.Data;

        public virtual bool IsLoopHorizontal() => Rmmz.dataMap.ScrollType == 2 || Rmmz.dataMap.ScrollType == 3;

        public virtual bool IsLoopVertical() => Rmmz.dataMap.ScrollType == 1 || Rmmz.dataMap.ScrollType == 3;

        public virtual bool IsDashDisabled() => Rmmz.dataMap.DisableDashing;

        public virtual List<Encounter> EncounterList() => Rmmz.dataMap.EncounterList;

        public virtual int EncounterStep() => Rmmz.dataMap.EncounterStep;

        public virtual bool IsOverworld() => Tileset()?.Mode == 0;

        public virtual float ScreenTileX() => Mathf.Round(((float)Graphics.Width / TileWidth()) * 16) / 16;

        public virtual float ScreenTileY() => Mathf.Round(((float)Graphics.Height / TileHeight()) * 16) / 16;

        public virtual float AdjustX(float x)
        {
            if (IsLoopHorizontal() && x < _displayX - (Width() - ScreenTileX()) / 2)
            {
                return x - _displayX + Rmmz.dataMap.Width;
            }
            else
            {
                return x - _displayX;
            }
        }

        public virtual float AdjustY(float y)
        {
            if (IsLoopVertical() && y < _displayY - (Height() - ScreenTileY()) / 2)
            {
                return y - _displayY + Rmmz.dataMap.Height;
            }
            else
            {
                return y - _displayY;
            }
        }

        public virtual int RoundX(int x) => IsLoopHorizontal() ? (x % Width()) : x;

        public virtual int RoundY(int y) => IsLoopVertical() ? (y % Height()) : y;

        public virtual int XWithDirection(int x, int d) => x + (d == 6 ? 1 : d == 4 ? -1 : 0);

        public virtual int YWithDirection(int y, int d) => y + (d == 2 ? 1 : d == 8 ? -1 : 0);

        public virtual int RoundXWithDirection(int x, int d) => RoundX(XWithDirection(x, d));

        public virtual int RoundYWithDirection(int y, int d) => RoundY(YWithDirection(y, d));

        public virtual int DeltaX(int x1, int x2)
        {
            int result = x1 - x2;
            if (IsLoopHorizontal() && Math.Abs(result) > Width() / 2)
            {
                result = result < 0 ? result + Width() : result - Width();
            }
            return result;
        }

        public virtual int DeltaY(int y1, int y2)
        {
            int result = y1 - y2;
            if (IsLoopVertical() && Math.Abs(result) > Height() / 2)
            {
                result = result < 0 ? result + Height() : result - Height();
            }
            return result;
        }

        public virtual int Distance(int x1, int y1, int x2, int y2) =>
            Math.Abs(DeltaX(x1, x2)) + Math.Abs(DeltaY(y1, y2));

        public virtual int CanvasToMapX(float x)
        {
            float tileWidth = TileWidth();
            float originX = _displayX * tileWidth;
            int mapX = (int)Math.Floor((originX + x) / tileWidth);
            return RoundX(mapX);
        }

        public virtual int CanvasToMapY(float y)
        {
            float tileHeight = TileHeight();
            float originY = _displayY * tileHeight;
            int mapY = (int)Math.Floor((originY + y) / tileHeight);
            return RoundY(mapY);
        }

        public virtual void Autoplay()
        {
            if (Rmmz.dataMap.AutoplayBgm)
            {
                if (Rmmz.gamePlayer.IsInVehicle())
                {
                    Rmmz.gameSystem.SaveWalkingBgm2();
                }
                else
                {
                    Rmmz.AudioManager.PlayBgm(Rmmz.dataMap.Bgm);
                }
            }
            if (Rmmz.dataMap.AutoplayBgs)
            {
                Rmmz.AudioManager.PlayBgs(Rmmz.dataMap.Bgs);
            }
        }

        public virtual void RefreshIfNeeded()
        {
            if (_needsRefresh)
            {
                Refresh();
            }
        }

        public virtual void Refresh()
        {
            foreach (var ev in Events())
            {
                ev.Refresh();
            }
            foreach (var ce in _commonEvents)
            {
                ce.Refresh();
            }
            RefreshTileEvents();
            _needsRefresh = false;
        }

        public virtual void RefreshTileEvents()
        {
            _tileEvents = Events().Where(e => e.IsTile()).ToList();
        }

        public virtual List<Game_Event> EventsXy(int x, int y) => Events().Where(e => e.Pos(x, y)).ToList();

        public virtual List<Game_Event> EventsXyNt(int x, int y) => Events().Where(e => e.PosNt(x, y)).ToList();

        public virtual List<Game_Event> TileEventsXy(int x, int y) => _tileEvents.Where(e => e.PosNt(x, y)).ToList();

        public virtual int EventIdXy(int x, int y)
        {
            var list = EventsXy(x, y);
            return list.Count == 0 ? 0 : list[0].EventId();
        }
        
        public virtual void ScrollDown(float distance)
        {
            if (IsLoopVertical())
            {
                _displayY += distance;
                _displayY %= Rmmz.dataMap.Height;
                if (_parallaxLoopY) { _parallaxY += distance; }
            }
            else if (Height() >= ScreenTileY())
            {
                float lastY = _displayY;
                _displayY = Mathf.Min(_displayY + distance, Height() - ScreenTileY());
                _parallaxY += _displayY - lastY;
            }
        }

        public virtual void ScrollLeft(float distance)
        {
            if (IsLoopHorizontal())
            {
                _displayX += Rmmz.dataMap.Width - distance;
                _displayX %= Rmmz.dataMap.Width;
                if (_parallaxLoopX) { _parallaxX -= distance; }
            }
            else if (Width() >= ScreenTileX())
            {
                float lastX = _displayX;
                _displayX = Mathf.Max(_displayX - distance, 0);
                _parallaxX += _displayX - lastX;
            }
        }

        public virtual void ScrollRight(float distance)
        {
            if (IsLoopHorizontal())
            {
                _displayX += distance;
                _displayX %= Rmmz.dataMap.Width;
                if (_parallaxLoopX) { _parallaxX += distance; }
            }
            else if (Width() >= ScreenTileX())
            {
                float lastX = _displayX;
                _displayX = Mathf.Min(_displayX + distance, Width() - ScreenTileX());
                _parallaxX += _displayX - lastX;
            }
        }

        public virtual void ScrollUp(float distance)
        {
            if (IsLoopVertical())
            {
                _displayY += Rmmz.dataMap.Height - distance;
                _displayY %= Rmmz.dataMap.Height;
                if (_parallaxLoopY) { _parallaxY -= distance; }
            }
            else if (Height() >= ScreenTileY())
            {
                float lastY = _displayY;
                _displayY = Mathf.Max(_displayY - distance, 0);
                _parallaxY += _displayY - lastY;
            }
        }
        
        public virtual bool IsValid(int x, int y)
        {
            return x >= 0 && x < Width() && y >= 0 && y < Height();
        }

        public virtual bool CheckPassage(int x, int y, int bit)
        {
            var flags = TilesetFlags();
            var tiles = AllTiles(x, y);
            foreach (var tile in tiles)
            {
                var flag = flags[tile];
                if ((flag & 0x10) != 0)
                {
                    continue;
                }
                if ((flag & bit) == 0)
                {
                    return true;
                }
                if ((flag & bit) == bit)
                {
                    return false;
                }
            }
            return false;
        }

        public virtual int TileId(int x, int y, int z)
        {
            int width = Rmmz.dataMap.Width;
            int height = Rmmz.dataMap.Height;
            int index = (z * height + y) * width + x;
            return Rmmz.dataMap.Data.ElementAtOrDefault(index);
        }

        public virtual List<int> LayeredTiles(int x, int y)
        {
            var tiles = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                tiles.Add(TileId(x, y, 3 - i));
            }
            return tiles;
        }

        public virtual List<int> AllTiles(int x, int y)
        {
            var tiles = TileEventsXy(x, y).Select(e => e.TileId()).ToList();
            tiles.AddRange(LayeredTiles(x, y));
            return tiles;
        }

        public virtual int AutotileType(int x, int y, int z)
        {
            int tileId = TileId(x, y, z);
            return tileId >= 2048 ? (tileId - 2048) / 48 : -1;
        }

        public virtual bool IsPassable(int x, int y, int d)
        {
            return CheckPassage(x, y, (1 << (d / 2 - 1)) & 0x0f);
        }

        public virtual bool IsBoatPassable(int x, int y)
        {
            return CheckPassage(x, y, 0x0200);
        }

        public virtual bool IsShipPassable(int x, int y)
        {
            return CheckPassage(x, y, 0x0400);
        }

        public virtual bool IsAirshipLandOk(int x, int y)
        {
            return CheckPassage(x, y, 0x0800) && CheckPassage(x, y, 0x0f);
        }

        public virtual bool CheckLayeredTilesFlags(int x, int y, int bit)
        {
            var flags = TilesetFlags();
            return LayeredTiles(x, y).Any(tileId => (flags[tileId] & bit) != 0);
        }

        public virtual bool IsLadder(int x, int y)
        {
            return IsValid(x, y) && CheckLayeredTilesFlags(x, y, 0x20);
        }

        public virtual bool IsBush(int x, int y)
        {
            return IsValid(x, y) && CheckLayeredTilesFlags(x, y, 0x40);
        }

        public virtual bool IsCounter(int x, int y)
        {
            return IsValid(x, y) && CheckLayeredTilesFlags(x, y, 0x80);
        }

        public virtual bool IsDamageFloor(int x, int y)
        {
            return IsValid(x, y) && CheckLayeredTilesFlags(x, y, 0x100);
        }
        
        public virtual int TerrainTag(int x, int y)
        {
            if (IsValid(x, y))
            {
                var flags = TilesetFlags();
                var tiles = LayeredTiles(x, y);
                foreach (var tile in tiles)
                {
                    int tag = flags[tile] >> 12;
                    if (tag > 0)
                    {
                        return tag;
                    }
                }
            }
            return 0;
        }

        public virtual int RegionId(int x, int y)
        {
            return IsValid(x, y) ? TileId(x, y, 5) : 0;
        }

        public virtual bool IsScrolling() => _scrollRest > 0;

        public virtual void StartScroll(int direction, float distance, int speed)
        {
            _scrollDirection = direction;
            _scrollRest = distance;
            _scrollSpeed = speed;
        }

        public virtual void Update(bool sceneActive)
        {
            RefreshIfNeeded();
            if (sceneActive)
            {
                UpdateInterpreter();
            }
            UpdateScroll();
            UpdateEvents();
            UpdateVehicles();
            UpdateParallax();
        }

        public virtual void UpdateScroll()
        {
            if (IsScrolling())
            {
                float lastX = _displayX;
                float lastY = _displayY;
                DoScroll(_scrollDirection, ScrollDistance());
                if (_displayX == lastX && _displayY == lastY)
                {
                    _scrollRest = 0;
                }
                else
                {
                    _scrollRest -= ScrollDistance();
                }
            }
        }

        public virtual float ScrollDistance() => Mathf.Pow(2, _scrollSpeed) / 256f;

        public virtual void DoScroll(int direction, float distance)
        {
            switch (direction)
            {
                case 2: ScrollDown(distance); break;
                case 4: ScrollLeft(distance); break;
                case 6: ScrollRight(distance); break;
                case 8: ScrollUp(distance); break;
            }
        }

        public virtual void UpdateEvents()
        {
            foreach (var ev in _events)
            {
                ev?.Update();
            }
            foreach (var commonEvent in _commonEvents)
            {
                commonEvent.Update();
            }
        }

        protected virtual void UpdateVehicles()
        {
            foreach (var vehicle in _vehicles)
            {
                vehicle.Update();
            }
        }
        
        public virtual void UpdateParallax()
        {
            if (_parallaxLoopX)
            {
                _parallaxX += _parallaxSx / (float)TileWidth() / 2f;
            }
            if (_parallaxLoopY)
            {
                _parallaxY += _parallaxSy / (float)TileHeight() / 2f;
            }
        }

        public virtual void ChangeTileset(int tilesetId)
        {
            _tilesetId = tilesetId;
            Refresh();
        }

        public virtual void ChangeBattleback(string battleback1Name, string battleback2Name)
        {
            _battleback1Name = battleback1Name;
            _battleback2Name = battleback2Name;
        }

        public virtual void ChangeParallax(string name, bool loopX, bool loopY, int sx, int sy)
        {
            _parallaxName = name;
            _parallaxZero = Rmmz.ImageManager.IsZeroParallax(_parallaxName);
            if (_parallaxLoopX && !loopX)
            {
                _parallaxX = 0;
            }
            if (_parallaxLoopY && !loopY)
            {
                _parallaxY = 0;
            }
            _parallaxLoopX = loopX;
            _parallaxLoopY = loopY;
            _parallaxSx = sx;
            _parallaxSy = sy;
        }

        public virtual void UpdateInterpreter()
        {
            while (true)
            {
                _interpreter.Update();
                if (_interpreter.IsRunning()) { return; }

                if (_interpreter.EventId() > 0)
                {
                    UnlockEvent(_interpreter.EventId());
                    _interpreter.Clear();
                }
                if (!SetupStartingEvent())
                {
                    return;
                }
            }
        }

        public virtual void UnlockEvent(int eventId)
        {
            if (_events.ElementAtOrDefault(eventId) != null)
            {
                _events[eventId].Unlock();
            }
        }

        public virtual bool SetupStartingEvent()
        {
            RefreshIfNeeded();
            if (_interpreter.SetupReservedCommonEvent())
            {
                return true;
            }
            if (SetupTestEvent())
            {
                return true;
            }
            if (SetupStartingMapEvent())
            {
                return true;
            }
            if (SetupAutorunCommonEvent())
            {
                return true;
            }
            return false;
        }

        public virtual bool SetupTestEvent()
        {
            /*
            if (Global.TestEvent != null)
            {
                _interpreter.Setup(Global.TestEvent, 0);
                Global.TestEvent = null;
                return true;
            }
            */
            return false;
        }

        public virtual bool SetupStartingMapEvent()
        {
            foreach (var ev in Events())
            {
                if (ev.IsStarting())
                {
                    ev.ClearStartingFlag();
                    _interpreter.Setup(ev.List(), ev.EventId());
                    return true;
                }
            }
            return false;
        }

        public virtual bool SetupAutorunCommonEvent()
        {
            foreach (var commonEvent in AutorunCommonEvents())
            {
                if (Rmmz.gameSwitches.Value(commonEvent.SwitchId))
                {
                    _interpreter.Setup(commonEvent.List);
                    return true;
                }
            }
            return false;
        }

        public virtual bool IsAnyEventStarting() => Events().Exists(ev => ev.IsStarting());
    }
}