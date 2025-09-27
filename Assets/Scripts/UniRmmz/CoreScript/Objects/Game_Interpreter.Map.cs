using System;
using System.Linq;

namespace UniRmmz
{
   public partial class Game_Interpreter
   {
       // マップ名表示の変更
       protected virtual bool Command281(object[] parameters)
       {
           if (Convert.ToInt32(parameters[0]) == 0)
           {
               Rmmz.gameMap.EnableNameDisplay();
           }
           else
           {
               Rmmz.gameMap.DisableNameDisplay();
           }
           return true;
       }

       // タイルセットの変更
       protected virtual bool Command282(object[] parameters)
       {
           int tilesetId = Convert.ToInt32(parameters[0]);
           var tileset = Rmmz.dataTilesets[tilesetId];
           
           if (tileset != null)
           {
               bool allReady = tileset.TilesetNames
                   .Select(tilesetName => Rmmz.ImageManager.LoadTileset(tilesetName))
                   .All(bitmap => bitmap.IsReady());
                   
               if (allReady)
               {
                   Rmmz.gameMap.ChangeTileset(tilesetId);
                   return true;
               }
           }
           
           return false;
       }

       // 戦闘背景の変更
       protected virtual bool Command283(object[] parameters)
       {
           Rmmz.gameMap.ChangeBattleback(Convert.ToString(parameters[0]), Convert.ToString(parameters[1]));
           return true;
       }

       // 遠景の変更
       protected virtual bool Command284(object[] parameters)
       {
           Rmmz.gameMap.ChangeParallax(
               Convert.ToString(parameters[0]),
               Convert.ToInt32(parameters[1]) != 0,
               Convert.ToInt32(parameters[2]) != 0,
               Convert.ToInt32(parameters[3]),
               Convert.ToInt32(parameters[4])
           );
           return true;
       }

       // 指定位置の情報取得
       protected virtual bool Command285(object[] parameters)
       {
           int x, y, value;
           int locationType = Convert.ToInt32(parameters[2]);
           
           if (locationType == 0)
           {
               // Direct designation
               x = Convert.ToInt32(parameters[3]);
               y = Convert.ToInt32(parameters[4]);
           }
           else if (locationType == 1)
           {
               // Designation with variables
               x = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[3]));
               y = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[4]));
           }
           else
           {
               // Designation by a character
               var character = Character(Convert.ToInt32(parameters[3]));
               x = character.X;
               y = character.Y;
           }
           
           int infoType = Convert.ToInt32(parameters[1]);
           switch (infoType)
           {
               case 0: // Terrain Tag
                   value = Rmmz.gameMap.TerrainTag(x, y);
                   break;
               case 1: // Event ID
                   value = Rmmz.gameMap.EventIdXy(x, y);
                   break;
               case 2: // Tile ID (Layer 1)
               case 3: // Tile ID (Layer 2)
               case 4: // Tile ID (Layer 3)
               case 5: // Tile ID (Layer 4)
                   value = Rmmz.gameMap.TileId(x, y, infoType - 2);
                   break;
               default: // Region ID
                   value = Rmmz.gameMap.RegionId(x, y);
                   break;
           }
           
           Rmmz.gameVariables.SetValue(Convert.ToInt32(parameters[0]), value);
           return true;
       }
   }
}