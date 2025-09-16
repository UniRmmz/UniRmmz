using System;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // 所持金の増減
        private bool Command125(object[] parameters)
        {
            int operation = Convert.ToInt32(parameters[0]);
            int operandType = Convert.ToInt32(parameters[1]);
            int operand = Convert.ToInt32(parameters[2]);
            int value = OperateValue(operation, operandType, operand);
            Rmmz.gameParty.GainGold(value);
            return true;
        }
        
        // アイテムの増減
        private bool Command126(object[] parameters)
        {
            int itemId = Convert.ToInt32(parameters[0]);
            int operation = Convert.ToInt32(parameters[1]);
            int operandType = Convert.ToInt32(parameters[2]);
            int operand = Convert.ToInt32(parameters[3]);
            int value = OperateValue(operation, operandType, operand);
            Rmmz.gameParty.GainItem(Rmmz.dataItems[itemId], value);
            return true;
        }
        
        // 武器の増減
        private bool Command127(object[] parameters)
        {
            int weaponId = Convert.ToInt32(parameters[0]);
            int operation = Convert.ToInt32(parameters[1]);
            int operandType = Convert.ToInt32(parameters[2]);
            int operand = Convert.ToInt32(parameters[3]);
            bool includeEquip = Convert.ToInt32(parameters[4]) != 0;
            int value = OperateValue(operation, operandType, operand);
            Rmmz.gameParty.GainItem(Rmmz.dataWeapons[weaponId], value, includeEquip);
            return true;
        }
        
        // 防具の増減
        private bool Command128(object[] parameters)
        {
            int armorId = Convert.ToInt32(parameters[0]);
            int operation = Convert.ToInt32(parameters[1]);
            int operandType = Convert.ToInt32(parameters[2]);
            int operand = Convert.ToInt32(parameters[3]);
            bool includeEquip = Convert.ToInt32(parameters[4]) != 0;
            int value = OperateValue(operation, operandType, operand);
            Rmmz.gameParty.GainItem(Rmmz.dataArmors[armorId], value, includeEquip);
            return true;
        }
        
        // メンバーの入れ替え
        public bool Command129(object[] parameters)
        {
            int actorId = Convert.ToInt32(parameters[0]);
            var actor = Rmmz.gameActors.Actor(actorId);
            if (actor != null)
            {
                if (Convert.ToInt32(parameters[1]) == 0)
                {
                    // Add
                    if (Convert.ToInt32(parameters[2]) != 0)
                    {
                        // Initialize
                        Rmmz.gameActors.Actor(actorId).Setup(actorId);
                    }
                    Rmmz.gameParty.AddActor(actorId);
                }
                else
                {
                    // Remove
                    Rmmz.gameParty.RemoveActor(actorId);
                }
            }
            return true;
        }
    }
}