using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    public static partial class RmmzEval
    {
        private static Dictionary<string, Func<Game_Battler, Game_Battler, Game_Variables, double>> _damageFormulaMap = new();
        
        public static float EvaluateDamageFormula(string formula, Game_Battler a, Game_Battler b, Game_Variables v)
        {
            if (_damageFormulaMap.TryGetValue(formula, out var func))
            {
                return (float)func.Invoke(a, b, v);
            }
            
            throw new Exception($"{formula}に対するメソッドが実装されていません");            
        }

        private static void Clear()
        {
            _damageFormulaMap.Clear();
        }
        
    }
}