using UnityEngine;

namespace UniRmmz
{
    public interface IEffekseerContainer : IRmmzDrawable2d
    {
        bool NeedDrawEffect();
        int GetEffectLayer();
        
        Vector2 TargetPosition(Camera camera);

        bool IsMirror();
    }
}