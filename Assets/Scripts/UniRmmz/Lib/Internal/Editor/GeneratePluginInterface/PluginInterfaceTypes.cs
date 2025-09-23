using System;
using System.Collections.Generic;

namespace UniRmmz.Editor
{
    /// <summary>
    /// プラグインパラメータ定義
    /// </summary>
    public class PluginParameter
    {
        public string name;
        public string text;
        public string desc;
        public string type;
        public string defaultValue;
        public List<PluginOption> options = new List<PluginOption>();
        public List<string> tags = new List<string>();
    }

    /// <summary>
    /// プラグイン選択肢定義
    /// </summary>
    public class PluginOption
    {
        public string text;
        public string value;
    }

    /// <summary>
    /// 構造体定義
    /// </summary>
    public class PluginStruct
    {
        public string name;
        public List<PluginParameter> parameters = new List<PluginParameter>();
    }

    /// <summary>
    /// プラグインコマンド定義
    /// </summary>
    public class PluginCommand
    {
        public string name;
        public string text;
        public string desc;
        public List<PluginParameter> arguments = new List<PluginParameter>();
    }

    /// <summary>
    /// プラグイン情報全体
    /// </summary>
    public class PluginInfo
    {
        public string plugindesc;
        public string author;
        public string version;
        
        public List<PluginParameter> parameters = new List<PluginParameter>();
        public List<PluginCommand> commands = new List<PluginCommand>();
        public List<PluginStruct> structs = new List<PluginStruct>();
    }
}