using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for a troop and the battle-related data.
    /// </summary>
    [Serializable]
    public partial class Game_Troop //: Game_Unit
    {
        private static readonly string[] LETTER_TABLE_HALF = {
            " A", " B", " C", " D", " E", " F", " G", " H", " I", " J", " K", " L", " M",
            " N", " O", " P", " Q", " R", " S", " T", " U", " V", " W", " X", " Y", " Z"
        };
        
        private static readonly string[] LETTER_TABLE_FULL = {
            "Ａ", "Ｂ", "Ｃ", "Ｄ", "Ｅ", "Ｆ", "Ｇ", "Ｈ", "Ｉ", "Ｊ", "Ｋ", "Ｌ", "Ｍ",
            "Ｎ", "Ｏ", "Ｐ", "Ｑ", "Ｒ", "Ｓ", "Ｔ", "Ｕ", "Ｖ", "Ｗ", "Ｘ", "Ｙ", "Ｚ"
        };

        protected Game_Interpreter _interpreter;
        protected int _troopId;
        protected Dictionary<int, bool> _eventFlags;
        protected List<Game_Enemy> _enemies;
        protected int _turnCount;
        protected Dictionary<string, int> _namesCount;

        protected Game_Troop()
        {
            _interpreter = Game_Interpreter.Create();
            Clear();
        }

        public virtual bool IsEventRunning()
        {
            return _interpreter.IsRunning();
        }

        public virtual void UpdateInterpreter()
        {
            _interpreter.Update();
        }

        public virtual int TurnCount()
        {
            return _turnCount;
        }

        public override IEnumerable<Game_Battler> Members()
        {
            return _enemies;
        }

        public virtual IEnumerable<Game_Enemy> EnemyMembers()
        {
            return _enemies;
        }

        public virtual void Clear()
        {
            _interpreter.Clear();
            _troopId = 0;
            _eventFlags = new Dictionary<int, bool>();
            _enemies = new List<Game_Enemy>();
            _turnCount = 0;
            _namesCount = new Dictionary<string, int>();
        }

        public virtual DataTroop Troop()
        {
            return Rmmz.dataTroops[_troopId];
        }

        public virtual void Setup(int troopId)
        {
            Clear();
            _troopId = troopId;
            _enemies = new List<Game_Enemy>();
            
            foreach (var member in Troop().Members)
            {
                if (Rmmz.dataEnemies[member.EnemyId] != null)
                {
                    int enemyId = member.EnemyId;
                    float x = member.X;
                    float y = member.Y;
                    var enemy = Game_Enemy.Create(enemyId, x, y);
                    
                    if (member.Hidden)
                    {
                        enemy.Hide();
                    }
                    
                    _enemies.Add(enemy);
                }
            }
            
            MakeUniqueNames();
        }

        public virtual void MakeUniqueNames()
        {
            var table = LetterTable();
            foreach (var enemy in EnemyMembers())
            {
                if (enemy.IsAlive() && enemy.IsLetterEmpty())
                {
                    string name = enemy.OriginalName();
                    int n = _namesCount.GetValueOrDefault(name);
                    enemy.SetLetter(table[n % table.Length]);
                    _namesCount[name] = n + 1;
                }
            }
            UpdatePluralFlags();
        }

        protected virtual void UpdatePluralFlags()
        {
            foreach (var enemy in EnemyMembers())
            {
                string name = enemy.OriginalName();
                if (_namesCount.ContainsKey(name) && _namesCount[name] >= 2)
                {
                    enemy.SetPlural(true);
                }
            }
        }

        protected virtual string[] LetterTable()
        {
            return Rmmz.gameSystem.IsCJK() ? LETTER_TABLE_FULL : LETTER_TABLE_HALF;
        }

        public virtual List<string> EnemyNames()
        {
            var names = new List<string>();
            foreach (var enemy in EnemyMembers())
            {
                string name = enemy.OriginalName();
                if (enemy.IsAlive() && !names.Contains(name))
                {
                    names.Add(name);
                }
            }
            return names;
        }

        public virtual bool MeetsConditions(DataTroopPage page)
        {
            var c = page.Conditions;
            
            // Check if any condition is set
            if (!c.TurnEnding && !c.TurnValid && !c.EnemyValid && !c.ActorValid && !c.SwitchValid)
            {
                return false; // No conditions set
            }

            // Turn ending condition
            if (c.TurnEnding)
            {
                if (!Rmmz.BattleManager.IsTurnEnd())
                {
                    return false;
                }
            }

            // Turn count condition
            if (c.TurnValid)
            {
                int n = _turnCount;
                int a = c.TurnA;
                int b = c.TurnB;
                
                if (b == 0 && n != a)
                {
                    return false;
                }
                if (b > 0 && (n < 1 || n < a || n % b != a % b))
                {
                    return false;
                }
            }

            // Enemy HP condition
            if (c.EnemyValid)
            {
                var enemies = Rmmz.gameTroop.EnemyMembers();
                var enemy = enemies.ElementAtOrDefault(c.EnemyIndex);
                if (enemy == null || enemy.HpRate() * 100 > c.EnemyHp)
                {
                    return false;
                }
            }

            // Actor HP condition
            if (c.ActorValid)
            {
                var actor = Rmmz.gameActors.Actor(c.ActorId);
                if (actor == null || actor.HpRate() * 100 > c.ActorHp)
                {
                    return false;
                }
            }

            // Switch condition
            if (c.SwitchValid)
            {
                if (!Rmmz.gameSwitches.Value(c.SwitchId))
                {
                    return false;
                }
            }

            return true;
        }

        public virtual void SetupBattleEvent()
        {
            if (!_interpreter.IsRunning())
            {
                if (_interpreter.SetupReservedCommonEvent())
                {
                    return;
                }
                
                var pages = Troop().Pages;
                for (int i = 0; i < pages.Length; i++)
                {
                    var page = pages[i];
                    if (MeetsConditions(page) && !_eventFlags.ContainsKey(i))
                    {
                        _interpreter.Setup(page.List);
                        if (page.Span <= 1)
                        {
                            _eventFlags[i] = true;
                        }
                        break;
                    }
                }
            }
        }

        public virtual void IncreaseTurn()
        {
            var pages = Troop().Pages;
            for (int i = 0; i < pages.Length; i++)
            {
                var page = pages[i];
                if (page.Span == 1)
                {
                    _eventFlags[i] = false;
                }
            }
            _turnCount++;
        }

        public virtual int ExpTotal()
        {
            return DeadMembers().Cast<Game_Enemy>().Sum(enemy => enemy.Exp());
        }

        public virtual int GoldTotal()
        {
            var members = DeadMembers().Cast<Game_Enemy>();
            return Mathf.RoundToInt(members.Sum(enemy => enemy.Gold()) * GoldRate());
        }

        protected virtual float GoldRate()
        {
            return Rmmz.gameParty.HasGoldDouble() ? 2 : 1;
        }

        public virtual List<DataCommonItem> MakeDropItems()
        {
            var members = DeadMembers().Cast<Game_Enemy>();
            var result = new List<DataCommonItem>();
            foreach (var enemy in members)
            {
                result.AddRange(enemy.MakeDropItems());
            }
            return result;
        }

        public virtual bool IsTpbTurnEnd()
        {
            var members = Members();
            if (!members.Any())
            {
                return false;
            }

            int turnMax = members.Max(member => member.TurnCount());
            return turnMax > _turnCount;
        }
    }
}