Shader "Custom/Transparent-1 Color Shader" {

    Properties {
        _Color("Color", color) = (1, 1, 1, 1)
    }

    SubShader {

        Tags {
            "Queue" = "Transparent-1"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "true"
            "PreviewType" = "Plane"
        }

        Pass {
            // Cull Back

            ZTest Off

            ZWrite Off


            // BlendOp BlendOp
            //
            // Add Add source and destination together.
            // Sub Subtract destination from source.
            // RevSub  Subtract source from destination.
            // Min Use the smaller of source and destination.
            // Max Use the larger of source and destination.
            BlendOp Add


            // Blend SrcFactor DstFactor, SrcFactorA DstFactorA
            //
            // One The value of one - use this to let either the source or the destination color come through fully.
            // Zero  The value zero - use this to remove either the source or the destination values.
            // SrcColor  The value of this stage is multiplied by the source color value.
            // SrcAlpha  The value of this stage is multiplied by the source alpha value.
            // DstColor  The value of this stage is multiplied by frame buffer source color value.
            // DstAlpha  The value of this stage is multiplied by frame buffer source alpha value.
            // OneMinusSrcColor  The value of this stage is multiplied by (1 - source color).
            // OneMinusSrcAlpha  The value of this stage is multiplied by (1 - source alpha).
            // OneMinusDstColor  The value of this stage is multiplied by (1 - destination color).
            // OneMinusDstAlpha  The value of this stage is multiplied by (1 - destination alpha).
            Blend SrcAlpha OneMinusSrcAlpha


            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            fixed4 _Color;


            float4 vert(float4 vertex : POSITION) : SV_POSITION {
                return mul(UNITY_MATRIX_MVP, vertex);
            }

            // pixel shader; no inputs needed
            fixed4 frag() : SV_Target {
                return  _Color;
            }

            ENDCG
        }
    }

    // FallBack "Unlit/Color"

}