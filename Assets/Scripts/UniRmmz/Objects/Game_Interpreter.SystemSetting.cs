using System;

namespace UniRmmz
{
   public partial class Game_Interpreter
   {
       // 戦闘BGMの変更
       public virtual bool Command132(object[] parameters)
       {
           var soundData = ConvertEx.ToSoundData(parameters[0]);
           Rmmz.gameSystem.SetBattleBgm(soundData);
           return true;
       }

       // 勝利MEの変更
       public virtual bool Command133(object[] parameters)
       {
           var soundData = ConvertEx.ToSoundData(parameters[0]);
           Rmmz.gameSystem.SetVictoryMe(soundData);
           return true;
       }

       // セーブ禁止の変更
       public virtual bool Command134(object[] parameters)
       {
           if (Convert.ToInt32(parameters[0]) == 0)
           {
               Rmmz.gameSystem.DisableSave();
           }
           else
           {
               Rmmz.gameSystem.EnableSave();
           }
           return true;
       }

       // メニュー禁止の変更
       public virtual bool Command135(object[] parameters)
       {
           if (Convert.ToInt32(parameters[0]) == 0)
           {
               Rmmz.gameSystem.DisableMenu();
           }
           else
           {
               Rmmz.gameSystem.EnableMenu();
           }
           return true;
       }

       // エンカウント禁止の変更
       public virtual bool Command136(object[] parameters)
       {
           if (Convert.ToInt32(parameters[0]) == 0)
           {
               Rmmz.gameSystem.DisableEncounter();
           }
           else
           {
               Rmmz.gameSystem.EnableEncounter();
           }
           Rmmz.gamePlayer.MakeEncounterCount();
           return true;
       }

       // 並び替え禁止の変更
       public virtual bool Command137(object[] parameters)
       {
           if (Convert.ToInt32(parameters[0]) == 0)
           {
               Rmmz.gameSystem.DisableFormation();
           }
           else
           {
               Rmmz.gameSystem.EnableFormation();
           }
           return true;
       }

       // ウィンドウカラー変更
       public virtual bool Command138(object[] parameters)
       {
           var tone = ConvertEx.ToArray<int>(parameters[0]);
           Rmmz.gameSystem.SetWindowTone(tone);
           return true;
       }

       // 敗北MEの変更
       public virtual bool Command139(object[] parameters)
       {
           var soundData = ConvertEx.ToSoundData(parameters[0]);
           Rmmz.gameSystem.SetDefeatMe(soundData);
           return true;
       }

       // 乗り物BGMの変更
       public virtual bool Command140(object[] parameters)
       {
           var vehicleType = (Game_Vehicle.VehicleTypes)Convert.ToInt32(parameters[0]);
           var vehicle = Rmmz.gameMap.Vehicle(vehicleType);
           if (vehicle != null)
           {
               var soundData = ConvertEx.ToSoundData(parameters[1]);
               vehicle.SetBgm(soundData);
           }
           return true;
       }

       // 主人公のグラフィックの変更
       public virtual bool Command322(object[] parameters)
       {
           var actor = Rmmz.gameActors.Actor(Convert.ToInt32(parameters[0]));
           if (actor != null)
           {
               actor.SetCharacterImage(Convert.ToString(parameters[1]), Convert.ToInt32(parameters[2]));
               actor.SetFaceImage(Convert.ToString(parameters[3]), Convert.ToInt32(parameters[4]));
               actor.SetBattlerImage(Convert.ToString(parameters[5]));
           }
           Rmmz.gamePlayer.Refresh();
           return true;
       }

       // 乗り物グラフィックの変更
       public virtual bool Command323(object[] parameters)
       {
           var vehicleType = (Game_Vehicle.VehicleTypes)Convert.ToInt32(parameters[0]);
           var vehicle = Rmmz.gameMap.Vehicle(vehicleType);
           if (vehicle != null)
           {
               vehicle.SetImage(Convert.ToString(parameters[1]), Convert.ToInt32(parameters[2]));
           }
           return true;
       }
   }
}