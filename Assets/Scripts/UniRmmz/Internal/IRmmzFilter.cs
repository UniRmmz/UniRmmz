using System;
using UnityEngine.UI;

namespace UniRmmz
{
    public interface IRmmzFilter : IDisposable
    {
        public void Bind(RawImage image);
        
        public void Bind(RmmzContainer image);
    }
}