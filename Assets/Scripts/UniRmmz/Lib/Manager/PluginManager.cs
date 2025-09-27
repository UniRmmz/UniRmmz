using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace UniRmmz
{
    /// <summary>
    /// The static class that manages the plugins.
    /// </summary>
    public partial class PluginManager
    {
        public class PluginItem
        {
            public int Order { get; set; }
            public string Name { get; set; }
            public Dictionary<string, string> RawParameter { get; set; }
            public object ParsedParameter { get; set; }
        }
        
        protected readonly Dictionary<string, PluginItem> _plugins = new (); 
        protected readonly Dictionary<string, Action<Game_Interpreter, JObject>> _commands = new ();
        
        public IEnumerable<PluginItem> UsingPlugins => _plugins.Values.OrderBy(plugin => plugin.Order);

        public virtual void Setup(List<DataPlugin> plugins)
        {
            foreach (var plugin in plugins)
            {
                if (plugin.Status)
                {
                    RegisterPlugin(plugin.Name, plugin.Parameters);
                }
            }
        }

        public virtual Dictionary<string, string> Parameters(string name)
        {
            var key = name.ToLowerInvariant();
            return _plugins.GetValueOrDefault(key)?.RawParameter;
        }
        
        public virtual T Parameters<T>(string name) where T : class
        {
            var key = name.ToLowerInvariant();
            var plugin = _plugins.GetValueOrDefault(key);
            if (plugin != null)
            {
                if (plugin.ParsedParameter == null)
                {
                    var json = ConvertToJson(plugin.RawParameter);
                    plugin.ParsedParameter = JsonEx.Parse<T>(json);    
                }

                return plugin.ParsedParameter as T;
            }
            return null;
        }

        protected virtual void RegisterPlugin(string name, Dictionary<string, string> parameters)
        {
            var key = name.ToLowerInvariant();
            var plugin = new PluginItem()
            {
                Order = _plugins.Count,
                Name = name,
                RawParameter = parameters,
                ParsedParameter = null
            };
            _plugins[key] = plugin;
        }

        public virtual void RegisterCommand(string pluginName, string commandName, Action<Game_Interpreter, JObject> func)
        {
            var key = $"{pluginName}:{commandName}";
            _commands[key] = func;
            
            Debug.Log($"Registered plugin command: {key}");
        }

        public virtual void CallCommand(Game_Interpreter self, string pluginName, string commandName, 
            JObject args)
        {
            var key = $"{pluginName}:{commandName}";
            
            if (_commands.ContainsKey(key))
            {
                try
                {
                    var func = _commands[key];
                    func?.Invoke(self, args);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Plugin command execution error [{key}]: {ex.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"Plugin command not found: {key}");
            }
        }
        
        #region UniRmz

        private static string ConvertToJson(Dictionary<string, string> rawParameter)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            foreach ((var key, var value) in rawParameter)
            {
                var tmp = Regex.Unescape(value);
                tmp = tmp.Replace(@"""{", "{").Replace(@"""[", "[").Replace(@"}""", "}").Replace(@"]""", "]");
                sb.AppendLine($"{key} : {tmp},");
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
        
        public static void LoadConfig(Action<DataPlugin[]> onLoaded) 
        {
            string fileName = "plugins.js";
            Debug.Log($"loading {fileName}");
            System.Collections.IEnumerator LoadCoroutine(Action<DataPlugin[]> onLoaded, string fileName)
            {
                string filePath = string.Join("/", Rmmz.RootPath, "js", fileName);
                using (UnityWebRequest uwr = UnityWebRequest.Get(filePath))
                {
                    yield return uwr.SendWebRequest();

                    if (uwr.result == UnityWebRequest.Result.Success)
                    {
                        string json = uwr.downloadHandler.text;
                        json = json.Replace("var $plugins =", "").TrimEnd().TrimEnd(';');// jsonとしてロードできるようにする
                        var data = JsonEx.Parse<DataPlugin[]>(json);
                        onLoaded.Invoke(data);
                    }
                    else
                    {
                        throw new RmmzError($"File load error: {fileName}, {uwr.error}");
                    }
                }
            }

            RmmzRoot.RunCoroutine(LoadCoroutine(onLoaded, fileName));
        }
        
        #endregion // UniRmmz

    }
}