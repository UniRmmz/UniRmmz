using System;
using System.Linq;

namespace UniRmmz
{
    public partial class Game_Interpreter
    {
        // 透明状態の変更
        protected virtual bool Command211(object[] parameters)
        {
            bool isTransparent = Convert.ToInt32(parameters[0]) == 0;
            Rmmz.gamePlayer.SetTransparent(isTransparent);
            return true;
        }
        
        // アニメーションの表示
        protected virtual bool Command212(object[] parameters)
        {
            _characterId = Convert.ToInt32(parameters[0]);
            var character = Character(_characterId);
            if (character != null)
            {
                var targets = new object[] { character }.ToList();
                Rmmz.gameTemp.RequestAnimation(targets, Convert.ToInt32(parameters[1]));
                if (Convert.ToBoolean(parameters[2]))
                {
                    SetWaitMode("animation");
                }
            }
            return true;
        }
        
        // フキアシアイコンの表示
        protected virtual bool Command213(object[] parameters)
        {
            _characterId = Convert.ToInt32(parameters[0]);
            var character = Character(_characterId);
            if (character != null)
            {
                Rmmz.gameTemp.RequestBalloon(character, Convert.ToInt32(parameters[1]));
                if (Convert.ToBoolean(parameters[2]))
                {
                    SetWaitMode("balloon");
                }
            }

            return true;
        }
        
        // イベントの一時消去
        protected virtual bool Command214(object[] parameters)
        {
            if (IsOnCurrentMap() && _eventId > 0)
            {
                Rmmz.gameMap.EraseEvent(_eventId);
            }
            return true;
        }
        
        // 隊列歩行の変更
        protected virtual bool Command216(object[] parameters)
        {
            if (Convert.ToInt32(parameters[0]) == 0)
            {
                Rmmz.gamePlayer.ShowFollowers();
            }
            else
            {
                Rmmz.gamePlayer.HideFollowers();
            }
            Rmmz.gamePlayer.Refresh();
            return true;
        }
        
        // 隊列メンバーの集合
        protected virtual bool Command217(object[] parameters)
        {
            if (!Rmmz.gameParty.InBattle())
            {
                Rmmz.gamePlayer.GatherFollowers();
                SetWaitMode("gather");
            }
            return true;
        }
    }
}