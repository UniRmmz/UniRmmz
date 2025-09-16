using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting a skill type on the skill screen.
    /// </summary>
    public partial class Window_SkillType : Window_Command
    {
        protected Game_Actor _actor = null;
        protected Window_SkillList _skillWindow;

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _actor = null;
        }

        public virtual void SetActor(Game_Actor actor)
        {
            if (_actor != actor)
            {
                _actor = actor;
                Refresh();
                SelectLast();
            }
        }

        protected override void MakeCommandList()
        {
            if (_actor != null)
            {
                List<int> skillTypes = _actor.SkillTypes();
                foreach (int stypeId in skillTypes)
                {
                    string name = Rmmz.DataSystem.SkillTypes[stypeId];
                    AddCommand(name, "skill", true, stypeId);
                }
            }
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            if (_skillWindow != null)
            {
                _skillWindow.SetStypeId((int)CurrentExt());
            }
        }

        public virtual void SetSkillWindow(Window_SkillList skillWindow)
        {
            _skillWindow = skillWindow;
        }

        protected virtual void SelectLast()
        {
            var skill = _actor?.LastMenuSkill();
            if (skill != null)
            {
                SelectExt(skill.StypeId);
            }
            else
            {
                ForceSelect(0);
            }
        }
    }

}