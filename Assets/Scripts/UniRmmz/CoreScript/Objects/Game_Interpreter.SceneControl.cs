using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // 戦闘の処理
        protected virtual bool Command301(object[] parameters)
        {
            if (!Rmmz.gameParty.InBattle())
            {
                int troopId;
                int param0 = Convert.ToInt32(parameters[0]);
       
                if (param0 == 0)
                {
                    // Direct designation
                    troopId = Convert.ToInt32(parameters[1]);
                }
                else if (param0 == 1)
                {
                    // Designation with a variable
                    troopId = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[1]));
                }
                else
                {
                    // Same as Random Encounters
                    troopId = Rmmz.gamePlayer.MakeEncounterTroopId();
                }
       
                if (Rmmz.dataTroops[troopId] != null)
                {
                    Rmmz.BattleManager.Setup(troopId, Convert.ToBoolean(parameters[2]), Convert.ToBoolean(parameters[3]));
                    Rmmz.BattleManager.SetEventCallback(n => {
                        _branch[_indent] = n;
                    });
                    Rmmz.gamePlayer.MakeEncounterCount();
                    Scene_Battle.Push();
                }
            }
            return true;
        }

        // ショップの処理
       public virtual bool Command302(object[] parameters)
       {
           if (!Rmmz.gameParty.InBattle())
           {
               var goods = new List<ShopGoods> { new ShopGoods(parameters) };
               while (NextEventCode() == 605)
               {
                   _index++;
                   goods.Add(new ShopGoods(CurrentCommand().Parameters));
               }
               Scene_Shop.Push();
               Rmmz.SceneManager.PrepareNextScene(goods, Convert.ToBoolean(parameters[4]));
           }
           return true;
       }

       // 名前入力の処理
       public virtual bool Command303(object[] parameters)
       {
           if (!Rmmz.gameParty.InBattle())
           {
               int actorId = Convert.ToInt32(parameters[0]);
               if (Rmmz.dataActors.ElementAtOrDefault(actorId) != null)
               {
                   Scene_Name.Push();
                   Rmmz.SceneManager.PrepareNextScene(actorId, Convert.ToInt32(parameters[1]));
               }
           }
           return true;
       }

       // メニュー画面を開く
       public virtual bool Command351(object[] parameters)
       {
           if (!Rmmz.gameParty.InBattle())
           {
               Scene_Menu.Push();
               Window_MenuCommand.InitCommandPosition();
           }
           return true;
       }

       // セーブ画面を開く
       public virtual bool Command352(object[] parameters)
       {
           if (!Rmmz.gameParty.InBattle())
           {
               Scene_Save.Push();
           }
           return true;
       }

       // ゲームオーバー
       public virtual bool Command353(object[] parameters)
       {
           Scene_Gameover.Goto();
           return true;
       }

       // タイトル画面に戻す
       public virtual bool Command354(object[] parameters)
       {
           Scene_Title.Goto();
           return true;
       }

    }
}