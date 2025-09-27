using System.Collections.Generic;

namespace UniRmmz
{
    /// <summary>
    /// The game object class for a common event. It contains functionality for
    /// running parallel process events.
    /// </summary>
    public partial class Game_CommonEvent
    {
        protected int _commontEventId;
        protected Game_Interpreter _interpreter;
        
        protected Game_CommonEvent(int commonEventId)
        {
            _commontEventId = commonEventId;
            Refresh();
        }

        public virtual DataCommonEvent Event()
        {
            return Rmmz.dataCommonEvents[_commontEventId];
        }

        public virtual List<DataEventCommand> List()
        {
            return Event().List;
        }

        public virtual void Refresh()
        {
            if (IsActive())
            {
                if (_interpreter == null)
                {
                    _interpreter = Game_Interpreter.Create();
                }
            }
            else
            {
                _interpreter = null;
            }
        }

        public virtual bool IsActive()
        {
            var ev = Event();
            return ev.Trigger == 2 && Rmmz.gameSwitches.Value(ev.SwitchId);
        }

        public virtual void Update()
        {
            if (_interpreter != null)
            {
                if (!_interpreter.IsRunning())
                {
                    _interpreter.Setup(List());
                }

                _interpreter.Update();
            }
        }
        
    }
}