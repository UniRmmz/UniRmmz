using System;
using System.Collections.Generic;
using System.Linq;

namespace UniRmmz
{
    /// <summary>
    /// The wrapper class for an actor array.
    /// </summary>
    [Serializable]
    public partial class Game_Actors
    {
        protected List<Game_Actor> _data = new();
        
        public virtual Game_Actor Actor(int actorId)
        {
            if (Rmmz.dataActors[actorId] != null)
            {
                while (_data.Count < actorId + 1)
                {
                    _data.Add(null);
                }
                
                if (_data[actorId] == null)
                {
                    _data[actorId] = Game_Actor.Create(actorId);
                }

                return _data[actorId];
            }
            return null;
        }

    }
}