Shader "UniRmmz/BlurFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 3
        _KernelSize ("Kernel Size", Int) = 5
        _BlendColor ("Blend Color", Vector) = (0, 0, 0, 0)
        [Enum(UnityEngine.Rendering.BlendMode)]
		_SrcBlend("Src Factor", Int) = 1// One
		[Enum(UnityEngine.Rendering.BlendMode)]
		_DstBlend("Dst Factor", Int) = 10// OneMinusSrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)]
		_SrcBlendA("Src Factor Alpha", Int) = 1// One
		[Enum(UnityEngine.Rendering.BlendMode)]
		_DstBlendA("Dst Factor Alpha", Int) = 10// OneMinusSrcAlpha
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
            #pragma multi_compile_local _ USE_UNIRMMZ_FILTER

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurSize;
            int _KernelSize;
            float4 _BlendColor;

            struct appdata
            {
#ifdef USE_UNIRMMZ_FILTER
                float4 vertex : TEXCOORD0;
                float4 color : COLOR;
#else
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 uv : TEXCOORD0;
#endif
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.color = v.color;
#ifdef USE_UNIRMMZ_FILTER
                o.vertex = float4(v.vertex.xy * 2 + float2(-1, 1), 0, 1);
                o.uv = TRANSFORM_TEX(v.vertex.zw, _MainTex);
#else
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
#endif
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 col = float4(0,0,0,0);
                float2 offset = _BlurSize / _ScreenParams.xy;

                int halfKernel = _KernelSize / 2;
                float sigma = _BlurSize / 2;
                float totalWeight = 0;

                for (int x = -halfKernel; x <= halfKernel; x++)
                {
                    for (int y = -halfKernel; y <= halfKernel; y++)
                    {
                        float weight = exp(-(x*x + y*y) / (2.0 * sigma * sigma));
                        float2 offsetCoord = uv + float2(x, y) * offset;
                        col += tex2D(_MainTex, offsetCoord) * weight;
                        totalWeight += weight;
                    }
                }

                col /= totalWeight;

                col.rgb = lerp(col.rgb, _BlendColor.rgb, _BlendColor.a);
                return col;
            }

            ENDCG
        }
    }
}
