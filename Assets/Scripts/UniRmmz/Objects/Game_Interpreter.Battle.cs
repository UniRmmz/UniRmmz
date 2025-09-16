using System;
using System.Collections.Generic;

namespace UniRmmz
{
   public partial class Game_Interpreter
   {
       // 敵キャラのHP増減
       public virtual bool Command331(object[] parameters)
       {
           int value = OperateValue(Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]));
           IterateEnemyIndex(Convert.ToInt32(parameters[0]), enemy =>
           {
               ChangeHp(enemy, value, Convert.ToBoolean(parameters[4]));
           });
           return true;
       }

       // 敵キャラのMP増減
       public virtual bool Command332(object[] parameters)
       {
           int value = OperateValue(Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]));
           IterateEnemyIndex(Convert.ToInt32(parameters[0]), enemy =>
           {
               enemy.GainMp(value);
           });
           return true;
       }

       // 敵キャラのTP増減
       public virtual bool Command342(object[] parameters)
       {
           int value = OperateValue(Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]));
           IterateEnemyIndex(Convert.ToInt32(parameters[0]), enemy =>
           {
               enemy.GainTp(value);
           });
           return true;
       }

       // 敵キャラのステート
       public virtual bool Command333(object[] parameters)
       {
           IterateEnemyIndex(Convert.ToInt32(parameters[0]), enemy =>
           {
               bool alreadyDead = enemy.IsDead();
               
               if (Convert.ToInt32(parameters[1]) == 0)
               {
                   enemy.AddState(Convert.ToInt32(parameters[2]));
               }
               else
               {
                   enemy.RemoveState(Convert.ToInt32(parameters[2]));
               }
               
               if (enemy.IsDead() && !alreadyDead)
               {
                   enemy.PerformCollapse();
               }
               
               enemy.ClearResult();
           });
           return true;
       }

       // 敵キャラの全回復
       public virtual bool Command334(object[] parameters)
       {
           IterateEnemyIndex(Convert.ToInt32(parameters[0]), enemy =>
           {
               enemy.RecoverAll();
           });
           return true;
       }

       // 敵キャラの出現
       public virtual bool Command335(object[] parameters)
       {
           IterateEnemyIndex(Convert.ToInt32(parameters[0]), enemy =>
           {
               enemy.Appear();
               Rmmz.gameTroop.MakeUniqueNames();
           });
           return true;
       }

       // 敵キャラの変身
       public virtual bool Command336(object[] parameters)
       {
           IterateEnemyIndex(Convert.ToInt32(parameters[0]), enemy =>
           {
               enemy.Transform(Convert.ToInt32(parameters[1]));
               Rmmz.gameTroop.MakeUniqueNames();
           });
           return true;
       }

       // 戦闘アニメーションの表示
       public virtual bool Command337(object[] parameters)
       {
           int param = Convert.ToInt32(parameters[0]);
           if (parameters.Length > 2 && Convert.ToBoolean(parameters[2]))
           {
               param = -1;
           }
           
           var targets = new List<object>();
           IterateEnemyIndex(param, enemy =>
           {
               if (enemy.IsAlive())
               {
                   targets.Add(enemy);
               }
           });
           
           Rmmz.gameTemp.RequestAnimation(targets, Convert.ToInt32(parameters[1]));
           return true;
       }

       // 戦闘行動の強制
       public virtual bool Command339(object[] parameters)
       {
           IterateBattler(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), battler =>
           {
               if (!battler.IsDeathStateAffected())
               {
                   battler.ForceAction(Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]));
                   Rmmz.BattleManager.ForceAction(battler);
                   SetWaitMode("action");
               }
           });
           return true;
       }

       // 戦闘中止
       public virtual bool Command340(object[] parameters)
       {
           Rmmz.BattleManager.Abort();
           return true;
       }

       // 勝ったとき
       public virtual bool Command601(object[] parameters)
       {
           if (_branch[_indent] != 0)
           {
               SkipBranch();
           }
           return true;
       }

       // 逃げたとき
       public virtual bool Command602(object[] parameters)
       {
           if (_branch[_indent] != 1)
           {
               SkipBranch();
           }
           return true;
       }

       // 負けたとき
       public virtual bool Command603(object[] parameters)
       {
           if (_branch[_indent] != 2)
           {
               SkipBranch();
           }
           return true;
       }
   }
}