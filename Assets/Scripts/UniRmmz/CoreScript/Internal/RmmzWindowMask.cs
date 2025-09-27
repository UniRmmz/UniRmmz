using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UniRmmz
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class RmmzWindowMask : UnityEngine.UI.Graphic, IMaterialModifier
    {
#if DEBUG
        protected Color _debugColor = RmmzDebugUtility.GetRandomColor();
#endif
        private Material _maskMaterial;
        private Window _parentWindow;
        protected override void Awake()
        {
            base.Awake();
            
            // 親と大きさを一致させる
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameObject.Destroy(_maskMaterial);
        }
        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            _parentWindow ??= GetComponentInParent<Window>();
            if (_parentWindow.IsClosed())
            {
                return;
            }
            Rect rect = rectTransform.rect;

            Vector2 bottomLeft = new Vector2(rect.xMin, rect.yMin);
            Vector2 topLeft = new Vector2(rect.xMin, rect.yMax);
            Vector2 topRight = new Vector2(rect.xMax, rect.yMax);
            Vector2 bottomRight = new Vector2(rect.xMax, rect.yMin);

            var color = Color.white;
#if DEBUG
            color = _debugColor;
#endif
            color = new Color(color.a, color.g, color.b, 0.5f);
            vh.AddVert(bottomLeft, color, Vector2.zero);
            vh.AddVert(topLeft, color, Vector2.zero);
            vh.AddVert(topRight, color, Vector2.zero);
            vh.AddVert(bottomRight, color, Vector2.zero);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(0, 2, 3);
        }

        public Material GetModifiedMaterial(Material baseMaterial)
        {
            _maskMaterial ??= new Material(Shader.Find("UI/Default"));
            _maskMaterial.CopyPropertiesFromMaterial(baseMaterial);
            _maskMaterial.SetInt("_ColorMask", 0);// カラーバッファは更新しない
            _maskMaterial.SetInt("_Stencil", 1);
            _maskMaterial.SetInt("_StencilComp", (int)CompareFunction.Always);
            _maskMaterial.SetInt("_StencilOp", (int)StencilOp.Replace);
            _maskMaterial.SetInt("_StencilReadMask", 255);
            _maskMaterial.SetInt("_StencilWriteMask", 255);
            
            return _maskMaterial;
        }
        
        #region UniRmmz
        
        public static RmmzWindowMask Create(string name = "") => RmmzWindowMask._Create<RmmzWindowMask>(name);
        public static T _Create<T>(string name = "") where T : Component
        {
            var obj = new GameObject(name);
            obj.layer = Rmmz.RmmzDontRenderLayer;
            return obj.AddComponent<T>();
        }
        
        #endregion
    }
}