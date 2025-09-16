using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for self switches.
    /// </summary>
    [Serializable]
    public partial class Game_SelfSwitches
    {
        public struct Key
        {
            public int MapId;
            public int EventId;
            public int SelfSwitchId;

            public Key(int mapId, int eventId, string selfSwitchId)
            {
                MapId = mapId;
                EventId = eventId;
                switch (selfSwitchId)
                {
                    case "A":
                        SelfSwitchId = 0;
                        break;
                    
                    case "B":
                        SelfSwitchId = 1;
                        break;
                    
                    case "C":
                        SelfSwitchId = 2;
                        break;
                    
                    case "D":
                    default:
                        SelfSwitchId = 3;
                        break;
                }
                
            }

            public override string ToString()
            {
                return $"{MapId}_{EventId}_{SelfSwitchId}";
            }
        }
        
        private Dictionary<string, bool> _data = new();
        
        protected Game_SelfSwitches() {}

        public void Clear()
        {
            _data.Clear();
        }

        public bool Value(Key key)
        {
            return _data.GetValueOrDefault(key.ToString());
        }

        public void SetValue(Key key, bool value)
        {
            if (value)
            {
                _data[key.ToString()] = value;
            }
            else
            {
                _data.Remove(key.ToString());
            }
            
            OnChange();
        }
        public void OnChange()
        {
            Rmmz.gameMap.RequestRefresh();
        }
    }
}