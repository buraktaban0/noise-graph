// Unity based gaussian blue shader

Shader "Hidden/Procedural/Multiply" {
    Properties{ _MainTex("", any) = "" {} }
        CGINCLUDE

#include "UnityCG.cginc"
        struct v2f {
        float4 pos : SV_POSITION;
        half4 uv : TEXCOORD0;
    };

#pragma multi_compile TEX_MODE FLOAT_MODE

    sampler2D _TexA;

#if TEX_MODE
    sampler2D _TexB;
#elif FLOAT_MODE
    float _Factor;
#endif

    v2f vert(appdata_img v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = half4(v.texcoord.xy, 0, 0);
        return o;
    }


    float frag(v2f i) : SV_Target
    {
        float value = tex2D(_TexA, i.uv.xy);

#if TEX_MODE
        value *= tex2D(_TexB, i.uv.xy);
#elif FLOAT_MODE
        value *= _Factor;
#endif

        return value;
    }

        ENDCG
        SubShader {
        Pass{
             ZTest Always Cull Off ZWrite Off

             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             ENDCG
        }

    }
    Fallback off
}