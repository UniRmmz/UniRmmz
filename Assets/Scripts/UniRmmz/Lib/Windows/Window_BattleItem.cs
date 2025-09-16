using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting an item to use on the battle screen.
    /// </summary>
    public partial class Window_BattleItem : Window_ItemList
    {
        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Hide();
        }

        protected override bool Includes(DataCommonItem item)
        {
            return Rmmz.gameParty.CanUse(item);
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