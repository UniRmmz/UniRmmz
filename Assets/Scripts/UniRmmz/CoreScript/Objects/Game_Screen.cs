using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for screen effect data, such as changes in color tone
    /// and flashes.
    /// </summary>
    [Serializable]
    public partial class Game_Screen
    {
        protected float _brightness;
        protected int _fadeOutDuration;
        protected int _fadeInDuration;
        protected Vector4 _tone;
        protected Vector4 _toneTarget;
        protected int _toneDuration;
        protected float[] _flashColor;
        protected int _flashDuration;
        protected int _shakePower;
        protected int _shakeSpeed;
        protected int _shakeDuration;
        protected int _shakeDirection;
        protected float _shake;
        protected float _zoomX;
        protected float _zoomY;
        protected float _zoomScale;
        protected float _zoomScaleTarget;
        protected int _zoomDuration;
        protected Weather.WeatherTypes _weatherType;
        protected float _weatherPower;
        protected float _weatherPowerTarget;
        protected int _weatherDuration;
        protected List<Game_Picture> _pictures;

        protected Game_Screen()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            Clear();
        }

        public virtual void Clear()
        {
            ClearFade();
            ClearTone();
            ClearFlash();
            ClearShake();
            ClearZoom();
            ClearWeather();
            ClearPictures();
        }

        public virtual void OnBattleStart()
        {
            ClearFade();
            ClearFlash();
            ClearShake();
            ClearZoom();
            EraseBattlePictures();
        }

        public virtual int Brightness()
        {
            return (int)_brightness;
        }

        public virtual Vector4 Tone()
        {
            return _tone;
        }

        public virtual Color32 FlashColor()
        {
            return new Color32((byte)_flashColor[0], (byte)_flashColor[1], (byte)_flashColor[2], (byte)_flashColor[3]);
        }

        public virtual float Shake()
        {
            return _shake;
        }

        public virtual float ZoomX()
        {
            return _zoomX;
        }

        public virtual float ZoomY()
        {
            return _zoomY;
        }

        public virtual float ZoomScale()
        {
            return _zoomScale;
        }

        public virtual Weather.WeatherTypes WeatherType()
        {
            return _weatherType;
        }

        public virtual float WeatherPower()
        {
            return _weatherPower;
        }

        public virtual Game_Picture Picture(int pictureId)
        {
            int realPictureId = RealPictureId(pictureId);
            return realPictureId < _pictures.Count ? _pictures[realPictureId] : null;
        }

        public virtual int RealPictureId(int pictureId)
        {
            if (Rmmz.gameParty.InBattle())
            {
                return pictureId + MaxPictures();
            }
            else
            {
                return pictureId;
            }
        }

        public virtual void ClearFade()
        {
            _brightness = 255f;
            _fadeOutDuration = 0;
            _fadeInDuration = 0;
        }

        public virtual void ClearTone()
        {
            _tone = Vector4.zero;
            _toneTarget = Vector4.zero;
            _toneDuration = 0;
        }

        public virtual void ClearFlash()
        {
            _flashColor = new float[]{0, 0, 0, 0};
            _flashDuration = 0;
        }

        public virtual void ClearShake()
        {
            _shakePower = 0;
            _shakeSpeed = 0;
            _shakeDuration = 0;
            _shakeDirection = 1;
            _shake = 0;
        }

        public virtual void ClearZoom()
        {
            _zoomX = 0;
            _zoomY = 0;
            _zoomScale = 1;
            _zoomScaleTarget = 1;
            _zoomDuration = 0;
        }

        public virtual void ClearWeather()
        {
            _weatherType = Weather.WeatherTypes.None;
            _weatherPower = 0;
            _weatherPowerTarget = 0;
            _weatherDuration = 0;
        }

        public virtual void ClearPictures()
        {
            _pictures = new List<Game_Picture>();
        }

        public virtual void EraseBattlePictures()
        {
            _pictures = _pictures.Take(MaxPictures() + 1).ToList();
        }

        public virtual int MaxPictures()
        {
            return 100;
        }

        public virtual void StartFadeOut(int duration)
        {
            _fadeOutDuration = duration;
            _fadeInDuration = 0;
        }

        public virtual void StartFadeIn(int duration)
        {
            _fadeInDuration = duration;
            _fadeOutDuration = 0;
        }

        public virtual void StartTint(Vector4 tone, int duration)
        {
            _toneTarget = tone;
            _toneDuration = duration;
            if (_toneDuration == 0)
            {
                _tone = _toneTarget;
            }
        }

        public virtual void StartFlash(Color32 color, int duration)
        {
            _flashColor = new float[] {color.r, color.g, color.b, color.a};
            _flashDuration = duration;
        }

        public virtual void StartShake(int power, int speed, int duration)
        {
            _shakePower = power;
            _shakeSpeed = speed;
            _shakeDuration = duration;
        }

        public virtual void StartZoom(float x, float y, float scale, int duration)
        {
            _zoomX = x;
            _zoomY = y;
            _zoomScaleTarget = scale;
            _zoomDuration = duration;
        }

        public virtual void SetZoom(float x, float y, float scale)
        {
            _zoomX = x;
            _zoomY = y;
            _zoomScale = scale;
        }

        public virtual void ChangeWeather(Weather.WeatherTypes type, float power, int duration)
        {
            if (type != Weather.WeatherTypes.None || duration == 0)
            {
                _weatherType = type;
            }
            _weatherPowerTarget = type == Weather.WeatherTypes.None ? 0 : power;
            _weatherDuration = duration;
            if (duration == 0)
            {
                _weatherPower = _weatherPowerTarget;
            }
        }

        public virtual void Update()
        {
            UpdateFadeOut();
            UpdateFadeIn();
            UpdateTone();
            UpdateFlash();
            UpdateShake();
            UpdateZoom();
            UpdateWeather();
            UpdatePictures();
        }

        public virtual void UpdateFadeOut()
        {
            if (_fadeOutDuration > 0)
            {
                int d = _fadeOutDuration;
                _brightness = (_brightness * (d - 1)) / d;
                _fadeOutDuration--;
            }
        }

        public virtual void UpdateFadeIn()
        {
            if (_fadeInDuration > 0)
            {
                int d = _fadeInDuration;
                _brightness = (_brightness * (d - 1) + 255) / d;
                _fadeInDuration--;
            }
        }

        public virtual void UpdateTone()
        {
            if (_toneDuration > 0)
            {
                int d = _toneDuration;
                for (int i = 0; i < 4; i++)
                {
                    _tone[i] = (_tone[i] * (d - 1) + _toneTarget[i]) / d;
                }
                _toneDuration--;
            }
        }

        public virtual void UpdateFlash()
        {
            if (_flashDuration > 0)
            {
                int d = _flashDuration;
                _flashColor[3] *= (float)(d - 1) / d;
                _flashDuration--;
            }
        }

        public virtual void UpdateShake()
        {
            if (_shakeDuration > 0 || _shake != 0)
            {
                float delta = (_shakePower * _shakeSpeed * _shakeDirection) / 10f;
                if (_shakeDuration <= 1 && _shake * (_shake + delta) < 0)
                {
                    _shake = 0;
                }
                else
                {
                    _shake += delta;
                }
                if (_shake > _shakePower * 2)
                {
                    _shakeDirection = -1;
                }
                if (_shake < -_shakePower * 2)
                {
                    _shakeDirection = 1;
                }
                _shakeDuration--;
            }
        }

        public virtual void UpdateZoom()
        {
            if (_zoomDuration > 0)
            {
                int d = _zoomDuration;
                float t = _zoomScaleTarget;
                _zoomScale = (_zoomScale * (d - 1) + t) / d;
                _zoomDuration--;
            }
        }

        public virtual void UpdateWeather()
        {
            if (_weatherDuration > 0)
            {
                int d = _weatherDuration;
                float t = _weatherPowerTarget;
                _weatherPower = (_weatherPower * (d - 1) + t) / d;
                _weatherDuration--;
                if (_weatherDuration == 0 && _weatherPowerTarget == 0)
                {
                    _weatherType = Weather.WeatherTypes.None;
                }
            }
        }

        public virtual void UpdatePictures()
        {
            for (int i = 0; i < _pictures.Count; i++)
            {
                if (_pictures[i] != null)
                {
                    _pictures[i].Update();
                }
            }
        }

        public virtual void StartFlashForDamage()
        {
            StartFlash(new Color32(255, 0, 0, 128 ), 8);
        }

        public virtual void ShowPicture(
            int pictureId, string name, int origin, float x, float y, 
            float scaleX, float scaleY, int opacity, int blendMode)
        {
            int realPictureId = RealPictureId(pictureId);
            var picture = Game_Picture.Create();
            picture.Show(name, origin, x, y, scaleX, scaleY, opacity, blendMode);
            
            // Ensure the list has enough capacity
            while (_pictures.Count <= realPictureId)
            {
                _pictures.Add(null);
            }
            _pictures[realPictureId] = picture;
        }

        public virtual void MovePicture(
            int pictureId, int origin, float x, float y, float scaleX, float scaleY,
            float opacity, int blendMode, int duration, int easingType)
        {
            var picture = Picture(pictureId);
            if (picture != null)
            {
                picture.Move(origin, x, y, scaleX, scaleY, opacity, blendMode,
                             duration, easingType);
            }
        }

        public virtual void RotatePicture(int pictureId, float speed)
        {
            var picture = Picture(pictureId);
            if (picture != null)
            {
                picture.Rotate(speed);
            }
        }

        public virtual void TintPicture(int pictureId, Vector4 tone, int duration)
        {
            var picture = Picture(pictureId);
            if (picture != null)
            {
                picture.Tint(tone, duration);
            }
        }

        public virtual void ErasePicture(int pictureId)
        {
            int realPictureId = RealPictureId(pictureId);
            if (realPictureId < _pictures.Count)
            {
                _pictures[realPictureId] = null;
            }
        }
    }
}