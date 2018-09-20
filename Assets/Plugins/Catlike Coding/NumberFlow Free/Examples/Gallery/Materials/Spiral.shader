// Copyright 2014, Catlike Coding, http://catlikecoding.com
Shader "Catlike Coding/NumberFlow/Examples/Gallery/Spiral" {
	Properties {
		_MainTex ("Normalmap (RGB) plus offset (A)", 2D) = "white" {}
		_ColorMap ("Color Map (RGB)", 2D) = "white"
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300

		CGPROGRAM
		#pragma surface surf Lambert
		#pragma target 3.0
		#pragma glsl

		sampler2D _MainTex, _ColorMap;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
		};

		float filterWidth (float2 uv) {
			float2 fw = max(abs(ddx(uv)), abs(ddy(uv)));
			return max(fw.x, fw.y);
		}

		float colorIndex (float2 uv) {
			float bump = tex2D(_MainTex, uv).a;
			
			float2 vec = float2(uv.x, uv.y) - 0.5;
			float vecLength = length(vec);
			vec /= vecLength;
			
			float frequency = (vecLength + bump * 0.25) * 5;
			
			float2 arm;
			sincos((vecLength * frequency - _Time.y * 0.1) * 6.2832, arm.x, arm.y);
			
			return abs(lerp(bump, dot(arm, vec), vecLength + vecLength));
		}

		void surf (Input IN, inout SurfaceOutput o) {
			float filter = filterWidth(IN.uv_MainTex) * 0.25;
			float result;
			// Filter over an area to reduce aliasing when the texture becomes small on screen.
			result = (
				colorIndex(IN.uv_MainTex - float2(filter, filter)) +
				colorIndex(IN.uv_MainTex - float2(filter, -filter)) +
				colorIndex(IN.uv_MainTex - float2(-filter, -filter)) +
				colorIndex(IN.uv_MainTex - float2(-filter, filter))) * 0.25;
			float4 c = tex2D(_ColorMap, float2(result, 1));
			o.Albedo = c.rgb;
			o.Emission = c.rgb * c.a;
			o.Normal = tex2D(_MainTex, IN.uv_MainTex).rgb * 2 - 1;
		}
		
		ENDCG
	}

	FallBack "Diffuse"
}