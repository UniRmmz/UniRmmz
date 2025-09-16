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
        private string _name;
        private int _origin;
        private float _x;
        private float _y;
        private float _scaleX;
        private float _scaleY;
        private int _opacity;
        private int _blendMode;
        
        private float _targetX;
        private float _targetY;
        private float _targetScaleX;
        private float _targetScaleY;
        private float _targetOpacity;
        private int _duration;
        private int _wholeDuration;
        private int _easingType;
        private float _easingExponent;
        
        private Vector4 _tone;
        private Vector4 _toneTarget;
        private int _toneDuration;
        
        private float _angle;
        private float _rotationSpeed;

        protected Game_Picture()
        {
            Initialize();
        }

        public void Initialize()
        {
            InitBasic();
            InitTarget();
            InitTone();
            InitRotation();
        }

        public string Name => _name;
        public int Origin => _origin;
        public float X => _x;
        public float Y => _y;
        public float ScaleX => _scaleX;
        public float ScaleY => _scaleY;
        public int Opacity => _opacity;
        public int BlendMode => _blendMode;
        public Vector4 Tone => _tone;
        public float Angle => _angle;

        private void InitBasic()
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

        private void InitTarget()
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

        private void InitTone()
        {
            _tone = Vector4.zero;
            _toneTarget = Vector4.zero;
            _toneDuration = 0;
        }

        private void InitRotation()
        {
            _angle = 0;
            _rotationSpeed = 0;
        }

        public void Show(string name, int origin, float x, float y, float scaleX, float scaleY, int opacity, int blendMode)
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

        public void Move(int origin, float x, float y, float scaleX, float scaleY, float opacity, int blendMode, int duration, int easingType)
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

        public void Rotate(float speed)
        {
            _rotationSpeed = speed;
        }

        public void Tint(Vector4 tone, int duration)
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

        public void Update()
        {
            UpdateMove();
            UpdateTone();
            UpdateRotation();
        }

        private void UpdateMove()
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

        private void UpdateTone()
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

        private void UpdateRotation()
        {
            if (_rotationSpeed != 0)
            {
                _angle += _rotationSpeed / 2;
            }
        }

        private float ApplyEasing(float current, float target)
        {
            float d = _duration;
            float wd = _wholeDuration;
            float lt = CalcEasing((wd - d) / wd);
            float t = CalcEasing((wd - d + 1) / wd);
            float start = (current - target * lt) / (1 - lt);
            return start + (target - start) * t;
        }

        private float CalcEasing(float t)
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

        private float EaseIn(float t, float exponent)
        {
            return (float)Math.Pow(t, exponent);
        }

        private float EaseOut(float t, float exponent)
        {
            return 1 - (float)Math.Pow(1 - t, exponent);
        }

        private float EaseInOut(float t, float exponent)
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