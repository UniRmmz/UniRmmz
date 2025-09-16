Shader "UniRmmz/Tiling"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FrameRect ("Frame (x, y, w, h)", Vector) = (0, 0, 1, 1)
        _Tiling ("Offset and Scale (x, y, w, h)", Vector) = (0, 0, 1, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha 
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _FrameRect;
            float4 _Tiling;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = frac(_Tiling.xy + _Tiling.zw * i.uv);
                uv = _FrameRect.xy + uv * _FrameRect.zw;
                return tex2D(_MainTex, float2(uv.x, 1 - uv.y));
            }
            ENDCG
        }
    }
}
