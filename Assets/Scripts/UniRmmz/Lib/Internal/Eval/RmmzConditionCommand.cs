using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    public partial class RmmzConditionCommand
    {
        private static Dictionary<string, Func<Game_Interpreter, bool>> _codeMap = new();
        
        public static bool Execute(RmmzJavascriptCode code, Game_Interpreter self)
        {
            if (_codeMap.TryGetValue(code.GenerateKey(), out var func))
            {
                return func.Invoke(self);;
            }
            
            Debug.LogWarning($"{code.GenerateKey()}に対するメソッドが実装されていません");
            return false;
        }
        
        private static void Add(string key, Func<Game_Interpreter, bool> action)
        {
            _codeMap.Add(RmmzJavascriptCode.NormalizeKey(key), action);
        }

        private static void Clear()
        {
            _codeMap.Clear();
        }
    }
}