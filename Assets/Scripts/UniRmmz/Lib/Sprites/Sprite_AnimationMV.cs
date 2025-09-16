using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying an old format animation.
    /// </summary>
    public partial class Sprite_AnimationMV : Sprite_AnimationBase
    {
        protected List<Sprite> _targets;
        protected DataAnimation DataAnimation;
        protected bool _mirror;
        protected int _delay;
        protected int _rate;
        protected int _duration;
        protected Color _flashColor;
        protected int _flashDuration;
        protected int _screenFlashDuration;
        protected int _hidingDuration;
        protected int _hue1;
        protected int _hue2;
        protected Bitmap _bitmap1;
        protected Bitmap _bitmap2;
        protected List<Sprite> _cellSprites;
        protected ScreenSprite _screenFlashSprite;
        
        public override List<object> TargetObjects { get; set; }
        
        public class AnimationCellData
        {
            public float pattern;      // パターン番号（0-199: bitmap1, 100-199: bitmap2）
            public float x;          // X座標
            public float y;          // Y座標
            public float scale;      // スケール（100 = 100%）
            public float rotation;   // 回転角度（度）
            public float mirror;      // 水平反転フラグ
            public float opacity;      // 不透明度（0-255）
            public float blendMode;    // ブレンドモード

            public AnimationCellData(float[] param)
            {
                pattern = param[0];
                x = param[1];
                y = param[2];
                scale = param[3];
                rotation = param[4];
                mirror = param[5];
                opacity = param[6];
                blendMode = param[7];
            }
        }

        protected override void InitMembers()
        {
            _targets = new List<Sprite>();
            DataAnimation = null;
            _mirror = false;
            _delay = 0;
            _rate = 4;
            _duration = 0;
            _flashColor = new Color(0, 0, 0, 0);
            _flashDuration = 0;
            _screenFlashDuration = 0;
            _hidingDuration = 0;
            _hue1 = 0;
            _hue2 = 0;
            _bitmap1 = null;
            _bitmap2 = null;
            _cellSprites = new List<Sprite>();
            _screenFlashSprite = null;
            Z = 8;
        }

        public override void Setup(List<Sprite> targets, DataAnimation dataAnimation, bool mirror, int delay, Sprite_AnimationBase _)
        {
            _targets = targets;
            DataAnimation = dataAnimation;
            _mirror = mirror;
            _delay = delay;
            
            if (DataAnimation != null)
            {
                SetupRate();
                SetupDuration();
                LoadBitmaps();
                CreateCellSprites();
                CreateScreenFlashSprite();
            }
        }

        protected virtual void SetupRate()
        {
            _rate = 4;
        }

        protected virtual void SetupDuration()
        {
            _duration = DataAnimation.Frames.Length * _rate + 1;
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateMain();
            UpdateFlash();
            UpdateScreenFlash();
            UpdateHiding();
        }

        protected virtual void UpdateFlash()
        {
            if (_flashDuration > 0)
            {
                float d = _flashDuration--;
                _flashColor.a *= (d - 1) / d;
                
                foreach (var target in _targets)
                {
                    target.SetBlendColor(_flashColor);
                }
            }
        }

        protected virtual void UpdateScreenFlash()
        {
            if (_screenFlashDuration > 0)
            {
                float d = _screenFlashDuration--;
                if (_screenFlashSprite != null)
                {
                    _screenFlashSprite.X = -AbsoluteX();
                    _screenFlashSprite.Y = -AbsoluteY();
                    _screenFlashSprite.Opacity = Mathf.RoundToInt(_screenFlashSprite.Opacity * (d - 1) / d);
                    _screenFlashSprite.Visible = _screenFlashDuration > 0;
                }
            }
        }

        protected virtual float AbsoluteX()
        {
            float x = 0;
            var current = transform;
            while (current != null)
            {
                x += current.localPosition.x;
                current = current.parent;
            }
            
            return x;
        }

        protected virtual float AbsoluteY()
        {
            float y = 0;
            var current = transform;
            while (current != null)
            {
                y += current.localPosition.y;
                current = current.parent;
            }
            
            return -y;
        }

        protected virtual void UpdateHiding()
        {
            if (_hidingDuration > 0)
            {
                _hidingDuration--;
                if (_hidingDuration == 0)
                {
                    foreach (var target in _targets)
                    {
                        target.Show();
                    }
                }
            }
        }

        public override bool IsPlaying()
        {
            return _duration > 0;
        }

        protected virtual void LoadBitmaps()
        {
            if (DataAnimation != null)
            {
                string name1 = DataAnimation.Animation1Name;
                string name2 = DataAnimation.Animation2Name;
                _hue1 = DataAnimation.Animation1Hue;
                _hue2 = DataAnimation.Animation2Hue;
                _bitmap1 = Rmmz.ImageManager.LoadAnimation(name1);
                _bitmap2 = Rmmz.ImageManager.LoadAnimation(name2);
            }
        }

        public virtual bool IsReady()
        {
            return _bitmap1?.IsReady() == true && _bitmap2?.IsReady() == true;
        }

        protected virtual void CreateCellSprites()
        {
            _cellSprites.Clear();
            
            for (int i = 0; i < 16; i++)
            {
                var sprite = Sprite.Create($"cell{i}");
                sprite.Anchor = new Vector2(0.5f, 0.5f);
                _cellSprites.Add(sprite);
                this.AddChild(sprite);
            }
        }

        protected virtual void CreateScreenFlashSprite()
        {
            _screenFlashSprite = ScreenSprite.Create("flash");
            this.AddChild(_screenFlashSprite);
        }

        protected virtual void UpdateMain()
        {
            if (IsPlaying() && IsReady())
            {
                if (_delay > 0)
                {
                    _delay--;
                }
                else
                {
                    _duration--;
                    UpdatePosition();
                    
                    if (_duration % _rate == 0)
                    {
                        UpdateFrame();
                    }
                    
                    if (_duration <= 0)
                    {
                        OnEnd();
                    }
                }
            }
        }

        protected virtual void UpdatePosition()
        {
            if (DataAnimation.Position == 3)
            {
                // Screen center
                X = Graphics.Width / 2f;
                Y = Graphics.Height / 2f;
            }
            else if (_targets.Count > 0)
            {
                var target = _targets[0];
                if (target != null)
                {
                    var targetParent = target.transform.parent;
                    var grandparent = targetParent?.parent;
                    
                    X = target.X;
                    Y = target.Y;
                    
                    if (transform.parent == grandparent)
                    {
                        if (targetParent != null)
                        {
                            X += targetParent.localPosition.x;
                            Y -= targetParent.localPosition.y;
                        }
                    }
                    
                    if (DataAnimation.Position == 0)
                    {
                        Y -= target.Height;
                    }
                    else if (DataAnimation.Position == 1)
                    {
                        Y -= target.Height / 2f;
                    }
                }
            }
        }

        protected virtual void UpdateFrame()
        {
            if (_duration > 0 && DataAnimation?.Frames != null)
            {
                int frameIndex = CurrentFrameIndex();
                
                if (frameIndex >= 0 && frameIndex < DataAnimation.Frames.Length)
                {
                    UpdateAllCellSprites(DataAnimation.Frames[frameIndex]);
                    
                    if (DataAnimation.Timings != null)
                    {
                        foreach (var timing in DataAnimation.Timings)
                        {
                            if (timing.Frame == frameIndex)
                            {
                                ProcessTimingData(timing);
                            }
                        }
                    }
                }
            }
        }

        protected virtual int CurrentFrameIndex()
        {
            if (DataAnimation?.Frames != null)
            {
                return DataAnimation.Frames.Length - Mathf.FloorToInt((_duration + _rate - 1f) / _rate);
            }
            return 0;
        }

        protected virtual void UpdateAllCellSprites(float[][] cells)
        {
            if (_targets.Count > 0 && cells != null)
            {
                for (int i = 0; i < _cellSprites.Count; i++)
                {
                    var sprite = _cellSprites[i];
                    
                    if (i < cells.Length)
                    {
                        var cell = new AnimationCellData(cells[i]);
                        UpdateCellSprite(sprite, cell);
                    }
                    else
                    {
                        sprite.Visible = false;
                    }
                }
            }
        }

        protected virtual void UpdateCellSprite(Sprite sprite, AnimationCellData cell)
        {
            int pattern = (int)cell.pattern;
            
            if (pattern >= 0)
            {
                int sx = (pattern % 5) * 192;
                int sy = Mathf.FloorToInt((pattern % 100) / 5f) * 192;
                bool mirror = _mirror;
                
                sprite.Bitmap = pattern < 100 ? _bitmap1 : _bitmap2;
                sprite.SetHue(pattern < 100 ? _hue1 : _hue2);
                sprite.SetFrame(sx, sy, 192, 192);
                sprite.X = cell.x;
                sprite.Y = cell.y;
                sprite.Rotation = cell.rotation * Mathf.PI / 180f;
                float s = cell.scale / 100f;

                if (cell.mirror != 0)
                {
                    s *= -1;
                }
                
                if (mirror)
                {
                    sprite.X *= -1;
                    sprite.Rotation *= -1;
                    s *= -1;
                }

                sprite.Scale = new Vector2(s, cell.scale / 100f);
                sprite.Opacity = (int)cell.opacity;
                sprite.BlendMode = (BlendModes)cell.blendMode;
                sprite.Visible = true;
            }
            else
            {
                sprite.Visible = false;
            }
        }

        protected virtual void ProcessTimingData(DataAnimationTimingMV timingMv)
        {
            int duration = timingMv.FlashDuration * _rate;
            
            switch (timingMv.FlashScope)
            {
                case 1:
                    {
                        var tmp = new Color(timingMv.FlashColor[0], timingMv.FlashColor[1], timingMv.FlashColor[2], timingMv.FlashColor[3]);
                        StartFlash(tmp, duration);
                    }
                    break;
                case 2:
                    {
                        var tmp = new Color(timingMv.FlashColor[0], timingMv.FlashColor[1], timingMv.FlashColor[2], timingMv.FlashColor[3]);
                        StartScreenFlash(tmp, duration);
                    }
                    break;
                case 3:
                    StartHiding(duration);
                    break;
            }
            
            if (timingMv.Se != null)
            {
                Rmmz.AudioManager.PlaySe(timingMv.Se);
            }
        }

        protected virtual void StartFlash(Color color, int duration)
        {
            _flashColor = color;
            _flashDuration = duration;
        }

        protected virtual void StartScreenFlash(Color color, int duration)
        {
            _screenFlashDuration = duration;
            
            if (_screenFlashSprite != null)
            {
                Color32 color32 = color; 
                _screenFlashSprite.SetColor(color32.r, color32.g, color32.b);
                _screenFlashSprite.Opacity = color32.a;
            }
        }

        protected virtual void StartHiding(int duration)
        {
            _hidingDuration = duration;
            foreach (var target in _targets)
            {
                target.Hide();
            }
        }

        protected virtual void OnEnd()
        {
            _flashDuration = 0;
            _screenFlashDuration = 0;
            _hidingDuration = 0;
            foreach (var target in _targets)
            {
                target.SetBlendColor(new Color(0, 0, 0, 0));
                target.Show();
            }
        }
    }

}