using System;
using UnityEngine;
using UnityEngine.UI;

namespace UniRmmz
{
    public partial class ColorFilter : IRmmzFilter
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

        public ColorFilter()
        {
            _material = new Material(Shader.Find("UniRmmz/ColorFilter"));
        }
        public void Dispose()
        {
            GameObject.Destroy(_material);
        }

        public void SetHue(float hue)
        {
            _material.SetFloat("_Hue", hue);
        }

        public void SetColorTone(Vector4 colorTone)
        {
            _material.SetVector("_ColorTone", colorTone);
        }
        
        public void SetBlendColor(Color32 color)
        {
            Color tmp = color;
            _material.SetVector("_BlendColor", tmp);
        }
        
        public void SetBrightness(int brightness)
        {
            _material.SetFloat("_Brightness", (float)brightness / 255);
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