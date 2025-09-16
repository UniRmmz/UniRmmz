using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

namespace UniRmmz
{
    public abstract partial class Scene_Base : RmmzContainer
    {
        private bool _started = false;
        private bool _active = false;
        private int _fadeSign = 0;
        private int _fadeDuration = 0;
        private int _fadeWhite = 0;
        private float _fadeOpacity = 0;
        private ColorFilter _colorFilter;
        protected WindowLayer _windowLayer;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public virtual void Initialize()
        {
            _started = false;
            _active = false;
            _fadeSign = 0;
            _fadeDuration = 0;
            _fadeWhite = 0;
            _fadeOpacity = 0;
            CreateColorFilter();
        }
        
        public virtual void Prepare(params object[] objects) {}

        public virtual void Create() {}

        public virtual bool IsSceneActive()
        {
            return _active;
        }

        public virtual bool IsReady()
        {
            return Rmmz.ImageManager.IsReady() &&
                   Rmmz.EffectManager.IsReady() &&
                   Rmmz.FontManager.IsReady();
        }

        public virtual void StartScene()
        {
            _started = true;
            _active = true;
        }

        public override void UpdateRmmz()
        {
            UpdateFade();
            UpdateColorFilter();
            base.UpdateRmmz();
        }

        public virtual void StopScene()
        {
            _active = false;
        }

        public virtual bool IsStarted()
        {
            return _started;
        }

        public virtual bool IsBusy()
        {
            return IsFading();
        }

        public virtual bool IsFading()
        {
            return _fadeDuration > 0;
        }

        public virtual void Terminate()
        {
            // Cleanup if needed
        }
        
        protected virtual void CreateWindowLayer()
        {
            _windowLayer = WindowLayer.Create("WindowLayer");
            _windowLayer.X = (float)(UniRmmz.Graphics.Width - UniRmmz.Graphics.BoxWidth) / 2;
            _windowLayer.Y = (float)(UniRmmz.Graphics.Height - UniRmmz.Graphics.BoxHeight) / 2;
            this.AddChild(_windowLayer);
        }

        protected virtual void AddWindow(Window window)
        {
            // 前面にくるウィンドウでクリッピングしたいので、逆順に挿入する
            _windowLayer.AddChildToFirst(window);
        }
        
        protected virtual void AddWindow(Sprite window)
        {
            _windowLayer.AddChild(window);
        }
        
        public void StartFadeIn(int duration, bool white)
        {
            _fadeSign = 1;
            _fadeDuration = duration > 0 ? duration : 30;
            _fadeWhite = white ? 1 : 0;
            _fadeOpacity = 255;
            UpdateColorFilter();
        }

        public void StartFadeOut(int duration, bool white = false)
        {
            _fadeSign = -1;
            _fadeDuration = duration > 0 ? duration : 30;
            _fadeWhite = white ? 1 : 0;
            _fadeOpacity = 0;
            UpdateColorFilter();
        }

        public void CreateColorFilter()
        {
            _colorFilter = new ColorFilter();
            AddFilter(_colorFilter);
        }

        public void UpdateColorFilter()
        {
            int c = _fadeWhite == 1 ? 255 : 0;
            var blendColor = new Color32((byte)c, (byte)c, (byte)c, (byte)_fadeOpacity);
            _colorFilter.SetBlendColor(blendColor);
        }

        public void UpdateFade()
        {
            if (_fadeDuration > 0)
            {
                int d = _fadeDuration;
                if (_fadeSign > 0) 
                {
                    _fadeOpacity -= _fadeOpacity / d;
                }
                else 
                {
                    _fadeOpacity += (255 - _fadeOpacity) / d;
                }
                _fadeDuration--;
            }
        }

        public void PopScene()
        {
            Rmmz.SceneManager.Pop();
        }

        public void CheckGameover()
        {
            // Placeholder for gameover logic
        }

        public void FadeOutAll()
        {
            int time = SlowFadeSpeed();
            StartFadeOut(time, false);
        }

        public int FadeSpeed()
        {
            return 24;
        }

        public int SlowFadeSpeed()
        {
            return FadeSpeed() * 2;
        }

        public void ScaleSprite(Sprite sprite)
        {
            float ratioX = Graphics.Width / sprite.texture.width;
            float ratioY = Graphics.Height / sprite.texture.height;
            float scale = Mathf.Max(ratioX, ratioY, 1.0f);
            sprite.Scale = new Vector3(scale, scale, 1);
        }

        public void CenterSprite(Sprite sprite)
        {
            sprite.X = (float)Graphics.Width / 2;
            sprite.Y = (float)Graphics.Height / 2;
            sprite.Anchor = new Vector2(0.5f, 0.5f);
        }
        
        protected virtual bool IsBottomHelpMode() => true;
        protected virtual bool IsBottomButtonMode() => false;
        
        protected virtual bool IsRightInputMode() => true;
        
        protected virtual int MainCommandWidth() => 240;
        
        public virtual int ButtonAreaTop()
        {
            if (IsBottomButtonMode())
            {
                return Graphics.BoxHeight - ButtonAreaHeight();
            }
            else 
            {
                return 0;
            }
        }

        public virtual int ButtonAreaBottom()
        {
            return ButtonAreaTop() + ButtonAreaHeight();
        }

        public virtual int ButtonAreaHeight()
        {
            return 52;
        }

        public virtual int ButtonY()
        {
            int offsetY = Mathf.FloorToInt((ButtonAreaHeight() - 48) / 2);
            return ButtonAreaTop() + offsetY;
        }

        public int CalcWindowHeight(int numLines, bool selectable)
        {
            if (selectable) 
            {
                return Window_Selectable.Prototype.FittingHeight(numLines);
            }
            else 
            {
                return Window_Base.Prototype.FittingHeight(numLines);
            }
        }

        public void RequestAutosave()
        {
            if (IsAutosaveEnabled())
            {
                ExecuteAutosave();
            }
        }

        public bool IsAutosaveEnabled()
        {
            return true;
        }

        public void ExecuteAutosave()
        {
            Debug.Log("Autosave executed");
        }

        public void OnAutosaveSuccess()
        {
            Debug.Log("Autosave successful");
        }

        public void OnAutosaveFailure()
        {
            Debug.Log("Autosave failed");
        }

    }

}