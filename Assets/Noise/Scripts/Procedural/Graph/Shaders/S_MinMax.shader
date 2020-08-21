// Unity based gaussian blue shader

Shader "Hidden/Procedural/MinMax" {
    Properties{ _MainTex("", any) = "" {} }
        CGINCLUDE

#include "UnityCG.cginc"
        struct v2f {
        float4 pos : SV_POSITION;
        half4 uv : TEXCOORD0;
    };



    sampler2D _MainTex;
    float4 _MainTex_ST;
    float4 _MainTex_TexelSize;
    RWStructuredBuffer<float> _Samples;

    v2f vert(appdata_img v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = half4(v.texcoord.xy, 0, 0);
        return o;
    }

#define W 16

    float2 frag_0(v2f i) : SV_Target
    {
        float dx = _MainTex_TexelSize.x * 0.5;
        float4 f = float4(dx, dx, -dx, -dx);
        float2 mm = float2(1024, -1024)
        for (int x = -W; x <= W; x++)
        {
            for (int y = -W; y <= W; y++)
            {
                float2 uv = i.uv + float2(x * dx, y * dx);
                float val = tex2D(_MainTex, uv);
                mm.x = min(mm.x, val);
                mm.y = max(mm.y, val);
            }
        }

        int j = (int)(1024 * )
        _Samples[(int)()]

        /*
    float f0 = tex2D(_MainTex, i.uv + float2(-dx, -dx));
    float f1 = tex2D(_MainTex, i.uv + float2(dx, -dx));
    float f2 = tex2D(_MainTex, i.uv + float2(-dx, dx));
    float f3 = tex2D(_MainTex, i.uv + float2(dx, dx));
    float maximum = f0;
        maximum = max(maximum, f1);
        maximum = max(maximum, f2);
        maximum = max(maximum, f3);

        float minimum = f0;
        minimum = min(minimum, f1);
        minimum = min(minimum, f2);
        minimum = min(minimum, f3);

        return float2(minimum, maximum);*/
    }

        float2 frag_1(v2f i) : SV_Target
    {
        float dx = _MainTex_TexelSize.x * 0.5;
    float2 f0 = tex2D(_MainTex, i.uv + float2(-dx, -dx));
    float2 f1 = tex2D(_MainTex, i.uv + float2(dx, -dx));
    float2 f2 = tex2D(_MainTex, i.uv + float2(-dx, dx));
    float2 f3 = tex2D(_MainTex, i.uv + float2(dx, dx));

    float maximum = f0.y;
    maximum = max(maximum, f1.y);
    maximum = max(maximum, f2.y);
    maximum = max(maximum, f3.y);

    float minimum = f0.x;
    minimum = min(minimum, f1.x);
    minimum = min(minimum, f2.x);
    minimum = min(minimum, f3.x);

    return float2(minimum, maximum);
    }


        ENDCG
        SubShader {
        Pass{
             ZTest Always Cull Off ZWrite Off

             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag_0
             ENDCG
        }

            Pass{
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_1
            ENDCG
        }

    }
    Fallback off
}