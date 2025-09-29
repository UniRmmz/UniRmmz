using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting whether to fight or escape on the battle screen.
    /// </summary>
    public partial class Window_PartyCommand //: Window_Command
    {
        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Openness = 0;
            Deactivate();
        }

        protected override void MakeCommandList()
        {
            AddCommand(Rmmz.TextManager.Fight, "fight");
            AddCommand(Rmmz.TextManager.Escape, "escape", Rmmz.BattleManager.CanEscape());
        }

        public virtual void Setup()
        {
            Refresh();
            ForceSelect(0);
            Activate();
            Open();
        }
    }
}