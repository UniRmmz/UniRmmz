using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The scene class of the map screen.
    /// </summary>
    public partial class Scene_Map : Scene_Message
    {
        protected int _waitCount;
        protected int _encounterEffectDuration;
        protected bool _mapLoaded;
        protected int _touchCount;
        protected bool _menuEnabled;
        protected bool _transfer;
        protected bool _lastMapWasNull;
        protected bool _menuCalling;

        protected Window_MapName _mapNameWindow;
        protected Sprite_Button _menuButton;
        protected Spriteset_Map _spriteset;

        public override void Create()
        {
            base.Create();
            _transfer = Rmmz.gamePlayer.IsTransferring();
            _lastMapWasNull = Rmmz.dataMap == null;
            if (_transfer)
            {
                Rmmz.DataManager.LoadMapData(Rmmz.gamePlayer.NewMapId());
                OnTransfer();
            }
            else
            {
                Rmmz.DataManager.LoadMapData(Rmmz.gameMap.MapId());
            }
        }

        public override bool IsReady()
        {
            if (!_mapLoaded && Rmmz.DataManager.IsMapLoaded())
            {
                OnMapLoaded();
                _mapLoaded = true;
            }
            return _mapLoaded && base.IsReady();
        }

        protected virtual void OnMapLoaded()
        {
            if (_transfer)
            {
                Rmmz.gamePlayer.PerformTransfer();
            }
            CreateDisplayObjects();
        }

        protected virtual void OnTransfer()
        {
            Rmmz.ImageManager.Clear();
            Rmmz.EffectManager.Clear();
        }

        public override void StartScene()
        {
            base.StartScene();
            Rmmz.SceneManager.ClearStack();
            if (_transfer)
            {
                FadeInForTransfer();
                OnTransferEnd();
            }
            else if (NeedsFadeIn())
            {
                StartFadeIn(FadeSpeed(), false);
            }

            _menuCalling = false;
        }

        protected virtual void OnTransferEnd()
        {
            _mapNameWindow.Open();
            Rmmz.gameMap.Autoplay();
            if (ShouldAutosave())
            {
                RequestAutosave();
            }
        }

        protected virtual bool ShouldAutosave() => !_lastMapWasNull;

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateDestination();
            UpdateMenuButton();
            UpdateMapNameWindow();
            UpdateMainMultiply();

            if (IsSceneChangeOk())
            {
                UpdateScene();
            }
            else if (Rmmz.SceneManager.IsNextScene(typeof (Scene_Battle)))
            {
                UpdateEncounterEffect();
            }

            UpdateWaitCount();
        }

        protected virtual void UpdateMainMultiply()
        {
            if (IsFastForward())
            {
                UpdateMain();
            }
            UpdateMain();
        }

        protected virtual void UpdateMain()
        {
            Rmmz.gameMap.Update(IsSceneActive());
            Rmmz.gamePlayer.Update(IsPlayerActive());
            Rmmz.gameTimer.Update(IsSceneActive());
            Rmmz.gameScreen.Update();
        }

        protected virtual bool IsPlayerActive() => IsSceneActive() && !IsFading();

        protected virtual bool IsFastForward()
        {
            return Rmmz.gameMap.IsEventRunning() &&
                   !Rmmz.SceneManager.IsSceneChanging() && 
                   (Input.IsLongPressed("ok") || TouchInput.IsLongPressed());
        }

        public override void StopScene()
        {
            base.StopScene();
            Rmmz.gamePlayer.Straighten();
            _mapNameWindow.Close();
            if (NeedsSlowFadeOut())
            {
                StartFadeOut(SlowFadeSpeed(), false);
            }
            else if (Rmmz.SceneManager.IsNextScene(typeof (Scene_Map)))
            {
                FadeOutForTransfer();
            }
            else if (Rmmz.SceneManager.IsNextScene(typeof (Scene_Battle)))
            {
                LaunchBattle();
            }
        }

        public override bool IsBusy()
        {
            return IsMessageWindowClosing() || _waitCount > 0 ||
                   _encounterEffectDuration > 0 || base.IsBusy();
        }

        public override void Terminate()
        {
            base.Terminate();
            if (!Rmmz.SceneManager.IsNextScene(typeof (Scene_Battle)))
            {
                _spriteset.UpdateRmmz();
                _mapNameWindow.Hide();
                HideMenuButton();
                Rmmz.SceneManager.SnapForBackground();
            }
            Rmmz.gameScreen.ClearZoom();
        }

        protected virtual bool NeedsFadeIn() => Rmmz.SceneManager.IsPreviousScene(typeof (Scene_Battle)) || Rmmz.SceneManager.IsPreviousScene(typeof (Scene_Load));
        protected virtual bool NeedsSlowFadeOut() => Rmmz.SceneManager.IsNextScene(typeof (Scene_Title)) || Rmmz.SceneManager.IsNextScene(typeof (Scene_Gameover));

        protected virtual bool UpdateWaitCount()
        {
            if (_waitCount > 0)
            {
                _waitCount--;
                return true;
            }
            return false;
        }

        protected virtual void UpdateDestination()
        {
            if (IsMapTouchOk())
            {
                ProcessMapTouch();
            }
            else
            {
                Rmmz.gameTemp.ClearDestination();
                _touchCount = 0;
            }
        }

        protected virtual void UpdateMenuButton()
        {
            if (_menuButton != null)
            {
                bool menuEnabled = IsMenuEnabled();
                if (menuEnabled == _menuEnabled)
                {
                    _menuButton.Visible = _menuEnabled;
                }
                else
                {
                    _menuEnabled = menuEnabled;
                }
            }
        }

        protected virtual void HideMenuButton()
        {
            if (_menuButton != null)
            {
                _menuButton.Visible = false;
                _menuEnabled = false;
            }
        }

        protected virtual void UpdateMapNameWindow()
        {
            if (Rmmz.gameMessage.IsBusy())
            {
                _mapNameWindow.Close();
            }
        }

        protected virtual bool IsMenuEnabled() => Rmmz.gameSystem.IsMenuEnabled() && !Rmmz.gameMap.IsEventRunning();
        protected virtual bool IsMapTouchOk() => IsSceneActive() && Rmmz.gamePlayer.CanMove();

        protected virtual void ProcessMapTouch()
        {
            if (TouchInput.IsTriggered() || _touchCount > 0)
            {
                if (TouchInput.IsPressed() && !IsAnyButtonPressed())
                {
                    if (_touchCount == 0 || _touchCount >= 15) OnMapTouch();
                    _touchCount++;
                }
                else
                {
                    _touchCount = 0;
                }
            }
        }

        protected virtual bool IsAnyButtonPressed() => _menuButton?.IsPressed() ?? false;

        protected virtual void OnMapTouch()
        {
            int x = Rmmz.gameMap.CanvasToMapX(TouchInput.X);
            int y = Rmmz.gameMap.CanvasToMapY(TouchInput.Y);
            Rmmz.gameTemp.SetDestination(x, y);
        }

        protected virtual bool IsSceneChangeOk() => IsSceneActive() && !Rmmz.gameMessage.IsBusy();

        protected virtual void UpdateScene()
        {
            CheckGameover();
            if (!Rmmz.SceneManager.IsSceneChanging())
            {
                UpdateTransferPlayer();
            }
            if (!Rmmz.SceneManager.IsSceneChanging())
            {
                UpdateEncounter();
            }
            if (!Rmmz.SceneManager.IsSceneChanging())
            {
                UpdateCallMenu();
            }
            if (!Rmmz.SceneManager.IsSceneChanging())
            {
                UpdateCallDebug();
            }
        }

        protected virtual void CreateDisplayObjects()
        {
            CreateSpriteset();
            CreateWindowLayer();
            CreateAllWindows();
            CreateButtons();
        }

        protected virtual void CreateSpriteset()
        {
            _spriteset = Spriteset_Map.Create("spritesetMap");
            this.AddChild(_spriteset);
            _spriteset.UpdateRmmz();
        }

        protected override void CreateAllWindows()
        {
            CreateMapNameWindow();
            base.CreateAllWindows();
        }

        protected virtual void CreateMapNameWindow()
        {
            var rect = MapNameWindowRect();
            _mapNameWindow = Window_MapName.Create(rect, "mapName");
            AddWindow(_mapNameWindow);
        }

        protected virtual Rect MapNameWindowRect()
        {
            return new Rect(0, 0, 360, CalcWindowHeight(1, false));
        }

        protected virtual void CreateButtons()
        {
            if (Rmmz.ConfigManager.TouchUI)
            {
                CreateMenuButton();
            }
        }

        protected virtual void CreateMenuButton()
        {
            _menuButton = Sprite_Button.Create("menu");
            _menuButton.Initialize(Input.ButtonTypes.Menu);
            _menuButton.X = Graphics.BoxWidth - _menuButton.Width - 4;
            _menuButton.Y = ButtonY();
            _menuButton.Visible = false;
            AddWindow(_menuButton);
        }

        protected virtual void UpdateTransferPlayer()
        {
            if (Rmmz.gamePlayer.IsTransferring())
            {
                Scene_Map.Goto();
            }
        }

        protected virtual void UpdateEncounter()
        {
            if (Rmmz.gamePlayer.ExecuteEncounter())
            {
                Scene_Battle.Push();
            }
        }

        protected virtual void UpdateCallMenu()
        {
            if (IsMenuEnabled())
            {
                if (IsMenuCalled())
                {
                    _menuCalling = true;
                }
                if (_menuCalling && !Rmmz.gamePlayer.IsMoving())
                {
                    CallMenu();
                }
            }
            else
            {
                _menuCalling = false;
            }
        }

        protected virtual bool IsMenuCalled() => Input.IsTriggered("menu") || TouchInput.IsCancelled();

        protected virtual void CallMenu()
        {
            Rmmz.SoundManager.PlayOk();
            Scene_Menu.Push();
            Window_MenuCommand.InitCommandPosition();
            Rmmz.gameTemp.ClearDestination();
            _mapNameWindow.Hide();
            _waitCount = 2;
        }

        protected virtual void UpdateCallDebug()
        {
            /*
            if (IsDebugCalled())
            {
                SceneManager.Push<Scene_Debug>();
            }
            */
        }

        //protected virtual bool IsDebugCalled() => Input.IsTriggered("debug") && Rmmz.gameTemp.IsPlaytest();

        protected virtual void FadeInForTransfer()
        {
            int fadeType = Rmmz.gamePlayer.FadeType();
            if (fadeType == 0 || fadeType == 1)
            {
                StartFadeIn(FadeSpeed(), fadeType == 1);
            }
        }

        protected virtual void FadeOutForTransfer()
        {
            int fadeType = Rmmz.gamePlayer.FadeType();
            if (fadeType == 0 || fadeType == 1)
            {
                StartFadeOut(FadeSpeed(), fadeType == 1);
            }
        }

        protected virtual void LaunchBattle()
        {
            Rmmz.BattleManager.SaveBgmAndBgs();
            StopAudioOnBattleStart();
            Rmmz.SoundManager.PlayBattleStart();
            StartEncounterEffect();
            _mapNameWindow.Hide();
        }

        protected virtual void StopAudioOnBattleStart()
        {
            if (!Rmmz.AudioManager.IsCurrentBgm(Rmmz.gameSystem.BattleBgm()))
            {
                Rmmz.AudioManager.StopBgm();
            }
            Rmmz.AudioManager.StopBgs();
            Rmmz.AudioManager.StopMe();
            Rmmz.AudioManager.StopSe();
        }

        protected virtual void StartEncounterEffect()
        {
            _spriteset.HideCharacters();
            _encounterEffectDuration = EncounterEffectSpeed();
        }

        protected virtual void UpdateEncounterEffect()
        {
            if (_encounterEffectDuration > 0)
            {
                _encounterEffectDuration--;
                int speed = EncounterEffectSpeed();
                int n = speed - _encounterEffectDuration;
                float p = (float)n / speed;
                float q = ((p - 1) * 20 * p + 5) * p + 1;

                float zoomX = Rmmz.gamePlayer.ScreenX();
                float zoomY = Rmmz.gamePlayer.ScreenY() - 24;

                if (n == 2)
                {
                    Rmmz.gameScreen.SetZoom(zoomX, zoomY, 1);
                    SnapForBattleBackground();
                    StartFlashForEncounter(speed / 2);
                }

                Rmmz.gameScreen.SetZoom(zoomX, zoomY, q);

                if (n == speed / 6)
                {
                    StartFlashForEncounter(speed / 2);
                }
                if (n == speed / 2)
                {
                    Rmmz.BattleManager.PlayBattleBgm();
                    StartFadeOut(FadeSpeed());
                }
            }
        }

        protected void SnapForBattleBackground()
        {
            _windowLayer.Visible = false;
            Rmmz.SceneManager.SnapForBackground();
            _windowLayer.Visible = true;
        }

        protected virtual void StartFlashForEncounter(int duration)
        {
            var color = new Color32(255, 255, 255, 255);
            Rmmz.gameScreen.StartFlash(color, duration);
        }

        protected virtual int EncounterEffectSpeed() => 60;
    }

}