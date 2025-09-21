using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // スクリプト
        public bool Command355(object[] parameters)
        {
            var script = new RmmzJavascriptCode();
            script.AddLine(Convert.ToString(CurrentCommand().Parameters[0]));
            while (NextEventCode() == 655)
            {
                _index++;
                script.AddLine(Convert.ToString(CurrentCommand().Parameters[0]));
            }
            RmmzScriptCommand.ExecuteScriptCommand(script, this);
            return true;
        }
        
        // プラグインコマンド（MV）
        public bool Command356(object[] parameters)
        {
            var args = Convert.ToString(parameters[0]).Split(" ").ToList();
            var command = args[0];
            args.RemoveAt(0);
            PluginCommand(command, args.ToArray());
            return true;
        }

        public virtual void PluginCommand(string command, string[] args)
        {
            // deprecated
        }

        // プラグインコマンド
        public bool Command357(object[] parameters)
        {
            string pluginName = Utils.ExtractFileName(parameters[0].ToString());
            PluginManager.CallCommand(this, pluginName, parameters[1].ToString(), parameters[3] as JObject);
            return true;
        }
    }
}