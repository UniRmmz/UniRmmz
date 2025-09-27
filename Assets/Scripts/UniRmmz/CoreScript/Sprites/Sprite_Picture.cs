using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying a picture.
    /// </summary>
    public partial class Sprite_Picture : Sprite_Clickable
    {
        protected int _pictureId;
        protected string _pictureName;

        public virtual void Initialize(int pictureId)
        {
            _pictureId = pictureId;
            _pictureName = "";
            UpdateRmmz();
        }

        public virtual Game_Picture Picture()
        {
            return Rmmz.gameScreen.Picture(_pictureId);
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateBitmap();
            if (Visible)
            {
                UpdateOrigin();
                UpdatePosition();
                UpdateScale();
                UpdateTone();
                UpdateOther();
            }
        }

        protected virtual void UpdateBitmap()
        {
            var picture = Picture();
            if (picture != null)
            {
                string pictureName = picture.Name;
                if (_pictureName != pictureName)
                {
                    _pictureName = pictureName;
                    LoadBitmap();
                }

                Visible = true;
            }
            else
            {
                _pictureName = "";
                Bitmap = null;
                Visible = false;
            }
        }

        protected virtual void UpdateOrigin()
        {
            var picture = Picture();
            if (picture.Origin == 0)
            {
                Anchor = new Vector2(0, 0);
            }
            else
            {
                Anchor = new Vector2(0.5f, 0.5f);
            }
        }

        protected virtual void UpdatePosition()
        {
            var picture = Picture();
            X = (float)Mathf.Round(picture.X);
            Y = (float)Mathf.Round(picture.Y);
        }

        protected virtual void UpdateScale()
        {
            var picture = Picture();
            Scale = new Vector2(picture.ScaleX, picture.ScaleY) / 100f;
        }

        protected virtual void UpdateTone()
        {
            var picture = Picture();
            if (picture.Tone != Vector4.zero)
            {
                SetColorTone(picture.Tone);
            }
            else
            {
                SetColorTone(Vector4.zero);
            }
        }

        protected virtual void UpdateOther()
        {
            var picture = Picture();
            Opacity = picture.Opacity;
            switch (picture.BlendMode)
            {
                case 0:
                    BlendMode = BlendModes.Normal;
                    break;
                
                case 1:
                    BlendMode = BlendModes.Add;
                    break;
                
                case 2:
                    BlendMode = BlendModes.Multiply;
                    break;
                
                case 3:
                    BlendMode = BlendModes.Screen;
                    break;
            }
            
            Rotation = picture.Angle * Mathf.PI / 180f;
        }

        protected virtual void LoadBitmap()
        {
            Bitmap = Rmmz.ImageManager.LoadPicture(_pictureName);
        }
    }

}