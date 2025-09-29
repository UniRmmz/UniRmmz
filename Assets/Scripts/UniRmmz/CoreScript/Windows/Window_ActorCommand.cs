using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting an actor's action on the battle screen.
    /// </summary>
    public partial class Window_ActorCommand //: Window_Command
    {
        protected Game_Actor _actor;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Openness = 0;
            Deactivate();
            _actor = null;
        }

        protected override void MakeCommandList()
        {
            if (_actor != null)
            {
                AddAttackCommand();
                AddSkillCommands();
                AddGuardCommand();
                AddItemCommand();
            }
        }

        protected virtual void AddAttackCommand()
        {
            AddCommand(Rmmz.TextManager.Attack, "attack", _actor.CanAttack());
        }

        protected virtual void AddSkillCommands()
        {
            var skillTypes = _actor.SkillTypes();
            foreach (int stypeId in skillTypes)
            {
                string name = Rmmz.dataSystem.SkillTypes[stypeId];
                AddCommand(name, "skill", true, stypeId);
            }
        }

        protected virtual void AddGuardCommand()
        {
            AddCommand(Rmmz.TextManager.Guard, "guard", _actor.CanGuard());
        }

        protected virtual void AddItemCommand()
        {
            AddCommand(Rmmz.TextManager.Item, "item");
        }

        public virtual void Setup(Game_Actor actor)
        {
            _actor = actor;
            Refresh();
            SelectLast();
            Activate();
            Open();
        }

        public virtual Game_Actor Actor()
        {
            return _actor;
        }

        protected override void ProcessOk()
        {
            if (_actor != null)
            {
                if (Rmmz.ConfigManager.CommandRemember)
                {
                    _actor.SetLastCommandSymbol(CurrentSymbol());
                }
                else
                {
                    _actor.SetLastCommandSymbol("");
                }
            }
            base.ProcessOk();
        }

        protected virtual void SelectLast()
        {
            ForceSelect(0);
            if (_actor != null && Rmmz.ConfigManager.CommandRemember)
            {
                string symbol = _actor.LastCommandSymbol();
                SelectSymbol(symbol);
                if (symbol == "skill")
                {
                    var skill = _actor.LastBattleSkill();
                    if (skill != null)
                    {
                        SelectExt(skill.StypeId);
                    }
                }
            }
        }
    }
}