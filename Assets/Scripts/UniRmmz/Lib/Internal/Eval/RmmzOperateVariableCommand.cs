using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    public partial class RmmzOperateVariableCommand
    {
        private static Dictionary<string, Func<Game_Interpreter, float>> _codeMap = new();
        
        public static float Execute(RmmzJavascriptCode code, Game_Interpreter self)
        {
            if (_codeMap.TryGetValue(code.GenerateKey(), out var func))
            {
                return func.Invoke(self);;
            }
            
            Debug.LogWarning($"{code.GenerateKey()}に対するメソッドが実装されていません");
            return 0;
        }
        
        private static void Add(string key, Func<Game_Interpreter, float> action)
        {
            _codeMap.Add(RmmzJavascriptCode.NormalizeKey(key), action);
        }

        private static void Clear()
        {
            _codeMap.Clear();
        }
    }
}