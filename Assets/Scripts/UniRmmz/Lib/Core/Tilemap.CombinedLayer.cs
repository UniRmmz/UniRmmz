using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    public partial class Tilemap
    {
        public partial class CombinedLayer : RmmzContainer
        {
            private List<Tilemap.Layer> _children = new();
            
            protected override void Awake()
            {
                base.Awake();
                
                for (int i = 0; i < 2; i++)
                {
                    var layer = Tilemap.Layer.Create();
                    this.AddChild(layer);
                    _children.Add(layer);
                }
            }
            
            public void SetBitmaps(IEnumerable<Bitmap> bitmaps)
            {
                foreach (var child in _children)
                {
                    child.SetBitmaps(bitmaps);
                }
            }
            
            public void Clear()
            {
                foreach (var child in _children)
                {
                    child.Clear();
                }
            }
            
            public int Size()
            {
                int total = 0;
                foreach (var child in _children)
                {
                    total += child.Size();
                }
                return total;
            }
            
            public void AddRect(int setNumber, float sx, float sy, float dx, float dy, float w, float h)
            {
                foreach (var child in _children)
                {
                    if (child.Size() < Layer.MaxSize)
                    {
                        child.AddRect(setNumber, sx, sy, dx, dy, w, h);
                        break;
                    }
                }
            }

            public bool IsReady()
            {
                return _children.All(layer => layer.IsReady());
            }
            
            public void Render(Renderer renderer)
            {
                foreach (var child in _children)
                {
                    child.Render(renderer);
                }
            }
            
            public static CombinedLayer Create(string name = "") => RmmzContainer._Create<CombinedLayer>(name);
        }
    }
}