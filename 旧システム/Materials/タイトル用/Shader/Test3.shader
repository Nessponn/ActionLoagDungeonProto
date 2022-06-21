Shader "Unlit/Test3"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            // ピクセルシェーダー
            float4 frag(VertexOutput output) : SV_Target {
                float3 col = 0.5 + 0.5 * cos(_Time - output.uv.xyx + float3(0, 2, 4));
                return float4(col, 1);
            }

            ENDCG
        }
    }
}
