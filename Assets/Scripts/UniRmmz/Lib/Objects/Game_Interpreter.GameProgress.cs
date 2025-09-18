using System;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // スイッチの操作
        private bool Command121(object[] parameters)
        {
            int startId = Convert.ToInt32(parameters[0]);
            int endId = Convert.ToInt32(parameters[1]);
            int value = Convert.ToInt32(parameters[2]);
            for (int i = startId; i <= endId; i++)
            {
                Rmmz.gameSwitches.SetValue(i, value == 0);
            }

            return true;
        }

        // 変数の操作
        public bool Command122(object[] parameters)
        {
            var startId = Convert.ToInt32(parameters[0]);
            var endId = Convert.ToInt32(parameters[1]);
            var operationType = Convert.ToInt32(parameters[2]);
            var operand = Convert.ToInt32(parameters[3]);
            float value = 0;
            var randomMax = 1;

            switch (operand)
            {
                case 0: // Constant
                    value = Convert.ToInt32(parameters[4]);
                    break;
                case 1: // Variable
                    value = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[4]));
                    break;
                case 2: // Random
                    value = Convert.ToInt32(parameters[4]);
                    randomMax = Convert.ToInt32(parameters[5]) - Convert.ToInt32(parameters[4]) + 1;
                    randomMax = Mathf.Max(randomMax, 1);
                    break;
                case 3: // Game Data
                    value = GameDataOperand(
                        Convert.ToInt32(parameters[4]),
                        Convert.ToInt32(parameters[5]),
                        Convert.ToInt32(parameters[6]));
                    break;
                case 4: // Script
                    value = EvaluateScript(parameters[4] as string);
                    break;
            }

            for (var i = startId; i <= endId; i++)
            {
                var realValue = value + RmmzMath.RandomInt(randomMax);
                OperateVariable(i, operationType, realValue);
                /*
                if (typeof value === "number") {
                    const realValue = value + RmmzMath.RandomInt(randomMax);
                    this.operateVariable(i, operationType, realValue);
                } else {
                    this.operateVariable(i, operationType, value);
                }
                */
            }

            return true;
        }

        public virtual float GameDataOperand(int type, int param1, int param2)
        {
            Game_Actor actor;
            //Game_Enemy enemy;
            Game_Character character;

            switch (type)
            {
                case 0: // Item
                    return Rmmz.gameParty.NumItems(Rmmz.dataItems[param1]);
                case 1: // Weapon
                    return Rmmz.gameParty.NumItems(Rmmz.dataWeapons[param1]);
                case 2: // Armor
                    return Rmmz.gameParty.NumItems(Rmmz.dataArmors[param1]);
                case 3: // Actor
                    actor = Rmmz.gameActors.Actor(param1);
                    if (actor != null)
                    {
                        /*
                        switch (param2)
                        {
                            case 0: // Level
                                return actor.Level();
                            case 1: // EXP
                                return actor.CurrentExp();
                            case 2: // HP
                                return actor.Hp();
                            case 3: // MP
                                return actor.Mp();
                            case 12: // TP
                                return actor.Tp();
                            default:
                                // Parameter
                                if (param2 >= 4 && param2 <= 11)
                                {
                                    return actor.Param(param2 - 4);
                                }

                                break;
                        }
                        */
                    }

                    break;
                case 4: // Enemy
                    /*
                    var members = Rmmz.gameTroop.Members();
                    if (param1 < members.Count)
                    {
                        enemy = members[param1];
                        if (enemy != null)
                        {
                            switch (param2)
                            {
                                case 0: // HP
                                    return enemy.Hp();
                                case 1: // MP
                                    return enemy.Mp();
                                case 10: // TP
                                    return enemy.Tp();
                                default:
                                    // Parameter
                                    if (param2 >= 2 && param2 <= 9)
                                    {
                                        return enemy.Param(param2 - 2);
                                    }

                                    break;
                            }
                        }
                    }
                    */
                    break;
                case 5: // Character
                    character = Character(param1);
                    if (character != null)
                    {
                        switch (param2)
                        {
                            case 0: // Map X
                                return character.X;
                            case 1: // Map Y
                                return character.Y;
                            case 2: // Direction
                                return character.Direction();
                            case 3: // Screen X
                                return character.ScreenX();
                            case 4: // Screen Y
                                return character.ScreenY();
                        }
                    }

                    break;
                case 6: // Party
                    var partyMembers = Rmmz.gameParty.Members().ToList();
                    if (param1 < partyMembers.Count)
                    {
                        actor = partyMembers[param1] as Game_Actor;
                        return actor?.ActorId() ?? 0;
                    }

                    break;
                case 8: // Last
                    return Rmmz.gameTemp.LastActionData(param1);
                case 7: // Other
                    switch (param1)
                    {
                        case 0: // Map ID
                            return Rmmz.gameMap.MapId();
                        case 1: // Party Members
                            return Rmmz.gameParty.Size();
                        case 2: // Gold
                            return Rmmz.gameParty.Gold();
                        case 3: // Steps
                            return Rmmz.gameParty.Steps();
                        case 4: // Play Time
                            //return Rmmz.gameSystem.PlayTime();
                        case 5: // Timer
                            return Rmmz.gameTimer.Seconds();
                        case 6: // Save Count
                            return Rmmz.gameSystem.SaveCount;
                        case 7: // Battle Count
                            return Rmmz.gameSystem.BattleCount;
                        case 8: // Win Count
                            return Rmmz.gameSystem.WinCount;
                        case 9: // Escape Count
                            return Rmmz.gameSystem.EscapeCount;
                    }
                    break;
            }
            return 0;
        }

        public virtual void OperateVariable(int variableId, int operationType, float value)
        {
            try
            {
                var oldValue = Rmmz.gameVariables.Value(variableId);

                switch (operationType)
                {
                    case 0: // Set
                        Rmmz.gameVariables.SetValue(variableId, value);
                        break;
                    case 1: // Add
                        Rmmz.gameVariables.SetValue(variableId, oldValue + value);
                        break;
                    case 2: // Sub
                        Rmmz.gameVariables.SetValue(variableId, oldValue - value);
                        break;
                    case 3: // Mul
                        Rmmz.gameVariables.SetValue(variableId, oldValue * value);
                        break;
                    case 4: // Div
                        Rmmz.gameVariables.SetValue(variableId, oldValue / value);
                        break;
                    case 5: // Mod
                        Rmmz.gameVariables.SetValue(variableId, oldValue % value);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in OperateVariable: {e.Message}");
                Rmmz.gameVariables.SetValue(variableId, 0);
            }
        }

        // セルフスイッチ制御
        public bool Command123(object[] parameters)
        {
            if (_eventId > 0)
            {
                var key = new Game_SelfSwitches.Key(_mapId, _eventId, parameters[0] as string);
                Rmmz.gameSelfSwitches.SetValue(key, Convert.ToInt32(parameters[1]) == 0);
            }

            return true;
        }

        // タイマー制御
        public bool Command124(object[] parameters)
        {
            if (Convert.ToInt32(parameters[0]) == 0)
            {
                Rmmz.gameTimer.Start(Convert.ToInt32(parameters[1]) * 60);
            }
            else
            {
                Rmmz.gameTimer.Stop();
            }

            return true;
        }

        private float EvaluateScript(string script)
        {
            Debug.LogWarning($"Script evaluation not implemented: {script}");
            return 0;
        }
    }
}