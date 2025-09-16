using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UniRmmz
{
    public class RmmzTextRenderer
    {
        private TextMeshProUGUI _text;
        private Camera _camera;
        
        public class Context
        {
            public string FontFace;
            public int FontSize = 16;
            public bool FontBold;
            public bool FontItalic;
            public Color TextColor = Color.white;
            public Color OutlineColor = new Color(0, 0, 0, 0.5f);
            public int OutlineWidth = 3;
            public int PaintOpacity = 255;
        }
        
        public RmmzTextRenderer(Camera camera, TextMeshProUGUI text)
        {
            _camera = camera;
            _text = text;
        }
        
        public void DrawText(string content, int x, int y, int maxWidth, int lineHeight, Bitmap.TextAlign align, Context context, RenderTexture texture)
        {
            if (maxWidth == 0)
            {
                maxWidth = MeasureTextWidth(content, context);
            }
            _text.gameObject.SetActive(true);
            
            _text.text = content;
            float tx = x;
            float ty = Mathf.Round(y + lineHeight / 2f /*+ context.FontSize * 0.35f*/);
            _text.lineSpacing = 0;
            _text.lineSpacingAdjustment = lineHeight - context.FontSize;
            switch (align)
            {
                case Bitmap.TextAlign.Left:
                    _text.alignment = TMPro.TextAlignmentOptions.TopLeft;
                    _text.rectTransform.anchorMin = new Vector2(0f, 1f);
                    _text.rectTransform.anchorMax = new Vector2(0f, 1f);
                    _text.rectTransform.pivot = new Vector2(0f, 0.5f);
                    break;
                
                case Bitmap.TextAlign.Center:
                    _text.alignment = TMPro.TextAlignmentOptions.Top;
                    _text.rectTransform.anchorMin = new Vector2(0f, 1f);
                    _text.rectTransform.anchorMax = new Vector2(0f, 1f);
                    _text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    tx += (int)(maxWidth / 2);
                    break;
                
                case Bitmap.TextAlign.Right:
                    _text.alignment = TMPro.TextAlignmentOptions.TopRight;
                    _text.rectTransform.anchorMin = new Vector2(0f, 1f);
                    _text.rectTransform.anchorMax = new Vector2(0f, 1f);
                    _text.rectTransform.pivot = new Vector2(1f, 0.5f);
                    tx += maxWidth;
                    break;
            }
            
            _text.font = Rmmz.FontManager.Font(context.FontFace);
            //_text.fontMaterial = new Material(Shader.Find("TextMeshPro/Distance Field"));
            _text.fontMaterial.mainTexture = _text.font.material.mainTexture;// 何故か明示的にフォントアトラスを設定しないと反映されない
            _text.enableAutoSizing = true;
            _text.fontSizeMax = context.FontSize;

            // Style
            _text.fontStyle = TMPro.FontStyles.Normal;
            if (context.FontBold) _text.fontStyle |= TMPro.FontStyles.Bold;
            if (context.FontItalic) _text.fontStyle |= TMPro.FontStyles.Italic;
            
            // Color
            _text.color = context.TextColor;
            _text.alpha = (float)context.PaintOpacity / 255;
            
            // Outline
            _text.outlineColor = context.OutlineColor;
            var fontSizeFactor = 1f / ((float)context.FontSize / 26);
            switch (context.FontSize)
            {
                case 30:// damage digit
                    _text.outlineWidth = Mathf.Clamp01(context.OutlineWidth * 0.07f * fontSizeFactor);
                    _text.fontMaterial.SetFloat("_FaceDilate", _text.outlineWidth * 2f * fontSizeFactor);
                    break;
                
                case 20:// status digit
                    _text.outlineWidth = Mathf.Clamp01(context.OutlineWidth * 0.11f * fontSizeFactor);
                    _text.fontMaterial.SetFloat("_FaceDilate", _text.outlineWidth * 0.8f * fontSizeFactor);
                    break;
                
                default:// other
                    _text.outlineWidth = Mathf.Clamp01(context.OutlineWidth * 0.03f * fontSizeFactor);
                    _text.fontMaterial.SetFloat("_FaceDilate", _text.outlineWidth * 1.6f * fontSizeFactor);
                    break;
            }

            var size = _text.GetPreferredValues(maxWidth, lineHeight);
            _text.rectTransform.sizeDelta = new Vector2(maxWidth, size.y);
            _text.rectTransform.anchoredPosition = new Vector2(tx, -ty);// UnityとツクールMZではY方向が逆
            
            
            if (texture != null)
            {
                _camera.rect = new Rect(0, 0, 1, 1);
                _camera.clearFlags = CameraClearFlags.Nothing;
                _camera.cullingMask = 1 << Rmmz.RmmzOffscreenLayer;
                _camera.targetTexture = texture;
                _camera.Render();
                _camera.targetTexture = null;
            }
            
            _text.gameObject.SetActive(false);
        }
        
        public int MeasureTextWidth(string content, Context context)
        {
            // スペース単体だと文字サイズが正しく取れない問題のワークアラウンド
            if (content == " ")
            {
                return MeasureTextWidth("M M", context) - MeasureTextWidth("MM", context);
            }
            else if (content == "　")
            {
                return MeasureTextWidth("M　M", context) - MeasureTextWidth("MM", context);
            }
            
            _text.text = content;
            _text.font = Rmmz.FontManager.Font(context.FontFace);
            _text.enableAutoSizing = false;
            _text.fontSize = context.FontSize;
            
            // Style
            _text.fontStyle = TMPro.FontStyles.Normal;
            if (context.FontBold) _text.fontStyle |= TMPro.FontStyles.Bold;
            if (context.FontItalic) _text.fontStyle |= TMPro.FontStyles.Italic;
            
            return Mathf.CeilToInt(_text.preferredWidth);
        }
    }
}