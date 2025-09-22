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
        private Game_Interpreter _interpreter;
        private int _mapId;
        private int _tilesetId;
        private List<Game_Event> _events;
        private List<Game_CommonEvent> _commonEvents;
        private List<Game_Vehicle> _vehicles;
        private float _displayX;
        private float _displayY;
        private bool _nameDisplay;
        private int _scrollDirection;
        private float _scrollRest;
        private int _scrollSpeed;
        private string _parallaxName;
        private bool _parallaxZero;
        private bool _parallaxLoopX;
        private bool _parallaxLoopY;
        private float _parallaxSx;
        private float _parallaxSy;
        private float _parallaxX;
        private float _parallaxY;
        private string _battleback1Name;
        private string _battleback2Name;
        private bool _needsRefresh;
        private List<Game_Event> _tileEvents;

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

        public void Setup(int mapId)
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

        public bool IsEventRunning()
        {
            return _interpreter.IsRunning() || IsAnyEventStarting();
        }

        public int TileWidth()
        {
            if (Rmmz.dataSystem.TileSize > 0)
            {
                return Rmmz.dataSystem.TileSize;
            }
            return 48;
        }

        public int TileHeight() => TileWidth();
        public float BushDepth() => TileHeight() / 4f;
        public int MapId() => _mapId;
        public int TilesetId() => _tilesetId;
        public float DisplayX() => _displayX;
        public float DisplayY() => _displayY;
        public string ParallaxName() => _parallaxName;
        public string Battleback1Name() => _battleback1Name;
        public string Battleback2Name() => _battleback2Name;
        public void RequestRefresh() { _needsRefresh = true; }
        public bool IsNameDisplayEnabled() => _nameDisplay;
        public void DisableNameDisplay() { _nameDisplay = false; }
        public void EnableNameDisplay() { _nameDisplay = true; }

        public void CreateVehicles()
        {
            _vehicles.Add(Game_Vehicle.Create(Game_Vehicle.VehicleTypes.Boat));
            _vehicles.Add(Game_Vehicle.Create(Game_Vehicle.VehicleTypes.Ship));
            _vehicles.Add(Game_Vehicle.Create(Game_Vehicle.VehicleTypes.Airship));
        }

        public void RefreshVehicles()
        {
            foreach (var v in _vehicles)
            {
                v.Refresh();
            }
        }

        public IEnumerable<Game_Vehicle> Vehicles() => _vehicles;

        public Game_Vehicle Vehicle(Game_Vehicle.VehicleTypes type)
        {
            return type switch
            {
                Game_Vehicle.VehicleTypes.Boat => Boat(),
                Game_Vehicle.VehicleTypes.Ship => Ship(),
                Game_Vehicle.VehicleTypes.Airship => Airship(),
                _ => null
            };
        }

        public Game_Vehicle Boat() => _vehicles[0];
        public Game_Vehicle Ship() => _vehicles[1];
        public Game_Vehicle Airship() => _vehicles[2];

        public void SetupEvents()
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

        public List<Game_Event> Events()
        {
            return _events.Where(e => e != null).ToList();
        }

        public Game_Event Event(int eventId)
        {
            return _events[eventId];
        }
        
        
        public void EraseEvent(int eventId)
        {
            _events[eventId]?.Erase();
        }

        public IEnumerable<DataCommonEvent> AutorunCommonEvents()
        {
            return Rmmz.dataCommonEvents.Where(e => e != null && e.Trigger == 1);
        }

        public IEnumerable<DataCommonEvent> ParallelCommonEvents()
        {
            return Rmmz.dataCommonEvents.Where(e => e != null && e.Trigger == 2);
        }

        public void SetupScroll()
        {
            _scrollDirection = 2;
            _scrollRest = 0;
            _scrollSpeed = 4;
        }

        public void SetupParallax()
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

        public void SetupBattleback()
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

        public void SetDisplayPos(float x, float y)
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

        public float ParallaxOx() =>
            _parallaxZero 
                ? _parallaxX * TileWidth() 
                : (_parallaxLoopX ? (_parallaxX * TileWidth()) / 2 : 0);

        public float ParallaxOy() =>
            _parallaxZero 
                ? _parallaxY * TileHeight() 
                : (_parallaxLoopY ? (_parallaxY * TileHeight()) / 2 : 0);

        public DataTileset Tileset() => Rmmz.dataTilesets[_tilesetId];

        public int[] TilesetFlags() => Tileset()?.Flags ?? Array.Empty<int>();
        
        public string DisplayName() => Rmmz.dataMap.DisplayName;

        public int Width() => Rmmz.dataMap.Width;

        public int Height() => Rmmz.dataMap.Height;

        public int[] Data() => Rmmz.dataMap.Data;

        public bool IsLoopHorizontal() => Rmmz.dataMap.ScrollType == 2 || Rmmz.dataMap.ScrollType == 3;

        public bool IsLoopVertical() => Rmmz.dataMap.ScrollType == 1 || Rmmz.dataMap.ScrollType == 3;

        public bool IsDashDisabled() => Rmmz.dataMap.DisableDashing;

        public List<Encounter> EncounterList() => Rmmz.dataMap.EncounterList;

        public int EncounterStep() => Rmmz.dataMap.EncounterStep;

        public bool IsOverworld() => Tileset()?.Mode == 0;

        public float ScreenTileX() => Mathf.Round(((float)Graphics.Width / TileWidth()) * 16) / 16;

        public float ScreenTileY() => Mathf.Round(((float)Graphics.Height / TileHeight()) * 16) / 16;

        public float AdjustX(float x)
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

        public float AdjustY(float y)
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

        public int RoundX(int x) => IsLoopHorizontal() ? (x % Width()) : x;

        public int RoundY(int y) => IsLoopVertical() ? (y % Height()) : y;

        public int XWithDirection(int x, int d) => x + (d == 6 ? 1 : d == 4 ? -1 : 0);

        public int YWithDirection(int y, int d) => y + (d == 2 ? 1 : d == 8 ? -1 : 0);

        public int RoundXWithDirection(int x, int d) => RoundX(XWithDirection(x, d));

        public int RoundYWithDirection(int y, int d) => RoundY(YWithDirection(y, d));

        public int DeltaX(int x1, int x2)
        {
            int result = x1 - x2;
            if (IsLoopHorizontal() && Math.Abs(result) > Width() / 2)
            {
                result = result < 0 ? result + Width() : result - Width();
            }
            return result;
        }

        public int DeltaY(int y1, int y2)
        {
            int result = y1 - y2;
            if (IsLoopVertical() && Math.Abs(result) > Height() / 2)
            {
                result = result < 0 ? result + Height() : result - Height();
            }
            return result;
        }

        public int Distance(int x1, int y1, int x2, int y2) =>
            Math.Abs(DeltaX(x1, x2)) + Math.Abs(DeltaY(y1, y2));

        public int CanvasToMapX(float x)
        {
            float tileWidth = TileWidth();
            float originX = _displayX * tileWidth;
            int mapX = (int)Math.Floor((originX + x) / tileWidth);
            return RoundX(mapX);
        }

        public int CanvasToMapY(float y)
        {
            float tileHeight = TileHeight();
            float originY = _displayY * tileHeight;
            int mapY = (int)Math.Floor((originY + y) / tileHeight);
            return RoundY(mapY);
        }

        public void Autoplay()
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

        public void RefreshIfNeeded()
        {
            if (_needsRefresh)
            {
                Refresh();
            }
        }

        public void Refresh()
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

        public void RefreshTileEvents()
        {
            _tileEvents = Events().Where(e => e.IsTile()).ToList();
        }

        public List<Game_Event> EventsXy(int x, int y) => Events().Where(e => e.Pos(x, y)).ToList();

        public List<Game_Event> EventsXyNt(int x, int y) => Events().Where(e => e.PosNt(x, y)).ToList();

        public List<Game_Event> TileEventsXy(int x, int y) => _tileEvents.Where(e => e.PosNt(x, y)).ToList();

        public int EventIdXy(int x, int y)
        {
            var list = EventsXy(x, y);
            return list.Count == 0 ? 0 : list[0].EventId();
        }
        
        public void ScrollDown(float distance)
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

        public void ScrollLeft(float distance)
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

        public void ScrollRight(float distance)
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

        public void ScrollUp(float distance)
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
        
        public bool IsValid(int x, int y)
        {
            return x >= 0 && x < Width() && y >= 0 && y < Height();
        }

        public bool CheckPassage(int x, int y, int bit)
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

        public int TileId(int x, int y, int z)
        {
            int width = Rmmz.dataMap.Width;
            int height = Rmmz.dataMap.Height;
            int index = (z * height + y) * width + x;
            return Rmmz.dataMap.Data.ElementAtOrDefault(index);
        }

        public List<int> LayeredTiles(int x, int y)
        {
            var tiles = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                tiles.Add(TileId(x, y, 3 - i));
            }
            return tiles;
        }

        public List<int> AllTiles(int x, int y)
        {
            var tiles = TileEventsXy(x, y).Select(e => e.TileId()).ToList();
            tiles.AddRange(LayeredTiles(x, y));
            return tiles;
        }

        public int AutotileType(int x, int y, int z)
        {
            int tileId = TileId(x, y, z);
            return tileId >= 2048 ? (tileId - 2048) / 48 : -1;
        }

        public bool IsPassable(int x, int y, int d)
        {
            return CheckPassage(x, y, (1 << (d / 2 - 1)) & 0x0f);
        }

        public bool IsBoatPassable(int x, int y)
        {
            return CheckPassage(x, y, 0x0200);
        }

        public bool IsShipPassable(int x, int y)
        {
            return CheckPassage(x, y, 0x0400);
        }

        public bool IsAirshipLandOk(int x, int y)
        {
            return CheckPassage(x, y, 0x0800) && CheckPassage(x, y, 0x0f);
        }

        public bool CheckLayeredTilesFlags(int x, int y, int bit)
        {
            var flags = TilesetFlags();
            return LayeredTiles(x, y).Any(tileId => (flags[tileId] & bit) != 0);
        }

        public bool IsLadder(int x, int y)
        {
            return IsValid(x, y) && CheckLayeredTilesFlags(x, y, 0x20);
        }

        public bool IsBush(int x, int y)
        {
            return IsValid(x, y) && CheckLayeredTilesFlags(x, y, 0x40);
        }

        public bool IsCounter(int x, int y)
        {
            return IsValid(x, y) && CheckLayeredTilesFlags(x, y, 0x80);
        }

        public bool IsDamageFloor(int x, int y)
        {
            return IsValid(x, y) && CheckLayeredTilesFlags(x, y, 0x100);
        }
        
        public int TerrainTag(int x, int y)
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

        public int RegionId(int x, int y)
        {
            return IsValid(x, y) ? TileId(x, y, 5) : 0;
        }

        public bool IsScrolling() => _scrollRest > 0;

        public void StartScroll(int direction, float distance, int speed)
        {
            _scrollDirection = direction;
            _scrollRest = distance;
            _scrollSpeed = speed;
        }

        public void Update(bool sceneActive)
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

        public void UpdateScroll()
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

        public float ScrollDistance() => Mathf.Pow(2, _scrollSpeed) / 256f;

        public void DoScroll(int direction, float distance)
        {
            switch (direction)
            {
                case 2: ScrollDown(distance); break;
                case 4: ScrollLeft(distance); break;
                case 6: ScrollRight(distance); break;
                case 8: ScrollUp(distance); break;
            }
        }

        public void UpdateEvents()
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
        
        public void UpdateParallax()
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

        public void ChangeTileset(int tilesetId)
        {
            _tilesetId = tilesetId;
            Refresh();
        }

        public void ChangeBattleback(string battleback1Name, string battleback2Name)
        {
            _battleback1Name = battleback1Name;
            _battleback2Name = battleback2Name;
        }

        public void ChangeParallax(string name, bool loopX, bool loopY, int sx, int sy)
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

        public void UpdateInterpreter()
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

        public void UnlockEvent(int eventId)
        {
            if (_events.ElementAtOrDefault(eventId) != null)
            {
                _events[eventId].Unlock();
            }
        }

        public bool SetupStartingEvent()
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

        public bool SetupTestEvent()
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

        public bool SetupStartingMapEvent()
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

        public bool SetupAutorunCommonEvent()
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

        public bool IsAnyEventStarting() => Events().Exists(ev => ev.IsStarting());
    }
}