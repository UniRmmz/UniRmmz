using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using TMPro.EditorUtilities;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace UniRmmz.Editor
{
    public class GenerateFontAssetMenuCommand : EditorWindow
    {
        private const string RESOURCES_PATH = "Assets/Resources/Fonts";

        [MenuItem("UniRmmz/Tools/Generate Font Asset")]
        public static void Generate()
        {
            if (Application.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            EditorCoroutineUtility.StartCoroutineOwnerless(GenerateCoroutine());
        }

        private static IEnumerator GenerateCoroutine()
        {
            Debug.Log("フォントアセット生成を開始します...");

            // StreamingAssetsの特定フォルダからフォントファイルを検索
            List<string> fontFiles = FindFontFiles();

            if (fontFiles.Count == 0)
            {
                Debug.LogWarning("フォントファイルが見つかりませんでした。");
                yield break;
            }

            // 各フォントファイルを処理
            foreach (string fontFile in fontFiles)
            {
                yield return ProcessFontFile(fontFile);
            }

            // アセットデータベースを更新
            AssetDatabase.Refresh();
            //AssetDatabase.SaveAssets();

            Debug.Log($"フォントアセット生成完了");
        }
        
        private static List<string> FindFontFiles()
        {
            List<string> fontFiles = new List<string>();
            string searchPath = Path.Combine(Rmmz.RootPath, "fonts");

            if (!Directory.Exists(searchPath))
            {
                Debug.LogWarning($"フォルダが存在しません: {searchPath}");
                return fontFiles;
            }

            // サポートするフォント拡張子
            string[] fontExtensions = { "*.ttf", "*.otf" };

            foreach (string extension in fontExtensions)
            {
                string[] files = Directory.GetFiles(searchPath, extension, SearchOption.AllDirectories);
                fontFiles.AddRange(files);
            }

            Debug.Log($"{fontFiles.Count} 個のフォントファイルが見つかりました。");
            return fontFiles;
        }

        private static IEnumerator ProcessFontFile(string fontFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(fontFilePath);
            string extension = Path.GetExtension(fontFilePath).ToLower();

            Debug.Log($"処理中: {fileName}{extension}");
            
            // Resourcesフォルダにコピー
            string destinationPath = Path.Combine(RESOURCES_PATH, fileName + extension);
            File.Copy(fontFilePath, destinationPath, true);
            
            /*
            // まず空のTMP_FontAssetを作成
            TMP_FontAsset fontAsset = ScriptableObject.CreateInstance<TMP_FontAsset>();
            AssetDatabase.CreateAsset(fontAsset, assetPath);
            */
            string assetPath = Path.Combine(RESOURCES_PATH, fileName + " SDF.asset");
            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);


            // アセットとしてインポート
            AssetDatabase.ImportAsset(destinationPath);

            // フォントアセットを取得
            Font font = AssetDatabase.LoadAssetAtPath<Font>(destinationPath);
            if (font == null)
            {
                Debug.LogError($"フォントの読み込みに失敗: {destinationPath}");
                yield break;
            }
            
            var window = GetWindow<TMPro_FontAssetCreatorWindow>();
            window.titleContent = new GUIContent("Font Asset Creator");
            window.Focus();

            // TMPro_FontAssetCreatorWindow.ShowFontAtlasCreatorWindow(sourceFont) の代替
            if (fontAsset == null)
            {
                var showMethod = typeof(TMPro_FontAssetCreatorWindow).GetMethod("ShowFontAtlasCreatorWindow", 
                    new System.Type[] { typeof(Font) });
                showMethod.Invoke(null, new object[] { font });    
            }
            else
            {
                var showMethod = typeof(TMPro_FontAssetCreatorWindow).GetMethod("ShowFontAtlasCreatorWindow", 
                    new System.Type[] { typeof(TMP_FontAsset) });
                showMethod.Invoke(null, new object[] { fontAsset });
            }
            

            var modeField = typeof(TMPro_FontAssetCreatorWindow).GetField("m_CharacterSetSelectionMode",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            modeField.SetValue(window, 8);// Characters from File
            
            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Plugins/UniRmmz/japanese_full.txt");
            var charactersFile = typeof(TMPro_FontAssetCreatorWindow).GetField("m_CharactersFromFile",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            charactersFile.SetValue(window, textAsset);
            
            /*
            var logacyFontAssetField = typeof(TMPro_FontAssetCreatorWindow).GetField("m_SelectedFontAsset",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            logacyFontAssetField.SetValue(window, fontAsset);
            */
            
            yield return new WaitUntil(() => window == null);
        }
    }
}