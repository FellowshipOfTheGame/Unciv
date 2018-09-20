// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Copyright 2013, Catlike Coding, http://catlikecoding.com
// This forward shader is based on Unity 4's compiled diffuse surface shader.
// It does not support lightmaps nor shadows.
Shader "Catlike Coding/NumberFlow/Examples/Terrain/Water" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorDetail ("Color Detail", Range(0, 0.5)) = 0.25
		_Lighting ("Lighting", Range(0, 1)) = 1
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass {
			Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			
			#pragma vertex vert_surf
			#pragma fragment frag_surf
			
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

			sampler2D _MainTex;
			float _ColorDetail, _Lighting;
			
			struct Input {
				float2 uv_MainTex;
			};
			
			struct v2f_surf {
			  float4 pos : SV_POSITION;
			  float2 uv : TEXCOORD0;
			  fixed3 vlight : TEXCOORD1;
			  LIGHTING_COORDS(2, 3)
			};
			
			v2f_surf vert_surf (appdata_full v) {
				v2f_surf o;
				
				// Local vertex XZ are the UV.
				o.uv = v.vertex.xz;
				
				float wave = sin(100 * v.vertex.y + 3.14 / 4 - _Time.y);
				v.vertex.y = 0.5 - wave * wave * 0.0025;
				
				o.pos = UnityObjectToClipPos (v.vertex);
				
				// Fake normal.
				float3 worldN = float3(0, 0.7 + wave * 0.3, 0);
				float3 shlight = ShadeSH9 (float4(worldN,1.0));
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
				o.Gloss = 0.0;
				
				float height = tex2D(_MainTex, IN.uv).a;
				
				// Clip water below terrain surface so it won't show from the side.
				clip(0.501 - height);
				
				// Water is more opague and less distorted the closer it gets to the shoreline.
				float shoreline = smoothstep(0.44, 0.5, height);
				float distortion = (tex2D(_MainTex, IN.uv * 5).a - 0.5) * 0.6 + (tex2D(_MainTex, IN.uv * 17).a - 0.5) * 0.4;
				float wave = sin(100 * (height + distortion * _ColorDetail * (1 - shoreline)) - _Time.y);
				wave *= wave;
				o.Albedo = lerp(float3(0.1, 0.2, 0.4), float3(1, 1, 1), shoreline * wave);
				o.Alpha = 0.4 + 0.5 * wave * max(0.15, shoreline);
				
				// Fake normal.
				o.Normal = float3(0, 0.7 + wave * 0.3, 0);
				
				fixed4 c = LightingLambert (o, _WorldSpaceLightPos0.xyz, LIGHT_ATTENUATION(IN));
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
			
			sampler2D _MainTex;
			float _ColorDetail, _Lighting;
			
			struct Input {
				float2 uv_MainTex;
			};
			
			struct v2f_surf {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				half3 lightDir : TEXCOORD1;
				LIGHTING_COORDS(2, 3)
			};
			
			v2f_surf vert_surf (appdata_full v) {
				v2f_surf o;
				
				// Local vertex XZ are the UV.
				o.uv = v.vertex.xz;
				
				float wave = sin(100 * v.vertex.y + 3.14 / 4 - _Time.y);
				v.vertex.y = 0.5 - wave * wave * 0.0025;
				
				o.pos = UnityObjectToClipPos(v.vertex);
				float3 lightDir = WorldSpaceLightDir(v.vertex);
				o.lightDir = lightDir;
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
				o.Gloss = 0.0;
				
				float height = tex2D(_MainTex, IN.uv).a;
				
				// Clip water below terrain surface so it won't show from the side.
				clip(0.501 - height);
				
				// Water is more opague and less distorted the closer it gets to the shoreline.
				float shoreline = smoothstep(0.44, 0.5, height);
				float distortion = (tex2D(_MainTex, IN.uv * 5).a - 0.5) * 0.6 + (tex2D(_MainTex, IN.uv * 17).a - 0.5) * 0.4;
				float wave = sin(-_Time.y + 100 * (height + distortion * _ColorDetail * (1 - shoreline)));
				wave *= wave;
				o.Albedo = lerp(float3(0.1, 0.2, 0.4), float3(1, 1, 1), shoreline * wave);
				o.Alpha = 0.4 + 0.5 * wave * max(0.15, shoreline);
				
				// Fake normal.
				o.Normal = float3(0, 0.7 + wave * 0.3, 0);
				
				#ifndef USING_DIRECTIONAL_LIGHT
					fixed3 lightDir = normalize(IN.lightDir);
				#else
					fixed3 lightDir = IN.lightDir;
				#endif
				fixed4 c = LightingLambert (o, lightDir, LIGHT_ATTENUATION(IN));
				c.a = 0;
				// Blend between unlit and fully lit.
				c.rgb = lerp(float3(0, 0, 0), c.rgb, _Lighting);
				return c;
			}
			
			ENDCG
		}
	}
	Fallback "VertexLit"
}
