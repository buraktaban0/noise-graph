﻿// Unity based gaussian blue shader

Shader "Hidden/Procedural/Perlin" {
    Properties{ _MainTex("", any) = "" {} }
        CGINCLUDE

#include "UnityCG.cginc"
        struct v2f {
        float4 pos : SV_POSITION;
        half4 uv : TEXCOORD0;
    };

   static int p[] = { 151, 160, 137, 91, 90, 15,
        131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
        190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
        88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
        77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
        102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
        135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
        5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
        223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
        129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
        251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
        49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
        138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
        151, 160, 137, 91, 90, 15,
        131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
        190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
        88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
        77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
        102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
        135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
        5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
        223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
        129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
        251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
        49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
        138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
    };

    float _Frequency;
    float3 _Offset;

    v2f vert(appdata_img v) {
        v2f o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = half4(v.texcoord.xy, 0, 0);
        return o;
    }

    float grad(int hash, float x, float y, float z)
    {

        int h = hash & 15;
        float u = h < 8 ? x : y;

        float v = h < 4 ? y : h == 12 || h == 14 ? x : z;

        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    float perlin(float3 xyz)
    {
        int3 xyzi = int3(xyz);
        float3 xyzf = xyz - xyzi;
        float3 uvw = xyzf * xyzf * xyzf * (xyzf * (xyzf * 6 - 15) + 10);

        int a = p[xyzi.x] + xyzi.y;
        int aa = p[a] + xyzi.z;
        int ab = p[a + 1] + xyzi.z;
        int b = p[xyzi.x + 1] + xyzi.y;
        int ba = p[b] + xyzi.z;
        int bb = p[b + 1] + xyzi.z;

        float x1, x2, y1, y2;
        x1 = lerp(grad(p[aa], xyzf.x, xyzf.y, xyzf.z),
            grad(p[ba], xyzf.x - 1, xyzf.y, xyzf.z),
            uvw.x);
        x2 = lerp(grad(p[ab], xyzf.x, xyzf.y - 1, xyzf.z),
            grad(p[bb], xyzf.x - 1, xyzf.y - 1, xyzf.z),
            uvw.x);
        y1 = lerp(x1, x2, uvw.y);

        x1 = lerp(grad(p[aa + 1], xyzf.x, xyzf.y, xyzf.z - 1),
            grad(p[ba + 1], xyzf.x - 1, xyzf.y, xyzf.z - 1),
            uvw.x);
        x2 = lerp(grad(p[ab + 1], xyzf.x, xyzf.y - 1, xyzf.z - 1),
            grad(p[bb + 1], xyzf.x - 1, xyzf.y - 1, xyzf.z - 1),
            uvw.x);
        y2 = lerp(x1, x2, uvw.y);

        return lerp(y1, y2, uvw.z) * 0.4999 + 0.5;
    }

    float frag(v2f i) : SV_Target
    {
        return perlin(float3(i.uv.xy, 0.0) * _Frequency + _Offset);
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