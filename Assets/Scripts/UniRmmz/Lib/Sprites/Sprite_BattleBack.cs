using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying a background image in battle.
    /// </summary>
    public partial class Sprite_Battleback : TilingSprite
    {
        private int _type;

        public virtual void Initialize(int type)
        {
            _type = type;
            if (type == 0)
            {
                Bitmap = Battleback1Bitmap();
            }
            else
            {
                Bitmap = Battleback2Bitmap();
            }
        }

        public virtual void AdjustPosition()
        {
            Width = Mathf.Floor(1000 * Graphics.Width / 816f);
            Height = Mathf.Floor(740 * Graphics.Height / 624f);
            X = (Graphics.Width - Width) / 2;
            if (Rmmz.gameSystem.IsSideView())
            {
                Y = Graphics.Height - Height;
            }
            else
            {
                Y = 0;
            }
            
            float ratioX = Width / Bitmap.Width;
            float ratioY = Height / Bitmap.Height;
            float scale = Mathf.Max(ratioX, ratioY, 1.0f);
            Scale = new Vector2(scale, scale);
        }

        protected virtual Bitmap Battleback1Bitmap()
        {
            return Rmmz.ImageManager.LoadBattleback1(Battleback1Name());
        }

        protected virtual Bitmap Battleback2Bitmap()
        {
            return Rmmz.ImageManager.LoadBattleback2(Battleback2Name());
        }

        protected virtual string Battleback1Name()
        {
            if (Rmmz.BattleManager.IsBattleTest())
            {
                return Rmmz.dataSystem.Battleback1Name;
            }
            else if (!string.IsNullOrEmpty(Rmmz.gameMap.Battleback1Name()))
            {
                return Rmmz.gameMap.Battleback1Name();
            }
            else if (Rmmz.gameMap.IsOverworld())
            {
                return OverworldBattleback1Name();
            }
            else
            {
                return string.Empty;
            }
        }

        protected virtual string Battleback2Name()
        {
            if (Rmmz.BattleManager.IsBattleTest())
            {
                return Rmmz.dataSystem.Battleback2Name;
            }
            else if (!string.IsNullOrEmpty(Rmmz.gameMap.Battleback2Name()))
            {
                return Rmmz.gameMap.Battleback2Name();
            }
            else if (Rmmz.gameMap.IsOverworld())
            {
                return OverworldBattleback2Name();
            }
            else
            {
                return string.Empty;
            }
        }

        protected virtual string OverworldBattleback1Name()
        {
            if (Rmmz.gamePlayer.IsInVehicle())
            {
                return ShipBattleback1Name();
            }
            else
            {
                return NormalBattleback1Name();
            }
        }

        protected virtual string OverworldBattleback2Name()
        {
            if (Rmmz.gamePlayer.IsInVehicle())
            {
                return ShipBattleback2Name();
            }
            else
            {
                return NormalBattleback2Name();
            }
        }

        protected virtual string NormalBattleback1Name()
        {
            return TerrainBattleback1Name(AutotileType(1)) ??
                   TerrainBattleback1Name(AutotileType(0)) ??
                   DefaultBattleback1Name();
        }

        protected virtual string NormalBattleback2Name()
        {
            return TerrainBattleback2Name(AutotileType(1)) ??
                   TerrainBattleback2Name(AutotileType(0)) ??
                   DefaultBattleback2Name();
        }

        protected virtual string TerrainBattleback1Name(int type)
        {
            switch (type)
            {
                case 24:
                case 25:
                    return "Wasteland";
                case 26:
                case 27:
                    return "DirtField";
                case 32:
                case 33:
                    return "Desert";
                case 34:
                    return "Lava1";
                case 35:
                    return "Lava2";
                case 40:
                case 41:
                    return "Snowfield";
                case 42:
                    return "Clouds";
                case 4:
                case 5:
                    return "PoisonSwamp";
                default:
                    return null;
            }
        }

        protected virtual string TerrainBattleback2Name(int type)
        {
            switch (type)
            {
                case 20:
                case 21:
                    return "Forest";
                case 22:
                case 30:
                case 38:
                    return "Cliff";
                case 24:
                case 25:
                case 26:
                case 27:
                    return "Wasteland";
                case 32:
                case 33:
                    return "Desert";
                case 34:
                case 35:
                    return "Lava";
                case 40:
                case 41:
                    return "Snowfield";
                case 42:
                    return "Clouds";
                case 4:
                case 5:
                    return "PoisonSwamp";
                default:
                    return null;
            }
        }

        protected virtual string DefaultBattleback1Name()
        {
            return "Grassland";
        }

        protected virtual string DefaultBattleback2Name()
        {
            return "Grassland";
        }

        protected virtual string ShipBattleback1Name()
        {
            return "Ship";
        }

        protected virtual string ShipBattleback2Name()
        {
            return "Ship";
        }

        protected virtual int AutotileType(int z)
        {
            return Rmmz.gameMap.AutotileType(Rmmz.gamePlayer.X, Rmmz.gamePlayer.Y, z);
        }
    }
}
