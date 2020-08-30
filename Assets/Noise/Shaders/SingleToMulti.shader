// Unity based gaussian blue shader

Shader "Hidden/Procedural/SingleToMulti"
{
    Properties
    {
        _MainTex("", any) = "" {}
    }
    CGINCLUDE
#include "UnityCG.cginc"

    struct v2f
    {
        float4 pos : SV_POSITION;
        half4 uv : TEXCOORD0;
    };

    sampler2D _MainTex;

    v2f vert(appdata_img v)
    {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = half4(v.texcoord.xy, 0, 0);
        return o;
    }


    float4 frag(v2f i) : SV_Target
    {
        float r = tex2D(_MainTex, i.uv.xy).r;
        return float4(r, r, r, 1.0);
    }
    ENDCG
    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
#pragma vertex vert
            #pragma fragment frag
            ENDCG
        }

    }
    Fallback off
}