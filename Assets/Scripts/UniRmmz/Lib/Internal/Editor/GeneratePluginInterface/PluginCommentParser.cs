using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UniRmmz.Editor
{

    /// <summary>
    /// RPGツクールMZプラグインのコメント部分をパースするクラス
    /// </summary>
    public static class PluginCommentParser
    {
        /// <summary>
        /// プラグインファイルからコメント部分を解析
        /// </summary>
        /// <param name="pluginContent">プラグインファイルの内容</param>
        /// <returns>解析されたプラグイン情報</returns>
        public static PluginInfo ParsePluginComments(string pluginContent)
        {
            var pluginInfo = new PluginInfo();
            
            // コメントブロックを抽出
            var commentBlocks = ExtractCommentBlocks(pluginContent);
            
            foreach (var block in commentBlocks)
            {
                if (IsMainPluginComment(block))
                {
                    ParseMainPluginComment(block, pluginInfo);
                }
                else if (IsStructComment(block))
                {
                    var structInfo = ParseStructComment(block);
                    if (structInfo != null && pluginInfo.structs.All((v) => v.name != structInfo.name))
                    {
                        pluginInfo.structs.Add(structInfo);
                    }
                }
            }
            
            return pluginInfo;
        }

        /// <summary>
        /// コメントブロックを抽出
        /// </summary>
        /// <remarks>
        /// /*:
        /// * @target MZ
        /// * @plugindesc XXXX Plugin
        /// * @author XXX
        /// * @base XXX
        /// * @orderAfter XXX
        /// *
        /// * @param XXX
        /// * @text XXX
        /// * @desc XXX
        /// * @default XXX
        /// * @type XXX
        /// *
        /// * @help XXX.js
        /// * 
        /// * comment
        /// */
        /// 
        /// のような文字列をひとつのコメントブロックとして、
        /// 
        /// @target MZ
        /// @plugindesc XXXX Plugin
        /// @author XXX
        /// @base XXX
        /// @orderAfter XXX
        /// @param XXX
        /// @text XXX
        /// @desc XXX
        /// @default XXX
        /// @type XXX
        /// @help XXX.js
        /// 
        /// のように抽出する
        /// </remarks>
        private static List<string> ExtractCommentBlocks(string content)
        {
            var matches1 = Regex.Matches(content, @"/\*:(.*?)\*/", RegexOptions.Singleline);
            var matches2 = Regex.Matches(content, @"/\*(~struct~\w+:.*?)\*/", RegexOptions.Singleline);
            var blocks = new List<string>();
    
            foreach (Match match in matches1.Concat(matches2))
            {
                var commentContent = match.Groups[1].Value;
                var lines = commentContent.Split(System.Environment.NewLine);
                var tmp = new List<string>();
        
                foreach (var line in lines)
                {
                    if (line.StartsWith("~struct~"))
                    {
                        tmp.Add(line);
                    }
                    else
                    {
                        var tagMatch = Regex.Match(line, @"\s*\*\s*(@\w+.*?)$");
                        if (tagMatch.Success)
                        {
                            tmp.Add(tagMatch.Groups[1].Value.Trim());
                        }    
                    }
                }

                blocks.Add(string.Join(Environment.NewLine, tmp));
            }
    
            return blocks;
        }

        /// <summary>
        /// メインプラグインコメントかどうか判定
        /// </summary>
        private static bool IsMainPluginComment(string comment)
        {
            return comment.Contains("@target") || comment.Contains("@plugindesc");
        }

        /// <summary>
        /// 構造体コメントかどうか判定
        /// </summary>
        private static bool IsStructComment(string comment)
        {
            return comment.StartsWith("~struct~");
        }

        /// <summary>
        /// メインプラグインコメントを解析
        /// </summary>
        private static void ParseMainPluginComment(string comment, PluginInfo pluginInfo)
        {
            var lines = comment.Split(System.Environment.NewLine);
            
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("@plugindesc"))
                {
                    pluginInfo.plugindesc = ExtractValue(line);
                }
                else if (line.StartsWith("@author"))
                {
                    pluginInfo.author = ExtractValue(line);
                }
                else if (line.StartsWith("@version"))
                {
                    pluginInfo.version = ExtractValue(line);
                }
                else if (line.StartsWith("@param"))
                {
                    var param = ParseParameter(lines, ref i);
                    if (param != null && pluginInfo.parameters.All((v) => v.name != param.name))
                    {
                        pluginInfo.parameters.Add(param);
                    }
                }
                else if (line.StartsWith("@command"))
                {
                    var command = ParseCommand(lines, ref i);
                    if (command != null && pluginInfo.commands.All((v) => v.name != command.name))
                    {
                        pluginInfo.commands.Add(command);
                    }
                }
            }
        }

        /// <summary>
        /// 構造体コメントを解析
        /// </summary>
        private static PluginStruct ParseStructComment(string comment)
        {
            var lines = comment.Split('\n').Select(l => l.Trim()).ToArray();
            
            if (lines.Length == 0) return null;
            
            var structInfo = new PluginStruct();
            
            // 構造体名を抽出
            var firstLine = lines[0];
            var structMatch = Regex.Match(firstLine, @"~struct~(\w+):");
            if (structMatch.Success)
            {
                structInfo.name = structMatch.Groups[1].Value;
            }
            
            // パラメータを解析
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.StartsWith("@param"))
                {
                    var param = ParseParameter(lines, ref i);
                    if (param != null)
                    {
                        structInfo.parameters.Add(param);
                    }
                }
            }
            
            return structInfo;
        }

        /// <summary>
        /// パラメータを解析
        /// </summary>
        private static PluginParameter ParseParameter(string[] lines, ref int index)
        {
            var param = new PluginParameter();
            var line = lines[index];
            
            // パラメータ名を抽出
            param.name = ExtractValue(line);
            
            // 続く行を解析
            index++;
            while (index < lines.Length)
            {
                line = lines[index];
                
                if (line.StartsWith("@text"))
                {
                    param.text = ExtractValue(line);
                }
                else if (line.StartsWith("@desc"))
                {
                    param.desc = ExtractValue(line);
                }
                else if (line.StartsWith("@type"))
                {
                    param.type = ExtractValue(line);
                }
                else if (line.StartsWith("@default"))
                {
                    param.defaultValue = ExtractValue(line);
                }
                else if (line.StartsWith("@option"))
                {
                    var option = new PluginOption();
                    option.text = ExtractValue(line);
                    
                    // 次の行が@valueかチェック
                    if (index + 1 < lines.Length && lines[index + 1].Trim().StartsWith("@value"))
                    {
                        index++;
                        option.value = ExtractValue(lines[index]);
                    }
                    
                    param.options.Add(option);
                }
                else if (line.StartsWith("@") && !line.StartsWith("@value"))
                {
                    // 別のパラメータの開始
                    index--;
                    break;
                }
                
                index++;
            }
            
            return param;
        }

        /// <summary>
        /// コマンドを解析
        /// </summary>
        private static PluginCommand ParseCommand(string[] lines, ref int index)
        {
            var command = new PluginCommand();
            var line = lines[index];
            
            // コマンド名を抽出
            command.name = ExtractValue(line);
            
            // 続く行を解析
            index++;
            while (index < lines.Length)
            {
                line = lines[index];
                
                if (line.StartsWith("@text"))
                {
                    command.text = ExtractValue(line);
                }
                else if (line.StartsWith("@desc"))
                {
                    command.desc = ExtractValue(line);
                }
                else if (line.StartsWith("@arg"))
                {
                    var arg = ParseParameter(lines, ref index);
                    if (arg != null)
                    {
                        command.arguments.Add(arg);
                    }
                    continue; // ParseParameterでindexが進むので continue
                }
                else if (line.StartsWith("@") && !IsCommandRelatedTag(line))
                {
                    // 別のセクションの開始
                    index--;
                    break;
                }
                
                index++;
            }
            
            return command;
        }

        /// <summary>
        /// 値を抽出（@key value の value 部分）
        /// </summary>
        private static string ExtractValue(string line)
        {
            var match = Regex.Match(line, @"@\w+\s+(.*)");
            return match.Success ? match.Groups[1].Value.Trim() : "";
        }

        /// <summary>
        /// コマンド関連のタグかどうか判定
        /// </summary>
        private static bool IsCommandRelatedTag(string line)
        {
            return line.StartsWith("@text") || 
                   line.StartsWith("@desc") || 
                   line.StartsWith("@arg");
        }
    }

    /// <summary>
    /// 文字列拡張メソッド
    /// </summary>
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return char.ToUpper(str[0]) + str.Substring(1);
        }
    }
}