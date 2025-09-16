using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // テキスト表示
        private bool Command101(object[] parameters)
        {
            if (Rmmz.gameMessage.IsBusy())
            {
                return false;
            }
            Rmmz.gameMessage.SetFaceImage(parameters[0].ToString(), Convert.ToInt32(parameters[1]));
            Rmmz.gameMessage.SetBackground(Convert.ToInt32(parameters[2]));
            Rmmz.gameMessage.SetPositionType(Convert.ToInt32(parameters[3]));
            Rmmz.gameMessage.SetSpeakerName(parameters.ElementAtOrDefault(4)?.ToString());
            
            while (NextEventCode() == 401)
            {
                // Text data
                _index++;
                Rmmz.gameMessage.Add(CurrentCommand().Parameters[0].ToString());
            }
            
            switch (NextEventCode())
            {
                case 102: // Show Choices
                    _index++;
                    SetupChoices(CurrentCommand().Parameters);
                    break;
                case 103: // Input Number
                    _index++;
                    SetupNumInput(CurrentCommand().Parameters);
                    break;
                case 104: // Item Choice
                    _index++;
                    SetupItemChoice(CurrentCommand().Parameters);
                    break;
            }
            
            SetWaitMode("message");
            return true;
        }

        
        // 選択肢の表示
        private bool Command102(object[] parameters)
        {
            if (Rmmz.gameMessage.IsBusy())
            {
                return false;
            }
            SetupChoices(parameters);
            SetWaitMode("message");
            return true;
        }
        
        // 数値入力
        private bool Command103(object[] parameters)
        {
            if (Rmmz.gameMessage.IsBusy())
            {
                return false;
            }
            SetupNumInput(parameters);
            SetWaitMode("message");
            return true;
        }
        
        // アイテム選択
        private bool Command104(object[] parameters)
        {
            if (Rmmz.gameMessage.IsBusy())
            {
                return false;
            }
            SetupItemChoice(parameters);
            SetWaitMode("message");
            return true;
        }
        
        // スクロールテキスト表示
        private bool Command105(object[] parameters)
        {
            if (Rmmz.gameMessage.IsBusy())
            {
                return false;
            }
            Rmmz.gameMessage.SetScroll(Convert.ToInt32(parameters[0]), Convert.ToBoolean(parameters[1]));
            while (NextEventCode() == 405)
            {
                _index++;
                Rmmz.gameMessage.Add(Convert.ToString(CurrentCommand().Parameters[0]));
            }
            SetWaitMode("message");
            return true;
        }
        
        // 選択肢のとき
        private bool Command402(object[] parameters)
        {
            if (!_branch.ContainsKey(_indent) || _branch[_indent] != Convert.ToInt32(parameters[0]))
            {
                SkipBranch();
            }
            return true;
        }
        
        // キャンセルのとき
        private bool Command403(object[] parameters)
        {
            if (!_branch.ContainsKey(_indent) || _branch[_indent] >= 0)
            {
                SkipBranch();
            }
            return true;
        }

        private void SetupChoices(object[] parameters)
        {
            var choices = new List<string>(ConvertEx.ToStringArray(parameters[0]));
            int cancelType = Convert.ToInt32(parameters[1]) < choices.Count ? Convert.ToInt32(parameters[1]) : -2;
            int defaultType = parameters.Length > 2 ? Convert.ToInt32(parameters[2]) : 0;
            int positionType = parameters.Length > 3 ? Convert.ToInt32(parameters[3]) : 2;
            int background = parameters.Length > 4 ? Convert.ToInt32(parameters[4]) : 0;
            
            Rmmz.gameMessage.SetChoices(choices, defaultType, cancelType);
            Rmmz.gameMessage.SetChoiceBackground(background);
            Rmmz.gameMessage.SetChoicePositionType(positionType);
            Rmmz.gameMessage.SetChoiceCallback(n => {
                if (_branch.ContainsKey(_indent))
                {
                    _branch[_indent] = n;
                }
                else
                {
                    _branch.Add(_indent, n);
                }
            });
        }

        private void SetupNumInput(object[] parameters)
        {
            Rmmz.gameMessage.SetNumberInput(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]));
        }
        
        private void SetupItemChoice(object[] parameters)
        {
            int param2 = Convert.ToInt32(parameters.ElementAtOrDefault(1));
            if (param2 == 0)
            {
                param2 = 2;
            }
            Rmmz.gameMessage.SetItemChoice(Convert.ToInt32(parameters[0]), param2);
        }
    }
}