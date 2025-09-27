using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting "Go to Title" on the game end screen.
    /// </summary>
    public partial class Window_GameEnd : Window_Command
    {
        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Openness = 0;
            Open();
        }

        protected override void MakeCommandList()
        {
            AddCommand(Rmmz.TextManager.ToTitle, "toTitle");
            AddCommand(Rmmz.TextManager.Cancel, "cancel");
        }
    }
}