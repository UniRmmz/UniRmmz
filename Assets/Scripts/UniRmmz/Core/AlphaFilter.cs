using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    public partial class AlphaFilter : IRmmzFilter
    {
        private Material _material;

        private float _alpha = 1f;
        private float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                _material.SetFloat("_Alpha", _alpha);
            }
        }

        public AlphaFilter()
        {
            _material = new Material(Shader.Find("UniRmmz/AlphaFilter"));
        }
        
        public void Dispose()
        {
            GameObject.Destroy(_material);
        }

        public void Bind(RawImage image)
        {
            image.material = _material;
        }
        
        public void Bind(RmmzContainer image)
        {
            image.material = _material;
        }
    }
}