using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace UniRmmz
{
    /// <summary>
    /// The game object class for a battle action.
    /// </summary>
    [Serializable]
    public partial class Game_Action
    {
        public const int EFFECT_RECOVER_HP = 11;
        public const int EFFECT_RECOVER_MP = 12;
        public const int EFFECT_GAIN_TP = 13;
        public const int EFFECT_ADD_STATE = 21;
        public const int EFFECT_REMOVE_STATE = 22;
        public const int EFFECT_ADD_BUFF = 31;
        public const int EFFECT_ADD_DEBUFF = 32;
        public const int EFFECT_REMOVE_BUFF = 33;
        public const int EFFECT_REMOVE_DEBUFF = 34;
        public const int EFFECT_SPECIAL = 41;
        public const int EFFECT_GROW = 42;
        public const int EFFECT_LEARN_SKILL = 43;
        public const int EFFECT_COMMON_EVENT = 44;

        public const int SPECIAL_EFFECT_ESCAPE = 0;

        public const int HITTYPE_CERTAIN = 0;
        public const int HITTYPE_PHYSICAL = 1;
        public const int HITTYPE_MAGICAL = 2;

        protected int _subjectActorId;
        protected int _subjectEnemyIndex;
        protected bool _forcing;
        protected Game_Item _item;
        protected int _targetIndex;
        protected Game_Battler _reflectionTarget = null;

        protected Game_Action(Game_Battler subject = null, bool forcing = false)
        {
            _subjectActorId = 0;
            _subjectEnemyIndex = -1;
            _forcing = forcing;
            SetSubject(subject);
            Clear();
        }

        public virtual void Clear()
        {
            _item = Game_Item.Create();
            _targetIndex = -1;
        }

        public virtual void SetSubject(Game_Battler subject)
        {
            if (subject != null)
            {
                if (subject.IsActor())
                {
                    _subjectActorId = ((Game_Actor)subject).ActorId();
                    _subjectEnemyIndex = -1;
                }
                else
                {
                    _subjectEnemyIndex = subject.Index();
                    _subjectActorId = 0;
                }
            }
        }

        public virtual Game_Battler Subject()
        {
            if (_subjectActorId > 0)
            {
                return Rmmz.gameActors.Actor(_subjectActorId);
            }
            else
            {
                return Rmmz.gameTroop.Members().ElementAt(_subjectEnemyIndex);
            }
        }

        public virtual Game_Unit FriendsUnit()
        {
            return Subject().FriendsUnit();
        }

        public virtual Game_Unit OpponentsUnit()
        {
            return Subject().OpponentsUnit();
        }

        public virtual void SetEnemyAction(DataEnemyAction action)
        {
            if (action != null)
            {
                SetSkill(action.SkillId);
            }
            else
            {
                Clear();
            }
        }

        public virtual void SetAttack()
        {
            SetSkill(Subject().AttackSkillId());
        }

        public virtual void SetGuard()
        {
            SetSkill(Subject().GuardSkillId());
        }

        public virtual void SetSkill(int skillId)
        {
            _item.SetObject(Rmmz.dataSkills[skillId]);
        }

        public virtual void SetItem(int itemId)
        {
            _item.SetObject(Rmmz.dataItems[itemId]);
        }

        public virtual void SetItemObject(UsableItem obj)
        {
            _item.SetObject(obj);
        }

        public virtual void SetTarget(int targetIndex)
        {
            _targetIndex = targetIndex;
        }

        public virtual UsableItem Item()
        {
            return _item.Object<UsableItem>();
        }

        public virtual bool IsSkill()
        {
            return _item.IsSkill();
        }

        public virtual bool IsItem()
        {
            return _item.IsItem();
        }

        public virtual int NumRepeats()
        {
            int repeats = Item().Repeats;
            if (IsAttack())
            {
                repeats += Subject().AttackTimesAdd();
            }
            return Mathf.FloorToInt(repeats);
        }

        protected virtual bool CheckItemScope(int[] list)
        {
            return list.Contains(Item().Scope);
        }

        public virtual bool IsForOpponent()
        {
            return CheckItemScope(new int[] { 1, 2, 3, 4, 5, 6, 14 });
        }

        public virtual bool IsForFriend()
        {
            return CheckItemScope(new int[] { 7, 8, 9, 10, 11, 12, 13, 14 });
        }

        public virtual bool IsForEveryone()
        {
            return CheckItemScope(new int[] { 14 });
        }

        public virtual bool IsForAliveFriend()
        {
            return CheckItemScope(new int[] { 7, 8, 11, 14 });
        }

        public virtual bool IsForDeadFriend()
        {
            return CheckItemScope(new int[] { 9, 10 });
        }

        public virtual bool IsForUser()
        {
            return CheckItemScope(new int[] { 11 });
        }

        public virtual bool IsForOne()
        {
            return CheckItemScope(new int[] { 1, 3, 7, 9, 11, 12 });
        }

        public virtual bool IsForRandom()
        {
            return CheckItemScope(new int[] { 3, 4, 5, 6 });
        }

        public virtual bool IsForAll()
        {
            return CheckItemScope(new int[] { 2, 8, 10, 13, 14 });
        }

        public virtual bool NeedsSelection()
        {
            return CheckItemScope(new int[] { 1, 7, 9, 12 });
        }

        public virtual int NumTargets()
        {
            return IsForRandom() ? Item().Scope - 2 : 0;
        }

        protected virtual bool CheckDamageType(int[] list)
        {
            return list.Contains(Item().Damage.Type);
        }

        public virtual bool IsHpEffect()
        {
            return CheckDamageType(new int[] { 1, 3, 5 });
        }

        public virtual bool IsMpEffect()
        {
            return CheckDamageType(new int[] { 2, 4, 6 });
        }

        public virtual bool IsDamage()
        {
            return CheckDamageType(new int[] { 1, 2 });
        }

        public virtual bool IsRecover()
        {
            return CheckDamageType(new int[] { 3, 4 });
        }

        public virtual bool IsDrain()
        {
            return CheckDamageType(new int[] { 5, 6 });
        }

        public virtual bool IsHpRecover()
        {
            return CheckDamageType(new int[] { 3 });
        }

        public virtual bool IsMpRecover()
        {
            return CheckDamageType(new int[] { 4 });
        }

        public virtual bool IsCertainHit()
        {
            return Item().HitType == HITTYPE_CERTAIN;
        }

        public virtual bool IsPhysical()
        {
            return Item().HitType == HITTYPE_PHYSICAL;
        }

        public virtual bool IsMagical()
        {
            return Item().HitType == HITTYPE_MAGICAL;
        }

        public virtual bool IsAttack()
        {
            return Item() == Rmmz.dataSkills[Subject().AttackSkillId()];
        }

        public virtual bool IsGuard()
        {
            return Item() == Rmmz.dataSkills[Subject().GuardSkillId()];
        }

        public virtual bool IsMagicSkill()
        {
            if (IsSkill())
            {
                return Rmmz.DataSystem.MagicSkills.Contains(((DataSkill)Item()).StypeId);
            }
            else
            {
                return false;
            }
        }

        public virtual void DecideRandomTarget()
        {
            Game_Battler target;
            if (IsForDeadFriend())
            {
                target = FriendsUnit().RandomDeadTarget();
            }
            else if (IsForFriend())
            {
                target = FriendsUnit().RandomTarget();
            }
            else
            {
                target = OpponentsUnit().RandomTarget();
            }

            if (target != null)
            {
                _targetIndex = target.Index();
            }
            else
            {
                Clear();
            }
        }

        public virtual void SetConfusion()
        {
            SetAttack();
        }

        public virtual void Prepare()
        {
            if (Subject().IsConfused() && !_forcing)
            {
                SetConfusion();
            }
        }

        public virtual bool IsValid()
        {
            return (_forcing && Item() != null) || Subject().CanUse(Item());
        }

        public virtual int Speed()
        {
            int agi = Subject().Agi;
            int speed = agi + UnityEngine.Random.Range(0, Mathf.FloorToInt(5 + agi / 4f));
            if (Item() != null)
            {
                speed += Item().Speed;
            }
            if (IsAttack())
            {
                speed += Mathf.FloorToInt(Subject().AttackSpeed());
            }
            return speed;
        }

        public virtual List<Game_Battler> MakeTargets()
        {
            List<Game_Battler> targets = new List<Game_Battler>();
            if (!_forcing && Subject().IsConfused())
            {
                targets.Add(ConfusionTarget());
            }
            else if (IsForEveryone())
            {
                targets.AddRange(TargetsForEveryone());
            }
            else if (IsForOpponent())
            {
                targets.AddRange(TargetsForOpponents());
            }
            else if (IsForFriend())
            {
                targets.AddRange(TargetsForFriends());
            }
            return RepeatTargets(targets);
        }

        protected virtual List<Game_Battler> RepeatTargets(List<Game_Battler> targets)
        {
            List<Game_Battler> repeatedTargets = new List<Game_Battler>();
            int repeats = NumRepeats();
            foreach (Game_Battler target in targets)
            {
                if (target != null)
                {
                    for (int i = 0; i < repeats; i++)
                    {
                        repeatedTargets.Add(target);
                    }
                }
            }
            return repeatedTargets;
        }

        protected virtual Game_Battler ConfusionTarget()
        {
            switch (Subject().ConfusionLevel())
            {
                case 1:
                    return OpponentsUnit().RandomTarget();
                case 2:
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        return OpponentsUnit().RandomTarget();
                    }
                    return FriendsUnit().RandomTarget();
                default:
                    return FriendsUnit().RandomTarget();
            }
        }

        protected virtual List<Game_Battler> TargetsForEveryone()
        {
            var opponentMembers = OpponentsUnit().AliveMembers();
            var friendMembers = FriendsUnit().AliveMembers();
            return opponentMembers.Concat(friendMembers).ToList();
        }

        protected virtual List<Game_Battler> TargetsForOpponents()
        {
            var unit = OpponentsUnit();
            if (IsForRandom())
            {
                return RandomTargets(unit);
            }
            else
            {
                return TargetsForAlive(unit);
            }
        }

        protected virtual List<Game_Battler> TargetsForFriends()
        {
            var unit = FriendsUnit();
            if (IsForUser())
            {
                return new List<Game_Battler> { Subject() };
            }
            else if (IsForDeadFriend())
            {
                return TargetsForDead(unit);
            }
            else if (IsForAliveFriend())
            {
                return TargetsForAlive(unit);
            }
            else
            {
                return TargetsForDeadAndAlive(unit);
            }
        }

        protected virtual List<Game_Battler> RandomTargets(Game_Unit unit)
        {
            List<Game_Battler> targets = new List<Game_Battler>();
            for (int i = 0; i < NumTargets(); i++)
            {
                targets.Add(unit.RandomTarget());
            }
            return targets;
        }

        protected virtual List<Game_Battler> TargetsForDead(Game_Unit unit)
        {
            if (IsForOne())
            {
                return new List<Game_Battler> { unit.SmoothDeadTarget(_targetIndex) };
            }
            else
            {
                return unit.DeadMembers().ToList();
            }
        }

        protected virtual List<Game_Battler> TargetsForAlive(Game_Unit unit)
        {
            if (IsForOne())
            {
                if (_targetIndex < 0)
                {
                    return new List<Game_Battler> { unit.RandomTarget() };
                }
                else
                {
                    return new List<Game_Battler> { unit.SmoothTarget(_targetIndex) };
                }
            }
            else
            {
                return unit.AliveMembers().ToList();
            }
        }

        protected virtual List<Game_Battler> TargetsForDeadAndAlive(Game_Unit unit)
        {
            if (IsForOne())
            {
                return new List<Game_Battler> { unit.Members().ToList()[_targetIndex] };
            }
            else
            {
                return unit.Members().ToList();
            }
        }

        public virtual float Evaluate()
        {
            float value = 0;
            foreach (Game_Battler target in ItemTargetCandidates())
            {
                float targetValue = EvaluateWithTarget(target);
                if (IsForAll())
                {
                    value += targetValue;
                }
                else if (targetValue > value)
                {
                    value = targetValue;
                    _targetIndex = target.Index();
                }
            }
            value *= NumRepeats();
            if (value > 0)
            {
                value += UnityEngine.Random.value;
            }
            return value;
        }

        protected virtual List<Game_Battler> ItemTargetCandidates()
        {
            if (!IsValid())
            {
                return new List<Game_Battler>();
            }
            else if (IsForOpponent())
            {
                return OpponentsUnit().AliveMembers().ToList();
            }
            else if (IsForUser())
            {
                return new List<Game_Battler> { Subject() };
            }
            else if (IsForDeadFriend())
            {
                return FriendsUnit().DeadMembers().ToList();
            }
            else
            {
                return FriendsUnit().AliveMembers().ToList();
            }
        }

        protected virtual float EvaluateWithTarget(Game_Battler target)
        {
            if (IsHpEffect())
            {
                int value = MakeDamageValue(target, false);
                if (IsForOpponent())
                {
                    return (float)value / Mathf.Max(target.Hp, 1);
                }
                else
                {
                    int recovery = Mathf.Min(-value, target.Mhp - target.Hp);
                    return (float)recovery / target.Mhp;
                }
            }
            return 0;
        }

        public virtual bool TestApply(Game_Battler target)
        {
            return TestLifeAndDeath(target) &&
                   (Rmmz.gameParty.InBattle() ||
                    (IsHpRecover() && target.Hp < target.Mhp) ||
                    (IsMpRecover() && target.Mp < target.Mmp) ||
                    HasItemAnyValidEffects(target));
        }

        protected virtual bool TestLifeAndDeath(Game_Battler target)
        {
            if (IsForOpponent() || IsForAliveFriend())
            {
                return target.IsAlive();
            }
            else if (IsForDeadFriend())
            {
                return target.IsDead();
            }
            else
            {
                return true;
            }
        }

        protected virtual bool HasItemAnyValidEffects(Game_Battler target)
        {
            var effects = Item().Effects;
            return effects.Any(effect => TestItemEffect(target, effect));
        }

        protected virtual bool TestItemEffect(Game_Battler target, DataEffect dataEffect)
        {
            switch (dataEffect.Code)
            {
                case EFFECT_RECOVER_HP:
                    return target.Hp < target.Mhp || dataEffect.Value1 < 0 || dataEffect.Value2 < 0;
                case EFFECT_RECOVER_MP:
                    return target.Mp < target.Mmp || dataEffect.Value1 < 0 || dataEffect.Value2 < 0;
                case EFFECT_ADD_STATE:
                    return !target.IsStateAffected(dataEffect.DataId);
                case EFFECT_REMOVE_STATE:
                    return target.IsStateAffected(dataEffect.DataId);
                case EFFECT_ADD_BUFF:
                    return !target.IsMaxBuffAffected(dataEffect.DataId);
                case EFFECT_ADD_DEBUFF:
                    return !target.IsMaxDebuffAffected(dataEffect.DataId);
                case EFFECT_REMOVE_BUFF:
                    return target.IsBuffAffected(dataEffect.DataId);
                case EFFECT_REMOVE_DEBUFF:
                    return target.IsDebuffAffected(dataEffect.DataId);
                case EFFECT_LEARN_SKILL:
                    return target.IsActor() && !((Game_Actor)target).IsLearnedSkill(dataEffect.DataId);
                default:
                    return true;
            }
        }

        public virtual float ItemCnt(Game_Battler target)
        {
            if (IsPhysical() && target.CanMove())
            {
                return target.Cnt;
            }
            else
            {
                return 0;
            }
        }

        public virtual float ItemMrf(Game_Battler target)
        {
            if (IsMagical())
            {
                return target.Mrf;
            }
            else
            {
                return 0;
            }
        }

        public virtual float ItemHit(Game_Battler target)
        {
            float successRate = Item().SuccessRate;
            if (IsPhysical())
            {
                return successRate * 0.01f * Subject().Hit;
            }
            else
            {
                return successRate * 0.01f;
            }
        }

        public virtual float ItemEva(Game_Battler target)
        {
            if (IsPhysical())
            {
                return target.Eva;
            }
            else if (IsMagical())
            {
                return target.Mev;
            }
            else
            {
                return 0;
            }
        }

        public virtual float ItemCri(Game_Battler target)
        {
            return Item().Damage.Critical ? Subject().Cri * (1 - target.Cev) : 0;
        }

        public virtual void Apply(Game_Battler target)
        {
            var result = target.Result();
            Subject().ClearResult();
            result.Clear();
            result.used = TestApply(target);
            result.missed = result.used && UnityEngine.Random.value >= ItemHit(target);
            result.evaded = !result.missed && UnityEngine.Random.value < ItemEva(target);
            result.physical = IsPhysical();
            result.drain = IsDrain();

            if (result.IsHit())
            {
                if (Item().Damage.Type > 0)
                {
                    result.critical = UnityEngine.Random.value < ItemCri(target);
                    int value = MakeDamageValue(target, result.critical);
                    ExecuteDamage(target, value);
                }
                foreach (var effect in Item().Effects)
                {
                    ApplyItemEffect(target, effect);
                }
                ApplyItemUserEffect(target);
            }
            UpdateLastTarget(target);
        }

        protected virtual int MakeDamageValue(Game_Battler target, bool critical)
        {
            var item = Item();
            float baseValue = EvalDamageFormula(target);
            float value = baseValue * CalcElementRate(target);
            
            if (IsPhysical())
            {
                value *= target.Pdr;
            }
            if (IsMagical())
            {
                value *= target.Mdr;
            }
            if (baseValue < 0)
            {
                value *= target.Rec;
            }
            if (critical)
            {
                value = ApplyCritical(value);
            }
            value = ApplyVariance(value, item.Damage.Variance);
            value = ApplyGuard(value, target);
            return Mathf.RoundToInt(value);
        }

        protected virtual float EvalDamageFormula(Game_Battler target)
        {
            try
            {
                var item = Item();
                Game_Battler a = Subject();
                Game_Battler b = target;
                var v = Rmmz.gameVariables;
                int sign = new int[] { 3, 4 }.Contains(item.Damage.Type) ? -1 : 1;
                float value = Mathf.Max(RmmzEval.EvaluateDamageFormula(item.Damage.Formula, a, b, v), 0) * sign;
                return float.IsNaN(value) ? 0f : value;
            }
            catch
            {
                return 0;
            }
        }

        protected virtual float CalcElementRate(Game_Battler target)
        {
            if (Item().Damage.ElementId < 0)
            {
                return ElementsMaxRate(target, Subject().AttackElements().ToList());
            }
            else
            {
                return target.ElementRate(Item().Damage.ElementId);
            }
        }

        protected virtual float ElementsMaxRate(Game_Battler target, List<int> elements)
        {
            if (elements.Count > 0)
            {
                var rates = elements.Select(elementId => target.ElementRate(elementId));
                return rates.Max();
            }
            else
            {
                return 1;
            }
        }

        protected virtual float ApplyCritical(float damage)
        {
            return damage * 3;
        }

        protected virtual float ApplyVariance(float damage, int variance)
        {
            float amp = Mathf.Floor(Mathf.Max((Mathf.Abs(damage) * variance) / 100, 0));
            float v = UnityEngine.Random.Range(0, amp + 1) + UnityEngine.Random.Range(0, amp + 1) - amp;
            return damage >= 0 ? damage + v : damage - v;
        }

        protected virtual float ApplyGuard(float damage, Game_Battler target)
        {
            return damage / (damage > 0 && target.IsGuard() ? 2 * target.Grd : 1);
        }

        protected virtual void ExecuteDamage(Game_Battler target, int value)
        {
            var result = target.Result();
            if (value == 0)
            {
                result.critical = false;
            }
            if (IsHpEffect())
            {
                ExecuteHpDamage(target, value);
            }
            if (IsMpEffect())
            {
                ExecuteMpDamage(target, value);
            }
        }

        protected virtual void ExecuteHpDamage(Game_Battler target, int value)
        {
            if (IsDrain())
            {
                value = Mathf.Min(target.Hp, value);
            }
            MakeSuccess(target);
            target.GainHp(-value);
            if (value > 0)
            {
                target.OnDamage(value);
            }
            GainDrainedHp(value);
        }

        protected virtual void ExecuteMpDamage(Game_Battler target, int value)
        {
            if (!IsMpRecover())
            {
                value = Mathf.Min(target.Mp, value);
            }
            if (value != 0)
            {
                MakeSuccess(target);
            }
            target.GainMp(-value);
            GainDrainedMp(value);
        }

        protected virtual void GainDrainedHp(int value)
        {
            if (IsDrain())
            {
                Game_Battler gainTarget = Subject();
                if (_reflectionTarget != null)
                {
                    gainTarget = _reflectionTarget;
                }
                gainTarget.GainHp(value);
            }
        }

        protected virtual void GainDrainedMp(int value)
        {
            if (IsDrain())
            {
                Game_Battler gainTarget = Subject();
                if (_reflectionTarget != null)
                {
                    gainTarget = _reflectionTarget;
                }
                gainTarget.GainMp(value);
            }
        }

        protected virtual void ApplyItemEffect(Game_Battler target, DataEffect dataEffect)
        {
            switch (dataEffect.Code)
            {
                case EFFECT_RECOVER_HP:
                    ItemEffectRecoverHp(target, dataEffect);
                    break;
                case EFFECT_RECOVER_MP:
                    ItemEffectRecoverMp(target, dataEffect);
                    break;
                case EFFECT_GAIN_TP:
                    ItemEffectGainTp(target, dataEffect);
                    break;
                case EFFECT_ADD_STATE:
                    ItemEffectAddState(target, dataEffect);
                    break;
                case EFFECT_REMOVE_STATE:
                    ItemEffectRemoveState(target, dataEffect);
                    break;
                case EFFECT_ADD_BUFF:
                    ItemEffectAddBuff(target, dataEffect);
                    break;
                case EFFECT_ADD_DEBUFF:
                    ItemEffectAddDebuff(target, dataEffect);
                    break;
                case EFFECT_REMOVE_BUFF:
                    ItemEffectRemoveBuff(target, dataEffect);
                    break;
                case EFFECT_REMOVE_DEBUFF:
                    ItemEffectRemoveDebuff(target, dataEffect);
                    break;
                case EFFECT_SPECIAL:
                    ItemEffectSpecial(target, dataEffect);
                    break;
                case EFFECT_GROW:
                    ItemEffectGrow(target, dataEffect);
                    break;
                case EFFECT_LEARN_SKILL:
                    ItemEffectLearnSkill(target, dataEffect);
                    break;
                case EFFECT_COMMON_EVENT:
                    ItemEffectCommonEvent(target, dataEffect);
                    break;
            }
        }

        protected virtual void ItemEffectRecoverHp(Game_Battler target, DataEffect dataEffect)
        {
            float value = (target.Mhp * dataEffect.Value1 + dataEffect.Value2) * target.Rec;
            if (IsItem())
            {
                value *= Subject().Pha;
            }
            int finalValue = Mathf.FloorToInt(value);
            if (finalValue != 0)
            {
                target.GainHp(finalValue);
                MakeSuccess(target);
            }
        }

        protected virtual void ItemEffectRecoverMp(Game_Battler target, DataEffect dataEffect)
        {
            float value = (target.Mmp * dataEffect.Value1 + dataEffect.Value2) * target.Rec;
            if (IsItem())
            {
                value *= Subject().Pha;
            }
            int finalValue = Mathf.FloorToInt(value);
            if (finalValue != 0)
            {
                target.GainMp(finalValue);
                MakeSuccess(target);
            }
        }

        protected virtual void ItemEffectGainTp(Game_Battler target, DataEffect dataEffect)
        {
            int value = Mathf.FloorToInt(dataEffect.Value1);
            if (value != 0)
            {
                target.GainTp(value);
                MakeSuccess(target);
            }
        }

        protected virtual void ItemEffectAddState(Game_Battler target, DataEffect dataEffect)
        {
            if (dataEffect.DataId == 0)
            {
                ItemEffectAddAttackState(target, dataEffect);
            }
            else
            {
                ItemEffectAddNormalState(target, dataEffect);
            }
        }

        protected virtual void ItemEffectAddAttackState(Game_Battler target, DataEffect dataEffect)
        {
            foreach (int stateId in Subject().AttackStates())
            {
                float chance = dataEffect.Value1;
                chance *= target.StateRate(stateId);
                chance *= Subject().AttackStatesRate(stateId);
                chance *= LukEffectRate(target);
                if (UnityEngine.Random.value < chance)
                {
                    target.AddState(stateId);
                    MakeSuccess(target);
                }
            }
        }
        
        protected virtual void ItemEffectAddNormalState(Game_Battler target, DataEffect dataEffect)
        {
            float chance = dataEffect.Value1;
            if (!IsCertainHit())
            {
                chance *= target.StateRate(dataEffect.DataId);
                chance *= LukEffectRate(target);
            }
            if (UnityEngine.Random.value < chance)
            {
                target.AddState(dataEffect.DataId);
                MakeSuccess(target);
            }
        }

        protected virtual void ItemEffectRemoveState(Game_Battler target, DataEffect dataEffect)
        {
            float chance = dataEffect.Value1;
            if (UnityEngine.Random.value < chance)
            {
                target.RemoveState(dataEffect.DataId);
                MakeSuccess(target);
            }
        }

        protected virtual void ItemEffectAddBuff(Game_Battler target, DataEffect dataEffect)
        {
            target.AddBuff(dataEffect.DataId, (int)dataEffect.Value1);
            MakeSuccess(target);
        }

        protected virtual void ItemEffectAddDebuff(Game_Battler target, DataEffect dataEffect)
        {
            float chance = target.DebuffRate(dataEffect.DataId) * LukEffectRate(target);
            if (UnityEngine.Random.value < chance)
            {
                target.AddDebuff(dataEffect.DataId, (int)dataEffect.Value1);
                MakeSuccess(target);
            }
        }

        protected virtual void ItemEffectRemoveBuff(Game_Battler target, DataEffect dataEffect)
        {
            if (target.IsBuffAffected(dataEffect.DataId))
            {
                target.RemoveBuff(dataEffect.DataId);
                MakeSuccess(target);
            }
        }

        protected virtual void ItemEffectRemoveDebuff(Game_Battler target, DataEffect dataEffect)
        {
            if (target.IsDebuffAffected(dataEffect.DataId))
            {
                target.RemoveBuff(dataEffect.DataId);
                MakeSuccess(target);
            }
        }

        protected virtual void ItemEffectSpecial(Game_Battler target, DataEffect dataEffect)
        {
            if (dataEffect.DataId == SPECIAL_EFFECT_ESCAPE)
            {
                target.Escape();
                MakeSuccess(target);
            }
        }

        protected virtual void ItemEffectGrow(Game_Battler target, DataEffect dataEffect)
        {
            target.AddParam(dataEffect.DataId, Mathf.FloorToInt(dataEffect.Value1));
            MakeSuccess(target);
        }

        protected virtual void ItemEffectLearnSkill(Game_Battler target, DataEffect dataEffect)
        {
            if (target.IsActor())
            {
                ((Game_Actor)target).LearnSkill(dataEffect.DataId);
                MakeSuccess(target);
            }
        }

        protected virtual void ItemEffectCommonEvent(Game_Battler target, object effect)
        {
            // Common event handling
        }

        protected virtual void MakeSuccess(Game_Battler target)
        {
            target.Result().success = true;
        }

        protected virtual void ApplyItemUserEffect(Game_Battler target)
        {
            int value = Mathf.FloorToInt(Item().TpGain * Subject().Tcr);
            Subject().GainSilentTp(value);
        }

        protected virtual float LukEffectRate(Game_Battler target)
        {
            return Mathf.Max(1.0f + (Subject().Luk - target.Luk) * 0.001f, 0.0f);
        }

        public virtual void ApplyGlobal()
        {
            foreach (var effect in Item().Effects)
            {
                if (effect.Code == EFFECT_COMMON_EVENT)
                {
                    Rmmz.gameTemp.ReserveCommonEvent(effect.DataId);
                }
            }
            UpdateLastUsed();
            UpdateLastSubject();
        }

        protected virtual void UpdateLastUsed()
        {
            var item = Item();
            if (Rmmz.DataManager.IsSkill(item))
            {
                Rmmz.gameTemp.SetLastUsedSkillId(item.Id);
            }
            else if (Rmmz.DataManager.IsItem(item))
            {
                Rmmz.gameTemp.SetLastUsedItemId(item.Id);
            }
        }

        protected virtual void UpdateLastSubject()
        {
            Game_Battler subject = Subject();
            if (subject.IsActor())
            {
                Rmmz.gameTemp.SetLastSubjectActorId(((Game_Actor)subject).ActorId());
            }
            else
            {
                Rmmz.gameTemp.SetLastSubjectEnemyIndex(subject.Index() + 1);
            }
        }

        protected virtual void UpdateLastTarget(Game_Battler target)
        {
            if (target.IsActor())
            {
                Rmmz.gameTemp.SetLastTargetActorId(((Game_Actor)target).ActorId());
            }
            else
            {
                Rmmz.gameTemp.SetLastTargetEnemyIndex(target.Index() + 1);
            }
        }

        public void SetReflectionTarget(Game_Battler target)
        {
            _reflectionTarget = target;
        }
    }

}