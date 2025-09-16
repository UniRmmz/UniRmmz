using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    public partial class RmmzScriptCommand
    {
        private static Dictionary<string, Action<Game_Interpreter>> _codeMap = new();
        
        public static void ExecuteScriptCommand(RmmzJavascriptCode code, Game_Interpreter self)
        {
            if (_codeMap.TryGetValue(code.GenerateKey(), out var func))
            {
                func.Invoke(self);
                return;
            }
            
            throw new Exception($"{code.GenerateKey()}に対するメソッドが実装されていません");            
        }

        private static void Add(string key, Action<Game_Interpreter> action)
        {
            _codeMap.Add(RmmzJavascriptCode.NormalizeKey(key), action);
        }

        private static void Clear()
        {
            _codeMap.Clear();
        }
    }
}