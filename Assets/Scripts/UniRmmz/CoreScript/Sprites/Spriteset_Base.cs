using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of Spriteset_Map and Spriteset_Battle.
    /// </summary>
    public partial class Spriteset_Base //: Sprite
    {
        protected List<Sprite_AnimationBase> _animationSprites = new();
        protected Sprite _baseSprite;
        protected ScreenSprite _blackScreen;
        protected ColorFilter _baseColorFilter;
        protected Sprite _pictureContainer;
        protected Sprite_Timer _timerSprite;
        protected ColorFilter _overallColorFilter;
        protected MonoBehaviour _effectsContainer;

        protected override void Awake()
        {
            base.Awake();
            Initialize();
        }

        public virtual void Initialize()
        {
            SetFrame(0, 0, Graphics.Width, Graphics.Height);
            LoadSystemImages();
            CreateLowerLayer();
            CreateUpperLayer();
        }

        protected override void OnDestroy()
        {
            //RemoveAllAnimations();
        }

        protected virtual void LoadSystemImages()
        {
        }

        protected virtual void CreateLowerLayer()
        {
            CreateBaseSprite();
            CreateBaseFilters();
        }

        protected virtual void CreateUpperLayer()
        {
            CreatePictures();
            CreateTimer();
            CreateOverallFilters();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateBaseFilters();
            UpdateOverallFilters();
            UpdatePosition();
            UpdateAnimations();
        }

        protected virtual void CreateBaseSprite()
        {
            _baseSprite = Sprite.Create("baseSprite");
            _blackScreen = ScreenSprite.Create("blackScreen");
            _blackScreen.Opacity = 255;
            _baseSprite.AddChild(_blackScreen);
            this.AddChild(_baseSprite);
        }

        protected virtual void CreateBaseFilters()
        {
            _baseColorFilter = new ColorFilter();
            _baseSprite.AddFilter(_baseColorFilter);
        }

        protected virtual void CreatePictures()
        {
            var rect = PictureContainerRect();
            _pictureContainer = Sprite.Create("pictureContainer");
            _pictureContainer.SetFrame(rect.x, rect.y, rect.width, rect.height);
            for (int i = 1; i <= Rmmz.gameScreen.MaxPictures(); i++)
            {
                var sprite = Sprite_Picture.Create($"picture{i}");
                sprite.Initialize(i);
                _pictureContainer.AddChild(sprite);
            }
            this.AddChild(_pictureContainer);
        }

        protected virtual Rect PictureContainerRect()
        {
            return new Rect(0, 0, Graphics.Width, Graphics.Height);
        }

        protected virtual void CreateTimer()
        {
            _timerSprite = Sprite_Timer.Create("timer");
            this.AddChild(_timerSprite);
        }

        protected virtual void CreateOverallFilters()
        {
            _overallColorFilter = new ColorFilter();
            AddFilter(_overallColorFilter);
        }

        protected virtual void UpdateBaseFilters()
        {
            _baseColorFilter.SetColorTone(Rmmz.gameScreen.Tone());
        }

        protected virtual void UpdateOverallFilters()
        {
            _overallColorFilter.SetBlendColor(Rmmz.gameScreen.FlashColor());
            _overallColorFilter.SetBrightness(Rmmz.gameScreen.Brightness());
        }

        protected virtual void UpdatePosition()
        {
            float scale = Rmmz.gameScreen.ZoomScale();
            Scale = new Vector3(scale, scale, 1);
            X = Mathf.RoundToInt(-Rmmz.gameScreen.ZoomX() * (scale - 1));
            Y = Mathf.RoundToInt(Rmmz.gameScreen.ZoomY() * (scale - 1));
            X += Mathf.RoundToInt(Rmmz.gameScreen.Shake());
        }

        protected virtual Sprite FindTargetSprite(object target)
        {
            return null;
        }

        protected void UpdateAnimations()
        {
            for (int i = _animationSprites.Count - 1; i >= 0; i--)
            {
                var sprite = _animationSprites[i];
                if (!sprite.IsPlaying())
                {
                    RemoveAnimation(sprite);
                }
            }
            ProcessAnimationRequests();
        }

        protected virtual void ProcessAnimationRequests()
        {
            while (true)
            {
                var request = Rmmz.gameTemp.RetrieveAnimation();
                if (request != null)
                {
                    CreateAnimation(request); 
                }
                else
                {
                    break;
                }
            }
        }

        protected virtual void CreateAnimation(Game_Temp.AnimationRequest request)
        {
            var animation = Rmmz.dataAnimations[request.AnimationId];
            var targets = request.Targets;
            bool mirror = request.Mirror;
            int delay = AnimationBaseDelay();
            int nextDelay = AnimationNextDelay();

            if (IsAnimationForEach(animation))
            {
                foreach (var target in targets)
                {
                    CreateAnimationSprite(new List<object> { target }, animation, mirror, delay);
                    delay += nextDelay;
                }
            }
            else
            {
                CreateAnimationSprite(targets, animation, mirror, delay);
            }
        }

        protected virtual void CreateAnimationSprite(List<object> targets, DataAnimation dataAnimation, bool mirror, int delay)
        {
            bool isMV = IsMVAnimation(dataAnimation);
            Sprite_AnimationBase sprite = isMV ? Sprite_AnimationMV.Create("animation") : Sprite_Animation.Create("animation");
            List<Sprite> targetSprites = MakeTargetSprites(targets);
            int baseDelay = AnimationBaseDelay();
            var previous = delay > baseDelay ? LastAnimationSprite() : null;
            if (AnimationShouldMirror(targets[0]))
            {
                mirror = !mirror;
            }

            sprite.TargetObjects = targets;
            sprite.Setup(targetSprites, dataAnimation, mirror, delay, previous);
            _effectsContainer.AddChild(sprite);
            _animationSprites.Add(sprite);
        }

        protected virtual bool IsMVAnimation(DataAnimation dataAnimation)
        {
            return dataAnimation.Frames != null;
        }

        protected virtual List<Sprite> MakeTargetSprites(List<object> targets)
        {
            var result = new List<Sprite>();
            foreach (var target in targets)
            {
                var sprite = FindTargetSprite(target);
                if (sprite != null)
                {
                    result.Add(sprite);
                }
            }
            return result;
        }

        protected virtual Sprite_AnimationBase LastAnimationSprite()
        {
            return _animationSprites.Count > 0 ? _animationSprites[_animationSprites.Count - 1] : null;
        }

        protected virtual bool IsAnimationForEach(DataAnimation dataAnimation)
        {
            return IsMVAnimation(dataAnimation) ? dataAnimation.Position != 3 : dataAnimation.DisplayType == 0;
        }

        protected virtual int AnimationBaseDelay() => 8;

        protected virtual int AnimationNextDelay() => 12;

        protected virtual bool AnimationShouldMirror(object target)
        {
            return target is Game_Actor;
        }

        protected virtual void RemoveAnimation(Sprite_AnimationBase sprite)
        {
            _animationSprites.Remove(sprite);
            _effectsContainer.RemoveChild(sprite);
            foreach (var target in sprite.TargetObjects)
            {
                if (target is IAnimationTarget animationTarget)
                {
                    animationTarget.EndAnimation();
                }
            }
            GameObject.Destroy(sprite.gameObject);
        }

        protected virtual void RemoveAllAnimations()
        {
            var tmp = new List<Sprite_AnimationBase>(_animationSprites);
            foreach (var sprite in tmp)
            {
                RemoveAnimation(sprite);
            }
        }

        public bool IsAnimationPlaying()
        {
            return _animationSprites.Count > 0;
        }
    }
}