using UnityEngine;
using UnityEditor;
using System.Linq;

namespace UniRmmz.Editor
{
    [CustomEditor(typeof(RmmzFilterCanvas))]
    public class RmmzFilterCanvasCustomEditor : UnityEditor.Editor
    {
        private RmmzFilterCanvas _targetCanvas;
        private bool _showFiltersSection = true;
        private bool _showLastRenderedTextureSection = true;
        
        private GUIStyle _boxStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _previewStyle;

        private void OnEnable()
        {
            _targetCanvas = (RmmzFilterCanvas)target;
        }

        private void InitStyles()
        {
            if (_boxStyle == null)
            {
                _boxStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(10, 10, 10, 10),
                    margin = new RectOffset(0, 0, 10, 10)
                };
            }

            if (_headerStyle == null)
            {
                _headerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    margin = new RectOffset(0, 0, 5, 5)
                };
            }

            if (_previewStyle == null)
            {
                _previewStyle = new GUIStyle(GUI.skin.box)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { background = EditorGUIUtility.whiteTexture }
                };
            }
        }

        public override void OnInspectorGUI()
        {
            InitStyles();
            
            // 標準のインスペクタを描画
            DrawDefaultInspector();
            
            EditorGUILayout.Space(10);

            // Filters情報セクション
            _showFiltersSection = EditorGUILayout.Foldout(_showFiltersSection, "Filters Information", true);
            if (_showFiltersSection)
            {
                EditorGUILayout.BeginVertical(_boxStyle);
                
                var filters = _targetCanvas.Filters.ToList();
                if (filters.Any())
                {
                    EditorGUILayout.LabelField($"Filter Count: {filters.Count}");
                    
                    for (int i = 0; i < filters.Count; i++)
                    {
                        var filter = filters[i];
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.LabelField($"Filter {i+1}: {filter.GetType().Name}");
                        
                        if (filter is ColorFilter colorFilter)
                        {
                            // ColorFilterの特定のプロパティを表示
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField("Type: Color Filter");
                            // 必要なプロパティがパブリックの場合は表示
                            EditorGUI.indentLevel--;
                        }
                        else
                        {
                            EditorGUI.indentLevel++;
                            EditorGUILayout.LabelField("Type: Other Filter");
                            EditorGUI.indentLevel--;
                        }
                        
                        EditorGUILayout.EndVertical();
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No filters applied.", MessageType.Info);
                }
                
                EditorGUILayout.EndVertical();
            }
            
            // LastRenderedTextureセクション
            _showLastRenderedTextureSection = EditorGUILayout.Foldout(_showLastRenderedTextureSection, "Last Rendered Texture", true);
            if (_showLastRenderedTextureSection)
            {
                EditorGUILayout.BeginVertical(_boxStyle);
                
                Texture lastTexture = _targetCanvas.LastRenderedTexture;
                if (lastTexture != null)
                {
                    EditorGUILayout.LabelField("Texture Name", lastTexture.name);
                    EditorGUILayout.LabelField("Dimensions", $"{lastTexture.width} x {lastTexture.height}");
                    
                    GUILayout.Space(5);
                    float previewSize = 200;
                    Rect previewRect = GUILayoutUtility.GetRect(previewSize, previewSize);
                    EditorGUI.DrawPreviewTexture(previewRect, lastTexture, null, ScaleMode.ScaleToFit);
                }
                else
                {
                    EditorGUILayout.HelpBox("No texture has been rendered yet or the last rendered texture is no longer available.", MessageType.Info);
                }
                
                EditorGUILayout.EndVertical();
            }
            
            // リフレッシュボタン
            EditorGUILayout.Space(10);
            if (GUILayout.Button("Refresh Editor", GUILayout.Height(30)))
            {
                Repaint();
            }
        }
    }
}
