using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for an enemy.
    /// </summary>
    [Serializable]
    public partial class Game_Enemy : Game_Battler
    {
        protected int _enemyId;
        protected string _letter;
        protected bool _plural;
        protected float _screenX;
        protected float _screenY;

        protected Game_Enemy(int enemyId, float x, float y)
        {
            Setup(enemyId, x, y);
        }

        protected override void InitMembers()
        {
            base.InitMembers();
            _enemyId = 0;
            _letter = "";
            _plural = false;
            _screenX = 0;
            _screenY = 0;
        }

        protected virtual void Setup(int enemyId, float x, float y)
        {
            _enemyId = enemyId;
            _screenX = x;
            _screenY = y;
            RecoverAll();
        }

        public override bool IsEnemy()
        {
            return true;
        }

        public override Game_Unit FriendsUnit()
        {
            return Rmmz.gameTroop;
        }

        public override Game_Unit OpponentsUnit()
        {
            return Rmmz.gameParty;
        }

        public override int Index()
        {
            return Rmmz.gameTroop.Members().ToList().IndexOf(this);
        }

        public override bool IsBattleMember()
        {
            return Index() >= 0;
        }

        public virtual int EnemyId()
        {
            return _enemyId;
        }

        public virtual DataEnemy Enemy()
        {
            return Rmmz.dataEnemies[_enemyId];
        }

        protected override IEnumerable<ITraitsObject> TraitObjects()
        {
            foreach (var obj in base.TraitObjects())
            {
                yield return obj;
            }
            yield return Enemy();
        }

        protected override int ParamBase(int paramId)
        {
            return Enemy().Params[paramId];
        }

        public virtual int Exp()
        {
            return Enemy().Exp;
        }

        public virtual int Gold()
        {
            return Enemy().Gold;
        }

        public virtual List<DataCommonItem> MakeDropItems()
        {
            float rate = DropItemRate();
            var result = new List<DataCommonItem>();
            
            foreach (var di in Enemy().DropItems)
            {
                if (di.Kind > 0 && UnityEngine.Random.value * di.Denominator < rate)
                {
                    var item = ItemObject(di.Kind, di.DataId);
                    if (item != null)
                    {
                        result.Add(item);
                    }
                }
            }
            
            return result;
        }

        protected virtual float DropItemRate()
        {
            return Rmmz.gameParty.HasDropItemDouble() ? 2 : 1;
        }

        protected virtual DataCommonItem ItemObject(int kind, int dataId)
        {
            switch (kind)
            {
                case 1:
                    return Rmmz.dataItems[dataId];
                case 2:
                    return Rmmz.dataWeapons[dataId];
                case 3:
                    return Rmmz.dataArmors[dataId];
                default:
                    return null;
            }
        }

        public override bool IsSpriteVisible()
        {
            return true;
        }

        public virtual float ScreenX()
        {
            return _screenX;
        }

        public virtual float ScreenY()
        {
            return _screenY;
        }

        public virtual string BattlerName()
        {
            return Enemy().BattlerName;
        }

        public virtual int BattlerHue()
        {
            return Enemy().BattlerHue;
        }

        public virtual string OriginalName()
        {
            return Enemy().Name;
        }

        public override string Name()
        {
            return OriginalName() + (_plural ? _letter : "");
        }

        public virtual bool IsLetterEmpty()
        {
            return string.IsNullOrEmpty(_letter);
        }

        public virtual void SetLetter(string letter)
        {
            _letter = letter ?? "";
        }

        public virtual void SetPlural(bool plural)
        {
            _plural = plural;
        }

        public override void PerformActionStart(Game_Action action)
        {
            base.PerformActionStart(action);
            RequestEffect("whiten");
        }

        public override void PerformAction(Game_Action action)
        {
            base.PerformAction(action);
        }

        public override void PerformActionEnd()
        {
            base.PerformActionEnd();
        }

        public override void PerformDamage()
        {
            base.PerformDamage();
            Rmmz.SoundManager.PlayEnemyDamage();
            RequestEffect("blink");
        }

        public override void PerformCollapse()
        {
            base.PerformCollapse();
            switch (CollapseType())
            {
                case 0:
                    RequestEffect("collapse");
                    Rmmz.SoundManager.PlayEnemyCollapse();
                    break;
                case 1:
                    RequestEffect("bossCollapse");
                    Rmmz.SoundManager.PlayBossCollapse1();
                    break;
                case 2:
                    RequestEffect("instantCollapse");
                    break;
            }
        }

        public virtual void Transform(int enemyId)
        {
            string name = OriginalName();
            _enemyId = enemyId;
            if (OriginalName() != name)
            {
                _letter = "";
                _plural = false;
            }
            Refresh();
            if (NumActions() > 0)
            {
                MakeActions();
            }
        }

        public virtual bool MeetsCondition(DataEnemyAction action)
        {
            float param1 = action.ConditionParam1;
            float param2 = action.ConditionParam2;
            switch (action.ConditionType)
            {
                case 1:
                    return MeetsTurnCondition(param1, param2);
                case 2:
                    return MeetsHpCondition(param1, param2);
                case 3:
                    return MeetsMpCondition(param1, param2);
                case 4:
                    return MeetsStateCondition(param1);
                case 5:
                    return MeetsPartyLevelCondition(param1);
                case 6:
                    return MeetsSwitchCondition(param1);
                default:
                    return true;
            }
        }

        protected virtual bool MeetsTurnCondition(float param1, float param2)
        {
            int n = TurnCount();
            if (param2 == 0)
            {
                return n == param1;
            }
            else
            {
                return n > 0 && n >= param1 && n % param2 == param1 % param2;
            }
        }

        protected virtual bool MeetsHpCondition(float param1, float param2)
        {
            float hpRate = HpRate();
            return hpRate >= param1 && hpRate <= param2;
        }

        protected virtual bool MeetsMpCondition(float param1, float param2)
        {
            float mpRate = MpRate();
            return mpRate >= param1 && mpRate <= param2;
        }

        protected virtual bool MeetsStateCondition(float param)
        {
            return IsStateAffected((int)param);
        }

        protected virtual bool MeetsPartyLevelCondition(float param)
        {
            return Rmmz.gameParty.HighestLevel() >= param;
        }

        protected virtual bool MeetsSwitchCondition(float param)
        {
            return Rmmz.gameSwitches.Value((int)param);
        }

        protected virtual bool IsActionValid(DataEnemyAction action)
        {
            return MeetsCondition(action) && CanUse(Rmmz.dataSkills[action.SkillId]);
        }

        protected virtual DataEnemyAction SelectAction(List<DataEnemyAction> actionList, int ratingZero)
        {
            int sum = actionList.Sum(a => a.Rating - ratingZero);
            if (sum > 0)
            {
                int value = UnityEngine.Random.Range(0, sum);
                foreach (var action in actionList)
                {
                    value -= action.Rating - ratingZero;
                    if (value < 0)
                    {
                        return action;
                    }
                }
            }
            return null;
        }

        protected virtual void SelectAllActions(List<DataEnemyAction> actionList)
        {
            int ratingMax = actionList.Max(a => a.Rating);
            int ratingZero = ratingMax - 3;
            actionList = actionList.Where(a => a.Rating > ratingZero).ToList();
            
            for (int i = 0; i < NumActions(); i++)
            {
                Action(i).SetEnemyAction(SelectAction(actionList, ratingZero));
            }
        }

        public override void MakeActions()
        {
            base.MakeActions();
            if (NumActions() > 0)
            {
                var actionList = Enemy().Actions.Where(a => IsActionValid(a)).ToList();
                if (actionList.Count > 0)
                {
                    SelectAllActions(actionList);
                }
            }
            SetActionState("waiting");
        }
    }
}