using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

namespace UniRmmz.Editor
{
    public static class GeneratePluginInterface
    {
        private static string testPluginContent;

        [MenuItem("UniRmmz/Tools/GeneratePluginInterface")]
        public static void Execute()
        {
            Rmmz.InitializeUniRmmz(() =>
            {
                foreach (var plugin in Rmmz.PluginManager.UsingPlugins)
                {
                    Execute(plugin);
                }
                AssetDatabase.Refresh();    
            });
        }

        private static void Execute(PluginManager.PluginItem plugin)
        {
            var pluginName = plugin.Name;
            string content = File.ReadAllText(Rmmz.RootPath + $"/js/plugins/{pluginName}.js");
            var result = PluginCommentParser.ParsePluginComments(content);
            var generatedCode = GenerateCSharpCode(pluginName, result);
            
            var pluginRootFolder = Application.streamingAssetsPath + "/../Scripts/UniRmmz/Plugin";
            if (!Directory.Exists(pluginRootFolder))
            {
                Directory.CreateDirectory(pluginRootFolder);
            }
            
            var pluginFolder = pluginRootFolder + $"/{pluginName}";
            if (!Directory.Exists(pluginFolder))
            {
                Directory.CreateDirectory(pluginFolder);
            }

            var outputPath = pluginFolder + $"/{pluginName}.Generated.cs";
            File.WriteAllText(outputPath, generatedCode);
            
            EditorUtility.DisplayDialog("Success",
                $"Code generated successfully!\nSaved to: {outputPath}", "OK");
        }
        
        /// <summary>
        /// 解析結果をC#コードとして生成
        /// </summary>
        public static string GenerateCSharpCode(string pluginName, PluginInfo pluginInfo)
        {
            var code = new System.Text.StringBuilder();
            
            // ヘッダーコメント
            code.AppendLine($"/*");
            code.AppendLine($" * {pluginInfo.plugindesc}");
            code.AppendLine($" * @author {pluginInfo.author}");
            code.AppendLine($" */");
            code.AppendLine();
            code.AppendLine("using System;");
            code.AppendLine("using System.Collections.Generic;");
            code.AppendLine("using UnityEngine;");
            code.AppendLine("using Newtonsoft.Json;");
            code.AppendLine();
            
            // 名前空間とクラス定義
            code.AppendLine($"namespace UniRmmz.Plugin.{pluginName}");
            code.AppendLine("{");
            
            // 構造体を生成
            foreach (var structInfo in pluginInfo.structs)
            {
                GenerateStructCode(code, structInfo, 1);
                code.AppendLine();
            }
            
            // パラメータクラスを生成
            if (pluginInfo.parameters.Count > 0)
            {
                GeneratePluginParameter(code, pluginInfo.parameters, pluginName, 1);
                code.AppendLine();
            }
            
            // メインプラグインクラス
            code.AppendLine($"    public static class {pluginName}Plugin");
            code.AppendLine("    {");
            
            // 初期化メソッド
            code.AppendLine("        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]");
            code.AppendLine("        public static void Initialize()");
            code.AppendLine("        {");
            
            // コマンド登録
            foreach (var command in pluginInfo.commands)
            {
                code.AppendLine($"            PluginManager.RegisterCommand(\"{pluginName}\", \"{command.name}\", {ToCamelCase(command.name)});");
            }
            
            code.AppendLine("        }");
            
            // コマンドメソッド生成
            foreach (var command in pluginInfo.commands)
            {
                code.AppendLine();
                code.AppendLine($"        private static void {ToCamelCase(command.name)}(Dictionary<string, object> args)");
                code.AppendLine("        {");
                code.AppendLine("            // TODO: Implement command logic");
                code.AppendLine("        }");
            }
            
            code.AppendLine("    }");
            code.AppendLine("}");
            
            return code.ToString();
        }

        /// <summary>
        /// 構造体コードを生成
        /// </summary>
        private static void GenerateStructCode(System.Text.StringBuilder code, PluginStruct structInfo, int indent)
        {
            var indentStr = new string(' ', indent * 4);
            
            code.AppendLine($"{indentStr}[Serializable]");
            code.AppendLine($"{indentStr}public class {structInfo.name}");
            code.AppendLine($"{indentStr}{{");
            GenerateParameterItem(code, structInfo.parameters, indentStr);
            code.AppendLine($"{indentStr}}}");
        }

        /// <summary>
        /// プラグインパラメータクラスを生成
        /// </summary>
        private static void GeneratePluginParameter(System.Text.StringBuilder code, List<PluginParameter> parameters, string className, int indent)
        {
            var indentStr = new string(' ', indent * 4);
            
            code.AppendLine($"{indentStr}[Serializable]");
            code.AppendLine($"{indentStr}public class {className}Parameters");
            code.AppendLine($"{indentStr}{{");
            GenerateParameterItem(code, parameters, indentStr);
            code.AppendLine($"{indentStr}}}");
        }

        private static void GenerateParameterItem(StringBuilder code, List<PluginParameter> parameters, string indentStr)
        {
            foreach (var param in parameters)
            {
                var csharpType = ConvertParamType(param.type);
                if (!string.IsNullOrEmpty(param.desc))
                {
                    code.AppendLine($"{indentStr}    /// <summary>{param.desc}</summary>");    
                }
                if (param.options != null && param.options.Count > 0)
                {
                    code.AppendLine($"{indentStr}    /// <remarks>");
                    foreach (var item in param.options)
                    {
                        code.AppendLine($"{indentStr}    /// {item.value} --- {item.text}");    
                    }
                    code.AppendLine($"{indentStr}    /// </remarks>");
                }
                code.AppendLine($"{indentStr}    [JsonProperty(\"{param.name}\")]");
                code.AppendLine($"{indentStr}    public {csharpType} {ToPropertyName(param.name)};");
                code.AppendLine();
            }
        }

        /// <summary>
        /// ツクールのプラグインのパラメータ型をC#型に変換する
        /// </summary>
        private static string ConvertParamType(string pluginParamType)
        {
            if (pluginParamType == null)
            {
                // 指定ない場合は文字列扱い
                return "string";
            }

            bool isArray = false;
            if (pluginParamType.EndsWith("[]"))
            {
                pluginParamType = pluginParamType.Substring(0, pluginParamType.Length - 2);
                isArray = true;
            }
            
            var result = pluginParamType switch
            {
                "number" => "int",
                "string" => "string",
                "boolean" => "bool",
                "multiline_string" => "string",
                "variable" => "int",
                "switch" => "int",
                "common_event" => "int",
                "class" => "int",
                "select" => "string",
                var s when s.StartsWith("struct<") => ExtractStructTypeName(s),
                _ => "object"
            };
            
            return isArray ? result + "[]" : result;
        }

        /// <summary>
        /// 構造体型名を抽出
        /// </summary>
        private static string ExtractStructTypeName(string structType)
        {
            var match = Regex.Match(structType, @"struct<(\w+)>");
            return match.Success ? match.Groups[1].Value : "object";
        }
 
        public static string ToCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return char.ToUpper(str[0]) + str.Substring(1);
        }
        
        public static string ToPropertyName(string input)
        {
            // アンダースコア、ハイフン、スペースで分割
            var parts = input.Split(new char[] { '_', '-', ' ' }).ToList();
        
            // 各部分の先頭を大文字にして結合
            var result = string.Concat(parts.Select(part => 
                char.ToUpper(part[0]) + part.Substring(1)));

            return result;
        }
    }
}