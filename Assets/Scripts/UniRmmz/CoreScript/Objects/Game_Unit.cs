using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of Game_Party and Game_Troop.
    /// </summary>
    [Serializable]
    public abstract partial class Game_Unit
    {
        protected bool _inBattle;

        protected Game_Unit()
        {
            _inBattle = false;
        }

        public bool InBattle()
        {
            return _inBattle;
        }

        public abstract IEnumerable<Game_Battler> Members();

        public virtual IEnumerable<Game_Battler> AliveMembers()
        {
            return Members().Where(member => member.IsAlive());
        }

        public virtual IEnumerable<Game_Battler> DeadMembers()
        {
            return Members().Where(member => member.IsDead());
        }

        public IEnumerable<Game_Battler> MovableMembers()
        {
            return Members().Where(member => member.CanMove());
        }

        public void ClearActions()
        {
            foreach (var member in Members())
            {
                member.ClearActions();
            }
        }

        public int Agility()
        {
            var members = Members();
            if (!members.Any())
            {
                return 1;
            }

            int sum = members.Sum(member => member.Agi);
            return Math.Max(1, sum / members.Count());
        }

        public float TgrSum()
        {
            return AliveMembers().Sum(member => member.Tgr);
        }

        public Game_Battler RandomTarget()
        {
            var tgrSum = TgrSum();
            if (tgrSum <= 0) return null;

            var tgrRand = UnityEngine.Random.value * tgrSum;
            Game_Battler target = null;

            foreach (var member in AliveMembers())
            {
                tgrRand -= member.Tgr;
                if (tgrRand <= 0 && target == null)
                {
                    target = member;
                    break;
                }
            }

            return target;
        }

        public Game_Battler RandomDeadTarget()
        {
            var deadMembers = DeadMembers().ToList();
            if (!deadMembers.Any())
            {
                return null;
            }

            var randomIndex = RmmzMath.RandomInt(deadMembers.Count);
            return deadMembers[randomIndex];
        }

        public Game_Battler SmoothTarget(int index)
        {
            var members = Members().ToList();
            var safeIndex = Mathf.Max(0, index);

            if (safeIndex < members.Count)
            {
                var member = members[safeIndex];
                if (member != null && member.IsAlive())
                {
                    return member;
                }
            }

            var aliveMembers = AliveMembers().ToList();
            return aliveMembers.Count > 0 ? aliveMembers[0] : null;
        }

        public Game_Battler SmoothDeadTarget(int index)
        {
            var members = Members().ToList();
            var safeIndex = Mathf.Max(0, index);

            if (safeIndex < members.Count)
            {
                var member = members[safeIndex];
                if (member != null && member.IsDead())
                {
                    return member;
                }
            }

            var deadMembers = DeadMembers().ToList();
            return deadMembers.Count > 0 ? deadMembers[0] : null;
        }

        public void ClearResults()
        {
            foreach (var member in Members())
            {
                member.ClearResult();
            }
        }

        public void OnBattleStart(bool advantageous = false)
        {
            foreach (var member in Members())
            {
                member.OnBattleStart(advantageous);
            }

            _inBattle = true;
        }

        public void OnBattleEnd()
        {
            _inBattle = false;
            foreach (var member in Members())
            {
                member.OnBattleEnd();
            }
        }

        public void MakeActions()
        {
            foreach (var member in Members())
            {
                member.MakeActions();
            }
        }

        public void Select(Game_Battler activeMember)
        {
            foreach (var member in Members())
            {
                if (member == activeMember)
                {
                    member.Select();
                }
                else
                {
                    member.Deselect();
                }
            }
        }

        public virtual bool IsAllDead()
        {
            return !AliveMembers().Any();
        }

        public Game_Battler SubstituteBattler()
        {
            foreach (var member in Members())
            {
                if (member.IsSubstitute())
                {
                    return member;
                }
            }

            return null;
        }

        public float TpbBaseSpeed()
        {
            var members = Members();
            if (members.Count() == 0)
            {
                return 0;
            }

            return members.Max(member => member.TpbBaseSpeed());
        }

        public int TpbReferenceTime()
        {
            return Rmmz.BattleManager.IsActiveTpb() ? 240 : 60;
        }

        public void UpdateTpb()
        {
            foreach (var member in Members())
            {
                member.UpdateTpb();
            }
        }

        public Game_Battler FirstAliveMember()
        {
            var aliveMembers = AliveMembers().ToList();
            return aliveMembers.Count > 0 ? aliveMembers[0] : null;
        }

        public Game_Battler LastAliveMember()
        {
            var aliveMembers = AliveMembers().ToList();
            return aliveMembers.Count > 0 ? aliveMembers[aliveMembers.Count - 1] : null;
        }

        public bool HasAliveMember()
        {
            return AliveMembers().Any();
        }

        public bool HasDeadMember()
        {
            return DeadMembers().Any();
        }

        public int MemberCount()
        {
            return Members().Count();
        }

        public int AliveCount()
        {
            return AliveMembers().Count();
        }

        public int DeadCount()
        {
            return DeadMembers().Count();
        }

        public bool ContainsMember(Game_Battler member)
        {
            return Members().Contains(member);
        }

        public Game_Battler GetMember(int index)
        {
            var members = Members();
            return members.ElementAtOrDefault(index);
        }
        
        public int FindMemberIndex(Game_Battler member)
        {
            return Members().ToList().IndexOf(member);
        }

        public virtual bool CanInput()
        {
            return Members().Any(member => member.CanInput());
        }

        public bool CanMove()
        {
            return MovableMembers().Any();
        }
    }
}