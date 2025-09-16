Shader "UniRmmz/AlphaFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha ("Alpha", Float) = 1
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
            float _Alpha;

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

            fixed4 frag (v2f i) : SV_Target
            {
                float4 sample = tex2D(_MainTex, i.uv);
                float alpha = sample.a * _Alpha;

                return float4(sample.rgb, alpha);
            }
            ENDCG
        }
    }
}
