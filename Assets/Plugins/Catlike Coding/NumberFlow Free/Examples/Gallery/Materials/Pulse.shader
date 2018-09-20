// Copyright 2014, Catlike Coding, http://catlikecoding.com
Shader "Catlike Coding/NumberFlow/Examples/Galler/Pulse" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorMapIndex0 ("Color Map Index 0", Float) = 0
		_ColorMapIndex1 ("Color Map Index 1", Float) = 0
		_ColorMap ("Color Map (RGB)", 2D) = "white"
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex, _ColorMap;
		float _ColorMapIndex0, _ColorMapIndex1;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			// Use r to index first color map, g to index second color map, and b to offset the animation.
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			float t = sin(c.b * 3 - 0.1 * IN.uv_MainTex.x + IN.uv_MainTex.y * 2 + _Time.y * 1);
			o.Emission = tex2D(_ColorMap, float2(c.r * (t + 0.5), _ColorMapIndex0)) +
				tex2D(_ColorMap, float2(sin(_Time.y * 0.5 + IN.uv_MainTex.x + IN.uv_MainTex.y + c.b * 7) * c.g, _ColorMapIndex1));
		}
		
		ENDCG
	}

	FallBack "Diffuse"
}