using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // 注釈
        private bool Command108(object[] parameters)
        {
            _comments = new List<string>();
            _comments.Add((string)parameters[0]);
            while (NextEventCode() == 408)
            {
                _indent++;
                _comments.Add((string)CurrentCommand().Parameters[0]);
            }
            return true;
        }

        // スキップ
        private bool Command109(object[] parameters)
        {
            SkipBranch();
            return true;
        }

        // 条件分岐
        private bool Command111(object[] parameters)
        {
            bool result = false;
            int typeId = Convert.ToInt32(parameters[0]);
            
            switch (typeId)
            {
                case 0: // スイッチ
                    result = Rmmz.gameSwitches.Value(Convert.ToInt32(parameters[1])) ==
                             (Convert.ToInt32(parameters[2]) == 0);
                    break;
                case 1: // 変数
                    int value1 = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[1]));
                    int operationType = Convert.ToInt32(parameters[2]);
                    int value2;
                    if (operationType == 0)
                    {
                        value2 = Convert.ToInt32(parameters[3]);
                    }
                    else
                    {
                        value2 = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[3]));
                    }
                    switch (Convert.ToInt32(parameters[4]))
                    {
                        case 0: // 等しい (==)
                            result = value1 == value2;
                            break;
                        case 1: // 以上 (>=)
                            result = value1 >= value2;
                            break;
                        case 2: // 以下 (<=)
                            result = value1 <= value2;
                            break;
                        case 3: // より大きい (>)
                            result = value1 > value2;
                            break;
                        case 4: // より小さい (<)
                            result = value1 < value2;
                            break;
                        case 5: // 異なる (!=)
                            result = value1 != value2;
                            break;
                    }
                    break;
                case 2: // セルフスイッチ
                    var key = new Game_SelfSwitches.Key(_mapId, _eventId, (string)parameters[1]);
                    result = Rmmz.gameSelfSwitches.Value(key) == (Convert.ToInt32(parameters[2]) == 0);
                    break;
                case 3: // タイマー
                    if (Rmmz.gameTimer.IsWorking())
                    {
                        if (Convert.ToInt32(parameters[2]) == 0)
                        {
                            result = Rmmz.gameTimer.Seconds() >= Convert.ToInt32(parameters[1]);
                        }
                        else
                        {
                            result = Rmmz.gameTimer.Seconds() <= Convert.ToInt32(parameters[1]);
                        }
                    }
                    break;
                case 4: // アクター
                    /*
                    Game_Actor actor = Rmmz.gameActors.Actor(Convert.ToInt32(parameters[1]));
                    if (actor != null)
                    {
                        int n = Convert.ToInt32(parameters[2]);
                        switch (n)
                        {
                            case 0: // パーティ内
                                result = Rmmz.gameParty.Members().Contains(actor);
                                break;
                            case 1: // 名前
                                result = actor.Name() == parameters[3].ToString();
                                break;
                            case 2: // スキル
                                result = actor.HasSkill(Convert.ToInt32(parameters[3]));
                                break;
                            case 3: // 武器
                                result = actor.HasWeapon(Rmmz.dataWeapons[Convert.ToInt32(parameters[3])]);
                                break;
                            case 4: // 防具
                                result = actor.HasArmor(Rmmz.dataArmors[Convert.ToInt32(parameters[3])]);
                                break;
                            case 5: // ステート
                                result = actor.IsStateAffected(Convert.ToInt32(parameters[3]));
                                break;
                        }
                    }
                    */
                    break;
                case 5: // 敵キャラ
                    /*
                    Game_Enemy enemy = null;
                    int enemyIndex = Convert.ToInt32(parameters[1]);
                    if (enemyIndex < Rmmz.gameTroop.Members().Count)
                    {
                        enemy = Rmmz.gameTroop.Members()[enemyIndex];
                    }
                    if (enemy != null)
                    {
                        int n = Convert.ToInt32(parameters[2]);
                        switch (n)
                        {
                            case 0: // 出現
                                result = enemy.IsAlive();
                                break;
                            case 1: // ステート
                                result = enemy.IsStateAffected(Convert.ToInt32(parameters[3]));
                                break;
                        }
                    }
                    */
                    break;
                case 6: // キャラクター
                    /*
                    Game_Character character = Character(Convert.ToInt32(parameters[1]));
                    if (character != null)
                    {
                        int n = Convert.ToInt32(parameters[2]);
                        switch (n)
                        {
                            case 0: // 方向
                                result = character.Direction() == Convert.ToInt32(parameters[3]);
                                break;
                            case 1: // 特定の位置
                                result = character.X == Convert.ToInt32(parameters[3]) &&
                                        character.Y == Convert.ToInt32(parameters[4]);
                                break;
                        }
                    }
                    */
                    break;
                case 7: // お金
                    int gold = Rmmz.gameParty.Gold();
                    int cmpGold = Convert.ToInt32(parameters[1]);
                    switch (Convert.ToInt32(parameters[2]))
                    {
                        case 0:// Greater than or equal to
                            result = gold >= cmpGold;
                            break;
                        
                        case 1:// Less than or equal to
                            result = gold <= cmpGold;
                            break;
                        
                        case 2:// Less than
                            result = gold < cmpGold;
                            break;
                    }
                    break;
                case 8: // アイテム
                    result = Rmmz.gameParty.HasItem(Rmmz.dataItems[Convert.ToInt32(parameters[1])]);
                    break;
                case 9: // 武器
                    result = Rmmz.gameParty.HasItem(
                        Rmmz.dataWeapons[Convert.ToInt32(parameters[1])],
                        Convert.ToInt32(parameters[2]) != 0);
                    break;
                case 10: // 防具
                    result = Rmmz.gameParty.HasItem(
                        Rmmz.dataArmors[Convert.ToInt32(parameters[1])],
                        Convert.ToInt32(parameters[2]) != 0);
                    break;
                case 11: // ボタン
                    result = Input.IsPressed(parameters[1].ToString());
                    break;
                case 12: // スクリプト
                    //result = ExecuteScript(parameters[1].ToString());
                    break;
                case 13: // 乗り物
                    var vehicleType = (Game_Vehicle.VehicleTypes)Convert.ToInt32(parameters[1]);
                    result = Rmmz.gamePlayer.Vehicle() == Rmmz.gameMap.Vehicle(vehicleType);
                    break;
            }

            _branch[_indent] = result ? 1 : 0;
            if (_branch[_indent] == 0)
            {
                SkipBranch();
            }
            
            return true;
        }
        
        // それ以外のとき
        private bool Command411(object[] parameters)
        {
            if (_branch.GetValueOrDefault(_indent) > 0)
            {
                SkipBranch();
            }
            return true;
        }

        // ループ
        private bool Command112(object[] parameters)
        {
            return true;
        }
        
        // 以上繰り返し
        private bool Command413(object[] parameters)
        {
            do
            {
                _index--;
            } while (CurrentCommand().Indent != _indent);
            return true;
        } 

        // ループの中断
        private bool Command113(object[] parameters)
        {
            int depth = 0;
            while (_index < _list.Count - 1)
            {
                _index++;
                var command = CurrentCommand();
                if (command.Code == 112)
                {
                    depth++;
                }
                if (command.Code == 413)
                {
                    if (depth > 0)
                    {
                        depth--;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return true;
        }

        // イベント処理の中断
        private bool Command115(object[] parameters)
        {
            _index = _list.Count;
            return true;
        }

        // コモンイベント
        private bool Command117(object[] parameters)
        {
            int commonEventId = Convert.ToInt32(parameters[0]);
            var commonEvent = Rmmz.dataCommonEvents[commonEventId];
            if (commonEvent != null)
            {
                var eventId = IsOnCurrentMap() ? _eventId : 0;
                _childInterpreter = Game_Interpreter.Create(_depth + 1);
                _childInterpreter.Setup(commonEvent.List, eventId);
                return true;
            }
            return false;
        }

        // ラベル
        private bool Command118(object[] parameters)
        {
            return true;
        }

        // ラベルジャンプ
        private bool Command119(object[] parameters)
        {
            string labelName = parameters[0].ToString();
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].Code == 118 && _list[i].Parameters[0].ToString() == labelName)
                {
                    JumpTo(i);
                    break;
                }
            }
            return true;
        }

        private void JumpTo(int index)
        {
            int lastIndex = _index;
            int startIndex = Math.Min(index, lastIndex);
            int endIndex = Math.Max(index, lastIndex);
            int indent = _indent;
            for (int i = startIndex; i <= endIndex; i++)
            {
                int newIndent = _list[i].Indent;
                if (newIndent != indent)
                {
                    _branch[indent] = 0;
                    indent = newIndent;
                }
            }
            _index = index;
        }

    }
}