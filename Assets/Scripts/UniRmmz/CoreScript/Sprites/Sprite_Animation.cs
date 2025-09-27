using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Effekseer;

namespace UniRmmz
{
    public partial class Sprite_Animation : Sprite_AnimationBase, IEffekseerContainer
    {
        protected List<Sprite> _targets;
        protected DataAnimation _animation;
        protected bool _mirror;
        protected int _delay;
        protected Sprite_AnimationBase _previous;
        protected RmmzEffekseerAssetLoadRequest _effect;
        protected EffekseerHandle _handle;
        protected bool _playing;
        protected bool _started;
        protected int _frameIndex;
        protected int _maxTimingFrames;
        protected Color _flashColor;
        protected float _flashDuration;
        protected int _viewportSize;
        
        public override List<object> TargetObjects { get; set; }

        protected override void InitMembers()
        {
            _targets = new List<Sprite>();
            _animation = null;
            _mirror = false;
            _delay = 0;
            _previous = null;
            _effect = null;
            _handle = new EffekseerHandle();
            _playing = false;
            _started = false;
            _frameIndex = 0;
            _maxTimingFrames = 0;
            _flashColor = new Color(0, 0, 0, 0);
            _flashDuration = 0;
            _viewportSize = 128;//4096;
        }

        protected override void OnDestroy()
        {
            if (_handle.enabled)
            {
                _handle.Stop();
            }

            _effect = null;
            _handle = new EffekseerHandle();
            _playing = false;
            _started = false;
            base.OnDestroy();
        }

        public override void Setup(List<Sprite> targets, DataAnimation animation, bool mirror, int delay, Sprite_AnimationBase previous)
        {
            _targets = targets;
            _animation = animation;
            _mirror = mirror;
            _delay = delay;
            _previous = previous;
            _effect = Rmmz.EffectManager.Load(animation.EffectName);
            _playing = true;

            var timings = animation.SoundTimings.Select(timing => timing.Frame).Concat(animation.FlashTimings.Select(timings => timings.Frame));
            foreach (var timing in timings)
            {
                if (timing > _maxTimingFrames)
                {
                    _maxTimingFrames = timing;
                }
            }
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            if (_delay > 0)
            {
                _delay--;
            }
            else if (_playing)
            {
                if (!_started && CanStart())
                {
                    if (_effect != null)
                    {
                        if (_effect.IsLoaded)
                        {
                            _handle = Graphics.Effekseer.Play(_effect);
                            _started = true;
                        }
                        else
                        {
                            Rmmz.EffectManager.CheckErrors();
                        }
                    }
                    else
                    {
                        _started = true;
                    }
                }

                if (_started)
                {
                    UpdateEffectGeometry();
                    UpdateMain();
                    UpdateFlash();
                }
            }
        }

        private bool CanStart()
        {
            if (_previous != null && ShouldWaitForPrevious())
            {
                return !_previous.IsPlaying();
            }
            else
            {
                return true;
            }
        }

        private bool ShouldWaitForPrevious()
        {
            return false;
        }

        private void UpdateEffectGeometry()
        {
            float scale = _animation.Scale / 100f;
            float r = Mathf.PI / 180f;
            float rx = _animation.Rotation.X * r;
            float ry = _animation.Rotation.Y * r;
            float rz = _animation.Rotation.Z * r;

            if (_handle.enabled)
            {
                _handle.SetLocation(new Vector3(0, 0, 0));
                _handle.SetRotation(Quaternion.Euler(rx, ry, rz));
                _handle.SetScale(new Vector3(scale, scale, scale));
                _handle.speed = _animation.Speed / 100f;
            }
        }

        private void UpdateMain()
        {
            ProcessSoundTimings();
            ProcessFlashTimings();
            _frameIndex++;
            CheckEnd();
        }

        private void ProcessSoundTimings()
        {
            foreach (var timing in _animation.SoundTimings)
            {
                if (timing.Frame == _frameIndex)
                {
                    Rmmz.AudioManager.PlaySe(timing.Se);
                }
            }
        }

        private void ProcessFlashTimings()
        {
            foreach (var timing in _animation.FlashTimings)
            {
                if (timing.Frame == _frameIndex)
                {
                    _flashColor = new Color(timing.Color[0], timing.Color[1], timing.Color[2], timing.Color[3]);
                    _flashDuration = timing.Duration;
                }
            }
        }

        private void CheckEnd()
        {
            if (_frameIndex > _maxTimingFrames &&
                _flashDuration == 0 &&
                !(_handle.enabled && _handle.exists))
            {
                _playing = false;
            }
        }

        private void UpdateFlash()
        {
            if (_flashDuration > 0)
            {
                float d = _flashDuration--;
                _flashColor.a *= (d - 1) / d;
                foreach (var target in _targets)
                {
                    target.GetComponent<Sprite>()?.SetBlendColor(_flashColor);
                }
            }
        }

        public override bool IsPlaying()
        {
            return _playing;
        }

        public void SetRotation(float x, float y, float z)
        {
            if (_handle.enabled)
            {
                _handle.SetRotation(Quaternion.Euler(x, y, z));
            }
        }
        
        private Vector2 TargetSpritePosition(Sprite sprite)
        {
            Vector2 point = new Vector2(0, -sprite.Height / 2);
            if (_animation.AlignBottom)
            {
                point.y = 0;
            }
            // sprite.updateTransform();
            RmmzRoot.Instance.Canvas.RmmzLocalPointInRectangleToScreenPoint(sprite.rectTransform, point.x, point.y, out var screenPoint);
            return screenPoint;
        }

        #region UniRmmz

        public bool IsMirror() => _mirror;

        public Vector2 TargetPosition(Camera camera)
        {
            Vector2 pos = Vector2.zero;
            if (_animation.DisplayType == 2)
            {
                pos.x = camera.pixelWidth / 2;
                pos.y = camera.pixelHeight / 2;
            }
            else
            {
                foreach (var target in _targets)
                {
                    Vector2 tpos = TargetSpritePosition(target);
                    pos.x += tpos.x;
                    pos.y += tpos.y;
                }
                pos.x /= _targets.Count;
                pos.y /= _targets.Count;
            }
            
            return pos + new Vector2(_animation.OffsetX, _animation.OffsetY);
        }

        public bool NeedDrawEffect() => _targets.Count > 0 && _handle.enabled && _handle.exists;
        
        public int GetEffectLayer() => _handle.layer;
        
        #endregion
        
    }
}
