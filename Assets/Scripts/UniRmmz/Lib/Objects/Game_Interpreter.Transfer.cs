using System;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        private bool Command201(object[] parameters)
        {
            if (Rmmz.gameParty.InBattle() || Rmmz.gameMessage.IsBusy())
            {
                return false;
            }
    
            int mapId, x, y;
            if (Convert.ToInt32(parameters[0]) == 0)
            {
                // Direct designation
                mapId = Convert.ToInt32(parameters[1]);
                x = Convert.ToInt32(parameters[2]);
                y = Convert.ToInt32(parameters[3]);
            }
            else
            {
                // Designation with variables
                mapId = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[1]));
                x = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[2]));
                y = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[3]));
            }
    
            Rmmz.gamePlayer.ReserveTransfer(mapId, x, y, Convert.ToInt32(parameters[4]), Convert.ToInt32(parameters[5]));
            SetWaitMode("transfer");
            return true;
        }

        // 乗り物の位置設定
        private bool Command202(object[] parameters)
        {
            int mapId, x, y;
            if (Convert.ToInt32(parameters[1]) == 0)
            {
                // Direct designation
                mapId = Convert.ToInt32(parameters[2]);
                x = Convert.ToInt32(parameters[3]);
                y = Convert.ToInt32(parameters[4]);
            }
            else
            {
                // Designation with variables
                mapId = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[2]));
                x = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[3]));
                y = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[4]));
            }

            var vehicleType = (Game_Vehicle.VehicleTypes)Convert.ToInt32(parameters[0]);
            var vehicle = Rmmz.gameMap.Vehicle(vehicleType);
            if (vehicle != null)
            {
                vehicle.SetLocation(mapId, x, y);
            }
            return true;
        }

        private bool Command203(object[] parameters)
        {
            var character = Character(Convert.ToInt32(parameters[0]));
            if (character != null)
            {
                var mode = Convert.ToInt32(parameters[1]); 
                if (mode == 0)
                {
                    // Direct designation
                    character.Locate(Convert.ToInt32(parameters[2]), Convert.ToInt32(parameters[3]));
                }
                else if (mode == 1)
                {
                    // Designation with variables
                    var x = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[2]));
                    var y = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[3]));
                    character.Locate(x, y);
                }
                else
                {
                    // Exchange with another event
                    var character2 = Character(Convert.ToInt32(parameters[2]));
                    if (character2 != null)
                    {
                        character.Swap(character2);
                    }
                }
        
                var direction = Convert.ToInt32(parameters[4]);
                if (direction > 0)
                {
                    character.SetDirection(direction);
                }
            }
            return true;
        }

        // マップスクロール
        private bool Command204(object[] parameters)
        {
            if (!Rmmz.gameParty.InBattle())
            {
                if (Rmmz.gameMap.IsScrolling())
                {
                    SetWaitMode("scroll");
                    return false;
                }

                var direction = Convert.ToInt32(parameters[0]);
                var distance = Convert.ToInt32(parameters[1]);
                var speed = Convert.ToInt32(parameters[2]);
                Rmmz.gameMap.StartScroll(direction, distance, speed);
        
                if (Convert.ToInt32(parameters[3]) != 0)
                {
                    SetWaitMode("scroll");
                }
            }
            return true;
        }
        
        // 移動ルートの設定
        public bool Command205(object[] parameters)
        {
            Rmmz.gameMap.RefreshIfNeeded();
            _characterId = Convert.ToInt32(parameters[0]);
            var character = Character(_characterId);
            if (character != null)
            {
                var moveRoute = ConvertEx.ToMoveRoute(parameters[1]);
                character.ForceMoveRoute(moveRoute);
                if (moveRoute.Wait)
                {
                    SetWaitMode("route");
                }
            }
            return true;
        }
        
        // 乗り物の乗降
        public bool Command206(object[] parameters)
        {
            Rmmz.gamePlayer.GetOnOffVehicle();
            return true;
        }
    }
}