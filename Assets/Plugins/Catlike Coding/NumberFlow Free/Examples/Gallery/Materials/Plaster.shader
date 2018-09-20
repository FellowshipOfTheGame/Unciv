// Copyright 2014, Catlike Coding, http://catlikecoding.com
Shader "Catlike Coding/NumberFlow/Examples/Gallery/Plaster" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_BumpMap ("Normalmap", 2D) = "bump" {}
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 500

		CGPROGRAM
		#pragma surface surf Lambert

		float3 _Color;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			o.Albedo = _Color;
			// Combine normal map with itself at higher frequency for more detail.
			// Flipped and offset to make self-similarity less obvious.
			o.Normal = normalize(UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) + UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap.yx * 5 + 0.31)));
		}
		
		ENDCG 
	}
}