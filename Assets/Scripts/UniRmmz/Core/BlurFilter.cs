using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    public partial class BlurFilter : IRmmzFilter
    {
        private Material _material;
        private Sprite.BlendModes _blendMode;
        
        public Sprite.BlendModes BlendMode
        {
            get => _blendMode;
            set
            {
                _blendMode = value;
                switch (_blendMode)
                {
                    case Sprite.BlendModes.Normal:
                        _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        _material.SetInt("_SrcBlendA", (int)UnityEngine.Rendering.BlendMode.One);
                        _material.SetInt("_DstBlendA", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        break;
                        
                    case Sprite.BlendModes.Add:
                        _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        _material.SetInt("_SrcBlendA", (int)UnityEngine.Rendering.BlendMode.One);
                        _material.SetInt("_DstBlendA", (int)UnityEngine.Rendering.BlendMode.One);
                        break;
                    
                    case Sprite.BlendModes.Multiply:
                        _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                        _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        _material.SetInt("_SrcBlendA", (int)UnityEngine.Rendering.BlendMode.One);
                        _material.SetInt("_DstBlendA", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        break;
                    
                    case Sprite.BlendModes.Screen:
                        _material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusDstColor);
                        _material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        _material.SetInt("_SrcBlendA", (int)UnityEngine.Rendering.BlendMode.One);
                        _material.SetInt("_DstBlendA", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        break;
                }
            }
        }

        public BlurFilter()
        {
            _material = new Material(Shader.Find("UniRmmz/BlurFilter"));
        }
        public void Dispose()
        {
            GameObject.Destroy(_material);
        }

        public void SetBlurSize(float size)
        {
            _material.SetFloat("_BlurSize", size);
        }
        
        public void SetBlendColor(Color32 color)
        {
            Color tmp = color;
            _material.SetVector("_BlendColor", tmp);
        }

        public void Bind(RawImage image)
        {
            _material.DisableKeyword("USE_UNIRMMZ_FILTER");
            image.material = _material;
        }
        
        public void Bind(RmmzContainer image)
        {
            if (image is RmmzFilterCanvas)
            {
                _material.EnableKeyword("USE_UNIRMMZ_FILTER");    
            }
            else
            {
                _material.DisableKeyword("USE_UNIRMMZ_FILTER");
            }
            image.material = _material;
        }
    }
}