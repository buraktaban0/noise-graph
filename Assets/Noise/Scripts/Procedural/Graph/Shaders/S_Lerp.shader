// Unity based gaussian blue shader

Shader "Hidden/Procedural/Lerp" {
    Properties{ _MainTex("", any) = "" {} }
        CGINCLUDE

#include "UnityCG.cginc"
        struct v2f {
        float4 pos : SV_POSITION;
        half4 uv : TEXCOORD0;
    };

    sampler2D _TexA;
    sampler2D _TexB;
    float _Percent;

    v2f vert(appdata_img v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = half4(v.texcoord.xy, 0, 0);
        return o;
    }


    float frag(v2f i) : SV_Target
    {
        return lerp(tex2D(_TexA, i.uv.xy), tex2D(_TexB, i.uv.xy), _Percent);
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