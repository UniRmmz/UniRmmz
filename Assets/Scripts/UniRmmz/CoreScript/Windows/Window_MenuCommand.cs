using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The window for selecting a command on the menu screen.
    /// </summary>
    public partial class Window_MenuCommand //: Window_Command
    {
        protected static string _lastCommandSymbol = null;
        
        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            SelectLast();
            _canRepeat = false;
        }

        public static void InitCommandPosition()
        {
            _lastCommandSymbol = null;
        }
        protected override void MakeCommandList()
        {
            AddMainCommands();
            AddFormationCommand();
            AddOriginalCommands();
            AddOptionsCommand();
            AddSaveCommand();
            AddGameEndCommand();
        }

        protected virtual void AddMainCommands()
        {
            bool enabled = AreMainCommandsEnabled();
            if (NeedsCommand("item"))
            {
                AddCommand(Rmmz.TextManager.Item, "item", enabled);
            }
            if (NeedsCommand("skill"))
            {
                AddCommand(Rmmz.TextManager.Skill, "skill", enabled);
            }
            if (NeedsCommand("equip"))
            {
                AddCommand(Rmmz.TextManager.Equip, "equip", enabled);
            }
            if (NeedsCommand("status"))
            {
                AddCommand(Rmmz.TextManager.Status, "status", enabled);
            }
        }

        protected virtual void AddFormationCommand()
        {
            if (NeedsCommand("formation"))
            {
                bool enabled = IsFormationEnabled();
                AddCommand(Rmmz.TextManager.Formation, "formation", enabled);
            }
        }

        protected virtual void AddOriginalCommands()
        {
            // Override in derived classes
        }

        protected virtual void AddOptionsCommand()
        {
            if (NeedsCommand("options"))
            {
                bool enabled = IsOptionsEnabled();
                AddCommand(Rmmz.TextManager.Options, "options", enabled);
            }
        }

        protected virtual void AddSaveCommand()
        {
            if (NeedsCommand("save"))
            {
                bool enabled = IsSaveEnabled();
                AddCommand(Rmmz.TextManager.Save, "save", enabled);
            }
        }

        protected virtual void AddGameEndCommand()
        {
            bool enabled = IsGameEndEnabled();
            AddCommand(Rmmz.TextManager.GameEnd, "gameEnd", enabled);
        }

        protected virtual bool NeedsCommand(string name)
        {
            string[] table = { "item", "skill", "equip", "status", "formation", "save" };
            int index = System.Array.IndexOf(table, name);
            if (index >= 0)
            {
                return Rmmz.dataSystem.MenuCommands[index];
            }
            return true;
        }

        protected virtual bool AreMainCommandsEnabled()
        {
            return Rmmz.gameParty.Exists();
        }

        protected virtual bool IsFormationEnabled()
        {
            return Rmmz.gameParty.Size() >= 2 && Rmmz.gameSystem.IsFormationEnabled();
        }

        protected virtual bool IsOptionsEnabled() => true;

        protected virtual bool IsSaveEnabled()
        {
            return !Rmmz.DataManager.IsEventTest() && Rmmz.gameSystem.IsSaveEnabled();
        }

        protected virtual bool IsGameEndEnabled() => true;

        protected override void ProcessOk()
        {
            _lastCommandSymbol = CurrentSymbol();
            base.ProcessOk();
        }

        protected virtual void SelectLast()
        {
            SelectSymbol(_lastCommandSymbol);
        }
    }
}