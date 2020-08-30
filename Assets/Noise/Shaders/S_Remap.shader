// Unity based gaussian blue shader

Shader "Hidden/Procedural/Remap" {
    Properties{ _MainTex("", any) = "" {} }
        CGINCLUDE

#include "UnityCG.cginc"
        struct v2f {
        float4 pos : SV_POSITION;
        half4 uv : TEXCOORD0;
    };

    sampler2D _TexA;

    float4 _Map;

    v2f vert(appdata_img v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = half4(v.texcoord.xy, 0, 0);
        return o;
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1)*(b2 - b1) / (a2 - a1);
    }

    float frag(v2f i) : SV_Target
    {
        return map(tex2D(_TexA, i.uv.xy), _Map.x, _Map.y, _Map.z, _Map.w);
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