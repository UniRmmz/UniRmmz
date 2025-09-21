using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UniRmmz
{
    /// <summary>
    /// The static class that manages the plugins.
    /// </summary>
    public partial class PluginManager
    {
        private static readonly Dictionary<string, Dictionary<string, object>> _parameters = new (); 
        private static readonly Dictionary<string, Action<Game_Interpreter, JObject>> _commands = new ();

        protected PluginManager()
        {
            //Setup(plugins);
        }

        protected void Setup(List<DataPlugin> plugins)
        {
            foreach (var plugin in plugins)
            {
                if (plugin.Status)
                {
                    SetParameters(plugin.Name, plugin.Parameters);
                }
            }
        }

        public static Dictionary<string, object> Parameters(string name)
        {
            var key = name.ToLowerInvariant();
            return _parameters.ContainsKey(key) ? _parameters[key] : new Dictionary<string, object>();
        }

        public static void SetParameters(string name, Dictionary<string, object> parameters)
        {
            var key = name.ToLowerInvariant();
            _parameters[key] = parameters ?? new Dictionary<string, object>();
        }

        public static void RegisterCommand(string pluginName, string commandName, 
            Action<Game_Interpreter, JObject> func)
        {
            var key = $"{pluginName}:{commandName}";
            _commands[key] = func;
            
            Debug.Log($"Registered plugin command: {key}");
        }

        public static void CallCommand(Game_Interpreter self, string pluginName, string commandName, 
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

    }
}