using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    public partial class RmmzCharacterMoveRouteCommand
    {
        private static Dictionary<string, Action<Game_Character>> _codeMap = new();
        
        public static void Execute(RmmzJavascriptCode code, Game_Character self)
        {
            if (_codeMap.TryGetValue(code.GenerateKey(), out var func))
            {
                func.Invoke(self);
                return;
            }

            Debug.LogWarning($"{code.GenerateKey()}に対するメソッドが実装されていません");
        }

        private static void Add(string key, Action<Game_Character> action)
        {
            _codeMap.Add(RmmzJavascriptCode.NormalizeKey(key), action);
        }

        private static void Clear()
        {
            _codeMap.Clear();
        }
    }
}