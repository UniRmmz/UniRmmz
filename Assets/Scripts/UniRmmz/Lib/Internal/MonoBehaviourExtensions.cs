using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    public static class MonoBehaviourExtensions
    {
        public static T AddChild<T>(this MonoBehaviour self, T child) where T : Component
        {
            child.transform.SetParent(self.transform, false);
            return child;
        }
        
        public static T AddChildToFirst<T>(this MonoBehaviour self, T child) where T : Component
        {
            child.transform.SetParent(self.transform, false);
            child.transform.SetAsFirstSibling();
            return child;
        }
        
        public static void AddChild(this MonoBehaviour self, GameObject child)
        {
            var transform = self.transform;
            if (self is IRmmzDrawable2d drawable)
            {
                transform = drawable.FilterCanvasTransform;
            }
            child.transform.SetParent(transform, false);
        }

        public static void RemoveChild<T>(this MonoBehaviour self, T child) where T : Component
        {
            if (child.transform.IsChildOf(self.transform))
            {
                child.transform.SetParent(null, true);
            }
        }
        
        public static void AddSibling(this MonoBehaviour self, GameObject sibling)
        {
            var transform = self.transform.parent;
            if (self is IRmmzDrawable2d drawable)
            {
                transform = drawable.FilterCanvasTransform.parent;
            }
            sibling.transform.SetParent(transform, false);
        }
        
        public static void RemoveSibling<T>(this MonoBehaviour self, T sibling) where T : Component
        {
            if (sibling.transform.IsChildOf(self.transform.parent))
            {
                sibling.transform.SetParent(null, true);
            }
        }

        public static IEnumerable<T> Children<T>(this MonoBehaviour self) where T : IRmmzDrawable2d
        {
            var t = self.transform; 
            for (int i = 0; i < t.childCount; ++i)
            {
                foreach (var comp in t.GetChild(i).GetComponents<Component>())
                {
                    if (comp is RmmzFilterCanvas filterCanvas && 
                        filterCanvas.FilterContainer is T container)
                    {
                        yield return container;
                    }
                    else if (comp is T drawable)
                    {
                        yield return drawable;
                    }    
                }
            }
        }
    }
}