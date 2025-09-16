Shader "UniRmmz/ColorFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Hue ("Hue", Float) = 0
        _ColorTone ("Color Tone", Vector) = (0, 0, 0, 0)
        _BlendColor ("Blend Color", Vector) = (0, 0, 0, 0)
        _Brightness ("Brightness", Float) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]
		_SrcBlend("Src Factor", Int) = 1// One
		[Enum(UnityEngine.Rendering.BlendMode)]
		_DstBlend("Dst Factor", Int) = 10// OneMinusSrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)]
		_SrcBlendA("Src Factor Alpha", Int) = 1// One
		[Enum(UnityEngine.Rendering.BlendMode)]
		_DstBlendA("Dst Factor Alpha", Int) = 10// OneMinusSrcAlpha
        
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
        }
        LOD 100
        
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        
        Blend [_SrcBlend][_DstBlend], [_SrcBlendA][_DstBlendA]
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Hue;
            float4 _ColorTone;
            float4 _BlendColor;
            float _Brightness;

            struct appdata
            {
                float4 vertex : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = float4(v.vertex.xy * 2 + float2(-1, 1), 0, 1);
                o.uv = TRANSFORM_TEX(v.vertex.zw, _MainTex);
                return o;
            }

            float3 rgbToHsl(float3 rgb)
            {
                float r = rgb.r;
                float g = rgb.g;
                float b = rgb.b;
                float cmin = min(r, min(g, b));
                float cmax = max(r, max(g, b));
                float delta = cmax - cmin;
                float h = 0;
                float s = 0;
                float l = (cmin + cmax) / 2.0;

                if (delta > 0.0)
                {
                    if (r == cmax)
                        h = fmod((g - b) / delta + 6.0, 6.0) / 6.0;
                    else if (g == cmax)
                        h = ((b - r) / delta + 2.0) / 6.0;
                    else
                        h = ((r - g) / delta + 4.0) / 6.0;

                    if (l < 1.0)
                        s = delta / (1.0 - abs(2.0 * l - 1.0));
                }

                return float3(h, s, l);
            }

            float3 hslToRgb(float3 hsl)
            {
                float h = hsl.x;
                float s = hsl.y;
                float l = hsl.z;
                float c = (1.0 - abs(2.0 * l - 1.0)) * s;
                float x = c * (1.0 - abs(fmod(h * 6.0, 2.0) - 1.0));
                float m = l - c / 2.0;

                if (h < 1.0 / 6.0) return float3(c + m, x + m, m);
                else if (h < 2.0 / 6.0) return float3(x + m, c + m, m);
                else if (h < 3.0 / 6.0) return float3(m, c + m, x + m);
                else if (h < 4.0 / 6.0) return float3(m, x + m, c + m);
                else if (h < 5.0 / 6.0) return float3(x + m, m, c + m);
                else return float3(c + m, m, x + m);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 sample = tex2D(_MainTex, i.uv);
                float alpha = sample.a;

                float3 hsl = rgbToHsl(sample.rgb);
                hsl.x = fmod(hsl.x + _Hue / 360.0, 1.0);
                hsl.y *= (1.0 - _ColorTone.a / 255.0);

                float3 rgb = hslToRgb(hsl);

                float r = rgb.r;
                float g = rgb.g;
                float b = rgb.b;

                float r2 = _ColorTone.r / 255.0;
                float g2 = _ColorTone.g / 255.0;
                float b2 = _ColorTone.b / 255.0;

                float r3 = _BlendColor.r;
                float g3 = _BlendColor.g;
                float b3 = _BlendColor.b;
                float i3 = _BlendColor.a;
                float i1 = 1.0 - i3;

                r = clamp((r / alpha + r2) * alpha, 0.0, 1.0);
                g = clamp((g / alpha + g2) * alpha, 0.0, 1.0);
                b = clamp((b / alpha + b2) * alpha, 0.0, 1.0);

                r = clamp(r * i1 + r3 * i3 * alpha, 0.0, 1.0);
                g = clamp(g * i1 + g3 * i3 * alpha, 0.0, 1.0);
                b = clamp(b * i1 + b3 * i3 * alpha, 0.0, 1.0);

                r *= _Brightness;
                g *= _Brightness;
                b *= _Brightness;

                return float4(r, g, b, alpha);
            }
            ENDCG
        }
    }
}
