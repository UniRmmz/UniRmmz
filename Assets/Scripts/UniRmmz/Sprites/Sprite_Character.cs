using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The sprite for displaying a character.
    /// </summary>
    public partial class Sprite_Character : Sprite
    {
        private Game_Character _character;
        private int _balloonDuration;
        private int _tilesetId;
        private Sprite _upperBody;
        private Sprite _lowerBody;

        private int _tileId;
        private string _characterName;
        private int _characterIndex;
        private bool _isBigCharacter;
        private int _bushDepth;

        protected override void Awake()
        {
            base.Awake();
            InitMembers();
        }

        protected void InitMembers()
        {
            Anchor = new Vector2(0.5f, 1.0f);
            _character = null;
            _balloonDuration = 0;
            _tilesetId = 0;
            _upperBody = null;
            _lowerBody = null;
        }

        public void SetCharacter(Game_Character character)
        {
            _character = character;
        }

        public bool CheckCharacter(object character)
        {
            return _character == character;
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateBitmap();
            UpdateFrame();
            UpdatePosition();
            UpdateOther();
            UpdateVisibility();
        }

        protected override void UpdateVisibility()
        {
            base.UpdateVisibility();
            if (IsEmptyCharacter() || _character.IsTransparent())
            {
                Visible = false;
            }
        }

        public bool IsTile()
        {
            return _character.IsTile();
        }

        public bool IsObjectCharacter()
        {
            return _character.IsObjectCharacter();
        }

        public bool IsEmptyCharacter()
        {
            return _tileId == 0 && string.IsNullOrEmpty(_characterName);
        }

        public Bitmap TilesetBitmap(int tileId)
        {
            var tileset = Rmmz.gameMap.Tileset();
            int setNumber = 5 + (tileId / 256);
            return Rmmz.ImageManager.LoadTileset(tileset.TilesetNames[setNumber]);
        }

        private void UpdateBitmap()
        {
            if (IsImageChanged())
            {
                _tilesetId = Rmmz.gameMap.TilesetId();
                _tileId = _character.TileId();
                _characterName = _character.CharacterName();
                _characterIndex = _character.CharacterIndex();
                if (_tileId > 0)
                {
                    SetTileBitmap();
                }
                else
                {
                    SetCharacterBitmap();
                }
            }
        }

        private bool IsImageChanged()
        {
            return _tilesetId != Rmmz.gameMap.TilesetId() ||
                   _tileId != _character.TileId() ||
                   _characterName != _character.CharacterName() ||
                   _characterIndex != _character.CharacterIndex();
        }

        private void SetTileBitmap()
        {
            Bitmap = TilesetBitmap(_tileId);
        }

        private void SetCharacterBitmap()
        {
            Bitmap = Rmmz.ImageManager.LoadCharacter(_characterName);
            _isBigCharacter = Rmmz.ImageManager.IsBigCharacter(_characterName);
        }

        private void UpdateFrame()
        {
            if (_tileId > 0)
            {
                UpdateTileFrame();
            }
            else
            {
                UpdateCharacterFrame();
            }
        }

        private void UpdateTileFrame()
        {
            int tileId = _tileId;
            int pw = PatternWidth();
            int ph = PatternHeight();
            int sx = ((tileId / 128) % 2 * 8 + (tileId % 8)) * pw;
            int sy = ((tileId % 256) / 8 % 16) * ph;
            SetFrame(sx, sy, pw, ph);
        }

        private void UpdateCharacterFrame()
        {
            int pw = PatternWidth();
            int ph = PatternHeight();
            int sx = (CharacterBlockX() + CharacterPatternX()) * pw;
            int sy = (CharacterBlockY() + CharacterPatternY()) * ph;
            UpdateHalfBodySprites();
            if (_bushDepth > 0)
            {
                int d = _bushDepth;
                _upperBody.SetFrame(sx, sy, pw, ph - d);
                _lowerBody.SetFrame(sx, sy + ph - d, pw, d);
                SetFrame(sx, sy, 0, ph);
            }
            else
            {
                SetFrame(sx, sy, pw, ph);
            }
        }

        private int CharacterBlockX()
        {
            if (_isBigCharacter)
            {
                return 0;
            }
            else
            {
                int index = _character.CharacterIndex();
                return (index % 4) * 3;
            }
        }

        private int CharacterBlockY()
        {
            if (_isBigCharacter)
            {
                return 0;
            }
            else
            {
                int index = _character.CharacterIndex();
                return (index / 4) * 4;
            }
        }

        private int CharacterPatternX()
        {
            return _character.Pattern();
        }

        private int CharacterPatternY()
        {
            return (_character.Direction() - 2) / 2;
        }

        private int PatternWidth()
        {
            if (_tileId > 0)
            {
                return Rmmz.gameMap.TileWidth();
            }
            else if (_isBigCharacter)
            {
                return Bitmap.Width / 3;
            }
            else
            {
                return Bitmap.Width / 12;
            }
        }

        private int PatternHeight()
        {
            if (_tileId > 0)
            {
                return Rmmz.gameMap.TileHeight();
            }
            else if (_isBigCharacter)
            {
                return Bitmap.Height / 4;
            }
            else
            {
                return Bitmap.Height / 8;
            }
        }

        private void UpdateHalfBodySprites()
        {
            if (_bushDepth > 0)
            {
                CreateHalfBodySprites();
                _upperBody.Bitmap = Bitmap;
                _upperBody.Visible = true;
                _upperBody.Y = -_bushDepth;
                _lowerBody.Bitmap = Bitmap;
                _lowerBody.Visible = true;
                /*
                _upperBody.SetBlendColor(GetBlendColor());
                _lowerBody.SetBlendColor(GetBlendColor());
                _upperBody.SetColorTone(GetColorTone());
                _lowerBody.SetColorTone(GetColorTone());
                _upperBody.BlendMode = blendMode;
                _lowerBody.BlendMode = blendMode;
                */
            }
            else if (_upperBody != null)
            {
                _upperBody.Visible = false;
                _lowerBody.Visible = false;
            }
        }

        private void CreateHalfBodySprites()
        {
            if (_upperBody == null)
            {
                _upperBody = Sprite.Create("upperBody");
                _upperBody.Anchor = new Vector2(0.5f, 1.0f);
                this.AddChild(_upperBody);
            }
            if (_lowerBody == null)
            {
                _lowerBody = Sprite.Create("lowerBody");
                _lowerBody.Anchor = new Vector2(0.5f, 1.0f);
                _lowerBody.Opacity = 128;
                this.AddChild(_lowerBody);
            }
        }

        private void UpdatePosition()
        {
            X = _character.ScreenX();
            Y = _character.ScreenY();
            Z = _character.ScreenZ();
        }

        private void UpdateOther()
        {
            Opacity = _character.Opacity();
            //BlendMode = _character.BlendMode();
            _bushDepth = (int)_character.BushDepth();
        }
    }
}
