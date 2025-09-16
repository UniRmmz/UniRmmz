using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace UniRmmz
{
    public partial class Tilemap
    {
        public class Layer : RmmzContainer
        {
            public const int MaxGlTextures = 3;
            public const int MaxSize = 16000;
            private List<float[]> _elements = new List<float[]>();
            private bool _needsTexturesUpdate = false;
            private bool _needsVertexUpdate = false;
            private List<Texture> _images = new List<Texture>();

            public void SetBitmaps(IEnumerable<Bitmap> bitmaps)
            {
                _images = bitmaps.Select(bitmap => bitmap.BaseTexture).ToList();
                _needsTexturesUpdate = true;
            }

            public void Clear()
            {
                _elements.Clear();
                _needsVertexUpdate = true;
            }

            public int Size()
            {
                return _elements.Count;
            }

            public void AddRect(int setNumber, float sx, float sy, float dx, float dy, float w, float h)
            {
                _elements.Add(new float[] { setNumber, sx, sy, dx, dy, w, h });
            }

            public void Render(Tilemap.Renderer renderer)
            {
                if (_needsTexturesUpdate)
                {
                    renderer.UpdateTextures(_images);
                    _needsTexturesUpdate = false;
                }
                renderer.BindTextures(this);
                
                if (this._needsVertexUpdate) {
                    SetVerticesDirty();
                    this._needsVertexUpdate = false;
                }
            }

            public bool IsReady()
            {
                if (_images.Count == 0)
                {
                    return false;
                }

                foreach (var image in _images)
                {
                    if (image == null)
                    {
                        return false;
                    }
                }

                return true;
            }

            protected override void OnPopulateMesh(VertexHelper vh)
            {
                vh.Clear();
                Rect rect = rectTransform.rect;
                
                int index = 0;
                foreach (var item in _elements)
                {
                    int setNumber = (int)item[0];
                    int tid = setNumber >> 2;
                    float sxOffset = 1024f * (setNumber & 1);
                    float syOffset = 1024f * ((setNumber >> 1) & 1);
                    float sx = item[1] + sxOffset;
                    float sy = 1024 - item[2] + syOffset;
                    float dx = item[3];
                    float dy = - (item[4] + item[6]);
                    float w = item[5];
                    float h = item[6];
                    float frameLeft = (sx + 0.5f) / 2048;
                    float frameTop = (sy - 0.5f) / 2048;
                    float frameRight = (sx + w - 0.5f) / 2048;
                    float frameBottom = (sy - h + 0.5f) / 2048;

                    var bottomLeft = new Vector2(dx, dy);
                    var topLeft = new Vector2(dx, dy + h);
                    var topRight = new Vector2(dx + w, dy + h);
                    var bottomRight = new Vector2(dx + w, dy);

                    vh.AddVert(bottomLeft, Color.white, new Vector3(frameLeft, frameBottom, tid));
                    vh.AddVert(topLeft, Color.white, new Vector3(frameLeft, frameTop, tid));
                    vh.AddVert(topRight, Color.white, new Vector3(frameRight, frameTop, tid));
                    vh.AddVert(bottomRight, Color.white, new Vector3(frameRight, frameBottom, tid));

                    int vertIndex = index * 4;
                    vh.AddTriangle(vertIndex, vertIndex + 1, vertIndex + 2);
                    vh.AddTriangle(vertIndex, vertIndex + 2, vertIndex + 3);
                    index++;
                }
            }
            
            public static Layer Create(string name = "") => RmmzContainer._Create<Layer>(name);
        }
    }
}