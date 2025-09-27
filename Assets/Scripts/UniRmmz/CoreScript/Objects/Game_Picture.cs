using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for a picture.
    /// </summary>
    [Serializable]
    public partial class Game_Picture
    {
        protected string _name;
        protected int _origin;
        protected float _x;
        protected float _y;
        protected float _scaleX;
        protected float _scaleY;
        protected int _opacity;
        protected int _blendMode;
        
        protected float _targetX;
        protected float _targetY;
        protected float _targetScaleX;
        protected float _targetScaleY;
        protected float _targetOpacity;
        protected int _duration;
        protected int _wholeDuration;
        protected int _easingType;
        protected float _easingExponent;
        
        protected Vector4 _tone;
        protected Vector4 _toneTarget;
        protected int _toneDuration;
        
        protected float _angle;
        protected float _rotationSpeed;

        protected Game_Picture()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            InitBasic();
            InitTarget();
            InitTone();
            InitRotation();
        }

        public virtual string Name => _name;
        public virtual int Origin => _origin;
        public virtual float X => _x;
        public virtual float Y => _y;
        public virtual float ScaleX => _scaleX;
        public virtual float ScaleY => _scaleY;
        public virtual int Opacity => _opacity;
        public virtual int BlendMode => _blendMode;
        public virtual Vector4 Tone => _tone;
        public virtual float Angle => _angle;

        protected virtual void InitBasic()
        {
            _name = "";
            _origin = 0;
            _x = 0;
            _y = 0;
            _scaleX = 100;
            _scaleY = 100;
            _opacity = 255;
            _blendMode = 0;
        }

        protected virtual void InitTarget()
        {
            _targetX = _x;
            _targetY = _y;
            _targetScaleX = _scaleX;
            _targetScaleY = _scaleY;
            _targetOpacity = _opacity;
            _duration = 0;
            _wholeDuration = 0;
            _easingType = 0;
            _easingExponent = 0;
        }

        protected virtual void InitTone()
        {
            _tone = Vector4.zero;
            _toneTarget = Vector4.zero;
            _toneDuration = 0;
        }

        protected virtual void InitRotation()
        {
            _angle = 0;
            _rotationSpeed = 0;
        }

        public virtual void Show(string name, int origin, float x, float y, float scaleX, float scaleY, int opacity, int blendMode)
        {
            _name = name;
            _origin = origin;
            _x = x;
            _y = y;
            _scaleX = scaleX;
            _scaleY = scaleY;
            _opacity = opacity;
            _blendMode = blendMode;
            InitTarget();
            InitTone();
            InitRotation();
        }

        public virtual void Move(int origin, float x, float y, float scaleX, float scaleY, float opacity, int blendMode, int duration, int easingType)
        {
            _origin = origin;
            _targetX = x;
            _targetY = y;
            _targetScaleX = scaleX;
            _targetScaleY = scaleY;
            _targetOpacity = opacity;
            _blendMode = blendMode;
            _duration = duration;
            _wholeDuration = duration;
            _easingType = easingType;
            _easingExponent = 2;
        }

        public virtual void Rotate(float speed)
        {
            _rotationSpeed = speed;
        }

        public virtual void Tint(Vector4 tone, int duration)
        {
            if (_tone == Vector4.zero)
            {
                _tone = Vector4.zero;
            }
            _toneTarget = tone;
            _toneDuration = duration;
            if (_toneDuration == 0)
            {
                _tone = _toneTarget;
            }
        }

        public virtual void Update()
        {
            UpdateMove();
            UpdateTone();
            UpdateRotation();
        }

        protected virtual void UpdateMove()
        {
            if (_duration > 0)
            {
                _x = ApplyEasing(_x, _targetX);
                _y = ApplyEasing(_y, _targetY);
                _scaleX = ApplyEasing(_scaleX, _targetScaleX);
                _scaleY = ApplyEasing(_scaleY, _targetScaleY);
                _opacity = Mathf.FloorToInt(ApplyEasing((float)_opacity, (float)_targetOpacity));
                _duration--;
            }
        }

        protected virtual void UpdateTone()
        {
            if (_toneDuration > 0 && _tone != null && _toneTarget != null)
            {
                float d = _toneDuration;
                for (int i = 0; i < 4; i++)
                {
                    _tone[i] = (_tone[i] * (d - 1) + _toneTarget[i]) / d;
                }
                _toneDuration--;
            }
        }

        protected virtual void UpdateRotation()
        {
            if (_rotationSpeed != 0)
            {
                _angle += _rotationSpeed / 2;
            }
        }

        protected virtual float ApplyEasing(float current, float target)
        {
            float d = _duration;
            float wd = _wholeDuration;
            float lt = CalcEasing((wd - d) / wd);
            float t = CalcEasing((wd - d + 1) / wd);
            float start = (current - target * lt) / (1 - lt);
            return start + (target - start) * t;
        }

        protected virtual float CalcEasing(float t)
        {
            float exponent = _easingExponent;
            switch (_easingType)
            {
                case 1: // Slow start
                    return EaseIn(t, exponent);
                case 2: // Slow end
                    return EaseOut(t, exponent);
                case 3: // Slow start and end
                    return EaseInOut(t, exponent);
                default:
                    return t;
            }
        }

        protected virtual float EaseIn(float t, float exponent)
        {
            return (float)Math.Pow(t, exponent);
        }

        protected virtual float EaseOut(float t, float exponent)
        {
            return 1 - (float)Math.Pow(1 - t, exponent);
        }

        protected virtual float EaseInOut(float t, float exponent)
        {
            if (t < 0.5f)
            {
                return EaseIn(t * 2, exponent) / 2;
            }
            else
            {
                return EaseOut(t * 2 - 1, exponent) / 2 + 0.5f;
            }
        }
    }
}