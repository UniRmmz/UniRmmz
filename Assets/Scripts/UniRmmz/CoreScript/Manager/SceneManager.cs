using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UniRmmz
{
    public partial class SceneManager
    {
        protected Scene_Base _scene;
        protected Scene_Base _nextScene;
        protected Stack<Type> _stack = new();
        protected bool _exiting = false;
        protected Scene_Base _previousScene;
        protected Type _previousClass;
        protected Bitmap _backgroundBitmap;
        protected float _smoothDeltaTime = 1f;
        protected float _elapsedTime = 0f;

        SceneManager()
        {
            var s = Rmmz.RootPath;// init
        }
        
        public virtual void _Run(Type sceneClass)
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

        protected virtual void Initialize()
        {   
            CheckPluginErrors();
            InitGraphics();
            InitAudio();
            InitVideo();
            InitInput();
            SetupEventHandlers();
        }

        protected virtual void CheckPluginErrors()
        {
            // TODO
        }

        protected virtual void InitGraphics()
        {
            if (!Graphics.Initialize())
            {
                throw new Exception("Failed to initialize graphics");
            }
            Graphics.SetTickHandler(() => Update());
        }

        protected virtual void InitAudio()
        {
            RmmzWebAudio.Initialize();
        }
        
        protected virtual void InitVideo()
        {
            Video.Initialize(Graphics.Width, Graphics.Height);
        }

        protected virtual void InitInput()
        {
            Input.Initialize();
            TouchInput.Initialize();
        }

        protected virtual void SetupEventHandlers()
        {
            Application.logMessageReceived += OnError;
            RmmzRoot.Instance.OnUnload += OnUnload;
        }

        public virtual void Update()
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

        protected virtual int DetermineRepeatNumber(float deltaTime)
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
        
        protected virtual void Terminate() 
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

        public virtual void Stop()
        {
            Graphics.StopGameLoop();
        }

        protected virtual void CatchException(Exception e)
        {
            CatchNormalError(e);
            Stop();
        }

        protected virtual void CatchNormalError(Exception error)
        {
            Graphics.PrintError(error.GetType().Name, error.Message);
            Rmmz.AudioManager.StopAll();
        }

        protected virtual void UpdateMain()
        {
            UpdateFrameCount();
            UpdateInput();
            ChangeScene();
            UpdateScene();
        }

        protected virtual void UpdateFrameCount()
        {
            UniRmmz.Graphics.FrameCount++;
        }

        protected virtual void UpdateInput()
        {
            Input.Update();
            TouchInput.Update();
        }

        protected virtual void ChangeScene()
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

        protected virtual void UpdateScene()
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

        protected virtual bool IsGameActive()
        {
            return true;
        }

        protected virtual void OnSceneTerminate()
        {
            _scene.gameObject.SetActive(false);
            _previousScene = _scene;
            _previousClass = _previousScene.GetType();
            Graphics.SetStage(null);
        }

        protected virtual void OnSceneCreate()
        {
            Graphics.StartLoading();
        }
        
        protected virtual void OnBeforeSceneStart()
        {
            if (_previousScene != null)
            {
                MonoBehaviour.Destroy(_previousScene.gameObject);
                _previousScene = null;
                Resources.UnloadUnusedAssets();
            }
            
            // TODO effekseer
        }

        protected virtual void OnSceneStart()
        {
            UniRmmz.Graphics.EndLoading();
            UniRmmz.Graphics.SetStage(_scene);
        }

        public virtual bool IsSceneChanging() => _exiting || _nextScene != null;

        protected virtual bool IsCurrentSceneBusy() => _scene?.IsBusy() ?? false;

        public virtual bool IsNextScene(Type sceneClass) => _nextScene != null && sceneClass.IsAssignableFrom(_nextScene.GetType());
        
        public virtual bool IsPreviousScene(Type sceneClass) => sceneClass.IsAssignableFrom(_previousClass);

        // このメソッドは直接呼ばずに、代わりにFactory.csに定義されているメソッドを使用してください
        public virtual void _Goto(Type sceneClass)
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
        public virtual void _Push(Type sceneClass)
        {
            if (_scene != null)
            {
                _stack.Push(_scene.GetType());
            }
            _Goto(sceneClass);
        }

        public virtual void Pop()
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
        public virtual void Exit()
        {
            _Goto(null);
            _exiting = true;
        }

        public virtual void ClearStack()
        {
            _stack.Clear();
        }
        
        public virtual void PrepareNextScene(params object[] objects)
        {
            _nextScene.Prepare(objects);
        }
        
        public virtual Bitmap Snap()
        {
            return Bitmap.Snap(_scene);
        }

        public virtual void SnapForBackground()
        {
            if (_backgroundBitmap != null)
            {
                _backgroundBitmap.Dispose();
            }
            _backgroundBitmap = Snap();
        }

        public virtual Bitmap BackgroundBitmap() => _backgroundBitmap;

        public virtual void Resume()
        {
            TouchInput.Update();
            UniRmmz.Graphics.StartGameLoop();
            Debug.Log("Game loop resumed.");
        }

#region UniRmmz
        public virtual bool CanRenderScene() => _previousScene == null;
#endregion

    }

}