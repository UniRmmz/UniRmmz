using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniRmmz.Plugin.RegionBase;
using UnityEngine.Networking;

namespace UniRmmz
{
    /// <summary>
    /// The static class that manages the plugins.
    /// </summary>
    public partial class PluginManager
    {
        private readonly Dictionary<string, Dictionary<string, string>> _parameters = new (); 
        private readonly Dictionary<string, Action<Game_Interpreter, JObject>> _commands = new ();

        public virtual void Setup(List<DataPlugin> plugins)
        {
            foreach (var plugin in plugins)
            {
                if (plugin.Status)
                {
                    SetParameters(plugin.Name, plugin.Parameters);
                }
            }
        }

        public  Dictionary<string, string> Parameters(string name)
        {
            var key = name.ToLowerInvariant();
            return _parameters.ContainsKey(key) ? _parameters[key] : new Dictionary<string, string>();
        }
        
        public T Parameters<T>(string name) where T : class
        {
            var pluginKey = name.ToLowerInvariant();
            if (_parameters.TryGetValue(pluginKey, out var param))
            {
                var sb = new StringBuilder();
                sb.AppendLine("{");
                foreach ((var key, var value) in param)
                {
                    var tmp = Regex.Unescape(value);
                    tmp = tmp.Replace(@"""{", "{").Replace(@"""[", "[").Replace(@"}""", "}").Replace(@"]""", "]");
                    sb.AppendLine($"{key} : {tmp},");
                }
                sb.AppendLine("}");
                var json = sb.ToString();
                return JsonEx.Parse<T>(json);
            }
            return null;
        }

        public void SetParameters(string name, Dictionary<string, string> parameters)
        {
            var key = name.ToLowerInvariant();
            _parameters[key] = parameters ?? new Dictionary<string, string>();
        }

        public void RegisterCommand(string pluginName, string commandName, 
            Action<Game_Interpreter, JObject> func)
        {
            var key = $"{pluginName}:{commandName}";
            _commands[key] = func;
            
            Debug.Log($"Registered plugin command: {key}");
        }

        public void CallCommand(Game_Interpreter self, string pluginName, string commandName, 
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

    }
}