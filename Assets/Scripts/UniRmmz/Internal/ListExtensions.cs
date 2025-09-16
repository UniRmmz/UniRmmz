using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    public static class ListExtensions
    {
        public static void SetWithExpansion<T>(this List<T> self, int index, T value)
        {
            if (self != null)
            {
                int addCount = index - self.Count + 1;
                if (addCount > 0)
                {
                    self.AddRange(Enumerable.Repeat(default(T), addCount));
                }
                self[index] = value;    
            }
        }

        /// <summary>
        /// 指定された範囲の要素を含む新しいリストを取得する
        /// </summary>
        /// <param name="start">開始インデックス（負の値は末尾からの位置）</param>
        /// <returns>指定範囲の要素を含む新しいリスト</returns>
        public static List<T> Slice<T>(this List<T> self, int start = 0)
        {
            int count = self.Count;
            return self.Slice(start, count);
        }

        /// <summary>
        /// 指定された範囲の要素を含む新しいリストを取得する
        /// </summary>
        /// <param name="start">開始インデックス（負の値は末尾からの位置）</param>
        /// <param name="end">終了インデックス（この位置は含まれない、負の値は末尾からの位置）</param>
        /// <returns>指定範囲の要素を含む新しいリスト</returns>
        /// <example>
        /// var list = new List&lt;int&gt; { 0, 1, 2, 3, 4 };
        /// list.Slice(1, 3);  // [1, 2] (インデックス1から2まで)
        /// list.Slice(2);     // [2, 3, 4] (インデックス2から末尾まで)
        /// list.Slice(-2);    // [3, 4] (末尾から2要素)
        /// </example>
        public static List<T> Slice<T>(this List<T> self, int start, int end)
        {
            int count = self.Count;
        
            // 負のインデックスを正のインデックスに変換
            if (start < 0)
            {
                start = Math.Max(0, count + start);
            }
            if (end < 0)
            {
                end = Math.Max(0, count + end);
            }
        
            // 範囲チェック
            start = Math.Max(0, Math.Min(start, count));
            end = Math.Max(start, Math.Min(end, count));
        
            return self.Skip(start).Take(end - start).ToList();
        }
    }
}