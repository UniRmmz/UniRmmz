namespace UniRmmz
{
    public interface IAnimationTarget
    {
        void StartAnimation();
        bool IsAnimationPlaying();
        void EndAnimation();
    }
}