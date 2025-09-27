using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // 画面のフェードアウト
        protected virtual bool Command221(object[] _)
        {
            if (Rmmz.gameMessage.IsBusy())
            {
                return false;
            }
            Rmmz.gameScreen.StartFadeOut(FadeSpeed());
            Wait(FadeSpeed());
            return true;
        }
        
        // 画面のフェードイン
        protected virtual bool Command222(object[] _)
        {
            if (Rmmz.gameMessage.IsBusy())
            {
                return false;
            }
            Rmmz.gameScreen.StartFadeIn(FadeSpeed());
            Wait(FadeSpeed());
            return true;
        }
        
        // 画面の色調変更
        protected virtual bool Command223(object[] parameters)
        {
            Rmmz.gameScreen.StartTint(ConvertEx.ToVector4(parameters[0]), Convert.ToInt32(parameters[1]));
            if (Convert.ToBoolean(parameters[2]))
            {
                Wait(Convert.ToInt32(parameters[1]));
            }
            return true;
        }
        
        // 画面のフラッシュ
        protected virtual bool Command224(object[] parameters)
        {
            var tmp = ConvertEx.ToArray<float>(parameters[0]);
            var color = new Color32((byte)tmp[0], (byte)tmp[1], (byte)tmp[2], (byte)tmp[3]);
            Rmmz.gameScreen.StartFlash(color, Convert.ToInt32(parameters[1]));
            if (Convert.ToBoolean(parameters[2]))
            {
                Wait(Convert.ToInt32(parameters[1]));
            }
            return true;
        }
        
        // 画面のシェイク
        protected virtual bool Command225(object[] parameters)
        {
            Rmmz.gameScreen.StartShake(Convert.ToInt32(parameters[0]), Convert.ToInt32(parameters[1]), Convert.ToInt32(parameters[2]));
            if (Convert.ToBoolean(parameters[3]))
            {
                Wait(Convert.ToInt32(parameters[2]));
            }
            return true;
        }
        
        // 天候の設定
        protected virtual bool Command236(object[] parameters)
        {
            if (!Rmmz.gameParty.InBattle())
            {
                var type = Convert.ToString(parameters[0]) switch
                {
                    "rain" => Weather.WeatherTypes.Rain,
                    "storm" => Weather.WeatherTypes.Storm,
                    "snow" => Weather.WeatherTypes.Snow,
                    _ => Weather.WeatherTypes.None
                };
                Rmmz.gameScreen.ChangeWeather(type, Convert.ToSingle(parameters[1]), Convert.ToInt32(parameters[2]));
                if (Convert.ToBoolean(parameters[3]))
                {
                    Wait(Convert.ToInt32(parameters[2]));
                }
            }
            return true;
        }
    }
}