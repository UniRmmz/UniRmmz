using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for variables.
    /// </summary>
    [Serializable]
    public partial class Game_Variables
    {
        protected List<int> _data = new();

        protected Game_Variables() {}
        public virtual void Clear()
        {
            _data.Clear();
        }

        public virtual int Value(int variableId)
        {
            return _data.ElementAtOrDefault(variableId);
        }

        public virtual void SetValue(int variableId, int value)
        {
            if (variableId > 0 && variableId < Rmmz.dataSystem.Variables.Length)
            {
                _data.SetWithExpansion(variableId, value);
                OnChange();
            }
        }

        public virtual void SetValue(int variableId, float value)
        {
            SetValue(variableId, Mathf.FloorToInt(value));
        }

        public virtual void OnChange()
        {
            Rmmz.gameMap.RequestRefresh();
        }
    }
}