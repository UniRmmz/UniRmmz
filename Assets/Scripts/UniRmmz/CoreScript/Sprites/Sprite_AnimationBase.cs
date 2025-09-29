using System.Collections.Generic;

namespace UniRmmz
{
    public abstract partial class Sprite_AnimationBase //: Sprite
    {
        public abstract List<object> TargetObjects { get; set; }
        
        protected override void Awake()
        {
            base.Awake();
            InitMembers();
        }

        protected abstract void InitMembers();

        public abstract void Setup(List<Sprite> targets, DataAnimation dataAnimation, bool mirror, int delay, Sprite_AnimationBase _);

        public abstract bool IsPlaying();


    }
}