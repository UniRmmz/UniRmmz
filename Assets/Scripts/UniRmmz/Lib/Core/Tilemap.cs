using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    /// <summary>
    /// The tilemap which displays 2D tile-based game map.
    /// </summary>
    public partial class Tilemap : RmmzContainer
    {
        public int TileWidth = 48;
        public int TileHeight = 48;

        public int[] Flags;
        public int AnimationCount;
        public bool HorizontalWrap;
        public bool VerticalWrap;

       
        private int _margin = 20;

        private int _mapWidth;
        private int _mapHeight;
        private int[] _mapData;
        private List<Bitmap> _bitmaps = new();

        private bool _needsRepaint;
        private int _lastAnimationFrame;
        private int _lastStartX;
        private int _lastStartY;

        private CombinedLayer _lowerLayer;
        private CombinedLayer _upperLayer;

        private int _animationFrame;
        private bool _needsBitmapsUpdate;
        private Renderer _renderer;
        
        public const int TILE_ID_B = 0;
        public const int TILE_ID_C = 256;
        public const int TILE_ID_D = 512;
        public const int TILE_ID_E = 768;
        public const int TILE_ID_A5 = 1536;
        public const int TILE_ID_A1 = 2048;
        public const int TILE_ID_A2 = 2816;
        public const int TILE_ID_A3 = 4352;
        public const int TILE_ID_A4 = 5888;
        public const int TILE_ID_MAX = 8192;
        
        public static readonly int[][][] FLOOR_AUTOTILE_TABLE = new int[][][]
        {
            new[] { new[] {2, 4}, new[] {1, 4}, new[] {2, 3}, new[] {1, 3} },
            new[] { new[] {2, 0}, new[] {1, 4}, new[] {2, 3}, new[] {1, 3} },
            new[] { new[] {2, 4}, new[] {3, 0}, new[] {2, 3}, new[] {1, 3} },
            new[] { new[] {2, 0}, new[] {3, 0}, new[] {2, 3}, new[] {1, 3} },
            new[] { new[] {2, 4}, new[] {1, 4}, new[] {2, 3}, new[] {3, 1} },
            new[] { new[] {2, 0}, new[] {1, 4}, new[] {2, 3}, new[] {3, 1} },
            new[] { new[] {2, 4}, new[] {3, 0}, new[] {2, 3}, new[] {3, 1} },
            new[] { new[] {2, 0}, new[] {3, 0}, new[] {2, 3}, new[] {3, 1} },
            new[] { new[] {2, 4}, new[] {1, 4}, new[] {2, 1}, new[] {1, 3} },
            new[] { new[] {2, 0}, new[] {1, 4}, new[] {2, 1}, new[] {1, 3} },
            new[] { new[] {2, 4}, new[] {3, 0}, new[] {2, 1}, new[] {1, 3} },
            new[] { new[] {2, 0}, new[] {3, 0}, new[] {2, 1}, new[] {1, 3} },
            new[] { new[] {2, 4}, new[] {1, 4}, new[] {2, 1}, new[] {3, 1} },
            new[] { new[] {2, 0}, new[] {1, 4}, new[] {2, 1}, new[] {3, 1} },
            new[] { new[] {2, 4}, new[] {3, 0}, new[] {2, 1}, new[] {3, 1} },
            new[] { new[] {2, 0}, new[] {3, 0}, new[] {2, 1}, new[] {3, 1} },
            new[] { new[] {0, 4}, new[] {1, 4}, new[] {0, 3}, new[] {1, 3} },
            new[] { new[] {0, 4}, new[] {3, 0}, new[] {0, 3}, new[] {1, 3} },
            new[] { new[] {0, 4}, new[] {1, 4}, new[] {0, 3}, new[] {3, 1} },
            new[] { new[] {0, 4}, new[] {3, 0}, new[] {0, 3}, new[] {3, 1} },
            new[] { new[] {2, 2}, new[] {1, 2}, new[] {2, 3}, new[] {1, 3} },
            new[] { new[] {2, 2}, new[] {1, 2}, new[] {2, 3}, new[] {3, 1} },
            new[] { new[] {2, 2}, new[] {1, 2}, new[] {2, 1}, new[] {1, 3} },
            new[] { new[] {2, 2}, new[] {1, 2}, new[] {2, 1}, new[] {3, 1} },
            new[] { new[] {2, 4}, new[] {3, 4}, new[] {2, 3}, new[] {3, 3} },
            new[] { new[] {2, 4}, new[] {3, 4}, new[] {2, 1}, new[] {3, 3} },
            new[] { new[] {2, 0}, new[] {3, 4}, new[] {2, 3}, new[] {3, 3} },
            new[] { new[] {2, 0}, new[] {3, 4}, new[] {2, 1}, new[] {3, 3} },
            new[] { new[] {2, 4}, new[] {1, 4}, new[] {2, 5}, new[] {1, 5} },
            new[] { new[] {2, 0}, new[] {1, 4}, new[] {2, 5}, new[] {1, 5} },
            new[] { new[] {2, 4}, new[] {3, 0}, new[] {2, 5}, new[] {1, 5} },
            new[] { new[] {2, 0}, new[] {3, 0}, new[] {2, 5}, new[] {1, 5} },
            new[] { new[] {0, 4}, new[] {3, 4}, new[] {0, 3}, new[] {3, 3} },
            new[] { new[] {2, 2}, new[] {1, 2}, new[] {2, 5}, new[] {1, 5} },
            new[] { new[] {0, 2}, new[] {1, 2}, new[] {0, 3}, new[] {1, 3} },
            new[] { new[] {0, 2}, new[] {1, 2}, new[] {0, 3}, new[] {3, 1} },
            new[] { new[] {2, 2}, new[] {3, 2}, new[] {2, 3}, new[] {3, 3} },
            new[] { new[] {2, 2}, new[] {3, 2}, new[] {2, 1}, new[] {3, 3} },
            new[] { new[] {2, 4}, new[] {3, 4}, new[] {2, 5}, new[] {3, 5} },
            new[] { new[] {2, 0}, new[] {3, 4}, new[] {2, 5}, new[] {3, 5} },
            new[] { new[] {0, 4}, new[] {1, 4}, new[] {0, 5}, new[] {1, 5} },
            new[] { new[] {0, 4}, new[] {3, 0}, new[] {0, 5}, new[] {1, 5} },
            new[] { new[] {0, 2}, new[] {3, 2}, new[] {0, 3}, new[] {3, 3} },
            new[] { new[] {0, 2}, new[] {1, 2}, new[] {0, 5}, new[] {1, 5} },
            new[] { new[] {0, 4}, new[] {3, 4}, new[] {0, 5}, new[] {3, 5} },
            new[] { new[] {2, 2}, new[] {3, 2}, new[] {2, 5}, new[] {3, 5} },
            new[] { new[] {0, 2}, new[] {3, 2}, new[] {0, 5}, new[] {3, 5} },
            new[] { new[] {0, 0}, new[] {1, 0}, new[] {0, 1}, new[] {1, 1} }
        };

        // 壁オートタイル
        public static readonly int[][][] WALL_AUTOTILE_TABLE = new int[][][]
        {
            new[] { new[] {2, 2}, new[] {1, 2}, new[] {2, 1}, new[] {1, 1} },
            new[] { new[] {0, 2}, new[] {1, 2}, new[] {0, 1}, new[] {1, 1} },
            new[] { new[] {2, 0}, new[] {1, 0}, new[] {2, 1}, new[] {1, 1} },
            new[] { new[] {0, 0}, new[] {1, 0}, new[] {0, 1}, new[] {1, 1} },
            new[] { new[] {2, 2}, new[] {3, 2}, new[] {2, 1}, new[] {3, 1} },
            new[] { new[] {0, 2}, new[] {3, 2}, new[] {0, 1}, new[] {3, 1} },
            new[] { new[] {2, 0}, new[] {3, 0}, new[] {2, 1}, new[] {3, 1} },
            new[] { new[] {0, 0}, new[] {3, 0}, new[] {0, 1}, new[] {3, 1} },
            new[] { new[] {2, 2}, new[] {1, 2}, new[] {2, 3}, new[] {1, 3} },
            new[] { new[] {0, 2}, new[] {1, 2}, new[] {0, 3}, new[] {1, 3} },
            new[] { new[] {2, 0}, new[] {1, 0}, new[] {2, 3}, new[] {1, 3} },
            new[] { new[] {0, 0}, new[] {1, 0}, new[] {0, 3}, new[] {1, 3} },
            new[] { new[] {2, 2}, new[] {3, 2}, new[] {2, 3}, new[] {3, 3} },
            new[] { new[] {0, 2}, new[] {3, 2}, new[] {0, 3}, new[] {3, 3} },
            new[] { new[] {2, 0}, new[] {3, 0}, new[] {2, 3}, new[] {3, 3} },
            new[] { new[] {0, 0}, new[] {3, 0}, new[] {0, 3}, new[] {3, 3} }
        };

        // 滝オートタイル
        public static readonly int[][][] WATERFALL_AUTOTILE_TABLE = new int[][][]
        {
            new[] { new[] {2, 0}, new[] {1, 0}, new[] {2, 1}, new[] {1, 1} },
            new[] { new[] {0, 0}, new[] {1, 0}, new[] {0, 1}, new[] {1, 1} },
            new[] { new[] {2, 0}, new[] {3, 0}, new[] {2, 1}, new[] {3, 1} },
            new[] { new[] {0, 0}, new[] {3, 0}, new[] {0, 1}, new[] {3, 1} }
        };

        protected override void Awake()
        {
            base.Awake();
            
            Width = 816;  // デフォルトGraphics.width
            Height = 624; // デフォルトGraphics.height
            
            _renderer = new Renderer();
            CreateLayers();
            Refresh();
        }

        public override Vector2 Origin { get; set; }

        protected override void OnDestroy()
        {
            _renderer.Dispose();
        }

        /// <summary>
        /// Sets the tilemap data.
        /// </summary>
        /// <param name="width">The width of the map in number of tiles.</param>
        /// <param name="height">The height of the map in number of tiles.</param>
        /// <param name="data">The one dimensional array for the map data.</param>
        public void SetData(int width, int height, int[] data)
        {
            _mapWidth = width;
            _mapHeight = height;
            _mapData = data;
        }

        /// <summary>
        /// Checks whether the tileset is ready to render.
        /// </summary>
        /// <returns>True if the tilemap is ready.</returns>
        public bool IsReady()
        {
            foreach (var bitmap in _bitmaps)
            {
                if (!bitmap.IsReady())
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Updates the tilemap for each frame.
        /// </summary>
        public override void UpdateRmmz()
        {
            AnimationCount++;
            _animationFrame = Mathf.FloorToInt(AnimationCount / 30);
            base.UpdateRmmz();
        }
        
        /// <summary>
        /// Sets the bitmaps used as a tileset.
        /// </summary>
        /// <param name="bitmaps">The array of the tileset bitmaps.</param>
        public void SetBitmaps(List<Bitmap> bitmaps)
        {
            _bitmaps = bitmaps;
            foreach (var bitmap in _bitmaps)
            {
                if (!bitmap.IsReady())
                {
                    bitmap.AddLoadListener((bitmap) => UpdateBitmaps());
                }
            }
            
            _needsBitmapsUpdate = true;
            UpdateBitmaps();
        }

        /// <summary>
        /// Forces to repaint the entire tilemap.
        /// </summary>
        public void Refresh()
        {
            _needsRepaint = true;
        }

        /// <summary>
        /// Updates the transform on all children of this container for rendering.
        /// </summary>
        protected virtual void LateUpdate()
        {
            int ox = Mathf.CeilToInt(Origin.x);
            int oy = Mathf.CeilToInt(Origin.y);
            int startX = Mathf.FloorToInt((ox - _margin) / (float)TileWidth);
            int startY = Mathf.FloorToInt((oy - _margin) / (float)TileHeight);

            _lowerLayer.X = startX * TileWidth - ox;
            _lowerLayer.Y = startY * TileHeight - oy;
            _upperLayer.X = startX * TileWidth - ox;
            _upperLayer.Y = startY * TileHeight - oy;
            if (_needsRepaint || _lastAnimationFrame != _animationFrame || _lastStartX != startX || _lastStartY != startY)
            {
                _lastAnimationFrame = _animationFrame;
                _lastStartX = startX;
                _lastStartY = startY;
                AddAllSpots(startX, startY);
                _needsRepaint = false;
            }
            SortChildren();
            _lowerLayer.Render(_renderer);
            _upperLayer.Render(_renderer);
        }
        
        private void CreateLayers()
        {
            _lowerLayer = CombinedLayer.Create("LowerLayer");
            _lowerLayer.Z = 0;
            _upperLayer = CombinedLayer.Create("UpperLayer");
            _upperLayer.Z = 4;

            this.AddChild(_lowerLayer);
            this.AddChild(_upperLayer);
            _needsRepaint = true;
        }

        private void UpdateBitmaps()
        {
            if (_needsBitmapsUpdate && IsReady())
            {
                _lowerLayer.SetBitmaps(_bitmaps);
                _needsBitmapsUpdate = false;
                _needsRepaint = true;
            }
        }

        private void AddAllSpots(int startX, int startY)
        {
            _lowerLayer.Clear();
            _upperLayer.Clear();

            int widthWithMargin = (int)Width + _margin * 2;
            int heightWithMargin = (int)Height + _margin * 2;
            int tileCols = (widthWithMargin / TileWidth) + 2;
            int tileRows = (heightWithMargin / TileHeight) + 2;

            for (int y = 0; y < tileRows; y++)
            {
                for (int x = 0; x < tileCols; x++)
                {
                    AddSpot(startX, startY, x, y);
                }
            }
        }

        private void AddSpot(int startX, int startY, int x, int y)
        {
            int mx = startX + x;
            int my = startY + y;
            int dx = x * TileWidth;
            int dy = y * TileHeight;

            int tileId0 = ReadMapData(mx, my, 0);
            int tileId1 = ReadMapData(mx, my, 1);
            int tileId2 = ReadMapData(mx, my, 2);
            int tileId3 = ReadMapData(mx, my, 3);
            int shadowBits = ReadMapData(mx, my, 4);
            int upperTileId1 = ReadMapData(mx, my - 1, 1);

            AddSpotTile(tileId0, dx, dy);
            AddSpotTile(tileId1, dx, dy);
            AddShadow(_lowerLayer, shadowBits, dx, dy);

            if (IsTableTile(upperTileId1) && !IsTableTile(tileId1))
            {
                if (!Tilemap.IsShadowingTile(tileId0))
                {
                    AddTableEdge(_lowerLayer, upperTileId1, dx, dy);
                }
            }

            if (IsOverpassPosition(mx, my))
            {
                AddTile(_upperLayer, tileId2, dx, dy);
                AddTile(_upperLayer, tileId3, dx, dy);
            }
            else
            {
                AddSpotTile(tileId2, dx, dy);
                AddSpotTile(tileId3, dx, dy);
            }
        }

        private void AddSpotTile(int tileId, int dx, int dy)
        {
            if (IsHigherTile(tileId))
            {
                AddTile(_upperLayer, tileId, dx, dy);
            }
            else
            {
                AddTile(_lowerLayer, tileId, dx, dy);
            }
        }

        private void AddTile(CombinedLayer layer, int tileId, int dx, int dy)
        {
            if (Tilemap.IsVisibleTile(tileId))
            {
                if (Tilemap.IsAutotile(tileId))
                {
                    AddAutotile(layer, tileId, dx, dy);
                }
                else
                {
                    AddNormalTile(layer, tileId, dx, dy);
                }
            }
        }

        private void AddNormalTile(CombinedLayer layer, int tileId, int dx, int dy)
        {
            int setNumber;

            if (Tilemap.IsTileA5(tileId))
            {
                setNumber = 4;
            }
            else
            {
                setNumber = 5 + tileId / 256;
            }

            int w = TileWidth;
            int h = TileHeight;
            int sx = ((tileId / 128) % 2 * 8 + (tileId % 8)) * w;
            int sy = ((tileId % 256) / 8 % 16) * h;

            layer.AddRect(setNumber, sx, sy, dx, dy, w, h);
        }
        
        private void AddAutotile(CombinedLayer layer, int tileId, int dx, int dy)
        {
            int kind = Tilemap.GetAutotileKind(tileId);
            int shape = Tilemap.GetAutotileShape(tileId);
            int tx = kind % 8;
            int ty = kind / 8;
            int setNumber = 0;
            int bx = 0;
            int by = 0;
            int[][][] autotileTable = Tilemap.FLOOR_AUTOTILE_TABLE;
            bool isTable = false;

            if (Tilemap.IsTileA1(tileId))
            {
                int waterSurfaceIndex = new[] { 0, 1, 2, 1 }[_animationFrame % 4];
                setNumber = 0;
                if (kind == 0)
                {
                    bx = waterSurfaceIndex * 2;
                    by = 0;
                }
                else if (kind == 1)
                {
                    bx = waterSurfaceIndex * 2;
                    by = 3;
                }
                else if (kind == 2)
                {
                    bx = 6;
                    by = 0;
                }
                else if (kind == 3)
                {
                    bx = 6;
                    by = 3;
                }
                else
                {
                    bx = (tx / 4) * 8;
                    by = ty * 6 + ((tx / 2) % 2) * 3;
                    if (kind % 2 == 0)
                    {
                        bx += waterSurfaceIndex * 2;
                    }
                    else
                    {
                        bx += 6;
                        autotileTable = Tilemap.WATERFALL_AUTOTILE_TABLE;
                        by += _animationFrame % 3;
                    }
                }
            }
            else if (Tilemap.IsTileA2(tileId))
            {
                setNumber = 1;
                bx = tx * 2;
                by = (ty - 2) * 3;
                isTable = IsTableTile(tileId);
            }
            else if (Tilemap.IsTileA3(tileId))
            {
                setNumber = 2;
                bx = tx * 2;
                by = (ty - 6) * 2;
                autotileTable = Tilemap.WALL_AUTOTILE_TABLE;
            }
            else if (Tilemap.IsTileA4(tileId))
            {
                setNumber = 3;
                bx = tx * 2;
                by = (int)((ty - 10) * 2.5f + (ty % 2 == 1 ? 0.5f : 0f));
                if (ty % 2 == 1)
                {
                    autotileTable = Tilemap.WALL_AUTOTILE_TABLE;
                }
            }

            var table = autotileTable[shape];
            int w1 = TileWidth / 2;
            int h1 = TileHeight / 2;

            for (int i = 0; i < 4; i++)
            {
                int qsx = table[i][0];
                int qsy = table[i][1];
                int sx1 = (bx * 2 + qsx) * w1;
                int sy1 = (by * 2 + qsy) * h1;
                int dx1 = dx + (i % 2) * w1;
                int dy1 = dy + (i / 2) * h1;

                if (isTable && (qsy == 1 || qsy == 5))
                {
                    int qsx2 = (qsy == 1) ? (4 - qsx) % 4 : qsx;
                    int qsy2 = 3;
                    int sx2 = (bx * 2 + qsx2) * w1;
                    int sy2 = (by * 2 + qsy2) * h1;
                    layer.AddRect(setNumber, sx2, sy2, dx1, dy1, w1, h1 / 2);
                    layer.AddRect(setNumber, sx1, sy1, dx1, dy1 + h1 / 2, w1, h1 / 2);
                }
                else
                {
                    layer.AddRect(setNumber, sx1, sy1, dx1, dy1, w1, h1);
                }
            }
        }
        
        private void AddTableEdge(CombinedLayer layer, int tileId, int dx, int dy)
        {
            if (Tilemap.IsTileA2(tileId))
            {
                int[][][] autotileTable = Tilemap.FLOOR_AUTOTILE_TABLE;
                int kind = Tilemap.GetAutotileKind(tileId);
                int shape = Tilemap.GetAutotileShape(tileId);
                int tx = kind % 8;
                int ty = kind / 8;
                int setNumber = 1;
                int bx = tx * 2;
                int by = (ty - 2) * 3;
                var table = autotileTable[shape];
                int w1 = TileWidth / 2;
                int h1 = TileHeight / 2;

                for (int i = 0; i < 2; i++)
                {
                    int qsx = table[2 + i][0];
                    int qsy = table[2 + i][1];
                    int sx1 = (bx * 2 + qsx) * w1;
                    int sy1 = (by * 2 + qsy) * h1 + h1 / 2;
                    int dx1 = dx + (i % 2) * w1;
                    int dy1 = dy + (i / 2) * h1;
                    layer.AddRect(setNumber, sx1, sy1, dx1, dy1, w1, h1 / 2);
                }
            }
        }

        private void AddShadow(CombinedLayer layer, int shadowBits, int dx, int dy)
        {
            if ((shadowBits & 0x0F) != 0)
            {
                int w1 = TileWidth / 2;
                int h1 = TileHeight / 2;
                for (int i = 0; i < 4; i++)
                {
                    if ((shadowBits & (1 << i)) != 0)
                    {
                        int dx1 = dx + (i % 2) * w1;
                        int dy1 = dy + (i / 2) * h1;
                        layer.AddRect(-1, 0, 0, dx1, dy1, w1, h1);
                    }
                }
            }
        }
        
        private int ReadMapData(int x, int y, int z)
        {
            if (_mapData != null)
            {
                int width = _mapWidth;
                int height = _mapHeight;
                if (HorizontalWrap)
                {
                    x = (x % width + width) % width;
                }
                if (VerticalWrap)
                {
                    y = (y % height + height) % height;
                }
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    return _mapData[(z * height + y) * width + x];
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        
        private bool IsHigherTile(int tileId)
        {
            return (Flags[tileId] & 0x10) != 0;
        }
        
        private bool IsTableTile(int tileId)
        {
            return Tilemap.IsTileA2(tileId) && (Flags[tileId] & 0x80) != 0;
        }
        
        private bool IsOverpassPosition(int mx, int my)
        {
            return false;
        }

        private void SortChildren()
        {
            var children = transform.GetChildren<Graphic>().Select(v => ((IRmmzDrawable2d)v, v.transform)).ToList();
            children.Sort(CompareChildOrder);

            for (int i = 0; i < children.Count; i++)
            {
                children[i].transform.SetSiblingIndex(i);
            }
        }

        private int CompareChildOrder((IRmmzDrawable2d, Transform) a, (IRmmzDrawable2d, Transform) b)
        {
            int zComp = a.Item1.Z.CompareTo(b.Item1.Z);
            if (zComp != 0)
            {
                return zComp;
            }
            
            int yComp = a.Item1.Y.CompareTo(b.Item1.Y);
            if (yComp != 0)
            {
                return yComp;
            }
            
            return a.Item1.SpriteId - b.Item1.SpriteId;
        }
        
        public static bool IsVisibleTile(int tileId)
        {
            return tileId > 0 && tileId < TILE_ID_MAX;
        }

        public static bool IsAutotile(int tileId)
        {
            return tileId >= TILE_ID_A1;
        }

        public static int GetAutotileKind(int tileId)
        {
            return (tileId - TILE_ID_A1) / 48;
        }

        public static int GetAutotileShape(int tileId)
        {
            return (tileId - TILE_ID_A1) % 48;
        }

        public static int MakeAutotileId(int kind, int shape)
        {
            return TILE_ID_A1 + kind * 48 + shape;
        }

        public static bool IsSameKindTile(int tileID1, int tileID2)
        {
            if (IsAutotile(tileID1) && IsAutotile(tileID2))
            {
                return GetAutotileKind(tileID1) == GetAutotileKind(tileID2);
            }
            else
            {
                return tileID1 == tileID2;
            }
        }

        public static bool IsTileA1(int tileId)
        {
            return tileId >= TILE_ID_A1 && tileId < TILE_ID_A2;
        }

        public static bool IsTileA2(int tileId)
        {
            return tileId >= TILE_ID_A2 && tileId < TILE_ID_A3;
        }

        public static bool IsTileA3(int tileId)
        {
            return tileId >= TILE_ID_A3 && tileId < TILE_ID_A4;
        }

        public static bool IsTileA4(int tileId)
        {
            return tileId >= TILE_ID_A4 && tileId < TILE_ID_MAX;
        }

        public static bool IsTileA5(int tileId)
        {
            return tileId >= TILE_ID_A5 && tileId < TILE_ID_A1;
        }

        public static bool IsWaterTile(int tileId)
        {
            if (IsTileA1(tileId))
            {
                return !(tileId >= TILE_ID_A1 + 96 && tileId < TILE_ID_A1 + 192);
            }
            return false;
        }

        public static bool IsWaterfallTile(int tileId)
        {
            if (tileId >= TILE_ID_A1 + 192 && tileId < TILE_ID_A2)
            {
                return GetAutotileKind(tileId) % 2 == 1;
            }
            return false;
        }

        public static bool IsGroundTile(int tileId)
        {
            return IsTileA1(tileId) || IsTileA2(tileId) || IsTileA5(tileId);
        }

        public static bool IsShadowingTile(int tileId)
        {
            return IsTileA3(tileId) || IsTileA4(tileId);
        }

        public static bool IsRoofTile(int tileId)
        {
            return IsTileA3(tileId) && GetAutotileKind(tileId) % 16 < 8;
        }

        public static bool IsWallTopTile(int tileId)
        {
            return IsTileA4(tileId) && GetAutotileKind(tileId) % 16 < 8;
        }

        public static bool IsWallSideTile(int tileId)
        {
            return (IsTileA3(tileId) || IsTileA4(tileId)) && GetAutotileKind(tileId) % 16 >= 8;
        }

        public static bool IsWallTile(int tileId)
        {
            return IsWallTopTile(tileId) || IsWallSideTile(tileId);
        }

        public static bool IsFloorTypeAutotile(int tileId)
        {
            return (IsTileA1(tileId) && !IsWaterfallTile(tileId)) || IsTileA2(tileId) || IsWallTopTile(tileId);
        }

        public static bool IsWallTypeAutotile(int tileId)
        {
            return IsRoofTile(tileId) || IsWallSideTile(tileId);
        }

        public static bool IsWaterfallTypeAutotile(int tileId)
        {
            return IsWaterfallTile(tileId);
        }
        
        public static Tilemap Create(string name = "") => RmmzContainer._Create<Tilemap>(name);
    }
}