// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Copyright 2013, Catlike Coding, http://catlikecoding.com
// This forward shader is based on Unity 4's compiled diffuse surface shader.
// It does not support lightmaps nor shadows.
Shader "Catlike Coding/NumberFlow/Examples/Terrain/Terrain" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Normal and Height Map", 2D) = "white" {}
		_ColorMap ("Color Map", 2D) = "white" {}
		_ColorDetail ("Color Detail", Range(0, 0.5)) = 0.25
		_Slope ("Slope", Range(0, 1)) = 0.5
		_Snow ("Snow", Range(0, 0.5)) = 0
		_Detail ("Detail", Range(0, 0.25)) = 0.125
		_Lighting ("Lighting", Range(0, 1)) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma target 3.0
			
			// Needed for fetching normals in the vertex shader, used vertex lighting and SH.
			#pragma glsl
			
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdbase
			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_FORWARDBASE
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			#define INTERNAL_DATA
			#define WorldReflectionVector(data,normal) data.worldRefl
			#define WorldNormalVector(data,normal) normal

			sampler2D _MainTex, _ColorMap;
			float _Slope, _Snow, _Lighting, _Detail, _ColorDetail;
			
			struct v2f_surf {
			  float4 pos : SV_POSITION;
			  float2 uv : TEXCOORD0;
			  fixed3 vlight : TEXCOORD1;
			  LIGHTING_COORDS(2, 3)
			};
			
			float4 _MainTex_ST;
			
			v2f_surf vert_surf (appdata_full v) {
				v2f_surf o;
				
				// Local vertex XZ are the UV.
				o.uv = v.vertex.xz;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				
				float3 worldN = (tex2Dlod(_MainTex, float4(o.uv, 0, 0)) * 2 - 1).xzy;
				float3 shlight = ShadeSH9(float4(worldN, 1.0));
				o.vlight = shlight;
				#ifdef VERTEXLIGHT_ON
					float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					o.vlight += Shade4PointLights(
						unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
						unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
						unity_4LightAtten0, worldPos, worldN);
				#endif // VERTEXLIGHT_ON
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			
			fixed4 LightingLambert (SurfaceOutput s, fixed3 lightDir, fixed atten) {
				fixed diff = max (0, dot (s.Normal, lightDir));
				
				fixed4 c;
				c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
				c.a = s.Alpha;
				return c;
			}
			
			fixed4 frag_surf (v2f_surf IN) : COLOR {
				#ifdef UNITY_COMPILER_HLSL
					SurfaceOutput o = (SurfaceOutput)0;
				#else
					SurfaceOutput o;
				#endif
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;
				
				float4 normalAndHeight = tex2D(_MainTex, IN.uv);
				
				// Because the normal map is used in object space, swap Y and Z.
				float3 detailNormal = tex2D(_MainTex, IN.uv * 9).xzy * 2 + tex2D(_MainTex, IN.uv * 15).xzy - 1.5;
				o.Normal = normalize(normalAndHeight.xzy * 2 - 1 + detailNormal * _Detail);
				
				// Eliminate some effects underwater.
				float waterCancellation = smoothstep(0.45, 0.55, normalAndHeight.a);
				
				// Initial color based on perturbed height.
				float colorValue =
					normalAndHeight.a +
					(tex2D(_MainTex, IN.uv * 4).a - 0.5) * _ColorDetail * waterCancellation;
				o.Albedo = tex2D(_ColorMap, float2(colorValue, 0)).rgb;
				// Include slope coloration.
				o.Albedo = lerp(o.Albedo, float3(0.4, 0.2, 0.2), smoothstep(0.1, 0.5, 1 - o.Normal.y) * _Slope);
				// Include snow.
				o.Albedo = lerp(o.Albedo, float3(1, 1, 1), smoothstep(1 - _Snow, 1, o.Normal.y) * waterCancellation);
				
				#ifdef DIRECTIONAL
					// Fake attenuation for sunlight in water.
					fixed atten = smoothstep(0.2, 0.5, normalAndHeight.a);
				#else
					fixed atten = LIGHT_ATTENUATION(IN);
				#endif
				fixed4 c = LightingLambert (o, _WorldSpaceLightPos0.xyz, atten);
				c.rgb += o.Albedo * IN.vlight;
				// Blend between unlit and fully lit.
				c.rgb = lerp(o.Albedo, c.rgb, _Lighting);
				return c;
			}
			
			ENDCG
		}
		
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardAdd" }
			ZWrite Off Blend One One Fog { Color (0,0,0,0) }
			
			CGPROGRAM
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			#pragma target 3.0
			
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_fwdadd
			#include "HLSLSupport.cginc"
			#include "UnityShaderVariables.cginc"
			#define UNITY_PASS_FORWARDADD
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			#define INTERNAL_DATA
			#define WorldReflectionVector(data,normal) data.worldRefl
			#define WorldNormalVector(data,normal) normal
			
			sampler2D _MainTex, _ColorMap;
			float _Slope, _Snow, _Lighting, _Detail, _ColorDetail;
			
			struct Input {
				float2 uv_MainTex;
			};
			
			struct v2f_surf {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				half3 lightDir : TEXCOORD1;
				LIGHTING_COORDS(2, 3)
			};
			
			float4 _MainTex_ST;
			
			v2f_surf vert_surf (appdata_full v) {
				v2f_surf o;
				
				// Local vertex XZ are the UV.
				o.uv = v.vertex.xz;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				o.lightDir = WorldSpaceLightDir(v.vertex);
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
			
			fixed4 LightingLambert (SurfaceOutput s, fixed3 lightDir, fixed atten) {
				fixed diff = max (0, dot (s.Normal, lightDir));
				
				fixed4 c;
				c.rgb = s.Albedo * _LightColor0.rgb * (diff * atten * 2);
				c.a = s.Alpha;
				return c;
			}
			
			fixed4 frag_surf (v2f_surf IN) : COLOR {
				#ifdef UNITY_COMPILER_HLSL
					SurfaceOutput o = (SurfaceOutput)0;
				#else
					SurfaceOutput o;
				#endif
				o.Emission = 0.0;
				o.Specular = 0.0;
				o.Alpha = 0.0;
				o.Gloss = 0.0;
				
				float4 normalAndHeight = tex2D(_MainTex, IN.uv);
				
				// Because the normal map is used in object space, swap Y and Z.
				float3 detailNormal = tex2D(_MainTex, IN.uv * 9).xzy * 2 + tex2D(_MainTex, IN.uv * 15).xzy - 1.5;
				o.Normal = normalize(normalAndHeight.xzy * 2 - 1 + detailNormal * _Detail);
				
				// Eliminate some effects underwater.
				float waterCancellation = smoothstep(0.45, 0.55, normalAndHeight.a);
				
				// Initial color based on perturbed height.
				float colorValue =
					normalAndHeight.a +
					(tex2D(_MainTex, IN.uv * 4).a - 0.5) * _ColorDetail * waterCancellation;
				o.Albedo = tex2D(_ColorMap, float2(colorValue, 0)).rgb;
				// Include slope coloration.
				o.Albedo = lerp(o.Albedo, float3(0.4, 0.2, 0.2), smoothstep(0.1, 0.5, 1 - o.Normal.y) * _Slope);
				// Include snow.
				o.Albedo = lerp(o.Albedo, float3(1, 1, 1), smoothstep(1 - _Snow, 1, o.Normal.y) * waterCancellation);
				
				#ifndef USING_DIRECTIONAL_LIGHT
					fixed3 lightDir = normalize(IN.lightDir);
				#else
					fixed3 lightDir = IN.lightDir;
				#endif
				#if defined(POINT) || defined(SPOT)
					// Fake attenuation in water.
					IN._LightCoord *= 1 + 2 * (1 - smoothstep(0.2, 0.5, normalAndHeight.a));
				#endif
				fixed4 c = LightingLambert(o, lightDir, LIGHT_ATTENUATION(IN));
				// Blend between unlit and fully lit.
				c.rgb = lerp(float3(0, 0, 0), c.rgb, _Lighting);
				c.a = 0;
				return c;
			}
			
			ENDCG
		}
	}
}