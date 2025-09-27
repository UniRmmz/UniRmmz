using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of windows for selecting a command.
    /// </summary>
    public abstract partial class Window_Command : Window_Selectable
    {
        protected class Command
        {
            public string Name;
            public string Symbol;
            public bool Enabled;
            public object Ext;
        }

        protected List<Command> _list = new();

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            Refresh();
            Select(0);
            Activate();
        }

        protected override int MaxItems() => _list.Count;

        protected virtual void ClearCommandList()
        {
            _list.Clear();
        }

        protected virtual void MakeCommandList()
        {
            // override to add commands
        }

        protected void AddCommand(string name, string symbol, bool enabled = true, object ext = null)
        {
            _list.Add(new Command { Name = name, Symbol = symbol, Enabled = enabled, Ext = ext });
        }

        public virtual string CommandName(int index) => _list[index].Name;
        public virtual string CommandSymbol(int index) => _list[index].Symbol;
        public virtual bool IsCommandEnabled(int index) => _list[index].Enabled;

        protected Command CurrentData() => Index() >= 0 ? _list.ElementAtOrDefault(Index()) : null;
        
        public override bool IsCurrentItemEnabled()
        {
            return CurrentData()?.Enabled ?? false;
        }
        public virtual string CurrentSymbol() => CurrentData()?.Symbol;
        public virtual object CurrentExt() => CurrentData()?.Ext;

        public virtual int FindSymbol(string symbol)
        {
            return _list.FindIndex(cmd => cmd.Symbol == symbol);
        }

        public virtual void SelectSymbol(string symbol)
        {
            int index = FindSymbol(symbol);
            if (index >= 0)
            {
                ForceSelect(index);
            }
            else
            {
                ForceSelect(0);    
            }
        }

        public virtual int FindExt(object ext)
        {
            return _list.FindIndex(cmd => Equals(cmd.Ext, ext));
        }

        public virtual void SelectExt(object ext)
        {
            int index = FindExt(ext);
            if (index >= 0)
            {
                ForceSelect(index);
            }
            else
            {
                ForceSelect(0);    
            }
        }

        public override void DrawItem(int index)
        {
            var rect = ItemLineRect(index);
            var align = ItemTextAlign();
            ResetTextColor();
            ChangePaintOpacity(IsCommandEnabled(index));
            DrawText(CommandName(index), (int)rect.x, (int)rect.y, (int)rect.width, align);
        }

        protected virtual Bitmap.TextAlign ItemTextAlign() => Bitmap.TextAlign.Center;

        protected override bool IsOkEnabled() => true;

        protected override void CallOkHandler()
        {
            string symbol = CurrentSymbol();
            if (IsHandled(symbol))
            {
                CallHandler(symbol);
            }
            else if (IsHandled("ok"))
            {
                base.CallOkHandler();
            }
            else
            {
                Activate();
            }
        }

        public override void Refresh()
        {
            ClearCommandList();
            MakeCommandList();
            base.Refresh();
        }
    }
}

