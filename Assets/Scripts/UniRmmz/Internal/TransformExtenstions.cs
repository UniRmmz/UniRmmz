using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    public static class TransformExtenstions
    {
        public static T GetChild<T>(this Transform self, int index) where T : Component
        {
            return self.GetChild(index).GetComponent<T>();
        }
        
        public static IEnumerable<T> GetChildren<T>(this Transform self) where T : Component
        {
            int count = self.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                yield return self.GetChild<T>(i); 
            }
        }
    }
}