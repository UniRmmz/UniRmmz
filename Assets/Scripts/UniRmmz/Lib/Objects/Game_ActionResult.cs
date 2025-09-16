using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for a result of a battle action. For convinience, all
    /// member variables in this class are public.
    /// </summary>
    [Serializable]
    public partial class Game_ActionResult
    {
        public bool used;
        public bool missed;
        public bool evaded;
        public bool physical;
        public bool drain;
        public bool critical;
        public bool success;
        public bool hpAffected;
        public int hpDamage;
        public int mpDamage;
        public int tpDamage;
        public List<int> addedStates;
        public List<int> removedStates;
        public List<int> addedBuffs;
        public List<int> addedDebuffs;
        public List<int> removedBuffs;

        protected Game_ActionResult()
        {
            Clear();
        }

        public virtual void Clear()
        {
            used = false;
            missed = false;
            evaded = false;
            physical = false;
            drain = false;
            critical = false;
            success = false;
            hpAffected = false;
            hpDamage = 0;
            mpDamage = 0;
            tpDamage = 0;
            addedStates = new List<int>();
            removedStates = new List<int>();
            addedBuffs = new List<int>();
            addedDebuffs = new List<int>();
            removedBuffs = new List<int>();
        }

        public virtual IEnumerable<DataState> AddedStateObjects()
        {
            return addedStates.Select(id => Rmmz.dataStates[id]);
        }

        public virtual IEnumerable<DataState> RemovedStateObjects()
        {
            return removedStates.Select(id => Rmmz.dataStates[id]);
        }

        public virtual bool IsStatusAffected()
        {
            return addedStates.Count > 0 ||
                   removedStates.Count > 0 ||
                   addedBuffs.Count > 0 ||
                   addedDebuffs.Count > 0 ||
                   removedBuffs.Count > 0;
        }

        public virtual bool IsHit()
        {
            return used && !missed && !evaded;
        }

        public virtual bool IsStateAdded(int stateId)
        {
            return addedStates.Contains(stateId);
        }

        public virtual void PushAddedState(int stateId)
        {
            if (!IsStateAdded(stateId))
            {
                addedStates.Add(stateId);
            }
        }

        public virtual bool IsStateRemoved(int stateId)
        {
            return removedStates.Contains(stateId);
        }

        public virtual void PushRemovedState(int stateId)
        {
            if (!IsStateRemoved(stateId))
            {
                removedStates.Add(stateId);
            }
        }

        public virtual bool IsBuffAdded(int paramId)
        {
            return addedBuffs.Contains(paramId);
        }

        public virtual void PushAddedBuff(int paramId)
        {
            if (!IsBuffAdded(paramId))
            {
                addedBuffs.Add(paramId);
            }
        }

        public virtual bool IsDebuffAdded(int paramId)
        {
            return addedDebuffs.Contains(paramId);
        }

        public virtual void PushAddedDebuff(int paramId)
        {
            if (!IsDebuffAdded(paramId))
            {
                addedDebuffs.Add(paramId);
            }
        }

        public virtual bool IsBuffRemoved(int paramId)
        {
            return removedBuffs.Contains(paramId);
        }

        public virtual void PushRemovedBuff(int paramId)
        {
            if (!IsBuffRemoved(paramId))
            {
                removedBuffs.Add(paramId);
            }
        }
    }
}