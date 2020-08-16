// Unity based gaussian blue shader

Shader "Hidden/Procedural/CurveTransform" {
    Properties{ _MainTex("", any) = "" {} }
        CGINCLUDE

#include "UnityCG.cginc"
        struct v2f {
        float4 pos : SV_POSITION;
        half4 uv : TEXCOORD0;
    };

#pragma multi_compile EVALUATE_ONLY EVALUATE_AND_MULTIPLY

    sampler2D _TexA;

    float _CurveData[1024];
    int _SampleCount;

    float sampleCurve(float xf)
    {
        xf = clamp(xf, 0.0, 0.9999) * (_SampleCount - 1);
        int xi0 = (int)xf;
        return lerp(_CurveData[xi0], _CurveData[xi0 + 1], frac(xf));
    }

    v2f vert(appdata_img v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = half4(v.texcoord.xy, 0, 0);
        return o;
    }


    float frag(v2f i) : SV_Target
    {
        float value = tex2D(_TexA, i.uv.xy);

        float curveValue = sampleCurve(value);

#if EVALUATE_AND_MULTIPLY
        curveValue *= value;
#endif

        return curveValue;
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