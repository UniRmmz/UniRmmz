using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of Game_Battler. It mainly contains parameters calculation.
    /// </summary>
    [Serializable]
    public abstract partial class Game_BattlerBase
    {
        public const int TRAIT_ELEMENT_RATE = 11;
        public const int TRAIT_DEBUFF_RATE = 12;
        public const int TRAIT_STATE_RATE = 13;
        public const int TRAIT_STATE_RESIST = 14;
        public const int TRAIT_PARAM = 21;
        public const int TRAIT_XPARAM = 22;
        public const int TRAIT_SPARAM = 23;
        public const int TRAIT_ATTACK_ELEMENT = 31;
        public const int TRAIT_ATTACK_STATE = 32;
        public const int TRAIT_ATTACK_SPEED = 33;
        public const int TRAIT_ATTACK_TIMES = 34;
        public const int TRAIT_ATTACK_SKILL = 35;
        public const int TRAIT_STYPE_ADD = 41;
        public const int TRAIT_STYPE_SEAL = 42;
        public const int TRAIT_SKILL_ADD = 43;
        public const int TRAIT_SKILL_SEAL = 44;
        public const int TRAIT_EQUIP_WTYPE = 51;
        public const int TRAIT_EQUIP_ATYPE = 52;
        public const int TRAIT_EQUIP_LOCK = 53;
        public const int TRAIT_EQUIP_SEAL = 54;
        public const int TRAIT_SLOT_TYPE = 55;
        public const int TRAIT_ACTION_PLUS = 61;
        public const int TRAIT_SPECIAL_FLAG = 62;
        public const int TRAIT_COLLAPSE_TYPE = 63;
        public const int TRAIT_PARTY_ABILITY = 64;

        public const int FLAG_ID_AUTO_BATTLE = 0;
        public const int FLAG_ID_GUARD = 1;
        public const int FLAG_ID_SUBSTITUTE = 2;
        public const int FLAG_ID_PRESERVE_TP = 3;

        public const int ICON_BUFF_START = 32;
        public const int ICON_DEBUFF_START = 48;

        
        protected int _hp;
        protected int _mp;
        protected int _tp;
        protected bool _hidden = false;
        protected int[] _paramPlus = new int[8];
        protected List<int> _states = new List<int>();
        protected Dictionary<int, int> _stateTurns = new Dictionary<int, int>();
        protected int[] _buffs = new int[8];
        protected int[] _buffTurns = new int[8];
        
        /// <summary>
        /// Hit Points
        /// </summary>
        public virtual int Hp => _hp;
        
        /// <summary>
        /// Magic Points
        /// </summary>
        public virtual int Mp => _mp;
        
        /// <summary>
        /// Tactical Points
        /// </summary>
        public virtual int Tp => _tp;

        /// <summary>
        /// Maximum Hit Points
        /// </summary>
        public virtual int Mhp => Param(0);
        
        /// <summary>
        /// Maximum Magic Points
        /// </summary>
        public virtual int Mmp => Param(1);
        
        /// <summary>
        /// ATtacK power
        /// </summary>
        public virtual int Atk => Param(2);
        
        /// <summary>
        /// DEFense power 
        /// </summary>
        public virtual int Def => Param(3);
        
        /// <summary>
        /// Magic ATtack power
        /// </summary>
        public virtual int Mat => Param(4);
        
        /// <summary>
        /// Magic DeFense power
        /// </summary>
        public virtual int Mdf => Param(5);
        
        /// <summary>
        /// AGIlity
        /// </summary>
        public virtual int Agi => Param(6);
        
        /// <summary>
        /// LUcK
        /// </summary>
        public virtual int Luk => Param(7);
        
        /// <summary>
        /// HIT rate
        /// </summary>
        public virtual float Hit => Xparam(0);
        
        /// <summary>
        /// EVAsion rate
        /// </summary>
        public virtual float Eva => Xparam(1);
        
        /// <summary>
        /// CRItical rate
        /// </summary>
        public virtual float Cri => Xparam(2);
        
        /// <summary>
        /// Critical Evasion rate
        /// </summary>
        public virtual float Cev => Xparam(3);
        
        /// <summary>
        /// Magic Evasion rate
        /// </summary>
        public virtual float Mev => Xparam(4);
        
        /// <summary>
        /// Magic ReFlection rate
        /// </summary>
        public virtual float Mrf => Xparam(5);
        
        /// <summary>
        /// CouNTer attack rate
        /// </summary>
        public virtual float Cnt => Xparam(6);
        
        /// <summary>
        /// Hp ReGeneration rate
        /// </summary>
        public virtual float Hrg => Xparam(7);
        
        /// <summary>
        /// Mp ReGeneration rate
        /// </summary>
        public virtual float Mrg => Xparam(8);
        
        /// <summary>
        /// Tp ReGeneration rate
        /// </summary>
        public virtual float Trg => Xparam(9);
        
        /// <summary>
        /// Target Rate
        /// </summary>
        public virtual float Tgr => Sparam(0);
        
        /// <summary>
        /// GuaRD effect rate
        /// </summary>
        public virtual float Grd => Sparam(1);
        
        /// <summary>
        /// RECovery effect rate
        /// </summary>
        public virtual float Rec => Sparam(2);
        
        /// <summary>
        /// PHArmacology
        /// </summary>
        public virtual float Pha => Sparam(3);
        
        /// <summary>
        /// Mp Cost Rate
        /// </summary>
        public virtual float Mcr => Sparam(4);
        
        /// <summary>
        /// Tp Charge Rate
        /// </summary>
        public virtual float Tcr => Sparam(5);
        
        /// <summary>
        /// Pysical Damage Rate
        /// </summary>
        public virtual float Pdr => Sparam(6);
        
        /// <summary>
        /// Magic Damage Rate
        /// </summary>
        public virtual float Mdr => Sparam(7);
        
        /// <summary>
        /// Floor Damage Rate
        /// </summary>
        public virtual float Fdr => Sparam(8);
        
        /// <summary>
        /// Experience Rate
        /// </summary>
        public virtual float Exr => Sparam(9);

        protected Game_BattlerBase()
        {
            InitMembers();
        }

        protected virtual void InitMembers()
        {
            _hp = 1;
            _mp = 0;
            _tp = 0;
            _hidden = false;
            ClearParamPlus();
            ClearStates();
            ClearBuffs();
        }

        protected virtual void ClearParamPlus()
        {
            _paramPlus = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        protected virtual void ClearStates()
        {
            _states = new List<int>();
            _stateTurns = new Dictionary<int, int>();
        }

        public virtual void EraseState(int stateId)
        {
            _states.Remove(stateId);
            _stateTurns.Remove(stateId);
        }

        public virtual bool IsStateAffected(int stateId)
        {
            return _states.Contains(stateId);
        }

        public virtual bool IsDeathStateAffected()
        {
            return IsStateAffected(DeathStateId());
        }

        public virtual int DeathStateId()
        {
            return 1;
        }

        public virtual void ResetStateCounts(int stateId)
        {
            var state = Rmmz.dataStates[stateId];
            int variance = 1 + Mathf.Max(state.MaxTurns - state.MinTurns, 0);
            _stateTurns[stateId] = state.MinTurns + RmmzMath.RandomInt(variance);
        }

        public virtual bool IsStateExpired(int stateId)
        {
            return _stateTurns.ContainsKey(stateId) && _stateTurns[stateId] == 0;
        }

        public virtual void UpdateStateTurns()
        {
            foreach (int stateId in _states.ToList())
            {
                if (_stateTurns.ContainsKey(stateId) && _stateTurns[stateId] > 0)
                {
                    _stateTurns[stateId]--;
                }
            }
        }

        protected virtual void ClearBuffs()
        {
            _buffs = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            _buffTurns = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        }

        public virtual void EraseBuff(int paramId)
        {
            _buffs[paramId] = 0;
            _buffTurns[paramId] = 0;
        }

        public virtual int BuffLength()
        {
            return _buffs.Length;
        }

        public virtual int Buff(int paramId)
        {
            return _buffs[paramId];
        }

        public virtual bool IsBuffAffected(int paramId)
        {
            return _buffs[paramId] > 0;
        }

        public virtual bool IsDebuffAffected(int paramId)
        {
            return _buffs[paramId] < 0;
        }

        public virtual bool IsBuffOrDebuffAffected(int paramId)
        {
            return _buffs[paramId] != 0;
        }

        public virtual bool IsMaxBuffAffected(int paramId)
        {
            return _buffs[paramId] == 2;
        }

        public virtual bool IsMaxDebuffAffected(int paramId)
        {
            return _buffs[paramId] == -2;
        }

        public virtual void IncreaseBuff(int paramId)
        {
            if (!IsMaxBuffAffected(paramId))
            {
                _buffs[paramId]++;
            }
        }

        public virtual void DecreaseBuff(int paramId)
        {
            if (!IsMaxDebuffAffected(paramId))
            {
                _buffs[paramId]--;
            }
        }

        public virtual void OverwriteBuffTurns(int paramId, int turns)
        {
            if (_buffTurns[paramId] < turns)
            {
                _buffTurns[paramId] = turns;
            }
        }

        public virtual bool IsBuffExpired(int paramId)
        {
            return _buffTurns[paramId] == 0;
        }

        public virtual void UpdateBuffTurns()
        {
            for (int i = 0; i < _buffTurns.Length; i++)
            {
                if (_buffTurns[i] > 0)
                {
                    _buffTurns[i]--;
                }
            }
        }

        public virtual void Die()
        {
            _hp = 0;
            ClearStates();
            ClearBuffs();
        }

        public virtual void Revive()
        {
            if (_hp == 0)
            {
                _hp = 1;
            }
        }

        public virtual IEnumerable<DataState> States()
        {
            return _states.Select(id => Rmmz.dataStates[id]);
        }

        public virtual IEnumerable<int> StateIcons()
        {
            return States()
                .Select(state => state.IconIndex)
                .Where(iconIndex => iconIndex > 0);
        }

        public virtual IEnumerable<int> BuffIcons()
        {
            for (int i = 0; i < _buffs.Length; i++)
            {
                if (_buffs[i] != 0)
                {
                    yield return BuffIconIndex(_buffs[i], i);
                }
            }
        }

        protected virtual int BuffIconIndex(int buffLevel, int paramId)
        {
            if (buffLevel > 0)
            {
                return ICON_BUFF_START + (buffLevel - 1) * 8 + paramId;
            }
            else if (buffLevel < 0)
            {
                return ICON_DEBUFF_START + (-buffLevel - 1) * 8 + paramId;
            }
            else
            {
                return 0;
            }
        }

        public virtual IEnumerable<int> AllIcons()
        {
            return StateIcons().Concat(BuffIcons());
        }

        protected virtual IEnumerable<ITraitsObject> TraitObjects()
        {
            return States();
        }

        protected virtual IEnumerable<DataTrait> AllTraits()
        {
            return TraitObjects()
                .SelectMany(obj => obj.Traits);
        }

        protected virtual IEnumerable<DataTrait> Traits(int code)
        {
            return AllTraits().Where(trait => trait.Code == code);
        }

        protected virtual IEnumerable<DataTrait> TraitsWithId(int code, int id)
        {
            return AllTraits()
                .Where(trait => trait.Code == code && trait.DataId == id);
        }

        protected virtual float TraitsPi(int code, int id)
        {
            return TraitsWithId(code, id)
                .Aggregate(1.0f, (r, trait) => r * trait.Value);
        }

        protected virtual float TraitsSum(int code, int id)
        {
            return TraitsWithId(code, id)
                .Sum(trait => trait.Value);
        }

        protected virtual float TraitsSumAll(int code)
        {
            return Traits(code).Sum(trait => trait.Value);
        }

        protected virtual IEnumerable<int> TraitsSet(int code)
        {
            return Traits(code)
                .Select(trait => trait.DataId);
        }

        protected virtual int ParamBase(int _)
        {
            return 0;
        }

        protected virtual int ParamPlus(int paramId)
        {
            return _paramPlus[paramId];
        }

        protected virtual int ParamBasePlus(int paramId)
        {
            return Math.Max(0, ParamBase(paramId) + ParamPlus(paramId));
        }

        protected virtual int ParamMin(int paramId)
        {
            if (paramId == 0)
            {
                return 1; // MHP
            }
            else
            {
                return 0;
            }
        }

        protected virtual int ParamMax(int paramId)
        {
            return int.MaxValue;
        }

        protected virtual float ParamRate(int paramId)
        {
            return TraitsPi(TRAIT_PARAM, paramId);
        }

        protected virtual float ParamBuffRate(int paramId)
        {
            return _buffs[paramId] * 0.25f + 1.0f;
        }

        public virtual int Param(int paramId)
        {
            float value = ParamBasePlus(paramId) * ParamRate(paramId) * ParamBuffRate(paramId);
            int maxValue = ParamMax(paramId);
            int minValue = ParamMin(paramId);
            return Mathf.RoundToInt(Mathf.Clamp(value, minValue, maxValue));
        }

        public virtual float Xparam(int xparamId)
        {
            return TraitsSum(TRAIT_XPARAM, xparamId);
        }

        public virtual float Sparam(int sparamId)
        {
            return TraitsPi(TRAIT_SPARAM, sparamId);
        }

        public virtual float ElementRate(int elementId)
        {
            return TraitsPi(TRAIT_ELEMENT_RATE, elementId);
        }

        public virtual float DebuffRate(int paramId)
        {
            return TraitsPi(TRAIT_DEBUFF_RATE, paramId);
        }

        public virtual float StateRate(int stateId)
        {
            return TraitsPi(TRAIT_STATE_RATE, stateId);
        }

        protected virtual IEnumerable<int> StateResistSet()
        {
            return TraitsSet(TRAIT_STATE_RESIST);
        }

        public virtual bool IsStateResist(int stateId)
        {
            return StateResistSet().Contains(stateId);
        }

        public virtual IEnumerable<int> AttackElements()
        {
            return TraitsSet(TRAIT_ATTACK_ELEMENT);
        }

        public virtual IEnumerable<int> AttackStates()
        {
            return TraitsSet(TRAIT_ATTACK_STATE);
        }

        public virtual float AttackStatesRate(int stateId)
        {
            return TraitsSum(TRAIT_ATTACK_STATE, stateId);
        }

        public virtual float AttackSpeed()
        {
            return TraitsSumAll(TRAIT_ATTACK_SPEED);
        }

        public virtual int AttackTimesAdd()
        {
            return Mathf.Max(Mathf.FloorToInt(TraitsSumAll(TRAIT_ATTACK_TIMES)), 0);
        }

        public virtual int AttackSkillId()
        {
            var set = TraitsSet(TRAIT_ATTACK_SKILL);
            return set.Any() ? set.Max() : 1;
        }

        public virtual IEnumerable<int> AddedSkillTypes()
        {
            return TraitsSet(TRAIT_STYPE_ADD);
        }

        public virtual bool IsSkillTypeSealed(int stypeId)
        {
            return TraitsSet(TRAIT_STYPE_SEAL).Contains(stypeId);
        }

        public virtual IEnumerable<int> AddedSkills()
        {
            return TraitsSet(TRAIT_SKILL_ADD);
        }

        public virtual bool IsSkillSealed(int skillId)
        {
            return TraitsSet(TRAIT_SKILL_SEAL).Contains(skillId);
        }

        public virtual bool IsEquipWtypeOk(int wtypeId)
        {
            return TraitsSet(TRAIT_EQUIP_WTYPE).Contains(wtypeId);
        }

        public virtual bool IsEquipAtypeOk(int atypeId)
        {
            return TraitsSet(TRAIT_EQUIP_ATYPE).Contains(atypeId);
        }

        public virtual bool IsEquipTypeLocked(int etypeId)
        {
            return TraitsSet(TRAIT_EQUIP_LOCK).Contains(etypeId);
        }

        public virtual bool IsEquipTypeSealed(int etypeId)
        {
            return TraitsSet(TRAIT_EQUIP_SEAL).Contains(etypeId);
        }

        public virtual int SlotType()
        {
            var set = TraitsSet(TRAIT_SLOT_TYPE);
            return set.Any() ? set.Max() : 0;
        }

        public virtual bool IsDualWield()
        {
            return SlotType() == 1;
        }

        protected virtual IEnumerable<float> ActionPlusSet()
        {
            return Traits(TRAIT_ACTION_PLUS).Select(trait => trait.Value);
        }

        public virtual bool SpecialFlag(int flagId)
        {
            return Traits(TRAIT_SPECIAL_FLAG).Any(trait => trait.DataId == flagId);
        }

        public virtual int CollapseType()
        {
            var set = TraitsSet(TRAIT_COLLAPSE_TYPE);
            return set.Any() ? set.Max() : 0;
        }

        public virtual bool PartyAbility(int abilityId)
        {
            return Traits(TRAIT_PARTY_ABILITY).Any(trait => trait.DataId == abilityId);
        }

        public virtual bool IsAutoBattle()
        {
            return SpecialFlag(FLAG_ID_AUTO_BATTLE);
        }

        public virtual bool IsGuard()
        {
            return SpecialFlag(FLAG_ID_GUARD) && CanMove();
        }

        public virtual bool IsSubstitute()
        {
            return SpecialFlag(FLAG_ID_SUBSTITUTE) && CanMove();
        }

        public virtual bool IsPreserveTp()
        {
            return SpecialFlag(FLAG_ID_PRESERVE_TP);
        }

        public virtual void AddParam(int paramId, int value)
        {
            _paramPlus[paramId] += value;
            Refresh();
        }

        public virtual void SetHp(int hp)
        {
            _hp = hp;
            Refresh();
        }

        public virtual void SetMp(int mp)
        {
            _mp = mp;
            Refresh();
        }

        public virtual void SetTp(int tp)
        {
            _tp = tp;
            Refresh();
        }

        public virtual int MaxTp()
        {
            return 100;
        }

        public virtual void Refresh()
        {
            foreach (int stateId in StateResistSet())
            {
                EraseState(stateId);
            }
            _hp = Math.Clamp(_hp, 0, Mhp);
            _mp = Math.Clamp(_mp, 0, Mmp);
            _tp = Math.Clamp(_tp, 0, MaxTp());
        }

        public virtual void RecoverAll()
        {
            ClearStates();
            _hp = Mhp;
            _mp = Mmp;
        }

        public virtual float HpRate()
        {
            return (float)Hp / Mhp;
        }

        public virtual float MpRate()
        {
            return Mmp > 0 ? (float)Mp / Mmp : 0;
        }

        public virtual float TpRate()
        {
            return (float)Tp / MaxTp();
        }

        public virtual void Hide()
        {
            _hidden = true;
        }

        public virtual void Appear()
        {
            _hidden = false;
        }

        public virtual bool IsHidden()
        {
            return _hidden;
        }

        public virtual bool IsAppeared()
        {
            return !IsHidden();
        }

        public virtual bool IsDead()
        {
            return IsAppeared() && IsDeathStateAffected();
        }

        public virtual bool IsAlive()
        {
            return IsAppeared() && !IsDeathStateAffected();
        }

        public virtual bool IsDying()
        {
            return IsAlive() && _hp < Mhp / 4;
        }

        public virtual bool IsRestricted()
        {
            return IsAppeared() && Restriction() > 0;
        }

        public virtual bool CanInput()
        {
            return IsAppeared() && IsActor() && !IsRestricted() && !IsAutoBattle();
        }

        public virtual bool CanMove()
        {
            return IsAppeared() && Restriction() < 4;
        }

        public virtual bool IsConfused()
        {
            return IsAppeared() && Restriction() >= 1 && Restriction() <= 3;
        }

        public virtual int ConfusionLevel()
        {
            return IsConfused() ? Restriction() : 0;
        }

        public virtual bool IsActor()
        {
            return false;
        }

        public virtual bool IsEnemy()
        {
            return false;
        }

        protected virtual void SortStates()
        {
            _states.Sort((a, b) =>
            {
                int p1 = Rmmz.dataStates[a].Priority;
                int p2 = Rmmz.dataStates[b].Priority;
                if (p1 != p2)
                {
                    return p2 - p1;
                }
                return a - b;
            });
        }

        protected virtual int Restriction()
        {
            var restrictions = States().Select(state => state.Restriction);
            return restrictions.Any() ? restrictions.Max() : 0;
        }

        public virtual void AddNewState(int stateId)
        {
            if (stateId == DeathStateId())
            {
                Die();
            }
            bool restricted = IsRestricted();
            _states.Add(stateId);
            SortStates();
            if (!restricted && IsRestricted())
            {
                OnRestrict();
            }
        }

        protected virtual void OnRestrict()
        {
            // Override in derived classes
        }

        public virtual string MostImportantStateText()
        {
            foreach (DataState state in States())
            {
                if (!string.IsNullOrEmpty(state.Message3))
                {
                    return state.Message3;
                }
            }
            return "";
        }

        public virtual int StateMotionIndex()
        {
            var states = States();
            if (states.Any())
            {
                return states.ElementAt(0).Motion;
            }
            else
            {
                return 0;
            }
        }

        public virtual int StateOverlayIndex()
        {
            var states = States();
            if (states.Any())
            {
                return states.ElementAt(0).Overlay;
            }
            else
            {
                return 0;
            }
        }

        public virtual bool IsSkillWtypeOk(DataSkill _)
        {
            return true;
        }

        public virtual int SkillMpCost(DataSkill dataSkill)
        {
            return Mathf.FloorToInt(dataSkill.MpCost * Mcr);
        }

        public virtual int SkillTpCost(DataSkill dataSkill)
        {
            return dataSkill.TpCost;
        }

        public virtual bool CanPaySkillCost(DataSkill dataSkill)
        {
            return _tp >= SkillTpCost(dataSkill) && _mp >= SkillMpCost(dataSkill);
        }

        public virtual void PaySkillCost(DataSkill dataSkill)
        {
            _mp -= SkillMpCost(dataSkill);
            _tp -= SkillTpCost(dataSkill);
        }

        public virtual bool IsOccasionOk(UsableItem item)
        {
            if (Rmmz.gameParty.InBattle())
            {
                return item.Occasion == 0 || item.Occasion == 1;
            }
            else
            {
                return item.Occasion == 0 || item.Occasion == 2;
            }
        }

        public virtual bool MeetsUsableItemConditions(UsableItem item)
        {
            return CanMove() && IsOccasionOk(item);
        }

        public virtual bool MeetsSkillConditions(DataSkill dataSkill)
        {
            return MeetsUsableItemConditions(dataSkill) &&
                   IsSkillWtypeOk(dataSkill) &&
                   CanPaySkillCost(dataSkill) &&
                   !IsSkillSealed(dataSkill.Id) &&
                   !IsSkillTypeSealed(dataSkill.StypeId);
        }

        public virtual bool MeetsItemConditions(DataItem dataItem)
        {
            return MeetsUsableItemConditions(dataItem) && Rmmz.gameParty.HasItem(dataItem);
        }

        public virtual bool CanUse(DataCommonItem item)
        {
            if (item == null)
            {
                return false;
            }
            else if (Rmmz.DataManager.IsSkill(item))
            {
                return MeetsSkillConditions((DataSkill)item);
            }
            else if (Rmmz.DataManager.IsItem(item))
            {
                return MeetsItemConditions((DataItem)item);
            }
            else
            {
                return false;
            }
        }

        public virtual bool CanEquip(DataCommonItem item)
        {
            if (item == null)
            {
                return false;
            }
            else if (Rmmz.DataManager.IsWeapon(item))
            {
                return CanEquipWeapon((DataWeapon)item);
            }
            else if (Rmmz.DataManager.IsArmor(item))
            {
                return CanEquipArmor((DataArmor)item);
            }
            else
            {
                return false;
            }
        }

        public virtual bool CanEquipWeapon(DataWeapon item)
        {
            return IsEquipWtypeOk(item.WtypeId) && !IsEquipTypeSealed(item.EtypeId);
        }

        public virtual bool CanEquipArmor(DataArmor item)
        {
            return IsEquipAtypeOk(item.AtypeId) && !IsEquipTypeSealed(item.EtypeId);
        }

        public virtual int GuardSkillId()
        {
            return 2;
        }

        public virtual bool CanAttack()
        {
            return CanUse(Rmmz.dataSkills[AttackSkillId()]);
        }

        public virtual bool CanGuard()
        {
            return CanUse(Rmmz.dataSkills[GuardSkillId()]);
        }
        
        #region UniRmmz
        
        public virtual int Level => 0;
        
        #endregion

    }
}
