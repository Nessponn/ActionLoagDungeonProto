Shader "Unlit/TestShader"
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
// フラグメントシェーダー

            float4 frag(VertexOutput output) : SV_Target {
                float2 tex = output.uv;
                return float4(tex.x, tex.y, 1.0, 1.0); //座標に応じて色を変更
            }
            ENDCG
        }
    }
}
