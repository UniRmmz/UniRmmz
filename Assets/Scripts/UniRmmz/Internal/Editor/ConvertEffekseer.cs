using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniRmmz.Editor
{
    public class ConvertEffekseerMenuCommand : EditorWindow
    {
        private static readonly string SrcPath = Path.Combine(Rmmz.RootPath, "effects");
        private static readonly string DestPath = "Assets/Resources/Effekseer";

        [MenuItem("UniRmmz/Tools/Convert Effekseer")]
        public static void ConvertEffekseer()
        {
            try
            {
                // コピー元のディレクトリが存在するかチェック
                if (!Directory.Exists(SrcPath))
                {
                    Debug.LogError($"Source directory does not exist: {SrcPath}");
                    return;
                }

                // コピー先のディレクトリを作成（既に存在する場合は何もしない）
                if (!Directory.Exists(DestPath))
                {
                    Directory.CreateDirectory(DestPath);
                    Debug.Log($"Created destination directory: {DestPath}");
                }

                // フォルダを再帰的にコピー
                CopyDirectory(SrcPath, DestPath, true);

                Debug.Log($"Successfully copied from {SrcPath} to {DestPath}");

                // AssetDatabaseを更新してインポートを実行
                AssetDatabase.Refresh();
                Debug.Log("AssetDatabase refreshed. Import completed.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error during Effekseer conversion: {ex.Message}");
            }
        }

        /// <summary>
        /// ディレクトリを再帰的にコピーする
        /// </summary>
        /// <param name="sourceDir">コピー元ディレクトリ</param>
        /// <param name="destinationDir">コピー先ディレクトリ</param>
        /// <param name="recursive">サブディレクトリも含めてコピーするか</param>
        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // ソースディレクトリの情報を取得
            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // ディレクトリを作成
            Directory.CreateDirectory(destinationDir);

            // ファイルをコピー
            foreach (FileInfo file in dir.GetFiles())
            {
                if (file.Extension == ".meta") continue;// metaファイルは無視
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true); // 既存ファイルを上書き
                Debug.Log($"Copied file: {file.Name}");
            }

            // 再帰的にサブディレクトリをコピー
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                    Debug.Log($"Copied directory: {subDir.Name}");
                }
            }
        }
    }
}