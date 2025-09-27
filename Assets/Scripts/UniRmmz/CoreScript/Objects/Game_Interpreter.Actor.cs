using System;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // HPの増減
        protected virtual bool Command311(object[] parameters)
        {
            int value = OperateValue(Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]), Convert.ToInt32(parameters[4]));
            IterateActorEx(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), actor =>
            {
                ChangeHp(actor, value, Convert.ToBoolean(parameters[5]));
            });
            return true;
        }

        // MPの増減
        protected virtual bool Command312(object[] parameters)
        {
            int value = OperateValue(Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]), Convert.ToInt32(parameters[4]));
            IterateActorEx(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), actor =>
            {
                actor.GainMp(value);
            });
            return true;
        }

        // TPの増減
        protected virtual bool Command326(object[] parameters)
        {
            int value = OperateValue(Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]), Convert.ToInt32(parameters[4]));
            IterateActorEx(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), actor =>
            {
                actor.GainTp(value);
            });
            return true;
        }

        // ステートの変更
        protected virtual bool Command313(object[] parameters)
        {
            IterateActorEx(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), actor =>
            {
                bool alreadyDead = actor.IsDead();
                
                if (Convert.ToInt32(parameters[2]) == 0)
                {
                    actor.AddState(Convert.ToInt32(parameters[3]));
                }
                else
                {
                    actor.RemoveState(Convert.ToInt32(parameters[3]));
                }
                
                if (actor.IsDead() && !alreadyDead)
                {
                    actor.PerformCollapse();
                }
                
                actor.ClearResult();
            });
            return true;
        }

        // 全回復
        protected virtual bool Command314(object[] parameters)
        {
            IterateActorEx(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), actor =>
            {
                actor.RecoverAll();
            });
            return true;
        }

        // 経験値アップ
        protected virtual bool Command315(object[] parameters)
        {
            int value = OperateValue(Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]), Convert.ToInt32(parameters[4]));
            IterateActorEx(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), actor =>
            {
                actor.ChangeExp(actor.CurrentExp() + value, Convert.ToBoolean(parameters[5]));
            });
            return true;
        }

        // レベルの増減
        protected virtual bool Command316(object[] parameters)
        {
            int value = OperateValue(Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]), Convert.ToInt32(parameters[4]));
            IterateActorEx(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), actor =>
            {
                actor.ChangeLevel(actor.Level + value, Convert.ToBoolean(parameters[5]));
            });
            return true;
        }

        // 能力値の増減
        protected virtual bool Command317(object[] parameters)
        {
            int value = OperateValue(Convert.ToInt32(parameters[3]), Convert.ToInt32(parameters[4]), Convert.ToInt32(parameters[5]));
            IterateActorEx(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), actor =>
            {
                actor.AddParam(Convert.ToInt32(parameters[2]), value);
            });
            return true;
        }

        // スキルの増減
        protected virtual bool Command318(object[] parameters)
        {
            IterateActorEx(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), actor =>
            {
                if (Convert.ToInt32(parameters[2]) == 0)
                {
                    actor.LearnSkill(Convert.ToInt32(parameters[3]));
                }
                else
                {
                    actor.ForgetSkill(Convert.ToInt32(parameters[3]));
                }
            });
            return true;
        }

        // 装備の変更
        protected virtual bool Command319(object[] parameters)
        {
            var actor = Rmmz.gameActors.Actor(Convert.ToInt32(parameters[0]));
            if (actor != null)
            {
                actor.ChangeEquipById(Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]));
            }
            return true;
        }

        // 名前の変更
        protected virtual bool Command320(object[] parameters)
        {
            var actor = Rmmz.gameActors.Actor(Convert.ToInt32(parameters[0]));
            if (actor != null)
            {
                actor.SetName(Convert.ToString(parameters[1]));
            }
            return true;
        }

        // 職業の変更
        protected virtual bool Command321(object[] parameters)
        {
            var actor = Rmmz.gameActors.Actor(Convert.ToInt32(parameters[0]));
            if (actor != null && Rmmz.dataClasses.ElementAtOrDefault(Convert.ToInt32(parameters[1])) != null)
            {
                actor.ChangeClass(Convert.ToInt32(parameters[1]), Convert.ToBoolean(parameters[2]));
            }
            return true;
        }

        // 二つ名の変更
        protected virtual bool Command324(object[] parameters)
        {
            var actor = Rmmz.gameActors.Actor(Convert.ToInt32(parameters[0]));
            if (actor != null)
            {
                actor.SetNickname(Convert.ToString(parameters[1]));
            }
            return true;
        }

        // プロフィールの変更
        protected virtual bool Command325(object[] parameters)
        {
            var actor = Rmmz.gameActors.Actor(Convert.ToInt32(parameters[0]));
            if (actor != null)
            {
                actor.SetProfile(Convert.ToString(parameters[1]));
            }
            return true;
        }

    }
}