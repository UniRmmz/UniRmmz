using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The scene class of the map screen.
    /// </summary>
    public partial class Scene_Map : Scene_Message
    {
        private int _waitCount;
        private int _encounterEffectDuration;
        private bool _mapLoaded;
        private int _touchCount;
        private bool _menuEnabled;
        private bool _transfer;
        private bool _lastMapWasNull;
        private bool _menuCalling;

        private Window_MapName _mapNameWindow;
        private Sprite_Button _menuButton;
        private Spriteset_Map _spriteset;

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

        private void OnMapLoaded()
        {
            if (_transfer)
            {
                Rmmz.gamePlayer.PerformTransfer();
            }
            CreateDisplayObjects();
        }

        private void OnTransfer()
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

        private void OnTransferEnd()
        {
            _mapNameWindow.Open();
            Rmmz.gameMap.Autoplay();
            if (ShouldAutosave())
            {
                RequestAutosave();
            }
        }

        private bool ShouldAutosave() => !_lastMapWasNull;

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

        private void UpdateMainMultiply()
        {
            if (IsFastForward())
            {
                UpdateMain();
            }
            UpdateMain();
        }

        private void UpdateMain()
        {
            Rmmz.gameMap.Update(IsSceneActive());
            Rmmz.gamePlayer.Update(IsPlayerActive());
            Rmmz.gameTimer.Update(IsSceneActive());
            Rmmz.gameScreen.Update();
        }

        private bool IsPlayerActive() => IsSceneActive() && !IsFading();

        private bool IsFastForward()
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

        private bool NeedsFadeIn() => Rmmz.SceneManager.IsPreviousScene(typeof (Scene_Battle)) || Rmmz.SceneManager.IsPreviousScene(typeof (Scene_Load));
        private bool NeedsSlowFadeOut() => Rmmz.SceneManager.IsNextScene(typeof (Scene_Title)) || Rmmz.SceneManager.IsNextScene(typeof (Scene_Gameover));

        private bool UpdateWaitCount()
        {
            if (_waitCount > 0)
            {
                _waitCount--;
                return true;
            }
            return false;
        }

        private void UpdateDestination()
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

        private void UpdateMenuButton()
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

        private void HideMenuButton()
        {
            if (_menuButton != null)
            {
                _menuButton.Visible = false;
                _menuEnabled = false;
            }
        }

        private void UpdateMapNameWindow()
        {
            if (Rmmz.gameMessage.IsBusy())
            {
                _mapNameWindow.Close();
            }
        }

        private bool IsMenuEnabled() => Rmmz.gameSystem.IsMenuEnabled() && !Rmmz.gameMap.IsEventRunning();
        private bool IsMapTouchOk() => IsSceneActive() && Rmmz.gamePlayer.CanMove();

        private void ProcessMapTouch()
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

        private bool IsAnyButtonPressed() => _menuButton?.IsPressed() ?? false;

        private void OnMapTouch()
        {
            int x = Rmmz.gameMap.CanvasToMapX(TouchInput.X);
            int y = Rmmz.gameMap.CanvasToMapY(TouchInput.Y);
            Rmmz.gameTemp.SetDestination(x, y);
        }

        private bool IsSceneChangeOk() => IsSceneActive() && !Rmmz.gameMessage.IsBusy();

        private void UpdateScene()
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

        private void CreateDisplayObjects()
        {
            CreateSpriteset();
            CreateWindowLayer();
            CreateAllWindows();
            CreateButtons();
        }

        private void CreateSpriteset()
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

        private void CreateMapNameWindow()
        {
            var rect = MapNameWindowRect();
            _mapNameWindow = Window_MapName.Create(rect, "mapName");
            AddWindow(_mapNameWindow);
        }

        private Rect MapNameWindowRect()
        {
            return new Rect(0, 0, 360, CalcWindowHeight(1, false));
        }

        private void CreateButtons()
        {
            if (Rmmz.ConfigManager.TouchUI)
            {
                CreateMenuButton();
            }
        }

        private void CreateMenuButton()
        {
            _menuButton = Sprite_Button.Create("menu");
            _menuButton.Initialize(Input.ButtonTypes.Menu);
            _menuButton.X = Graphics.BoxWidth - _menuButton.Width - 4;
            _menuButton.Y = ButtonY();
            _menuButton.Visible = false;
            AddWindow(_menuButton);
        }

        private void UpdateTransferPlayer()
        {
            if (Rmmz.gamePlayer.IsTransferring())
            {
                Scene_Map.Goto();
            }
        }

        private void UpdateEncounter()
        {
            if (Rmmz.gamePlayer.ExecuteEncounter())
            {
                Scene_Battle.Push();
            }
        }

        private void UpdateCallMenu()
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

        private bool IsMenuCalled() => Input.IsTriggered("menu") || TouchInput.IsCancelled();

        private void CallMenu()
        {
            Rmmz.SoundManager.PlayOk();
            Scene_Menu.Push();
            Window_MenuCommand.InitCommandPosition();
            Rmmz.gameTemp.ClearDestination();
            _mapNameWindow.Hide();
            _waitCount = 2;
        }

        private void UpdateCallDebug()
        {
            /*
            if (IsDebugCalled())
            {
                SceneManager.Push<Scene_Debug>();
            }
            */
        }

        //private bool IsDebugCalled() => Input.IsTriggered("debug") && Rmmz.gameTemp.IsPlaytest();

        private void FadeInForTransfer()
        {
            int fadeType = Rmmz.gamePlayer.FadeType();
            if (fadeType == 0 || fadeType == 1)
            {
                StartFadeIn(FadeSpeed(), fadeType == 1);
            }
        }

        private void FadeOutForTransfer()
        {
            int fadeType = Rmmz.gamePlayer.FadeType();
            if (fadeType == 0 || fadeType == 1)
            {
                StartFadeOut(FadeSpeed(), fadeType == 1);
            }
        }

        private void LaunchBattle()
        {
            Rmmz.BattleManager.SaveBgmAndBgs();
            StopAudioOnBattleStart();
            Rmmz.SoundManager.PlayBattleStart();
            StartEncounterEffect();
            _mapNameWindow.Hide();
        }

        private void StopAudioOnBattleStart()
        {
            if (!Rmmz.AudioManager.IsCurrentBgm(Rmmz.gameSystem.BattleBgm()))
            {
                Rmmz.AudioManager.StopBgm();
            }
            Rmmz.AudioManager.StopBgs();
            Rmmz.AudioManager.StopMe();
            Rmmz.AudioManager.StopSe();
        }

        private void StartEncounterEffect()
        {
            _spriteset.HideCharacters();
            _encounterEffectDuration = EncounterEffectSpeed();
        }

        private void UpdateEncounterEffect()
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

        private void StartFlashForEncounter(int duration)
        {
            var color = new Color32(255, 255, 255, 255);
            Rmmz.gameScreen.StartFlash(color, duration);
        }

        private int EncounterEffectSpeed() => 60;
    }

}