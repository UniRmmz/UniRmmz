using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The superclass of windows for displaying actor status.
    /// </summary>
    public abstract partial class Window_StatusBase : Window_Selectable
    {
        protected Dictionary<string, Sprite> _additionalSprites = new Dictionary<string, Sprite>();

        public override void Initialize(Rect rect)
        {
            base.Initialize(rect);
            _additionalSprites = new Dictionary<string, Sprite>();
            LoadFaceImages();
        }

        protected virtual void LoadFaceImages()
        {
            foreach (Game_Actor actor in Rmmz.gameParty.Members())
            {
                Rmmz.ImageManager.LoadFace(actor.FaceName());
            }
        }

        public override void Refresh()
        {
            HideAdditionalSprites();
            base.Refresh();
        }

        protected virtual void HideAdditionalSprites()
        {
            foreach (var sprite in _additionalSprites.Values)
            {
                sprite.Hide();
            }
        }

        protected virtual void PlaceActorName(Game_Actor actor, int x, int y)
        {
            string key = string.Format("actor{0}-name", actor.ActorId());
            var sprite = CreateInnerSprite<Sprite_Name>(key, n => Sprite_Name.Create(n));
            sprite.Setup(actor);
            sprite.Move(x, y);
            sprite.Show();
        }

        protected virtual void PlaceStateIcon(Game_Actor actor, int x, int y)
        {
            string key = string.Format("actor{0}-stateIcon", actor.ActorId());
            var sprite = CreateInnerSprite<Sprite_StateIcon>(key, n => Sprite_StateIcon.Create(n));
            sprite.Setup(actor);
            sprite.Move(x, y);
            sprite.Show();
        }

        protected virtual void PlaceGauge(Game_Actor actor, string type, int x, int y)
        {
            string key = string.Format("actor{0}-gauge-{1}", actor.ActorId(), type);
            var sprite = CreateInnerSprite<Sprite_Gauge>(key, n => Sprite_Gauge.Create(n));
            sprite.Setup(actor, type);
            sprite.Move(x, y);
            sprite.Show();
        }

        protected virtual T CreateInnerSprite<T>(string key, Func<string, T> factory) where T : Sprite 
        {
            if (_additionalSprites.ContainsKey(key))
            {
                return (T)_additionalSprites[key];
            }
            else
            {
                var sprite = factory(key);
                _additionalSprites[key] = sprite;
                AddInnerChild(sprite);
                return sprite;
            }
        }

        protected virtual void PlaceTimeGauge(Game_Actor actor, int x, int y)
        {
            if (Rmmz.BattleManager.IsTpb())
            {
                PlaceGauge(actor, "time", x, y);
            }
        }

        protected virtual void PlaceBasicGauges(Game_Actor actor, int x, int y)
        {
            PlaceGauge(actor, "hp", x, y);
            PlaceGauge(actor, "mp", x, y + GaugeLineHeight());
            if (Rmmz.DataSystem.OptDisplayTp)
            {
                PlaceGauge(actor, "tp", x, y + GaugeLineHeight() * 2);
            }
        }

        protected virtual int GaugeLineHeight() => 24;

        protected virtual void DrawActorCharacter(Game_Actor actor, int x, int y)
        {
            DrawCharacter(actor.CharacterName(), actor.CharacterIndex(), x, y);
        }

        protected virtual void DrawActorFace(Game_Actor actor, int x, int y, int width = 0, int height = 0)
        {
            DrawFace(actor.FaceName(), actor.FaceIndex(), x, y, width, height);
        }

        protected virtual void DrawActorName(Game_Actor actor, int x, int y, int width = 168)
        {
            ChangeTextColor(Rmmz.ColorManager.HpColor(actor));
            DrawText(actor.Name(), x, y, width);
        }

        protected virtual void DrawActorClass(Game_Actor actor, int x, int y, int width = 168)
        {
            ResetTextColor();
            DrawText(actor.CurrentClass().Name, x, y, width);
        }

        protected virtual void DrawActorNickname(Game_Actor actor, int x, int y, int width = 270)
        {
            ResetTextColor();
            DrawText(actor.Nickname(), x, y, width);
        }

        protected virtual void DrawActorLevel(Game_Actor actor, int x, int y)
        {
            ChangeTextColor(Rmmz.ColorManager.SystemColor());
            DrawText(Rmmz.TextManager.LevelA, x, y, 48);
            ResetTextColor();
            DrawText(actor.Level.ToString(), x + 84, y, 36, Bitmap.TextAlign.Right);
        }

        protected virtual void DrawActorIcons(Game_Actor actor, int x, int y, int width = 144)
        {
            int iconWidth = Rmmz.ImageManager.IconWidth;
            var icons = actor.AllIcons().ToList();
            int maxIcons = Mathf.FloorToInt((float)width / iconWidth);
            int iconX = x;
            
            for (int i = 0; i < Mathf.Min(icons.Count, maxIcons); i++)
            {
                DrawIcon(icons[i], iconX, y + 2);
                iconX += iconWidth;
            }
        }

        protected virtual void DrawActorSimpleStatus(Game_Actor actor, int x, int y)
        {
            int lineHeight = (int)LineHeight();
            int x2 = x + 180;
            DrawActorName(actor, x, y);
            DrawActorLevel(actor, x, y + lineHeight * 1);
            DrawActorIcons(actor, x, y + lineHeight * 2);
            DrawActorClass(actor, x2, y);
            PlaceBasicGauges(actor, x2, y + lineHeight);
        }

        protected virtual string ActorSlotName(Game_Actor actor, int index)
        {
            List<int> slots = actor.EquipSlots();
            return Rmmz.DataSystem.EquipTypes[slots[index]];
        }

    }
}
