using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting a skill to use on the battle screen.
    /// </summary>
    public partial class Window_BattleSkill //: Window_SkillList
    {
        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Hide();
        }

        public override void Show()
        {
            SelectLast();
            ShowHelpWindow();
            base.Show();
        }

        public override void Hide()
        {
            HideHelpWindow();
            base.Hide();
        }
    }
}