using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of Game_Actor and Game_Enemy. It contains methods for sprites
    /// and actions.
    /// </summary>
    [Serializable]
    public abstract partial class Game_Battler : Game_BattlerBase
    {
        protected List<Game_Action> _actions;
        protected int _speed;
        protected Game_ActionResult _result;
        protected string _actionState;
        protected int _lastTargetIndex;
        protected bool _damagePopup;
        protected string _effectType;
        protected string _motionType;
        protected int _weaponImageId;
        protected bool _motionRefresh;
        protected bool _selected;
        protected string _tpbState;
        protected float _tpbChargeTime;
        protected float _tpbCastTime;
        protected float _tpbIdleTime;
        protected int _tpbTurnCount;
        protected bool _tpbTurnEnd;

        protected override void InitMembers()
        {
            base.InitMembers();
            _actions = new List<Game_Action>();
            _speed = 0;
            _result = Game_ActionResult.Create();
            _actionState = "";
            _lastTargetIndex = 0;
            _damagePopup = false;
            _effectType = null;
            _motionType = null;
            _weaponImageId = 0;
            _motionRefresh = false;
            _selected = false;
            _tpbState = "";
            _tpbChargeTime = 0;
            _tpbCastTime = 0;
            _tpbIdleTime = 0;
            _tpbTurnCount = 0;
            _tpbTurnEnd = false;
        }

        public virtual void ClearDamagePopup()
        {
            _damagePopup = false;
        }

        public virtual void ClearWeaponAnimation()
        {
            _weaponImageId = 0;
        }

        public virtual void ClearEffect()
        {
            _effectType = null;
        }

        public virtual void ClearMotion()
        {
            _motionType = null;
            _motionRefresh = false;
        }

        public virtual void RequestEffect(string effectType)
        {
            _effectType = effectType;
        }

        public virtual void RequestMotion(string motionType)
        {
            _motionType = motionType;
        }

        public virtual void RequestMotionRefresh()
        {
            _motionRefresh = true;
        }

        public virtual void CancelMotionRefresh()
        {
            _motionRefresh = false;
        }

        public virtual void Select()
        {
            _selected = true;
        }

        public virtual void Deselect()
        {
            _selected = false;
        }

        public virtual bool IsDamagePopupRequested()
        {
            return _damagePopup;
        }

        public virtual bool IsEffectRequested()
        {
            return !string.IsNullOrEmpty(_effectType);
        }

        public virtual bool IsMotionRequested()
        {
            return !string.IsNullOrEmpty(_motionType);
        }

        public virtual bool IsWeaponAnimationRequested()
        {
            return _weaponImageId > 0;
        }

        public virtual bool IsMotionRefreshRequested()
        {
            return _motionRefresh;
        }

        public virtual bool IsSelected()
        {
            return _selected;
        }

        public virtual string EffectType()
        {
            return _effectType;
        }

        public virtual string MotionType()
        {
            return _motionType;
        }

        public virtual int WeaponImageId()
        {
            return _weaponImageId;
        }

        public virtual void StartDamagePopup()
        {
            _damagePopup = true;
        }

        public virtual bool ShouldPopupDamage()
        {
            return _result.missed || _result.evaded || _result.hpAffected || _result.mpDamage != 0;
        }

        public virtual void StartWeaponAnimation(int weaponImageId)
        {
            _weaponImageId = weaponImageId;
        }

        public virtual Game_Action Action(int index)
        {
            return index < _actions.Count ? _actions[index] : null;
        }

        public virtual void SetAction(int index, Game_Action action)
        {
            while (_actions.Count <= index)
            {
                _actions.Add(null);
            }
            _actions[index] = action;
        }

        public virtual int NumActions()
        {
            return _actions.Count;
        }

        public virtual void ClearActions()
        {
            _actions.Clear();
        }

        public virtual Game_ActionResult Result()
        {
            return _result;
        }

        public virtual void ClearResult()
        {
            _result.Clear();
        }

        public virtual void ClearTpbChargeTime()
        {
            _tpbState = "charging";
            _tpbChargeTime = 0;
        }

        public virtual void ApplyTpbPenalty()
        {
            _tpbState = "charging";
            _tpbChargeTime -= 1;
        }

        public virtual void InitTpbChargeTime(bool advantageous)
        {
            float speed = TpbRelativeSpeed();
            _tpbState = "charging";
            _tpbChargeTime = advantageous ? 1 : speed * UnityEngine.Random.value * 0.5f;
            if (IsRestricted())
            {
                _tpbChargeTime = 0;
            }
        }

        public virtual float TpbChargeTime()
        {
            return _tpbChargeTime;
        }

        public virtual void StartTpbCasting()
        {
            _tpbState = "casting";
            _tpbCastTime = 0;
        }

        public virtual void StartTpbAction()
        {
            _tpbState = "acting";
        }

        public virtual bool IsTpbCharged()
        {
            return _tpbState == "charged";
        }

        public virtual bool IsTpbReady()
        {
            return _tpbState == "ready";
        }

        public virtual bool IsTpbTimeout()
        {
            return _tpbIdleTime >= 1;
        }

        public virtual void UpdateTpb()
        {
            if (CanMove())
            {
                UpdateTpbChargeTime();
                UpdateTpbCastTime();
                UpdateTpbAutoBattle();
            }
            if (IsAlive())
            {
                UpdateTpbIdleTime();
            }
        }

        protected virtual void UpdateTpbChargeTime()
        {
            if (_tpbState == "charging")
            {
                _tpbChargeTime += TpbAcceleration();
                if (_tpbChargeTime >= 1)
                {
                    _tpbChargeTime = 1;
                    OnTpbCharged();
                }
            }
        }

        protected virtual void UpdateTpbCastTime()
        {
            if (_tpbState == "casting")
            {
                _tpbCastTime += TpbAcceleration();
                if (_tpbCastTime >= TpbRequiredCastTime())
                {
                    _tpbCastTime = TpbRequiredCastTime();
                    _tpbState = "ready";
                }
            }
        }

        protected virtual void UpdateTpbAutoBattle()
        {
            if (IsTpbCharged() && !IsTpbTurnEnd() && IsAutoBattle())
            {
                MakeTpbActions();
            }
        }

        protected virtual void UpdateTpbIdleTime()
        {
            if (!CanMove() || IsTpbCharged())
            {
                _tpbIdleTime += TpbAcceleration();
            }
        }

        protected virtual float TpbAcceleration()
        {
            float speed = TpbRelativeSpeed();
            float referenceTime = Rmmz.gameParty.TpbReferenceTime();
            return speed / referenceTime;
        }

        protected virtual float TpbRelativeSpeed()
        {
            return TpbSpeed() / Rmmz.gameParty.TpbBaseSpeed();
        }

        protected virtual float TpbSpeed()
        {
            return Mathf.Sqrt(Agi) + 1;
        }

        public virtual float TpbBaseSpeed()
        {
            int baseAgility = ParamBasePlus(6);
            return Mathf.Sqrt(baseAgility) + 1;
        }

        protected virtual float TpbRequiredCastTime()
        {
            var validActions = _actions.Where(action => action.IsValid());
            var items = validActions.Select(action => action.Item());
            float delay = items.Sum(item => Mathf.Max(0, -item.Speed));
            return Mathf.Sqrt(delay) / TpbSpeed();
        }

        protected virtual void OnTpbCharged()
        {
            if (!ShouldDelayTpbCharge())
            {
                FinishTpbCharge();
            }
        }

        protected virtual bool ShouldDelayTpbCharge()
        {
            return !Rmmz.BattleManager.IsActiveTpb() && Rmmz.gameParty.CanInput();
        }

        protected virtual void FinishTpbCharge()
        {
            _tpbState = "charged";
            _tpbTurnEnd = true;
            _tpbIdleTime = 0;
        }

        public virtual bool IsTpbTurnEnd()
        {
            return _tpbTurnEnd;
        }

        public virtual void InitTpbTurn()
        {
            _tpbTurnEnd = false;
            _tpbTurnCount = 0;
            _tpbIdleTime = 0;
        }

        public virtual void StartTpbTurn()
        {
            _tpbTurnEnd = false;
            _tpbTurnCount++;
            _tpbIdleTime = 0;
            if (NumActions() == 0)
            {
                MakeTpbActions();
            }
        }

        protected virtual void MakeTpbActions()
        {
            MakeActions();
            if (CanInput())
            {
                SetActionState("undecided");
            }
            else
            {
                StartTpbCasting();
                SetActionState("waiting");
            }
        }

        public virtual void OnTpbTimeout()
        {
            OnAllActionsEnd();
            _tpbTurnEnd = true;
            _tpbIdleTime = 0;
        }

        public virtual int TurnCount()
        {
            if (Rmmz.BattleManager.IsTpb())
            {
                return _tpbTurnCount;
            }
            else
            {
                return Rmmz.gameTroop.TurnCount() + 1;
            }
        }

        public override bool CanInput()
        {
            if (Rmmz.BattleManager.IsTpb() && !IsTpbCharged())
            {
                return false;
            }
            return base.CanInput();
        }

        public override void Refresh()
        {
            base.Refresh();
            if (Hp == 0)
            {
                AddState(DeathStateId());
            }
            else
            {
                RemoveState(DeathStateId());
            }
        }

        public virtual void AddState(int stateId)
        {
            if (IsStateAddable(stateId))
            {
                if (!IsStateAffected(stateId))
                {
                    AddNewState(stateId);
                    Refresh();
                }
                ResetStateCounts(stateId);
                _result.PushAddedState(stateId);
            }
        }

        protected virtual bool IsStateAddable(int stateId)
        {
            return IsAlive() &&
                   Rmmz.dataStates[stateId] != null &&
                   !IsStateResist(stateId) &&
                   !IsStateRestrict(stateId);
        }

        protected virtual bool IsStateRestrict(int stateId)
        {
            return Rmmz.dataStates[stateId].RemoveByRestriction && IsRestricted();
        }

        protected override void OnRestrict()
        {
            base.OnRestrict();
            ClearTpbChargeTime();
            ClearActions();
            foreach (var state in States().ToList())
            {
                if (state.RemoveByRestriction)
                {
                    RemoveState(state.Id);
                }
            }
        }

        public virtual void RemoveState(int stateId)
        {
            if (IsStateAffected(stateId))
            {
                if (stateId == DeathStateId())
                {
                    Revive();
                }
                EraseState(stateId);
                Refresh();
                _result.PushRemovedState(stateId);
            }
        }

        public virtual void Escape()
        {
            if (Rmmz.gameParty.InBattle())
            {
                Hide();
            }
            ClearActions();
            ClearStates();
            Rmmz.SoundManager.PlayEscape();
        }

        public virtual void AddBuff(int paramId, int turns)
        {
            if (IsAlive())
            {
                IncreaseBuff(paramId);
                if (IsBuffAffected(paramId))
                {
                    OverwriteBuffTurns(paramId, turns);
                }
                _result.PushAddedBuff(paramId);
                Refresh();
            }
        }

        public virtual void AddDebuff(int paramId, int turns)
        {
            if (IsAlive())
            {
                DecreaseBuff(paramId);
                if (IsDebuffAffected(paramId))
                {
                    OverwriteBuffTurns(paramId, turns);
                }
                _result.PushAddedDebuff(paramId);
                Refresh();
            }
        }

        public virtual void RemoveBuff(int paramId)
        {
            if (IsAlive() && IsBuffOrDebuffAffected(paramId))
            {
                EraseBuff(paramId);
                _result.PushRemovedBuff(paramId);
                Refresh();
            }
        }

        public virtual void RemoveBattleStates()
        {
            foreach (var state in States().ToList())
            {
                if (state.RemoveAtBattleEnd)
                {
                    RemoveState(state.Id);
                }
            }
        }

        public virtual void RemoveAllBuffs()
        {
            for (int i = 0; i < BuffLength(); i++)
            {
                RemoveBuff(i);
            }
        }

        public virtual void RemoveStatesAuto(int timing)
        {
            foreach (var state in States().ToList())
            {
                if (IsStateExpired(state.Id) && state.AutoRemovalTiming == timing)
                {
                    RemoveState(state.Id);
                }
            }
        }

        public virtual void RemoveBuffsAuto()
        {
            for (int i = 0; i < BuffLength(); i++)
            {
                if (IsBuffExpired(i))
                {
                    RemoveBuff(i);
                }
            }
        }

        public virtual void RemoveStatesByDamage()
        {
            foreach (var state in States().ToList())
            {
                if (state.RemoveByDamage && UnityEngine.Random.Range(0, 100) < state.ChanceByDamage)
                {
                    RemoveState(state.Id);
                }
            }
        }

        protected virtual int MakeActionTimes()
        {
            var actionPlusSet = ActionPlusSet();
            return actionPlusSet.Aggregate(1, (r, p) => UnityEngine.Random.value < p ? r + 1 : r);
        }

        public virtual void MakeActions()
        {
            ClearActions();
            if (CanMove())
            {
                int actionTimes = MakeActionTimes();
                _actions = new List<Game_Action>();
                for (int i = 0; i < actionTimes; i++)
                {
                    _actions.Add(Game_Action.Create(this));
                }
            }
        }

        public virtual int Speed()
        {
            return _speed;
        }

        public virtual void MakeSpeed()
        {
            if (_actions.Count > 0)
            {
                _speed = _actions.Min(action => action.Speed());
            }
            else
            {
                _speed = 0;
            }
        }

        public virtual Game_Action CurrentAction()
        {
            return _actions.Count > 0 ? _actions[0] : null;
        }

        public virtual void RemoveCurrentAction()
        {
            if (_actions.Count > 0)
            {
                _actions.RemoveAt(0);
            }
        }

        public virtual void SetLastTarget(Game_Battler target)
        {
            _lastTargetIndex = target != null ? target.Index() : 0;
        }

        public virtual void ForceAction(int skillId, int targetIndex)
        {
            ClearActions();
            var action = Game_Action.Create(this, true);
            action.SetSkill(skillId);
            if (targetIndex == -2)
            {
                action.SetTarget(_lastTargetIndex);
            }
            else if (targetIndex == -1)
            {
                action.DecideRandomTarget();
            }
            else
            {
                action.SetTarget(targetIndex);
            }
            if (action.Item() != null)
            {
                _actions.Add(action);
            }
        }

        public virtual void UseItem(UsableItem item)
        {
            if (Rmmz.DataManager.IsSkill(item))
            {
                PaySkillCost((DataSkill)item);
            }
            else if (Rmmz.DataManager.IsItem(item))
            {
                ConsumeItem(item as DataItem);
            }
        }

        protected virtual void ConsumeItem(DataItem dataItem)
        {
            Rmmz.gameParty.ConsumeItem(dataItem);
        }

        public virtual void GainHp(int value)
        {
            _result.hpDamage = -value;
            _result.hpAffected = true;
            SetHp(Hp + value);
        }

        public virtual void GainMp(int value)
        {
            _result.mpDamage = -value;
            SetMp(Mp + value);
        }

        public virtual void GainTp(int value)
        {
            _result.tpDamage = -value;
            SetTp(Tp + value);
        }

        public virtual void GainSilentTp(int value)
        {
            SetTp(Tp + value);
        }

        public virtual void InitTp()
        {
            SetTp(UnityEngine.Random.Range(0, 25));
        }

        public virtual void ClearTp()
        {
            SetTp(0);
        }

        public virtual void ChargeTpByDamage(float damageRate)
        {
            int value = Mathf.FloorToInt(50 * damageRate * Tcr);
            GainSilentTp(value);
        }

        public virtual void RegenerateHp()
        {
            int minRecover = -MaxSlipDamage();
            int value = Mathf.Max(Mathf.FloorToInt(Mhp * Hrg), minRecover);
            if (value != 0)
            {
                GainHp(value);
            }
        }

        protected virtual int MaxSlipDamage()
        {
            return Rmmz.DataSystem.OptSlipDeath ? Hp : Mathf.Max(Hp - 1, 0);
        }

        public virtual void RegenerateMp()
        {
            int value = Mathf.FloorToInt(Mmp * Mrg);
            if (value != 0)
            {
                GainMp(value);
            }
        }

        public virtual void RegenerateTp()
        {
            int value = Mathf.FloorToInt(100 * Trg);
            GainSilentTp(value);
        }

        public virtual void RegenerateAll()
        {
            if (IsAlive())
            {
                RegenerateHp();
                RegenerateMp();
                RegenerateTp();
            }
        }

        public virtual void OnBattleStart(bool advantageous = false)
        {
            SetActionState("undecided");
            ClearMotion();
            InitTpbChargeTime(advantageous);
            InitTpbTurn();
            if (!IsPreserveTp())
            {
                InitTp();
            }
        }

        public virtual void OnAllActionsEnd()
        {
            ClearResult();
            RemoveStatesAuto(1);
            RemoveBuffsAuto();
        }

        public virtual void OnTurnEnd()
        {
            ClearResult();
            RegenerateAll();
            UpdateStateTurns();
            UpdateBuffTurns();
            RemoveStatesAuto(2);
        }

        public virtual void OnBattleEnd()
        {
            ClearResult();
            RemoveBattleStates();
            RemoveAllBuffs();
            ClearActions();
            if (!IsPreserveTp())
            {
                ClearTp();
            }
            Appear();
        }

        public virtual void OnDamage(int value)
        {
            RemoveStatesByDamage();
            ChargeTpByDamage((float)value / Mhp);
        }

        public virtual void SetActionState(string actionState)
        {
            _actionState = actionState;
            RequestMotionRefresh();
        }

        public virtual bool IsUndecided()
        {
            return _actionState == "undecided";
        }

        public virtual bool IsInputting()
        {
            return _actionState == "inputting";
        }

        public virtual bool IsWaiting()
        {
            return _actionState == "waiting";
        }

        public virtual bool IsActing()
        {
            return _actionState == "acting";
        }

        public virtual bool IsChanting()
        {
            if (IsWaiting())
            {
                return _actions.Any(action => action.IsMagicSkill());
            }
            return false;
        }

        public virtual bool IsGuardWaiting()
        {
            if (IsWaiting())
            {
                return _actions.Any(action => action.IsGuard());
            }
            return false;
        }

        public virtual void PerformActionStart(Game_Action action)
        {
            if (!action.IsGuard())
            {
                SetActionState("acting");
            }
        }

        public virtual void PerformAction(Game_Action action)
        {
        }

        public virtual void PerformActionEnd()
        {
        }

        public virtual void PerformDamage()
        {
        }

        public virtual void PerformMiss()
        {
            Rmmz.SoundManager.PlayMiss();
        }

        public virtual void PerformRecovery()
        {
            Rmmz.SoundManager.PlayRecovery();
        }

        public virtual void PerformEvasion()
        {
            Rmmz.SoundManager.PlayEvasion();
        }

        public virtual void PerformMagicEvasion()
        {
            Rmmz.SoundManager.PlayMagicEvasion();
        }

        public virtual void PerformCounter()
        {
            Rmmz.SoundManager.PlayEvasion();
        }

        public virtual void PerformReflection()
        {
            Rmmz.SoundManager.PlayReflection();
        }

        public virtual void PerformSubstitute(Game_Battler target)
        {
        }

        public virtual void PerformCollapse()
        {
        }

        public virtual int Index()
        {
            return 0;
        }

        public abstract bool IsBattleMember();
        public abstract string Name();
        public abstract Game_Unit FriendsUnit();
        public abstract Game_Unit OpponentsUnit();
        public abstract bool IsSpriteVisible();
        
        
    }
}