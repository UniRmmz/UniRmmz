using System;
using UnityEngine;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for the timer.
    /// </summary>
    [Serializable]
    public partial class Game_Timer
    {
        protected int _frames;
        protected bool _working;

        protected Game_Timer()
        {
            _frames = 0;
            _working = false;
        }

        public virtual void Update(bool sceneActive)
        {
            if (sceneActive && _working && _frames > 0)
            {
                _frames--;
                if (_frames == 0)
                {
                    OnExpire();
                }
            }
        }

        public virtual void Start(int count)
        {
            _frames = count;
            _working = true;
        }

        public virtual void Stop()
        {
            _working = false;
        }

        public virtual bool IsWorking()
        {
            return _working;
        }

        public virtual int Seconds()
        {
            return Mathf.FloorToInt(_frames / 60f);
        }

        public virtual int Frames()
        {
            return _frames;
        }

        public virtual void OnExpire()
        {
            Rmmz.BattleManager.Abort();
        }
    }
}