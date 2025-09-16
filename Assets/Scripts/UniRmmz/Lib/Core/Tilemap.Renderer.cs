using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniRmmz
{
    public partial class Tilemap
    {
        public class Renderer : IDisposable
        {
            public List<RenderTexture> _internalTextures = new();
            private Material _material;

            public Renderer()
            {
                _material = new Material(Shader.Find("UniRmmz/Tilemap"));
                CreateInternalTextures();
            }

            public void Dispose()
            {
                DestroyInternalTextures();
            }
            
            protected void CreateInternalTextures()
            {
                DestroyInternalTextures();
                for (int i = 0; i < Layer.MaxGlTextures; ++i)
                {
                    var baseTexture = new RenderTexture(2048, 2048, 0, RenderTextureFormat.ARGB32);
                    baseTexture.filterMode = FilterMode.Point;
                    baseTexture.Create();
                    _internalTextures.Add(baseTexture);
                }
            }

            protected void DestroyInternalTextures()
            {
                foreach (var internalTexture in _internalTextures)
                {
                    internalTexture.Release();
                    GameObject.Destroy(internalTexture);
                }
                _internalTextures.Clear();
            }

            public void UpdateTextures(List<Texture> images)
            {
                for (int i = 0; i < images.Count; ++i)
                {
                    var internalTexture = _internalTextures[i >> 2];
                    int x  = 1024 * (i % 2);
                    int y  = 1024 * ((i >> 1) % 2);
                    y += 1024 - images[i].height;// UnityのUV座標系で処理しやすいように調整
                    UnityEngine.Graphics.CopyTexture(images[i], 0, 0, 0, 0, images[i].width, images[i].height,
                        internalTexture, 0, 0, x, y);
                }

                for (int i = 0; i < _internalTextures.Count; ++i)
                {
                    _material.SetTexture($"_Tex{i}", _internalTextures[i]);                    
                }
            }

            public void BindTextures(Tilemap.Layer target)
            {
                target.material = _material;
            }
        }
    }
}