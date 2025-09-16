using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The interpreter for running event commands.
    /// </summary>
    [Serializable]
    public partial class Game_Interpreter
    {
        private int _depth;
        private int _mapId;
        private int _eventId;
        private List<DataEventCommand> _list = new();
        private int _index;
        private int _waitCount;
        private string _waitMode;
        private List<string> _comments = new();
        private int _characterId;
        private Game_Interpreter _childInterpreter;
        private Dictionary<int, int> _branch = new Dictionary<int, int>();
        private int _indent;
        private int _frameCount;
        private int _freezeChecker;
        
        protected Game_Interpreter(int depth = 0)
        {
            _depth = depth;
            CheckOverflow();
            Clear();
            _branch = new Dictionary<int, int>();
            _indent = 0;
            _frameCount = 0;
            _freezeChecker = 0;
        }

        private void CheckOverflow()
        {
            if (_depth >= 100)
            {
                throw new Exception("Common event calls exceeded the limit");
            }
        }

        public void Clear()
        {
            _mapId = 0;
            _eventId = 0;
            _list = null;
            _index = 0;
            _waitCount = 0;
            _waitMode = "";
            _comments.Clear();
            _characterId = 0;
            _childInterpreter = null;
        }

        public void Setup(List<DataEventCommand> list, int eventId = 0)
        {
            Clear();
            _mapId = Rmmz.gameMap.MapId();
            _eventId = eventId;
            _list = list;
            LoadImages();
        }

        private void LoadImages()
        {
            // [Note] The certain versions of MV had a more complicated preload scheme.
            //   However it is usually sufficient to preload face and picture images.
            var list = _list.GetRange(0, Math.Min(200, _list.Count));
            foreach (var command in list)
            {
                switch (command.Code)
                {
                    case 101: // テキスト表示
                        Rmmz.ImageManager.LoadFace(command.Parameters[0].ToString());
                        break;
                    case 231: // ピクチャの表示
                        Rmmz.ImageManager.LoadPicture(command.Parameters[1].ToString());
                        break;
                }
            }
        }

        public int EventId()
        {
            return _eventId;
        }

        public bool IsOnCurrentMap()
        {
            return _mapId == Rmmz.gameMap.MapId();
        }

        public bool SetupReservedCommonEvent()
        {
            if (Rmmz.gameTemp.IsCommonEventReserved())
            {
                var commonEvent = Rmmz.gameTemp.RetrieveCommonEvent();
                if (commonEvent != null)
                {
                    Setup(commonEvent.List);
                    return true;
                }
            }

            return false;
        }

        public bool IsRunning()
        {
            return _list != null;
        }

        public void Update()
        {
            while (IsRunning())
            {
                if (UpdateChild() || UpdateWait())
                {
                    break;
                }
                if (Rmmz.SceneManager.IsSceneChanging())
                {
                    break;
                }
                if (!ExecuteCommand())
                {
                    break;
                }
                if (CheckFreeze())
                {
                    break;
                }
            }
        }

        private bool UpdateChild()
        {
            if (_childInterpreter != null)
            {
                _childInterpreter.Update();
                if (_childInterpreter.IsRunning())
                {
                    return true;
                }
                else
                {
                    _childInterpreter = null;
                }
            }
            return false;
        }

        private bool UpdateWait()
        {
            return UpdateWaitCount() || UpdateWaitMode();
        }

        private bool UpdateWaitCount()
        {
            if (_waitCount > 0)
            {
                _waitCount--;
                return true;
            }
            return false;
        }

        private bool UpdateWaitMode()
        {
            Game_Character character = null;
            bool waiting = false;
            
            switch (_waitMode)
            {
                case "message":
                    waiting = Rmmz.gameMessage.IsBusy();
                    break;
                case "transfer":
                    waiting = Rmmz.gamePlayer.IsTransferring();
                    break;
                case "scroll":
                    waiting = Rmmz.gameMap.IsScrolling();
                    break;
                case "route":
                    character = Character(_characterId);
                    waiting = character != null && character.IsMoveRouteForcing();
                    break;
                case "animation":
                    character = Character(_characterId);
                    waiting = character != null && character.IsAnimationPlaying();
                    break;
                case "balloon":
                    character = Character(_characterId);
                    waiting = character != null && character.IsBalloonPlaying();
                    break;
                case "gather":
                    waiting = Rmmz.gamePlayer.AreFollowersGathering();
                    break;
                case "action":
                    waiting = Rmmz.BattleManager.IsActionForced();
                    break;
                case "video":
                    waiting = Video.IsPlaying();
                    break;
                case "image":
                    waiting = !Rmmz.ImageManager.IsReady();
                    break;
            }
            
            if (!waiting)
            {
                _waitMode = "";
            }
            
            return waiting;
        }

        public void SetWaitMode(string waitMode)
        {
            _waitMode = waitMode;
        }

        public void Wait(int duration)
        {
            _waitCount = duration;
        }

        private int FadeSpeed()
        {
            return 24;
        }

        private bool ExecuteCommand()
        {
            var command = CurrentCommand();
            if (command != null)
            {
                _indent = command.Indent;
                bool result = true;
                
                switch (command.Code)
                {
                    case 101: result = Command101(command.Parameters); break; // テキスト表示
                    case 102: result = Command102(command.Parameters); break; // 選択肢の表示
                    case 103: result = Command103(command.Parameters); break; // 数値入力の処理
                    case 104: result = Command104(command.Parameters); break; // アイテム選択の処理
                    case 105: result = Command105(command.Parameters); break; // スクロールテキスト表示
                    case 108: result = Command108(command.Parameters); break; // 注釈
                    case 109: result = Command109(command.Parameters); break; // スキップ
                    case 111: result = Command111(command.Parameters); break; // 条件分岐
                    case 112: result = Command112(command.Parameters); break; // ループ
                    case 113: result = Command113(command.Parameters); break; // ループの中断
                    case 115: result = Command115(command.Parameters); break; // イベント処理の中断
                    case 117: result = Command117(command.Parameters); break; // コモンイベント
                    case 118: result = Command118(command.Parameters); break; // ラベル
                    case 119: result = Command119(command.Parameters); break; // ラベルジャンプ
                    case 121: result = Command121(command.Parameters); break; // スイッチの操作
                    case 122: result = Command122(command.Parameters); break; // 変数の操作
                    case 123: result = Command123(command.Parameters); break; // セルフスイッチの操作
                    case 124: result = Command124(command.Parameters); break; // タイマーの操作
                    case 125: result = Command125(command.Parameters); break; // 所持金の増減
                    case 126: result = Command126(command.Parameters); break; // アイテムの増減
                    case 127: result = Command127(command.Parameters); break; // 武器の増減
                    case 128: result = Command128(command.Parameters); break; // 防具の増減
                    case 129: result = Command129(command.Parameters); break; // メンバーの入れ替え
                    case 132: result = Command132(command.Parameters); break; // 戦闘BGMの変更
                    case 133: result = Command133(command.Parameters); break; // 勝利MEの変更
                    case 134: result = Command134(command.Parameters); break; // セーブアクセスの許可
                    case 135: result = Command135(command.Parameters); break; // メニューアクセスの許可
                    case 136: result = Command136(command.Parameters); break; // エンカウントの禁止
                    case 137: result = Command137(command.Parameters); break; // 並び替えの禁止
                    case 138: result = Command138(command.Parameters); break; // ウィンドウカラーの変更
                    case 139: result = Command139(command.Parameters); break; // 敗北MEの変更
                    case 140: result = Command140(command.Parameters); break; // 乗り物BGMの変更
                    case 201: result = Command201(command.Parameters); break; // 場所移動
                    case 202: result = Command202(command.Parameters); break; // 乗り物の位置設定
                    case 203: result = Command203(command.Parameters); break; // イベントの位置設定
                    case 204: result = Command204(command.Parameters); break; // マップのスクロール
                    case 205: result = Command205(command.Parameters); break; // 移動ルートの設定
                    case 206: result = Command206(command.Parameters); break; // 乗り物の乗降
                    case 211: result = Command211(command.Parameters); break; // 透明状態の変更
                    case 212: result = Command212(command.Parameters); break; // アニメーションの表示
                    case 213: result = Command213(command.Parameters); break; // フキダシアイコンの表示
                    case 214: result = Command214(command.Parameters); break; // イベントの一時消去
                    case 216: result = Command216(command.Parameters); break; // 隊列歩行の変更
                    case 217: result = Command217(command.Parameters); break; // 隊列メンバーの集合
                    case 221: result = Command221(command.Parameters); break; // 画面のフェードアウト
                    case 222: result = Command222(command.Parameters); break; // 画面のフェードイン
                    case 223: result = Command223(command.Parameters); break; // 画面の色調変更
                    case 224: result = Command224(command.Parameters); break; // 画面のフラッシュ
                    case 225: result = Command225(command.Parameters); break; // 画面のシェイク
                    case 230: result = Command230(command.Parameters); break; // ウェイト
                    case 231: result = Command231(command.Parameters); break; // ピクチャの表示
                    case 232: result = Command232(command.Parameters); break; // ピクチャの移動
                    case 233: result = Command233(command.Parameters); break; // ピクチャの回転
                    case 234: result = Command234(command.Parameters); break; // ピクチャの色調変更
                    case 235: result = Command235(command.Parameters); break; // ピクチャの消去
                    case 236: result = Command236(command.Parameters); break; // 天候の設定
                    case 241: result = Command241(command.Parameters); break; // BGMの演奏
                    case 242: result = Command242(command.Parameters); break; // BGMのフェードアウト
                    case 243: result = Command243(command.Parameters); break; // BGMの保存
                    case 244: result = Command244(command.Parameters); break; // BGMの再開
                    case 245: result = Command245(command.Parameters); break; // BGSの演奏
                    case 246: result = Command246(command.Parameters); break; // BGSのフェードアウト
                    case 249: result = Command249(command.Parameters); break; // MEの演奏
                    case 250: result = Command250(command.Parameters); break; // SEの演奏
                    case 251: result = Command251(command.Parameters); break; // SEの停止
                    case 261: result = Command261(command.Parameters); break; // ムービーの再生
                    case 281: result = Command281(command.Parameters); break; // マップ名表示の変更
                    case 282: result = Command282(command.Parameters); break; // タイルセットの変更
                    case 283: result = Command283(command.Parameters); break; // 戦闘背景の変更
                    case 284: result = Command284(command.Parameters); break; // 遠景の変更
                    case 285: result = Command285(command.Parameters); break; // 指定位置の情報取得
                    case 301: result = Command301(command.Parameters); break; // 戦闘の処理
                    case 302: result = Command302(command.Parameters); break; // ショップの処理
                    case 303: result = Command303(command.Parameters); break; // 名前入力の処理
                    case 311: result = Command311(command.Parameters); break; // HPの増減
                    case 312: result = Command312(command.Parameters); break; // MPの増減
                    case 313: result = Command313(command.Parameters); break; // ステートの変更
                    case 314: result = Command314(command.Parameters); break; // 全回復
                    case 315: result = Command315(command.Parameters); break; // 経験値の増減
                    case 316: result = Command316(command.Parameters); break; // レベルの増減
                    case 317: result = Command317(command.Parameters); break; // 能力値の増減
                    case 318: result = Command318(command.Parameters); break; // スキルの増減
                    case 319: result = Command319(command.Parameters); break; // 装備の変更
                    case 320: result = Command320(command.Parameters); break; // 名前の変更
                    case 321: result = Command321(command.Parameters); break; // 職業の変更
                    case 322: result = Command322(command.Parameters); break; // アクター画像の変更
                    case 323: result = Command323(command.Parameters); break; // 乗り物画像の変更
                    case 324: result = Command324(command.Parameters); break; // 二つ名の変更
                    case 325: result = Command325(command.Parameters); break; // プロフィールの変更
                    case 326: result = Command326(command.Parameters); break; // TPの増減
                    case 331: result = Command331(command.Parameters); break; // 敵キャラのHP増減
                    case 332: result = Command332(command.Parameters); break; // 敵キャラのMP増減
                    case 333: result = Command333(command.Parameters); break; // 敵キャラのステート変更
                    case 334: result = Command334(command.Parameters); break; // 敵キャラの全回復
                    case 335: result = Command335(command.Parameters); break; // 敵キャラの出現
                    case 336: result = Command336(command.Parameters); break; // 敵キャラの変身
                    case 337: result = Command337(command.Parameters); break; // 戦闘アニメーションの表示
                    case 339: result = Command339(command.Parameters); break; // 戦闘行動の強制
                    case 340: result = Command340(command.Parameters); break; // 戦闘中止
                    case 342: result = Command342(command.Parameters); break; // 敵キャラのTP増減
                    case 351: result = Command351(command.Parameters); break; // メニュー画面を開く
                    case 352: result = Command352(command.Parameters); break; // セーブ画面を開く
                    case 353: result = Command353(command.Parameters); break; // ゲームオーバー
                    case 354: result = Command354(command.Parameters); break; // タイトル画面に戻す
                    case 355: result = Command355(command.Parameters); break; // スクリプト
                    case 356: result = Command356(command.Parameters); break; // プラグインコマンド(MV)
                    case 357: result = Command357(command.Parameters); break; // プラグインコマンド
                    // 条件分岐の分岐コマンド
                    case 402: result = Command402(command.Parameters); break; // 選択肢のとき
                    case 403: result = Command403(command.Parameters); break; // キャンセルのとき
                    case 411: result = Command411(command.Parameters); break; // それ以外のとき
                    case 413: result = Command413(command.Parameters); break; // 以上繰り返し
                    // 戦闘の分岐
                    case 601: result = Command601(command.Parameters); break; // 勝ったとき
                    case 602: result = Command602(command.Parameters); break; // 逃げたとき
                    case 603: result = Command603(command.Parameters); break; // 負けたとき
                    default : 
                        result = true;
                        break;
                }
                
                if (!result)
                {
                    return false;
                }
                
                _index++;
            }
            else
            {
                Terminate();
            }
            
            return true;
        }

        private bool CheckFreeze()
        {
            if (_frameCount != Graphics.FrameCount)
            {
                _frameCount = Graphics.FrameCount;
                _freezeChecker = 0;
            }
            
            if (_freezeChecker++ >= 100000)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Terminate()
        {
            _list = null;
            _comments.Clear();
        }

        private void SkipBranch()
        {
            while (_index + 1 < _list.Count && _list[_index + 1].Indent > _indent)
            {
                _index++;
            }
        }

        private DataEventCommand CurrentCommand()
        {
            if (_list != null && _index < _list.Count)
            {
                return _list[_index];
            }
            return null;
        }

        private int NextEventCode()
        {
            if (_list != null && _index + 1 < _list.Count)
            {
                return _list[_index + 1].Code;
            }
            else
            {
                return 0;
            }
        }

        private void IterateActorId(int param, Action<Game_Actor> callback)
        {
            if (param == 0)
            {
                foreach (Game_Actor actor in Rmmz.gameParty.Members())
                {
                    callback(actor);
                }
            }
            else
            {
                Game_Actor actor = Rmmz.gameActors.Actor(param);
                if (actor != null)
                {
                    callback(actor);
                }
            }
        }

        private void IterateActorEx(int param1, int param2, Action<Game_Actor> callback)
        {
            if (param1 == 0)
            {
                IterateActorId(param2, callback);
            }
            else
            {
                IterateActorId(Rmmz.gameVariables.Value(param2), callback);
            }
        }

        private void IterateActorIndex(int param, Action<Game_Actor> callback)
        {
            if (param < 0)
            {
                foreach (Game_Actor actor in Rmmz.gameParty.Members())
                {
                    callback(actor);
                }
            }
            else
            {
                var members = Rmmz.gameParty.Members().Cast<Game_Actor>().ToList();
                if (param < members.Count)
                {
                    Game_Actor actor = members[param];
                    if (actor != null)
                    {
                        callback(actor);
                    }
                }
            }
        }

        private void IterateEnemyIndex(int param, Action<Game_Enemy> callback)
        {
            if (param < 0)
            {
                foreach (Game_Enemy enemy in Rmmz.gameTroop.Members())
                {
                    callback(enemy);
                }
            }
            else
            {
                var members = Rmmz.gameTroop.Members().Cast<Game_Enemy>().ToList();
                if (param < members.Count)
                {
                    Game_Enemy enemy = members[param];
                    if (enemy != null)
                    {
                        callback(enemy);
                    }
                }
            }
        }
        
        private void IterateBattler(int param1, int param2, Action<Game_Battler> callback)
        {
            if (Rmmz.gameParty.InBattle())
            {
                if (param1 == 0)
                {
                    IterateEnemyIndex(param2, enemy => callback(enemy));
                }
                else
                {
                    IterateActorId(param2, actor => callback(actor));
                }
            }
        }

        private Game_Character Character(int param)
        {
            if (Rmmz.gameParty.InBattle())
            {
                return null;
            }
            else if (param < 0)
            {
                return Rmmz.gamePlayer;
            }
            else if (IsOnCurrentMap())
            {
                return Rmmz.gameMap.Event(param > 0 ? param : _eventId);
            }
            else
            {
                return null;
            }
        }

        private int OperateValue(int operation, int operandType, int operand)
        {
            int value = operandType == 0 ? operand : Rmmz.gameVariables.Value(operand);
            return operation == 0 ? value : -value;
        }

        private void ChangeHp(Game_Battler target, int value, bool allowDeath)
        {
            if (target.IsAlive())
            {
                if (!allowDeath && target.Hp <= -value)
                {
                    value = 1 - target.Hp;
                }
                target.GainHp(value);
                if (target.IsDead())
                {
                    target.PerformCollapse();
                }
            }
        }

        // ウェイト
        public bool Command230(object[] parameters)
        {
            Wait(Convert.ToInt32(parameters[0]));
            return true;
        }

    }
}