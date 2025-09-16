Shader "UniRmmz/Blit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.BlendMode)]
		_SrcBlend("Src Factor", Int) = 1// One
		[Enum(UnityEngine.Rendering.BlendMode)]
		_DstBlend("Dst Factor", Int) = 10// OneMinusSrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)]
		_SrcBlendA("Src Factor Alpha", Int) = 1// One
		[Enum(UnityEngine.Rendering.BlendMode)]
		_DstBlendA("Dst Factor Alpha", Int) = 10// OneMinusSrcAlpha
        _Offset ("Offset and Scale (x, y, w, h)", Vector) = (0, 0, 1, 1)
        _IsMirror ("Flip X", Int) = 0
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

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Offset;
            int _IsMirror;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = _Offset.xy + _Offset.zw * i.uv;
                if (_IsMirror != 0)
                {
                    uv.x = 1.0 - uv.x;
                }
                if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
                {
                    discard;
                }
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
