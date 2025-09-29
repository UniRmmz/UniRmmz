using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The set of sprites on the map screen.
    /// </summary>
    public partial class Spriteset_Map //: Spriteset_Base
    {
        protected List<Sprite_Balloon> _balloonSprites = new();
        protected List<Sprite_Character> _characterSprites;
        protected Tilemap _tilemap;
        protected TilingSprite _parallax;

        protected Sprite _shadowSprite;

        protected Sprite_Destination _destinationSprite;
        protected Weather _weather;
        protected DataTileset _dataTileset;
        protected string _parallaxName;

        public override void Initialize()
        {
            base.Initialize();
            _balloonSprites = new List<Sprite_Balloon>();
        }

        protected override void OnDestroy()
        {
            RemoveAllBalloons();
            base.OnDestroy();
        }

        protected override void LoadSystemImages()
        {
            base.LoadSystemImages();
            Rmmz.ImageManager.LoadSystem("Balloon");
            Rmmz.ImageManager.LoadSystem("Shadow1");
        }

        protected override void CreateLowerLayer()
        {
            base.CreateLowerLayer();
            CreateParallax();
            CreateTilemap();
            CreateCharacters();
            CreateShadow();
            CreateDestination();
            CreateWeather();
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateTileset();
            UpdateParallax();
            UpdateTilemap();
            UpdateShadow();
            UpdateWeather();
            UpdateAnimations();
            UpdateBalloons();
        }

        public void HideCharacters()
        {
            foreach (var sprite in _characterSprites)
            {
                if (!sprite.IsTile() && !sprite.IsObjectCharacter())
                {
                    sprite.Hide();
                }
            }
        }

        protected virtual void CreateParallax()
        {
            _parallax = TilingSprite.Create("parallax");
            _parallax.Move(0, 0, Graphics.Width, Graphics.Height);
            _baseSprite.AddChild(_parallax);
        }

        protected virtual void CreateTilemap()
        {
            _tilemap = Tilemap.Create("tilemap");
            _tilemap.TileWidth = Rmmz.gameMap.TileWidth(); 
            _tilemap.TileHeight = Rmmz.gameMap.TileHeight();
            _tilemap.SetData(Rmmz.gameMap.Width(), Rmmz.gameMap.Height(), Rmmz.gameMap.Data());
            _tilemap.HorizontalWrap = Rmmz.gameMap.IsLoopHorizontal();
            _tilemap.VerticalWrap = Rmmz.gameMap.IsLoopVertical();
            _baseSprite.AddChild(_tilemap);
            _effectsContainer = _tilemap;
            LoadTileset();
        }

        protected virtual void LoadTileset()
        {
            _dataTileset = Rmmz.gameMap.Tileset();
            if (_dataTileset != null)
            {
                var bitmaps = new List<Bitmap>();
                foreach (var name in _dataTileset.TilesetNames)
                {
                    bitmaps.Add(Rmmz.ImageManager.LoadTileset(name));
                }
                _tilemap.SetBitmaps(bitmaps);
                _tilemap.Flags = Rmmz.gameMap.TilesetFlags();
            }
        }

        protected virtual void CreateCharacters()
        {
            _characterSprites = new List<Sprite_Character>();
            foreach (var ev in Rmmz.gameMap.Events())
            {
                var sprite = Sprite_Character.Create(ev.Event().Name);
                sprite.SetCharacter(ev);
                _characterSprites.Add(sprite);
            }
            foreach (var vehicle in Rmmz.gameMap.Vehicles())
            {
                var sprite = Sprite_Character.Create(vehicle.CharacterName());
                sprite.SetCharacter(vehicle);
                _characterSprites.Add(sprite);
            }
            foreach (var follower in Rmmz.gamePlayer.Followers().ReverseData())
            {
                var sprite = Sprite_Character.Create("follower");
                sprite.SetCharacter(follower);
                _characterSprites.Add(sprite);
            }
            
            var player = Sprite_Character.Create("player");
            player.SetCharacter(Rmmz.gamePlayer);
            _characterSprites.Add(player);
            foreach (var sprite in _characterSprites)
            {
                _tilemap.AddChild(sprite);
            }
        }

        protected virtual void CreateShadow()
        {
            _shadowSprite = Sprite.Create("Airship Shadow");
            _shadowSprite.Bitmap = Rmmz.ImageManager.LoadSystem("Shadow1");
            _shadowSprite.Anchor = new Vector2(0.5f, 1.0f);
            _shadowSprite.Z = 6;
            _tilemap.AddChild(_shadowSprite);
        }

        protected virtual void CreateDestination()
        {
            _destinationSprite = Sprite_Destination.Create("destination");
            _destinationSprite.Z = 9;
            _tilemap.AddChild(_destinationSprite);
        }

        protected virtual void CreateWeather()
        {
            _weather = Weather.Create("weather");
            this.AddChild(_weather);
        }

        protected virtual void UpdateTileset()
        {
            if (_dataTileset != Rmmz.gameMap.Tileset())
            {
                LoadTileset();
            }
        }

        protected virtual void UpdateParallax()
        {
            if (_parallaxName != Rmmz.gameMap.ParallaxName())
            {
                _parallaxName = Rmmz.gameMap.ParallaxName();
                _parallax.Bitmap = Rmmz.ImageManager.LoadParallax(_parallaxName);
            }

            if (_parallax.Bitmap != null)
            {
                var bitmap = _parallax.Bitmap;
                _parallax.Origin = new Vector2(
                    Rmmz.gameMap.ParallaxOx() % bitmap.Width,
                    Rmmz.gameMap.ParallaxOy() % bitmap.Height
                );
            }
        }

        protected virtual void UpdateTilemap()
        {
            _tilemap.Origin = new Vector2(
                Rmmz.gameMap.DisplayX() * Rmmz.gameMap.TileWidth(),
                Rmmz.gameMap.DisplayY() * Rmmz.gameMap.TileHeight()
            );
        }

        protected virtual void UpdateShadow()
        {
            var airship = Rmmz.gameMap.Airship();
            _shadowSprite.X = airship.ShadowX();
            _shadowSprite.Y = airship.ShadowY();
            _shadowSprite.Opacity = airship.ShadowOpacity();
        }

        protected virtual void UpdateWeather()
        {
            _weather.Type = Rmmz.gameScreen.WeatherType();
            _weather.Power = Rmmz.gameScreen.WeatherPower();
            _weather.Origin = new Vector2(
                Rmmz.gameMap.DisplayX() * Rmmz.gameMap.TileWidth(),
                Rmmz.gameMap.DisplayY() * Rmmz.gameMap.TileHeight()
            );
        }

        protected virtual void UpdateBalloons()
        {
            for (int i = 0; i <_balloonSprites.Count; )
            {
                if (!_balloonSprites[i].IsPlaying())
                {
                    RemoveBalloon(_balloonSprites[i]);
                }
                else
                {
                    ++i;
                }
            }
            ProcessBalloonRequests();
        }

        protected virtual void ProcessBalloonRequests()
        {
            while (true)
            {
                var request = Rmmz.gameTemp.RetrieveBalloon();
                if (request != null)
                {
                    CreateBalloon(request);
                }
                else
                {
                    break;
                }
            }
        }

        protected virtual void CreateBalloon(Game_Temp.BalloonRequest request)
        {
            var targetSprite = FindTargetSprite(request.Target);
            if (targetSprite != null)
            {
                var sprite = Sprite_Balloon.Create("balloon");
                sprite.TargetObject = request.Target;
                sprite.Setup(targetSprite, request.BalloonId);
                _effectsContainer.AddChild(sprite);
                _balloonSprites.Add(sprite);
            }
        }

        protected virtual void RemoveBalloon(Sprite_Balloon sprite)
        {
            _balloonSprites.Remove(sprite);
            _effectsContainer.RemoveChild(sprite);
            sprite.TargetObject?.EndBalloon();
            GameObject.Destroy(sprite.gameObject);
        }

        protected virtual void RemoveAllBalloons()
        {
            foreach (var sprite in new List<Sprite_Balloon>(_balloonSprites))
            {
                RemoveBalloon(sprite);
            }
        }

        protected override Sprite FindTargetSprite(object target)
        {
            return _characterSprites.Find(sprite => sprite.CheckCharacter(target));
        }

        /*
        protected override int AnimationBaseDelay()
        {
            return 0;
        }
        */
    }
}
