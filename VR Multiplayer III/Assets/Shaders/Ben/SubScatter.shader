﻿Shader "Ben/SubScatter" {
	Properties {
		[Header(Main Properties)]
		[Space(10)]
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0

		[Header(Translucency Properties)]
		[Space(10)]
		_Distortion ("Distortion", Range (-5,1)) = 0
		_Power("Strength", Range (0.001,5)) = 0
		_Scale("Scale", Range(0,5)) = 0
		//_SubColor("Internal Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardTranslucent fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		//fixed4 _SubColor;
		float _Distortion;
		float _Power;
		float _Scale;

		

		#include "UnityPBSLighting.cginc"
		inline fixed4 LightingStandardTranslucent(SurfaceOutputStandard s, fixed3 viewDir, UnityGI gi)
		{
		//Original color
			fixed4 pbr = LightingStandard(s, viewDir, gi);

			//Translucency
			float3 L = gi.light.dir;
			float3 V = viewDir;
			float3 N = s.Normal;

			float3 H = normalize(L + N * _Distortion);
			float I = pow(saturate(dot(V, -H)), _Power) * _Scale;

			//Final Add
			pbr.rgb = pbr.rbg + gi.light.color * I;

			return pbr;
		}
		void LightingStandardTranslucent_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
		{
			LightingStandard_GI(s, data, gi);
		}
		
		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}