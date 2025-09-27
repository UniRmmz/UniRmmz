using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for the party. Information such as gold and items is
    /// included.
    /// </summary>
    [Serializable]
    public partial class Game_Party : Game_Unit
    {
        public const int ABILITY_ENCOUNTER_HALF = 0;
        public const int ABILITY_ENCOUNTER_NONE = 1;
        public const int ABILITY_CANCEL_SURPRISE = 2;
        public const int ABILITY_RAISE_PREEMPTIVE = 3;
        public const int ABILITY_GOLD_DOUBLE = 4;
        public const int ABILITY_DROP_ITEM_DOUBLE = 5;

        protected int _gold;
        protected int _steps;
        protected Game_Item _lastItem;
        protected int _menuActorId;
        protected int _targetActorId;
        protected List<int> _actors;
        
        // Inventory containers
        protected Dictionary<int, int> _items;
        protected Dictionary<int, int> _weapons;
        protected Dictionary<int, int> _armors;

        protected Game_Party()
        {
            _gold = 0;
            _steps = 0;
            _lastItem = Game_Item.Create();
            _menuActorId = 0;
            _targetActorId = 0;
            _actors = new List<int>();
            InitAllItems();
        }

        public void InitAllItems()
        {
            _items = new Dictionary<int, int>();
            _weapons = new Dictionary<int, int>();
            _armors = new Dictionary<int, int>();
        }

        public bool Exists()
        {
            return _actors.Count > 0;
        }

        public int Size()
        {
            return Members().Count();
        }

        public bool IsEmpty()
        {
            return Size() == 0;
        }

        public override IEnumerable<Game_Battler> Members()
        {
            if (InBattle())
            {
                foreach (var member in BattleMembers())
                {
                    yield return member;
                }    
            }
            else
            {
                foreach (var member in AllMembers())
                {
                    yield return member;
                }
            }
        }
        
        public IEnumerable<Game_Actor> AllMembers()
        {
            return _actors.Select(id => Rmmz.gameActors.Actor(id)).Where(actor => actor != null);
        }

        public IEnumerable<Game_Actor> BattleMembers()
        {
            return AllBattleMembers().Where(actor => actor.IsAppeared());
        }

        public IEnumerable<Game_Actor> HiddenBattleMembers()
        {
            return AllBattleMembers().Where(actor => actor.IsHidden());
        }

        public IEnumerable<Game_Actor> AllBattleMembers()
        {
            var members = AllMembers();
            return members.Take(MaxBattleMembers());
        }

        public virtual int MaxBattleMembers()
        {
            return 4;
        }

        public Game_Actor Leader()
        {
            return BattleMembers().ElementAtOrDefault(0);
        }

        public void RemoveInvalidMembers()
        {
            _actors.RemoveAll(actorId => Rmmz.dataActors.ElementAtOrDefault(actorId) == null);
        }

        public void ReviveBattleMembers()
        {
            foreach (var actor in BattleMembers())
            {
                if (actor.IsDead())
                {
                    actor.SetHp(1);
                }
            }
        }

        public List<DataItem> Items()
        {
            return _items.Keys.Select(id => Rmmz.dataItems[id]).Where(item => item != null).ToList();
        }

        public List<DataWeapon> Weapons()
        {
            return _weapons.Keys.Select(id => Rmmz.dataWeapons[id]).Where(weapon => weapon != null).ToList();
        }

        public List<DataArmor> Armors()
        {
            return _armors.Keys.Select(id => Rmmz.dataArmors[id]).Where(armor => armor != null).ToList();
        }

        public List<EquipableItem> EquipItems()
        {
            var items = new List<EquipableItem>();
            items.AddRange(Weapons());
            items.AddRange(Armors());
            return items;
        }

        public virtual IEnumerable<DataCommonItem> AllItems()
        {
            foreach (var item in Items())
            {
                yield return item;
            }
            
            foreach (var item in EquipItems())
            {
                yield return item;
            }
        }

        public Dictionary<int, int> ItemContainer(DataCommonItem item)
        {
            if (item == null)
            {
                return null;
            }
            else if (Rmmz.DataManager.IsItem(item))
            {
                return _items;
            }
            else if (Rmmz.DataManager.IsWeapon(item))
            {
                return _weapons;
            }
            else if (Rmmz.DataManager.IsArmor(item))
            {
                return _armors;
            }
            else
            {
                return null;    
            }
        }
        
        public void SetupStartingMembers()
        {
            _actors.Clear();
            foreach (var actorId in Rmmz.dataSystem.PartyMembers)
            {
                if (Rmmz.gameActors.Actor(actorId) != null)
                {
                    _actors.Add(actorId);
                }
            }
        }
        
        public string Name()
        {
            var numBattleMembers = BattleMembers().Count();
            if (numBattleMembers == 0)
            {
                return "";
            }
            else if (numBattleMembers == 1)
            {
                return Leader().Name();
            }
            else
            {
                return Rmmz.TextManager.PartyName.RmmzFormat(Leader().Name());
            }
        }
        
        public int HighestLevel()
        {
            var levels = Members().Cast<Game_Actor>().Select(actor => actor.Level);
            return levels.Any() ? levels.Max() : 1;
        }
        
        public void AddActor(int actorId)
        {
            if (!_actors.Contains(actorId))
            {
                _actors.Add(actorId);
                
                Rmmz.gamePlayer.Refresh();
                Rmmz.gameMap.RequestRefresh();
                Rmmz.gameTemp.RequestBattleRefresh();

                if (InBattle())
                {
                    var actor = Rmmz.gameActors.Actor(actorId);
                    if (BattleMembers().Contains(actor))
                    {
                        actor.OnBattleStart();
                    }
                }
            }
        }

        public void RemoveActor(int actorId)
        {
            if (_actors.Contains(actorId))
            {
                var actor = Rmmz.gameActors.Actor(actorId);
                var wasBattleMember = BattleMembers().Contains(actor);
                
                _actors.Remove(actorId);
                
                Rmmz.gamePlayer.Refresh();
                Rmmz.gameMap.RequestRefresh();
                Rmmz.gameTemp.RequestBattleRefresh();

                if (InBattle() && wasBattleMember)
                {
                    actor.OnBattleEnd();
                }
            }
        }

        public virtual int Gold()
        {
            return _gold;
        }

        public virtual void GainGold(int amount)
        {
            _gold = Mathf.Clamp(_gold + amount, 0, MaxGold());
        }

        public virtual void LoseGold(int amount)
        {
            GainGold(-amount);
        }

        public virtual int MaxGold()
        {
            return 99999999;
        }

        public virtual int Steps()
        {
            return _steps;
        }

        public virtual void IncreaseSteps()
        {
            _steps++;
        }

        public virtual int NumItems<T>(T item) where T : DataCommonItem
        {
            var container = ItemContainer(item);
            if (container == null) return 0;
            
            return container.GetValueOrDefault(item.Id);
        }

        public virtual int MaxItems<T>(T item) where T : DataCommonItem
        {
            return 99;
        }

        public virtual bool HasMaxItems<T>(T item) where T : DataCommonItem
        {
            return NumItems(item) >= MaxItems(item);
        }

        public virtual bool HasItem<T>(T item, bool includeEquip = false) where T : DataCommonItem
        {
            if (NumItems(item) > 0)
            {
                return true;
            }
            else if (includeEquip && IsAnyMemberEquipped(item))
            {
                return true;
            }
            else
            {
                return false;    
            }
        }

        public virtual bool IsAnyMemberEquipped<T>(T item) where T : DataCommonItem
        {
            //return Members().Any(actor => actor.Equips().Contains(item));
            return false;
        }

        public virtual void GainItem(DataCommonItem item, int amount, bool includeEquip = false) 
        {
            var container = ItemContainer(item);
            if (container != null)
            {
                var lastNumber = NumItems(item);
                var newNumber = lastNumber + amount;
            
                container[item.Id] = Mathf.Clamp(newNumber, 0, MaxItems(item));

                if (container[item.Id] == 0)
                {
                    container.Remove(item.Id);
                }
                if (includeEquip && newNumber < 0)
                {
                    DiscardMembersEquip(item as EquipableItem, -newNumber);
                }    
                Rmmz.gameMap.RequestRefresh();
            }
        }

        public virtual void DiscardMembersEquip<T>(T item, int amount) where T : EquipableItem
        {
            int n = amount;
            foreach (Game_Actor actor in Members())
            {
                while (n > 0 && actor.IsEquipped(item))
                {
                    actor.DiscardEquip(item);
                    n--;
                }
            }
        }

        public virtual void LoseItem<T>(T item, int amount, bool includeEquip = false) where T : DataCommonItem
        {
            GainItem(item, -amount, includeEquip);
        }

        public virtual void ConsumeItem(DataItem dataItem)
        {
            if (Rmmz.DataManager.IsItem(dataItem) && dataItem.Consumable)
            {
                LoseItem(dataItem, 1);
            }
        }
        
        public bool CanUse(DataCommonItem item)
        {
            return Members().Any(actor => actor.CanUse(item));
        }

        public override bool IsAllDead()
        {
            if (base.IsAllDead())
            {
                return InBattle() || !IsEmpty();
            }
            return false;
        }

        public virtual bool IsEscaped()
        {
            return IsAllDead() && HiddenBattleMembers().Any();
        }

        public virtual void OnPlayerWalk()
        {
            foreach (Game_Actor actor in Members())
            {
                actor.OnPlayerWalk();
            }
        }

        public Game_Actor MenuActor()
        {
            var actor = Rmmz.gameActors.Actor(_menuActorId);
            if (!Members().Contains(actor))
            {
                actor = Members().FirstOrDefault() as Game_Actor;
            }
            return actor;
        }

        public void SetMenuActor(Game_Actor actor)
        {
            _menuActorId = actor.ActorId();
        }

        public void MakeMenuActorNext()
        {
            var members = Members().ToList();
            var index = members.IndexOf(MenuActor());
            if (index >= 0)
            {
                index = (index + 1) % members.Count;
                SetMenuActor(members[index] as Game_Actor);
            }
            else if (members.Count > 0)
            {
                SetMenuActor(members[0] as Game_Actor);
            }
        }

        public void MakeMenuActorPrevious()
        {
            var members = Members().ToList();
            var index = members.IndexOf(MenuActor());
            if (index >= 0)
            {
                index = (index + members.Count - 1) % members.Count;
                SetMenuActor(members[index] as Game_Actor);
            }
            else if (members.Count > 0)
            {
                SetMenuActor(members[0] as Game_Actor);
            }
        }

        public Game_Actor TargetActor()
        {
            var actor = Rmmz.gameActors.Actor(_targetActorId);
            if (!Members().Contains(actor))
            {
                actor = Members().FirstOrDefault() as Game_Actor;
            }
            return actor;
        }

        public void SetTargetActor(Game_Actor actor)
        {
            _targetActorId = actor.ActorId();
        }

        public DataItem LastItem()
        {
            return _lastItem.Object<DataItem>();
        }

        public void SetLastItem(DataItem dataItem)
        {
            _lastItem.SetObject(dataItem);
        }
        
        public virtual void SwapOrder(int index1, int index2)
        {
            if (index1 >= 0 && index1 < _actors.Count && index2 >= 0 && index2 < _actors.Count)
            {
                var temp = _actors[index1];
                _actors[index1] = _actors[index2];
                _actors[index2] = temp;
                Rmmz.gamePlayer.Refresh();
            }
        }
        
        public virtual IEnumerable<string[]> CharactersForSavefile()
        {
            return BattleMembers().Select(actor => new string[]
            {
                actor.CharacterName(),
                actor.CharacterIndex().ToString()
            });
        }

        public virtual IEnumerable<string[]> FacesForSavefile()
        {
            return BattleMembers().Select(actor => new string[]
            {
                actor.FaceName(),
                actor.FaceIndex().ToString()
            });
        }

        public virtual bool PartyAbility(int abilityId)
        {
            return BattleMembers().Any(actor => actor.PartyAbility(abilityId));
        }

        public virtual bool HasEncounterHalf()
        {
            return PartyAbility(ABILITY_ENCOUNTER_HALF);
        }

        public virtual bool HasEncounterNone()
        {
            return PartyAbility(ABILITY_ENCOUNTER_NONE);
        }

        public virtual bool HasCancelSurprise()
        {
            return PartyAbility(ABILITY_CANCEL_SURPRISE);
        }

        public virtual bool HasRaisePreemptive()
        {
            return PartyAbility(ABILITY_RAISE_PREEMPTIVE);
        }

        public virtual bool HasGoldDouble()
        {
            return PartyAbility(ABILITY_GOLD_DOUBLE);
        }

        public virtual bool HasDropItemDouble()
        {
            return PartyAbility(ABILITY_DROP_ITEM_DOUBLE);
        }

        public virtual float RatePreemptive(int troopAgi)
        {
            float rate = Agility() >= troopAgi ? 0.05f : 0.03f;
            if (HasRaisePreemptive())
            {
                rate *= 4;
            }
            return rate;
        }

        public virtual float RateSurprise(int troopAgi)
        {
            float rate = Agility() >= troopAgi ? 0.03f : 0.05f;
            if (HasCancelSurprise())
            {
                rate = 0;
            }
            return rate;
        }

        public virtual void PerformVictory()
        {
            foreach (Game_Actor actor in Members())
            {
                actor.PerformVictory();
            }
        }

        public virtual void PerformEscape()
        {
            foreach (Game_Actor actor in Members())
            {
                actor.PerformEscape();
            }
        }

        public virtual void RemoveBattleStates()
        {
            foreach (var actor in Members())
            {
                actor.RemoveBattleStates();
            }
        }

        public virtual void RequestMotionRefresh()
        {
            foreach (var actor in Members())
            {
                actor.RequestMotionRefresh();
            }
        }

        public virtual void OnEscapeFailure()
        {
            foreach (Game_Actor actor in Members())
            {
                actor.OnEscapeFailure();
            }
        }
    }
}