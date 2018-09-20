// Copyright 2014, Catlike Coding, http://catlikecoding.com
Shader "Catlike Coding/Math/Noise Example" {
	Properties {
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf BlinnPhong vertex:vert

		struct Input {
			float height;
		};
		
		void vert (inout appdata_full v, out Input o) {
			// Pass along vertical vertex position.
			o.height = v.vertex.y * 10 - 1;
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			// Blue below zero, red above zero.
			float3 baseColor = lerp(float3(0.5,0.5,2), float3(2,0.5,0.5), step(0, IN.height));
			
			// Blend from green at zero to baseColor.
			float3 heightColor = lerp(
					float3(0.25,1,0.25),
					baseColor,
					abs(IN.height));
			
			// Blend in yellow on slopes.
			o.Albedo = lerp(
				float3(1,1,0),
				heightColor,
				smoothstep(0.0, 1, o.Normal.y));
			o.Gloss = 1;
			o.Specular = 1;
		}
		ENDCG
	}
	Fallback "VertexLit"
}