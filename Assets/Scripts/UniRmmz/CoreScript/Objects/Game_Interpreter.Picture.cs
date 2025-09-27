using System;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // ピクチャの表示
        private bool Command231(object[] parameters)
        {
            var point = PicturePoint(parameters);
            Rmmz.gameScreen.ShowPicture(
               Convert.ToInt32(parameters[0]), Convert.ToString(parameters[1]), Convert.ToInt32(parameters[2]),
               point.x, point.y, Convert.ToSingle(parameters[6]), Convert.ToSingle(parameters[7]),
               Convert.ToInt32(parameters[8]), Convert.ToInt32(parameters[9]));
            return true;
        }
        
        // ピクチャの移動
        private bool Command232(object[] parameters)
        {
            var point = PicturePoint(parameters);
            Rmmz.gameScreen.MovePicture(
                Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[2]),
                point.x, point.y, Convert.ToSingle(parameters[6]), Convert.ToSingle(parameters[7]),
                Convert.ToInt32(parameters[8]), Convert.ToInt32(parameters[9]),
                Convert.ToInt32(parameters[10]), Convert.ToInt32(parameters.ElementAtOrDefault(12)));
            if (Convert.ToBoolean(parameters[11]))
            {
                Wait(Convert.ToInt32(parameters[10]));
            }
            return true;
        }

        private Vector2 PicturePoint(object[] parameters)
        {
            Vector2 point;
            if (Convert.ToInt32(parameters[3]) == 0)
            {
                // Direct designation
                point.x = Convert.ToInt32(parameters[4]);
                point.y = Convert.ToInt32(parameters[5]);
            }
            else
            {
                // Designation with variables
                point.x = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[4]));
                point.y = Rmmz.gameVariables.Value(Convert.ToInt32(parameters[5]));
            }

            return point;
        }
        
        // ピクチャの回転
        private bool Command233(object[] parameters)
        {
            Rmmz.gameScreen.RotatePicture(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]));
            return true;
        }
        
        // ピクチャの色調変更
        private bool Command234(object[] parameters)
        {
            Rmmz.gameScreen.TintPicture(Convert.ToInt32(parameters[0]), 
                ConvertEx.ToVector4(parameters[1]),
                Convert.ToInt32(parameters[2]));
            if (Convert.ToBoolean(parameters[3]))
            {
                Wait(Convert.ToInt32(parameters[2]));
            }
            return true;
        }
        
        // ピクチャの消去
        private bool Command235(object[] parameters)
        {
            Rmmz.gameScreen.ErasePicture(Convert.ToInt32(parameters[0]));
            return true;
        }
    }
}