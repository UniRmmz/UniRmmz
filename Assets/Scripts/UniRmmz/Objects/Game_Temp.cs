using System;
using System.Collections.Generic;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for temporary data that is not included in save data.
    /// </summary>
    public partial class Game_Temp
    {
        private bool _isPlaytest;
        private int? _destinationX;
        private int? _destinationY;
        private object _touchTarget;
        private string _touchState;
        private bool _needsBattleRefresh;
        private List<int> _commonEventQueue;
        private List<AnimationRequest> _animationQueue;
        private List<BalloonRequest> _balloonQueue;
        private int[] _lastActionData;

        public class BalloonRequest
        {
            public Game_CharacterBase Target;
            public int BalloonId;
        }
        
        public class AnimationRequest
        {
            public List<object> Targets;
            public int AnimationId;
            public bool Mirror;
        };
        
        protected Game_Temp()
        {
            //_isPlaytest = Utils.IsOptionValid("test");
            _destinationX = null;
            _destinationY = null;
            _touchTarget = null;
            _touchState = "";
            _needsBattleRefresh = false;
            _commonEventQueue = new List<int>();
            _animationQueue = new List<AnimationRequest>();
            _balloonQueue = new List<BalloonRequest>();
            _lastActionData = new int[6] { 0, 0, 0, 0, 0, 0 };
        }
        
        public bool IsPlaytest()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
        
        public void SetDestination(int x, int y)
        {
            _destinationX = x;
            _destinationY = y;
        }

        public void ClearDestination()
        {
            _destinationX = null;
            _destinationY = null;
        }

        public bool IsDestinationValid()
        {
            return _destinationX != null;
        }

        public int DestinationX()
        {
            return _destinationX ?? 0;
        }

        public int DestinationY()
        {
            return _destinationY ?? 0;
        }

        public void SetTouchState(object target, string state)
        {
            _touchTarget = target;
            _touchState = state;
        }

        public void ClearTouchState()
        {
            _touchTarget = null;
            _touchState = "";
        }

        public object TouchTarget()
        {
            return _touchTarget;
        }

        public string TouchState()
        {
            return _touchState;
        }

        public void RequestBattleRefresh()
        {
            if (Rmmz.gameParty.InBattle())
            {
                _needsBattleRefresh = true;
            }
        }

        public void ClearBattleRefreshRequest()
        {
            _needsBattleRefresh = false;
        }

        public bool IsBattleRefreshRequested()
        {
            return _needsBattleRefresh;
        }

        public void ReserveCommonEvent(int commonEventId)
        {
            _commonEventQueue.Add(commonEventId);
        }

        public DataCommonEvent RetrieveCommonEvent()
        {
            if (_commonEventQueue.Count > 0)
            {
                int id = _commonEventQueue[0];
                _commonEventQueue.RemoveAt(0);
                return Rmmz.dataCommonEvents[id];
            }
            return null;
        }

        public void ClearCommonEventReservation()
        {
            _commonEventQueue.Clear();
        }

        public bool IsCommonEventReserved()
        {
            return _commonEventQueue.Count > 0;
        }

        public void RequestAnimation(List<object> targets, int animationId, bool mirror = false)
        {
            if (Rmmz.dataAnimations[animationId] != null)
            {
                var request = new AnimationRequest
                {
                    Targets = targets,
                    AnimationId = animationId,
                    Mirror = mirror
                };
                
                _animationQueue.Add(request);
                
                foreach (var target in targets)
                {
                    if (target is Game_CharacterBase animatable)
                    {
                        animatable.StartAnimation();
                    }
                }
            }
        }

        public AnimationRequest RetrieveAnimation()
        {
            if (_animationQueue.Count > 0)
            {
                var request = _animationQueue[0];
                _animationQueue.RemoveAt(0);
                return request;
            }
            return null;
        }

        public void RequestBalloon(Game_CharacterBase target, int balloonId)
        {
            var request = new BalloonRequest
            {
                Target = target,
                BalloonId = balloonId
            };
            
            _balloonQueue.Add(request);
            target.StartBalloon();
        }

        public BalloonRequest RetrieveBalloon()
        {
            if (_balloonQueue.Count > 0)
            {
                var request = _balloonQueue[0];
                _balloonQueue.RemoveAt(0);
                return request;
            }
            return null;
        }

        public int LastActionData(int type)
        {
            return type < _lastActionData.Length ? _lastActionData[type] : 0;
        }

        public void SetLastActionData(int type, int value)
        {
            if (type < _lastActionData.Length)
            {
                _lastActionData[type] = value;
            }
        }

        public void SetLastUsedSkillId(int skillID)
        {
            SetLastActionData(0, skillID);
        }

        public void SetLastUsedItemId(int itemID)
        {
            SetLastActionData(1, itemID);
        }

        public void SetLastSubjectActorId(int actorID)
        {
            SetLastActionData(2, actorID);
        }

        public void SetLastSubjectEnemyIndex(int enemyIndex)
        {
            SetLastActionData(3, enemyIndex);
        }

        public void SetLastTargetActorId(int actorID)
        {
            SetLastActionData(4, actorID);
        }

        public void SetLastTargetEnemyIndex(int enemyIndex)
        {
            SetLastActionData(5, enemyIndex);
        }
    }
}