using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using System.Collections.Specialized;

namespace UniRmmz.Editor
{
    public static class GenerateScriptCommand
    {
        [MenuItem("UniRmmz/Tools/GenerateScriptCommand")]
        public static void Generate()
        {
            if (Application.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            Rmmz.InitializeUniRmmz(() =>
            {
                EditorCoroutineUtility.StartCoroutineOwnerless(GenerateCoroutine());    
            });
        }
        
        private static IEnumerator GenerateCoroutine()
        {
            Rmmz.DataManager.LoadDatabase();
            yield return new WaitUntil(() => Rmmz.DataManager.IsDatabaseLoaded());

            var outputFolderPath = Path.GetFullPath(Path.Combine(Application.streamingAssetsPath,
                "..\\Scripts\\UniRmmz\\Generated\\"));
            var result = new RmmzCollectJavascriptResult();
            
            CollectFromCommonEvent(result);

            foreach (var data in Rmmz.dataMapInfos)
            {
                if (data == null)
                {
                    continue;
                }
                
                Rmmz.DataManager.LoadMapData(data.Id);
                yield return new WaitUntil(() => Rmmz.DataManager.IsMapLoaded());

                CollectFromMapEvent(result);
            }

            result.Distinct();
            
            var className1 = "RmmzScriptCommand";
            GenerateAndSave(className1, result.ScriptCommands, outputFolderPath + $"{className1}.Generated.cs");
            
            var className2 = "RmmzConditionCommand";
            GenerateAndSave(className2, result.ConditionCommands, outputFolderPath + $"{className2}.Generated.cs");
            
            var className3 = "RmmzOperateVariableCommand";
            GenerateAndSave(className3, result.OperateVariableCommands, outputFolderPath + $"{className3}.Generated.cs");
            
            var className4 = "RmmzCharacterMoveRouteCommand";
            GenerateAndSave(className4, result.CharacterMoveRouteCommands, outputFolderPath + $"{className4}.Generated.cs");
        }

        private static void CollectFromMapEvent(RmmzCollectJavascriptResult result)
        {
            for (int i = 0; i < Rmmz.dataMap.Events.Count; i++)
            {
                var ev = Rmmz.dataMap.Events[i];
                if (ev != null && ev.Id != 0)
                {
                    foreach (var page in ev.Pages)
                    {
                        for (int j = 0; j < page.List.Count; ++j)
                        {
                            j = CollectFromCommand(page.List, j, result);
                        }
                    }
                }
            }
        }

        private static int CollectFromCommand(List<DataEventCommand> list, int currentIndex, RmmzCollectJavascriptResult result)
        {
            if (list[currentIndex].Code == 111)
            {
                currentIndex = CollectFromConditionCommand(list, currentIndex, result);
            }
            if (list[currentIndex].Code == 122)
            {
                currentIndex = CollectFromOperateValueCommand(list, currentIndex, result);
            }
            if (list[currentIndex].Code == 355)
            {
                currentIndex = CollectFromScriptCommand(list, currentIndex, result);
            }
            if (list[currentIndex].Code == 205)
            {
                currentIndex = CollectFromCharacterMoveRouteCommand(list, currentIndex, result);
            }

            return currentIndex;
        }

        private static int CollectFromConditionCommand(List<DataEventCommand> list, int currentIndex, RmmzCollectJavascriptResult result)
        {
            var parameters = list[currentIndex].Parameters;
            int typeId = Convert.ToInt32(parameters[0]);
            if (typeId == 12)
            {
                var code = new RmmzJavascriptCode();
                code.AddLine(parameters[1].ToString());
                result.ConditionCommands.Add(code);
            }
            return currentIndex;
        }
        
        private static int CollectFromOperateValueCommand(List<DataEventCommand> list, int currentIndex, RmmzCollectJavascriptResult result)
        {
            var parameters = list[currentIndex].Parameters;
            var operand = Convert.ToInt32(parameters[3]);
            if (operand == 4)
            {
                var code = new RmmzJavascriptCode();
                code.AddLine(parameters[4].ToString());
                result.OperateVariableCommands.Add(code);
            }
            return currentIndex;
        }
        
        private static int CollectFromScriptCommand(List<DataEventCommand> list, int currentIndex, RmmzCollectJavascriptResult result)
        {
            var code = new RmmzJavascriptCode();
            code.AddLine(Convert.ToString(list[currentIndex].Parameters[0]));
            while (currentIndex + 1 < list.Count && list[currentIndex + 1].Code == 655)
            {
                code.AddLine(Convert.ToString(list[currentIndex + 1].Parameters[0]));
                currentIndex++;
            }
            result.ScriptCommands.Add(code);
            return currentIndex;
        }
        
        private static int CollectFromCharacterMoveRouteCommand(List<DataEventCommand> list, int currentIndex, RmmzCollectJavascriptResult result)
        {
            var parameters = list[currentIndex].parameters;
            var moveRoute = ConvertEx.ToMoveRoute(parameters[1]);
            foreach (var command in moveRoute.List)
            {
                if ((Game_Character.RouteCodes)command.Code == Game_Character.RouteCodes.Script)
                {
                    var code = new RmmzJavascriptCode();
                    code.AddLine(command.Parameters[0].ToString());
                    result.CharacterMoveRouteCommands.Add(code);
                }
            }
            
            return currentIndex;
        }

        private static void CollectFromCommonEvent(RmmzCollectJavascriptResult result)
        {
            foreach (var data in Rmmz.dataCommonEvents)
            {
                if (data == null)
                {
                    continue;
                }

                for (int i = 0; i < data.List.Count; ++i)
                {
                    i = CollectFromCommand(data.List, i, result);
                }
            }
        }

        private static void GenerateAndSave(string className, IEnumerable<RmmzJavascriptCode> allCodes, string outputPath)
        { 
            var generatedCode = GenerateRmmzScriptCode(className, allCodes);
        
            try
            {
                File.WriteAllText(outputPath, generatedCode);
                AssetDatabase.Refresh();
            
                EditorUtility.DisplayDialog("Success", 
                    $"Code generated successfully!\nSaved to: {outputPath}", "OK");
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Error", $"Failed to save file: {e.Message}", "OK");
            }
        }


        private static string GenerateRmmzScriptCode(string className, IEnumerable<RmmzJavascriptCode> allCodes)
        {
            var sb = new StringBuilder();
            
            // ファイルヘッダー
            sb.AppendLine("// <auto-generated />");
            sb.AppendLine();
        
            sb.AppendLine("using System;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();
        
            // 名前空間とクラス
            sb.AppendLine($"namespace UniRmmz");
            sb.AppendLine("{");
            sb.AppendLine($"    public static partial class {className}");
            sb.AppendLine("    {");
        
            // 静的コンストラクタ
            sb.AppendLine($"        static {className}()");
            sb.AppendLine("        {");
            sb.AppendLine("            Clear();");
        
            foreach (var code in allCodes)
            {
                if (code.IsEmpty())
                {
                    continue;
                }
            
                var csharpCode = code.Lines.Select(line => ConvertCodeToCSharp(line));

                sb.AppendLine($"            Add(@\"{code.GenerateCode()}\"");
                if (csharpCode.Count() == 1 && !csharpCode.First().EndsWith(";"))
                {
                    sb.AppendLine($"                 , (self) => {csharpCode.First()});");
                }
                else
                {
                    sb.AppendLine("                 , (self) => ");
                    sb.AppendLine("             {");
                    foreach (var line in csharpCode)
                    {
                        sb.AppendLine($"\t\t\t\t{line}");
                    }
                    sb.AppendLine("             });");
                }
                sb.AppendLine();
            }
            
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
        
            return sb.ToString();
        }
        
        private static string ConvertCodeToCSharp(string code)
        {
            var csharpCode = code;
            
            var replacements1 = new OrderedDictionary
            {
                { @"Math\.randomInt", "RmmzMath.RandomInt" },
                { @"\.meta\.(\w+)", ".Meta.Value(\"$1\")" },
            };
        
            foreach (DictionaryEntry pair in replacements1)
            {
                csharpCode = Regex.Replace(csharpCode, pair.Key as string, pair.Value as string);
            }
        
            csharpCode = AutoCapitalizeProperties(csharpCode);
        
            var replacements2 = new OrderedDictionary
            {
                { @"\bMath\.", "Mathf." },
                { @"===", "==" },
                { @"!==", "!=" },
                { @"\'", "\"" },
                { @"\b(let|const)\s+", "var " },
                { @"\bthis\b", "self" },
                { @"\$data([A-Z]\w*)", "Rmmz.data$1" },
                { @"\$game([A-Z]\w*)", "Rmmz.game$1" },
            };
        
            foreach (DictionaryEntry pair in replacements2)
            {
                csharpCode = Regex.Replace(csharpCode, pair.Key as string, pair.Value as string);
            }
        
            return $"{csharpCode}";
        }
    
        /// <summary>
        /// プロパティ/メソッド名と思われる単語を先頭大文字にする
        /// </summary>
        private static string AutoCapitalizeProperties(string formula)
        {
            // .プロパティ のパターンを検出して先頭大文字化
            var propertyPattern = @"\.\s*([a-z][a-zA-Z0-9_]*)\b";
        
            formula = Regex.Replace(formula, propertyPattern, match =>
            {
                var propertyName = match.Groups[1].Value;
            
                // 先頭を大文字に変換
                var capitalizedProperty = char.ToUpper(propertyName[0]) + propertyName.Substring(1);
            
                return $".{capitalizedProperty}";
            });

            // メソッド呼び出しのパターン object.method( を検出
            var methodPattern = @"\b([a-zA-Z_]\w*)\s*\.\s*([a-z][a-zA-Z0-9_]*)\s*\(";
        
            formula = Regex.Replace(formula, methodPattern, match =>
            {
                var objectName = match.Groups[1].Value;
                var methodName = match.Groups[2].Value;
            
                // 先頭を大文字に変換
                var capitalizedMethod = char.ToUpper(methodName[0]) + methodName.Substring(1);
            
                return $"{objectName}.{capitalizedMethod}(";
            });

            return formula;
        }
    
        private static string EscapeString(string input)
        {
            return input?.Replace("\"", "\"\"") ?? "";
        }
    }
}