using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting a command on the equipment screen.
    /// </summary>
    public partial class Window_EquipCommand : Window_HorzCommand
    {
        protected override int MaxCols() => 3;

        protected override void MakeCommandList()
        {
            AddCommand(Rmmz.TextManager.Equip2, "equip");
            AddCommand(Rmmz.TextManager.Optimize, "optimize");
            AddCommand(Rmmz.TextManager.Clear, "clear");
        }
    }
}