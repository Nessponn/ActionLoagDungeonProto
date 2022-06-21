Shader "Unlit/Test4"
{
    Properties
    {
        _HeartColor("_HeartColor",Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
             CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct VertexInput {
                float4 pos:  POSITION;    // 3D座標
                float2 uv:   TEXCOORD0;   // テクスチャ座標
            };

            struct VertexOutput {
                float4 sv_pos:    SV_POSITION; // 2D座標
                float2 uv:   TEXCOORD0;   // テクスチャ座標
            };

            // 頂点シェーダー
            VertexOutput vert(VertexInput input) {
                VertexOutput output;
                output.sv_pos = UnityObjectToClipPos(input.pos);
                output.uv = input.uv;

                return output;
            }

            int _Speed;
            float4 _Color;
            float4 _HeartColor;

            float heart(float2 st)
            {
                // 位置とか形の調整
                st = (st - float2(0.5, 0.38)) * (1 * float2(2.1,2.8));

                return pow(st.x, 2) + pow(st.y - sqrt(abs(st.x)), 2);
            }

            float wave(float2 st, float n)
            {
                st = (floor(st * n) + 0.5) / n;
                float d = distance(0.5, st);
                return (1 + sin(d * 3 - _Time.y * 3)) * 0.5;
            }

            // ピクセルシェーダー
            float4 frag(VertexOutput output) : SV_Target {
                //float2 pos = float2(0.5,0.5);
                //float circle = sin(length(pos)  - _Time * 110 * 2);
                //return float4(1 - circle * (1 - _Color.r), 1 - circle * (1 - _Color.g), 1 - circle * (1 - _Color.b), _Color.a);

                float n = 10;
                float2 st = frac(output.uv * n);
                //float2 st = (floor(output.uv* n) + 0.5) / n;
                //return float4(st.x, st.y, 0, 1);


                //float d = wave(output.uv * n);
                //return step(heart(output.uv * 2), abs(sin(d * 8 - _Time.w * 2)));
                //return heart(st, abs(sin(_Time.w * 2)));
                return (step(heart(st), abs(sin( 8 - _Time.w / 2))) ? _HeartColor :1);
            }
            ENDCG
        }
    }
}
