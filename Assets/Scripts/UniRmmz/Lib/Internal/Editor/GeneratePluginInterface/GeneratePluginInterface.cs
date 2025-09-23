using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace UniRmmz.Editor
{
    public static class GeneratePluginInterface
    {
        private static string testPluginContent;

        [MenuItem("UniRmmz/Tools/GeneratePluginInterface")]
        public static void Execute()
        {
            Rmmz.InitializeManager();
            //PluginManager.Create().LoadAndSetup();
            string content = File.ReadAllText(Rmmz.RootPath + "/js/plugins/RegionBase.js");
            var result = PluginCommentParser.ParsePluginComments(content);
            var generatedCode = GenerateCSharpCode("RegionBase", result);
            
            var pluginRootFolder = Application.streamingAssetsPath + "/../Scripts/UniRmmz/Plugin";
            if (!Directory.Exists(pluginRootFolder))
            {
                Directory.CreateDirectory(pluginRootFolder);
            }
            
            var pluginFolder = pluginRootFolder + "/RegionBase";
            if (!Directory.Exists(pluginFolder))
            {
                Directory.CreateDirectory(pluginFolder);
            }

            var outputPath = pluginFolder + "/RegionBase.Generated.cs";
            File.WriteAllText(outputPath, generatedCode);
            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// 解析結果をC#コードとして生成
        /// </summary>
        public static string GenerateCSharpCode(string className, PluginInfo pluginInfo)
        {
            var code = new System.Text.StringBuilder();
            
            // ヘッダーコメント
            code.AppendLine($"/*:");
            code.AppendLine($" * {pluginInfo.plugindesc}");
            code.AppendLine($" * @author {pluginInfo.author}");
            code.AppendLine($" */");
            code.AppendLine();
            code.AppendLine("using System;");
            code.AppendLine("using System.Collections.Generic;");
            code.AppendLine("using UnityEngine;");
            code.AppendLine();
            
            // 名前空間とクラス定義
            code.AppendLine($"namespace UniRmmz.Plugin.{className}");
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
                GenerateParametersClass(code, pluginInfo.parameters, className, 1);
                code.AppendLine();
            }
            
            // メインプラグインクラス
            code.AppendLine($"    public static class {className}Plugin");
            code.AppendLine("    {");
            
            // 初期化メソッド
            code.AppendLine("        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]");
            code.AppendLine("        public static void Initialize()");
            code.AppendLine("        {");
            
            // コマンド登録
            foreach (var command in pluginInfo.commands)
            {
                code.AppendLine($"            PluginManager.RegisterCommand(\"{className}\", \"{command.name}\", {command.name.ToCamelCase()});");
            }
            
            code.AppendLine("        }");
            
            // コマンドメソッド生成
            foreach (var command in pluginInfo.commands)
            {
                code.AppendLine();
                code.AppendLine($"        private static void {command.name.ToCamelCase()}(Dictionary<string, object> args)");
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
            
            foreach (var param in structInfo.parameters)
            {
                var csharpType = ConvertParamType(param.type);
                code.AppendLine($"{indentStr}    public {csharpType} {param.name};");
            }
            
            code.AppendLine($"{indentStr}}}");
        }

        /// <summary>
        /// パラメータクラスを生成
        /// </summary>
        private static void GenerateParametersClass(System.Text.StringBuilder code, List<PluginParameter> parameters, string className, int indent)
        {
            var indentStr = new string(' ', indent * 4);
            
            code.AppendLine($"{indentStr}[Serializable]");
            code.AppendLine($"{indentStr}public class {className}Parameters");
            code.AppendLine($"{indentStr}{{");
            
            foreach (var param in parameters)
            {
                var csharpType = ConvertParamType(param.type);
                code.AppendLine($"{indentStr}    public {csharpType} {param.name};");
            }
            
            code.AppendLine($"{indentStr}}}");
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
            
            return pluginParamType switch
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
                var s when s.EndsWith("[]") => $"List<{ConvertParamType(s.Substring(0, s.Length - 2))}>",
                _ => "object"
            };
        }

        /// <summary>
        /// 構造体型名を抽出
        /// </summary>
        private static string ExtractStructTypeName(string structType)
        {
            var match = Regex.Match(structType, @"struct<(\w+)>");
            return match.Success ? match.Groups[1].Value : "object";
        }
 
    }
}