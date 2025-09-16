using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The set of sprites on the battle screen.
    /// </summary>
    public partial class Spriteset_Battle : Spriteset_Base
    {
        protected bool _battlebackLocated = false;
        
        protected Sprite _backgroundSprite;
        protected Sprite_Battleback _back1Sprite;
        protected Sprite_Battleback _back2Sprite;
        protected Sprite _battleField;
        protected BlurFilter _backgroundFilter;
        
        protected List<Sprite_Enemy> _enemySprites;
        protected List<Sprite_Actor> _actorSprites;
        
        public virtual List<Sprite_Enemy> EnemySprites => _enemySprites;
        public virtual List<Sprite_Actor> ActorSprites => _actorSprites;
        public virtual Sprite BattleField => _battleField;
        
        public override void Initialize()
        {
            base.Initialize();
            _battlebackLocated = false;
        }

        protected override void LoadSystemImages()
        {
            base.LoadSystemImages();
            Rmmz.ImageManager.LoadSystem("Shadow2");
            Rmmz.ImageManager.LoadSystem("Weapons1");
            Rmmz.ImageManager.LoadSystem("Weapons2");
            Rmmz.ImageManager.LoadSystem("Weapons3");
        }

        protected override void CreateLowerLayer()
        {
            base.CreateLowerLayer();
            CreateBackground();
            CreateBattleback();
            CreateBattleField();
            CreateEnemies();
            CreateActors();
        }

        protected virtual void CreateBackground()
        {
            _backgroundSprite = Sprite.Create("backgroundSprite");
            _backgroundSprite.Bitmap = Rmmz.SceneManager.BackgroundBitmap();
            _backgroundFilter = new BlurFilter();
            _backgroundSprite.AddFilter(_backgroundFilter);
            _baseSprite.AddChild(_backgroundSprite);
        }

        protected virtual void CreateBattleback()
        {
            _back1Sprite = Sprite_Battleback.Create("back1Sprite");
            _back1Sprite.Initialize(0);
            
            _back2Sprite = Sprite_Battleback.Create("back2Sprite");
            _back2Sprite.Initialize(1);
            
            _baseSprite.AddChild(_back1Sprite);
            _baseSprite.AddChild(_back2Sprite);
        }

        protected virtual void CreateBattleField()
        {
            float width = Graphics.BoxWidth;
            float height = Graphics.BoxHeight;
            float x = (Graphics.Width - width) / 2;
            float y = (Graphics.Height - height) / 2;
            
            _battleField = Sprite.Create("battleField");
            _battleField.SetFrame(0, 0, width, height);
            _battleField.X = x;
            _battleField.Y = y - BattleFieldOffsetY();
            _baseSprite.AddChild(_battleField);
            _effectsContainer = _battleField;
        }

        protected virtual float BattleFieldOffsetY()
        {
            return 24;
        }

        public override void UpdateRmmz()
        {
            base.UpdateRmmz();
            UpdateActors();
            UpdateBattleback();
            UpdateAnimations();
        }

        protected virtual void UpdateBattleback()
        {
            if (!_battlebackLocated)
            {
                _back1Sprite.AdjustPosition();
                _back2Sprite.AdjustPosition();
                _battlebackLocated = true;
            }
        }

        protected virtual void CreateEnemies()
        {
            var enemies = Rmmz.gameTroop.Members();
            var sprites = new List<Sprite_Enemy>();
            
            foreach (var enemy in enemies)
            {
                var sprite = Sprite_Enemy.Create($"enemy_{enemy.Index()}");
                sprite.Initialize(enemy);
                sprites.Add(sprite);
            }
            
            // Sort sprites by Y position and sprite ID
            sprites.Sort(CompareEnemySprite);
            
            foreach (var sprite in sprites)
            {
                _battleField.AddChild(sprite);
            }
            
            _enemySprites = sprites;
        }

        protected virtual int CompareEnemySprite(Sprite_Enemy a, Sprite_Enemy b)
        {
            float yA = a.Y;
            float yB = b.Y;
            
            if (yA != yB)
            {
                return yA.CompareTo(yB);
            }
            else
            {
                return b.SpriteId.CompareTo(a.SpriteId);
            }
        }

        protected virtual void CreateActors()
        {
            _actorSprites = new List<Sprite_Actor>();
            
            if (Rmmz.gameSystem.IsSideView())
            {
                for (int i = 0; i < Rmmz.gameParty.MaxBattleMembers(); i++)
                {
                    var sprite = Sprite_Actor.Create($"actor_{i}");
                    _actorSprites.Add(sprite);
                    _battleField.AddChild(sprite);
                }
            }
        }

        protected virtual void UpdateActors()
        {
            var members = Rmmz.gameParty.BattleMembers().ToList();
            for (int i = 0; i < _actorSprites.Count; i++)
            {
                if (i < members.Count)
                {
                    _actorSprites[i].SetBattler(members[i]);
                }
                else
                {
                    _actorSprites[i].SetBattler(null);
                }
            }
        }

        protected override Sprite FindTargetSprite(object target)
        {
            return BattlerSprites().FirstOrDefault(sprite => sprite.CheckBattler(target));
        }

        protected virtual IEnumerable<Sprite_Battler> BattlerSprites()
        {
            foreach (var sprite in _enemySprites)
            {
                yield return sprite;
            }
            foreach (var sprite in _actorSprites)
            {
                yield return sprite;
            }
        }

        public virtual bool IsEffecting()
        {
            return BattlerSprites().Any(sprite => sprite.IsEffecting());
        }

        public virtual bool IsAnyoneMoving()
        {
            return BattlerSprites().Any(sprite => sprite.IsMoving());
        }

        public virtual bool IsBusy()
        {
            return IsAnimationPlaying() || IsAnyoneMoving();
        }
    }
}