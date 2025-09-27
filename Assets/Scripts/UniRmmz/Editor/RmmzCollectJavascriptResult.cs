using System.Collections.Generic;
using System.Linq;

namespace UniRmmz.Editor
{
    public class RmmzCollectJavascriptResult
    {
        public List<RmmzJavascriptCode> ScriptCommands = new List<RmmzJavascriptCode>();
        public List<RmmzJavascriptCode> ConditionCommands = new List<RmmzJavascriptCode>();
        public List<RmmzJavascriptCode> OperateVariableCommands = new List<RmmzJavascriptCode>();
        public List<RmmzJavascriptCode> CharacterMoveRouteCommands = new List<RmmzJavascriptCode>();

        public void Distinct()
        {
            ScriptCommands = ScriptCommands.Distinct().ToList();
            ConditionCommands = ConditionCommands.Distinct().ToList();
            OperateVariableCommands = OperateVariableCommands.Distinct().ToList();
            CharacterMoveRouteCommands = CharacterMoveRouteCommands.Distinct().ToList();
        }
    }
}