Shader "Custom/Transparent+0 Font Shader" {

	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1, 1, 1, 1)
	}
 
	SubShader {

		Tags {
			"Queue" = "Transparent"
		 	"RenderType" = "Transparent"
		 	"IgnoreProjector" = "True" 
			"PreviewType" = "Plane"
		}

        Pass {
            // Cull Back

            ZTest On

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

			Color [_Color]

			SetTexture [_MainTex] {
				combine primary, texture * primary
			}

		}
	}

}
