using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    /// <summary>
    /// The wrapper class for a follower array.
    /// </summary>
    [Serializable]
    public partial class Game_Followers
    {
        private bool _visible;
        private bool _gathering;
        private List<Game_Follower> _data;

        protected Game_Followers()
        {
            Initialize();
        }

        public void Initialize()
        {
            _visible = Rmmz.dataSystem.OptFollowers;
            _gathering = false;
            _data = new List<Game_Follower>();
            Setup();
        }

        public void Setup()
        {
            _data.Clear();
            for (int i = 1; i < Rmmz.gameParty.MaxBattleMembers(); i++)
            {
                _data.Add(Game_Follower.Create(i));
            }
        }

        public bool IsVisible()
        {
            return _visible;
        }

        public void Show()
        {
            _visible = true;
        }

        public void Hide()
        {
            _visible = false;
        }

        public List<Game_Follower> Data()
        {
            return new List<Game_Follower>(_data);
        }

        public List<Game_Follower> ReverseData()
        {
            var reversedData = new List<Game_Follower>(_data);
            reversedData.Reverse();
            return reversedData;
        }

        public Game_Follower Follower(int index)
        {
            return _data[index];
        }

        public void Refresh()
        {
            foreach (var follower in _data)
            {
                follower.Refresh();
            }
        }

        public void Update()
        {
            if (AreGathering())
            {
                if (!AreMoving())
                {
                    UpdateMove();
                }
                if (AreGathered())
                {
                    _gathering = false;
                }
            }
            foreach (var follower in _data)
            {
                follower.Update();
            }
        }

        public void UpdateMove()
        {
            for (int i = _data.Count - 1; i >= 0; i--)
            {
                Game_Character precedingCharacter = i > 0 ? _data[i - 1] : Rmmz.gamePlayer;
                _data[i].ChaseCharacter(precedingCharacter);
            }
        }

        public void JumpAll()
        {
            if (Rmmz.gamePlayer.IsJumping())
            {
                foreach (var follower in _data)
                {
                    int sx = Rmmz.gamePlayer.DeltaXFrom(follower.X);
                    int sy = Rmmz.gamePlayer.DeltaYFrom(follower.Y);
                    follower.Jump(sx, sy);
                }
            }
        }

        public void Synchronize(int x, int y, int d)
        {
            foreach (var follower in _data)
            {
                follower.Locate(x, y);
                follower.SetDirection(d);
            }
        }

        public void Gather()
        {
            _gathering = true;
        }

        public bool AreGathering()
        {
            return _gathering;
        }

        public IEnumerable<Game_Follower> VisibleFollowers()
        {
            return _data.Where(follower => follower.IsVisible());
        }

        public bool AreMoving()
        {
            return VisibleFollowers().Any(follower => follower.IsMoving());
        }

        public bool AreGathered()
        {
            return VisibleFollowers().All(follower => follower.IsGathered());
        }

        public bool IsSomeoneCollided(int x, int y)
        {
            return VisibleFollowers().Any(follower => follower.Pos(x, y));
        }
    }
}