using System;
using System.Collections.Generic;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for temporary data that is not included in save data.
    /// </summary>
    public partial class Game_Temp
    {
        protected bool _isPlaytest;
        protected int? _destinationX;
        protected int? _destinationY;
        protected object _touchTarget;
        protected string _touchState;
        protected bool _needsBattleRefresh;
        protected List<int> _commonEventQueue;
        protected List<AnimationRequest> _animationQueue;
        protected List<BalloonRequest> _balloonQueue;
        protected int[] _lastActionData;

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
        
         public virtual bool IsPlaytest()
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
        
        public virtual void SetDestination(int x, int y)
        {
            _destinationX = x;
            _destinationY = y;
        }

        public virtual void ClearDestination()
        {
            _destinationX = null;
            _destinationY = null;
        }

        public virtual bool IsDestinationValid()
        {
            return _destinationX != null;
        }

        public virtual int DestinationX()
        {
            return _destinationX ?? 0;
        }

        public virtual int DestinationY()
        {
            return _destinationY ?? 0;
        }

        public virtual void SetTouchState(object target, string state)
        {
            _touchTarget = target;
            _touchState = state;
        }

        public virtual void ClearTouchState()
        {
            _touchTarget = null;
            _touchState = "";
        }

        public virtual object TouchTarget()
        {
            return _touchTarget;
        }

        public virtual string TouchState()
        {
            return _touchState;
        }

        public virtual void RequestBattleRefresh()
        {
            if (Rmmz.gameParty.InBattle())
            {
                _needsBattleRefresh = true;
            }
        }

        public virtual void ClearBattleRefreshRequest()
        {
            _needsBattleRefresh = false;
        }

        public virtual bool IsBattleRefreshRequested()
        {
            return _needsBattleRefresh;
        }

        public virtual void ReserveCommonEvent(int commonEventId)
        {
            _commonEventQueue.Add(commonEventId);
        }

        public virtual DataCommonEvent RetrieveCommonEvent()
        {
            if (_commonEventQueue.Count > 0)
            {
                int id = _commonEventQueue[0];
                _commonEventQueue.RemoveAt(0);
                return Rmmz.dataCommonEvents[id];
            }
            return null;
        }

        public virtual void ClearCommonEventReservation()
        {
            _commonEventQueue.Clear();
        }

        public virtual bool IsCommonEventReserved()
        {
            return _commonEventQueue.Count > 0;
        }

        public virtual void RequestAnimation(List<object> targets, int animationId, bool mirror = false)
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

        public virtual AnimationRequest RetrieveAnimation()
        {
            if (_animationQueue.Count > 0)
            {
                var request = _animationQueue[0];
                _animationQueue.RemoveAt(0);
                return request;
            }
            return null;
        }

        public virtual void RequestBalloon(Game_CharacterBase target, int balloonId)
        {
            var request = new BalloonRequest
            {
                Target = target,
                BalloonId = balloonId
            };
            
            _balloonQueue.Add(request);
            target.StartBalloon();
        }

        public virtual BalloonRequest RetrieveBalloon()
        {
            if (_balloonQueue.Count > 0)
            {
                var request = _balloonQueue[0];
                _balloonQueue.RemoveAt(0);
                return request;
            }
            return null;
        }

        public virtual int LastActionData(int type)
        {
            return type < _lastActionData.Length ? _lastActionData[type] : 0;
        }

        public virtual void SetLastActionData(int type, int value)
        {
            if (type < _lastActionData.Length)
            {
                _lastActionData[type] = value;
            }
        }

        public virtual void SetLastUsedSkillId(int skillID)
        {
            SetLastActionData(0, skillID);
        }

        public virtual void SetLastUsedItemId(int itemID)
        {
            SetLastActionData(1, itemID);
        }

        public virtual void SetLastSubjectActorId(int actorID)
        {
            SetLastActionData(2, actorID);
        }

        public virtual void SetLastSubjectEnemyIndex(int enemyIndex)
        {
            SetLastActionData(3, enemyIndex);
        }

        public virtual void SetLastTargetActorId(int actorID)
        {
            SetLastActionData(4, actorID);
        }

        public virtual void SetLastTargetEnemyIndex(int enemyIndex)
        {
            SetLastActionData(5, enemyIndex);
        }
    }
}