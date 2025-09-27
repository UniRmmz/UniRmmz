using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting New Game/Continue on the title screen.
    /// </summary>
    public partial class Window_TitleCommand : Window_Command
    {
        protected static string _lastCommandSymbol = null;

        public static void InitCommandPosition()
        {
            _lastCommandSymbol = null;
        }

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Openness = 0;
            SelectLast();
        }

        protected override void MakeCommandList()
        {
            bool continueEnabled = IsContinueEnabled();
            AddCommand(Rmmz.TextManager.NewGame, "newGame");
            AddCommand(Rmmz.TextManager.Continue, "continue", continueEnabled);
            AddCommand(Rmmz.TextManager.Options, "options");
        }

        protected virtual bool IsContinueEnabled()
        {
            return Rmmz.DataManager.IsAnySavefileExists();
        }

        protected override void ProcessOk()
        {
            _lastCommandSymbol = CurrentSymbol();
            base.ProcessOk();
        }

        protected virtual void SelectLast()
        {
            if (!string.IsNullOrEmpty(_lastCommandSymbol))
            {
                SelectSymbol(_lastCommandSymbol);
            }
            else if (IsContinueEnabled())
            {
                SelectSymbol("continue");
            }
        }
    }
}