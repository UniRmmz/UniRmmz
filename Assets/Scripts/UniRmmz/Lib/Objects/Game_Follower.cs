using System;
using System.Linq;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for a follower. A follower is an allied character,
    /// other than the front character, displayed in the party.
    /// </summary>
    [Serializable]
    public partial class Game_Follower : Game_Character
    {
        private int _memberIndex;

        protected Game_Follower(int memberIndex) : base()
        {
            _memberIndex = memberIndex;
            SetTransparent(Rmmz.dataSystem.OptTransparent);
            SetThrough(true);
        }

        public void Refresh()
        {
            string characterName = IsVisible() ? Actor().CharacterName() : "";
            int characterIndex = IsVisible() ? Actor().CharacterIndex() : 0;
            SetImage(characterName, characterIndex);
        }

        public Game_Actor Actor()
        {
            return Rmmz.gameParty.BattleMembers().ElementAtOrDefault(_memberIndex);
        }

        public bool IsVisible()
        {
            return Actor() != null && Rmmz.gamePlayer.Followers().IsVisible();
        }

        public bool IsGathered()
        {
            return !IsMoving() && Pos(Rmmz.gamePlayer.X, Rmmz.gamePlayer.Y);
        }

        public override void Update()
        {
            base.Update();
            SetMoveSpeed(Rmmz.gamePlayer.RealMoveSpeed());
            SetOpacity(Rmmz.gamePlayer.Opacity());
            SetBlendMode(Rmmz.gamePlayer.BlendMode());
            SetWalkAnime(Rmmz.gamePlayer.HasWalkAnime());
            SetStepAnime(Rmmz.gamePlayer.HasStepAnime());
            SetDirectionFix(Rmmz.gamePlayer.IsDirectionFixed());
            SetTransparent(Rmmz.gamePlayer.IsTransparent());
        }

        public void ChaseCharacter(Game_Character character)
        {
            int sx = DeltaXFrom(character.X);
            int sy = DeltaYFrom(character.Y);
            if (sx != 0 && sy != 0)
            {
                MoveDiagonally(sx > 0 ? 4 : 6, sy > 0 ? 8 : 2);
            }
            else if (sx != 0)
            {
                MoveStraight(sx > 0 ? 4 : 6);
            }
            else if (sy != 0)
            {
                MoveStraight(sy > 0 ? 8 : 2);
            }
            SetMoveSpeed(Rmmz.gamePlayer.RealMoveSpeed());
        }
    }
}