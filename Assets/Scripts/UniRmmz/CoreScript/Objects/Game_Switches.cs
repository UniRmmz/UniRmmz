using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for switches.
    /// </summary>
    [Serializable]
    public partial class Game_Switches
    {
        protected List<bool> _data = new();
        
        protected Game_Switches() {}

        public virtual void Clear()
        {
            _data.Clear();
        }

        public virtual bool Value(int switchId)
        {
            return _data.ElementAtOrDefault(switchId);
        }

        public virtual void SetValue(int switchId, bool value)
        {
            if (switchId > 0 && switchId < Rmmz.dataSystem.Switches.Length)
            {
                _data.SetWithExpansion(switchId, value);
                OnChange();
            }
        }

        public virtual void OnChange()
        {
            Rmmz.gameMap.RequestRefresh();
        }
    }
}