using UnityEngine;

namespace UniRmmz
{
    public static class RmmzMath
    {
        public static int RandomInt(int max)
        {
            return UnityEngine.Random.Range(0, max);
        }
    }
}