using UnityEngine;

namespace UniRmmz
{
    public static class RmmzDebugUtility
    {
        private static readonly Color[] debugColors = new Color[]
        {
            new Color(1.0f, 0.2f, 0.2f),  // 赤
            new Color(1.0f, 0.6f, 0.2f),  // オレンジ
            new Color(1.0f, 1.0f, 0.2f),  // 黄色
            new Color(0.6f, 1.0f, 0.2f),  // 黄緑
            new Color(0.2f, 1.0f, 0.2f),  // 緑
            new Color(0.2f, 1.0f, 0.6f),  // 青緑
            new Color(0.2f, 1.0f, 1.0f),  // シアン
            new Color(0.2f, 0.6f, 1.0f),  // 空色
            new Color(0.2f, 0.2f, 1.0f),  // 青
            new Color(0.6f, 0.2f, 1.0f),  // 青紫
            new Color(1.0f, 0.2f, 1.0f),  // マゼンタ
            new Color(1.0f, 0.2f, 0.6f),  // ピンク
            new Color(0.8f, 0.4f, 0.2f),  // 茶色
            new Color(0.7f, 0.7f, 0.2f),  // オリーブ
            new Color(0.4f, 0.8f, 0.4f),  // ライム
            new Color(0.2f, 0.8f, 0.8f),  // ターコイズ
            new Color(0.4f, 0.4f, 0.8f),  // インディゴ
            new Color(0.8f, 0.4f, 0.8f),  // バイオレット
            new Color(0.9f, 0.5f, 0.3f),  // コーラル
            new Color(0.3f, 0.7f, 0.9f)   // スカイブルー
        };
        
        private static System.Random s_random = new System.Random();

        public static Color GetRandomColor()
        {
            return debugColors[s_random.Next(debugColors.Length)];
        }

        /// <summary>
        /// 指定されたインデックスの色を取得（デバッグ用）
        /// </summary>
        public static Color GetColorByIndex(int index)
        {
            return debugColors[index % debugColors.Length];
        }

    }
}