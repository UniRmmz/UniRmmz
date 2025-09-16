using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UniRmmz
{
    public partial class SceneManager
    {
        private Scene_Base _scene;
        private Scene_Base _nextScene;
        private Stack<Type> _stack = new();
        private bool _exiting = false;
        private Scene_Base _previousScene;
        private Type _previousClass;
        private Bitmap _backgroundBitmap;
        private float _smoothDeltaTime = 1f;
        private float _elapsedTime = 0f;

        SceneManager()
        {
            var s = Rmmz.RootPath;// init
        }
        
        public void _Run(Type sceneClass)
        {
            try
            {
                Initialize();
                _Goto(sceneClass);
                UniRmmz.Graphics.StartGameLoop();
            }
            catch (Exception e)
            {
                CatchException(e);
                throw;
            }
        }

        private void Initialize()
        {
            RmmzRoot.Initialize();
            
            CheckPluginErrors();
            InitGraphics();
            InitAudio();
            InitVideo();
            InitInput();
            SetupEventHandlers();
        }

        private void CheckPluginErrors()
        {
            // TODO
        }

        private void InitGraphics()
        {
            if (!Graphics.Initialize())
            {
                throw new Exception("Failed to initialize graphics");
            }
            Graphics.SetTickHandler(() => Update());
        }

        private void InitAudio()
        {
            RmmzWebAudio.Initialize();
        }
        
        private void InitVideo()
        {
            Video.Initialize(Graphics.Width, Graphics.Height);
        }

        private void InitInput()
        {
            Input.Initialize();
            TouchInput.Initialize();
        }

        private void SetupEventHandlers()
        {
            Application.logMessageReceived += OnError;
            RmmzRoot.Instance.OnUnload += OnUnload;
        }

        public void Update()
        {
            try
            {
                float targetFps = Application.targetFrameRate > 0 ? Application.targetFrameRate : 60;
                float deltaTime = Time.deltaTime * targetFps;// PIXIのTickerの実装によると、ここのdeltaTimeはTargetFPSとの乗算結果である
                int n = DetermineRepeatNumber(deltaTime);
                for (int i = 0; i < n; i++)
                {
                    UpdateMain();
                }
            }
            catch (Exception e)
            {
                CatchException(e);
                throw;
            }
        }

        private int DetermineRepeatNumber(float deltaTime)
        {
            _smoothDeltaTime = 0.8f * _smoothDeltaTime + Mathf.Min(deltaTime, 2f) * 0.2f;
            if (_smoothDeltaTime >= 0.9f)
            {
                _elapsedTime = 0f;
                return Mathf.RoundToInt(_smoothDeltaTime);
            }
            else
            {
                _elapsedTime += deltaTime;
                if (_elapsedTime >= 1f)
                {
                    _elapsedTime -= 1f;
                    return 1;
                }
                return 0;
            }
        }
        
        private void Terminate() 
        {
            Application.Quit(0);
        }

        protected void OnError(string message, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                Debug.LogError($"SceneManager.OnErrorが呼ばれました。{message}");    
            }
        }
        
        protected void OnUnload()
        {
            Rmmz.ImageManager.Clear();
            Rmmz.EffectManager.Clear();
            Rmmz.AudioManager.StopAll();
        }

        public void Stop()
        {
            Graphics.StopGameLoop();
        }

        private void CatchException(Exception e)
        {
            CatchNormalError(e);
            Stop();
        }

        private void CatchNormalError(Exception error)
        {
            Graphics.PrintError(error.GetType().Name, error.Message);
            Rmmz.AudioManager.StopAll();
        }

        private void UpdateMain()
        {
            UpdateFrameCount();
            UpdateInput();
            ChangeScene();
            UpdateScene();
        }

        private void UpdateFrameCount()
        {
            UniRmmz.Graphics.FrameCount++;
        }

        private void UpdateInput()
        {
            Input.Update();
            TouchInput.Update();
        }

        private void ChangeScene()
        {
            if (IsSceneChanging() && !IsCurrentSceneBusy())
            {
                if (_scene != null)
                {
                    _scene.Terminate();
                    OnSceneTerminate();
                }

                _scene = _nextScene;
                _nextScene = null;
                if (_scene != null)
                {
                    _scene.Create();
                    //OnSceneCreate();
                }
                if (_exiting)
                {
                    Terminate();
                }
            }
        }

        private void UpdateScene()
        {
            if (_scene != null)
            {
                if (_scene.IsStarted())
                {
                    if (IsGameActive())
                    {
                        _scene.UpdateRmmz();
                    }
                }
                else if (_scene.IsReady())
                {
                    OnBeforeSceneStart();
                    _scene.StartScene();
                    OnSceneStart();
                }
            }
        }

        private bool IsGameActive()
        {
            return true;
        }

        private void OnSceneTerminate()
        {
            _scene.gameObject.SetActive(false);
            _previousScene = _scene;
            _previousClass = _previousScene.GetType();
            Graphics.SetStage(null);
        }

        private void OnSceneCreate()
        {
            Graphics.StartLoading();
        }
        
        private void OnBeforeSceneStart()
        {
            if (_previousScene != null)
            {
                MonoBehaviour.Destroy(_previousScene.gameObject);
                _previousScene = null;
                Resources.UnloadUnusedAssets();
            }
            
            // TODO effekseer
        }

        private void OnSceneStart()
        {
            UniRmmz.Graphics.EndLoading();
            UniRmmz.Graphics.SetStage(_scene);
        }

        public bool IsSceneChanging() => _exiting || _nextScene != null;

        private bool IsCurrentSceneBusy() => _scene?.IsBusy() ?? false;

        public bool IsNextScene(Type sceneClass) => _nextScene != null && sceneClass.IsAssignableFrom(_nextScene.GetType());
        
        public bool IsPreviousScene(Type sceneClass) => sceneClass.IsAssignableFrom(_previousClass);

        // このメソッドは直接呼ばずに、代わりにFactory.csに定義されているメソッドを使用してください
        public void _Goto(Type sceneClass)
        {
            if (sceneClass != null)
            {
                var obj = new GameObject(sceneClass.Name, typeof(RectTransform));
                obj.layer = Rmmz.RmmzDefaultLayer;
                var rect = obj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f);
                rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.sizeDelta = new Vector2(0, 0);
                _nextScene = obj.AddComponent(sceneClass) as Scene_Base;
            }
            _scene?.StopScene();
        }

        // このメソッドは直接呼ばずに、代わりにFactory.csに定義されているメソッドを使用してください
        public void _Push(Type sceneClass)
        {
            if (_scene != null)
            {
                _stack.Push(_scene.GetType());
            }
            _Goto(sceneClass);
        }

        public void Pop()
        {
            if (_stack.Count > 0)
            {
                _Goto(_stack.Pop());
            }
            else
            {
                Exit();
            }
        }
        public void Exit()
        {
            _Goto(null);
            _exiting = true;
        }

        public void ClearStack()
        {
            _stack.Clear();
        }
        
        public void PrepareNextScene(params object[] objects)
        {
            _nextScene.Prepare(objects);
        }
        
        public Bitmap Snap()
        {
            return Bitmap.Snap(_scene);
        }

        public void SnapForBackground()
        {
            if (_backgroundBitmap != null)
            {
                _backgroundBitmap.Dispose();
            }
            _backgroundBitmap = Snap();
        }

        public Bitmap BackgroundBitmap() => _backgroundBitmap;

        public void Resume()
        {
            TouchInput.Update();
            UniRmmz.Graphics.StartGameLoop();
            Debug.Log("Game loop resumed.");
        }

#region UniRmmz
        public bool CanRenderScene() => _previousScene == null;
#endregion

    }

}