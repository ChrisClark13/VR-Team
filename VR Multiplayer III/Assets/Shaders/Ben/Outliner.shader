Shader "Ben/Outliner"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_BumpMap("Normals Map", 2D) = "bump" {}
		_Color("Main Color", Color) = (.5,.5,.5,1)
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_OutlineWidth("Outline Width", Range(1,1.1)) = 1
	}

	CGINCLUDE
	#include"UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	struct v2f
	{
		float4 pos : POSITION;
		//float4 color : COLOR;
		float3 normal : NORMAL;
	};

	float4 _OutlineColor;
	float _OutlineWidth;
	//sampler2D _BumpMap;

	v2f vert(appdata v)
	{
		v.vertex.xyz *= _OutlineWidth;
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		
		return o;
	}

	ENDCG

	SubShader
	{
		Tags{"Queue" = "Transparent"}
		LOD 300
		Pass//Outline Render
		{
		ZWrite Off

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		half4 frag(v2f i) : COLOR
		{
			return _OutlineColor;
		}
		ENDCG

		}

		Pass//normal Render
		{
			

			//CGPROGRAM
			//sampler2D _BumpMap;
			//void surf(Input IN, inout SurfaceOutput o)
			//{
			//	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv._BumpMap));
			//}
			//ENDCG
			ZWrite On
			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}
			Lighting On

			SetTexture[_MainTex]//makes it actually display the texture
			{
				ConstantColor[_Color]
			}

			SetTexture[_MainTex]
			{
				Combine previous * primary DOUBLE
			}
			

			
		}
	}
}



























