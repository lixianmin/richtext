Shader "Unlit/Text"
{
	Properties
	{
		_MainTex ("Alpha (A)", 2D) = "white" {}
		_SpriteTex ("Sprite Texture", 2D) = "white" {}
	}

	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Offset -1, -1
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 uv0 	: TEXCOORD0;
				float2 uv1 	: TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 uv0 	: TEXCOORD0;
				float2 uv1 	: TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _SpriteTex;
			float4 _SpriteTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex 	= mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv0 		= TRANSFORM_TEX(v.uv0, _MainTex);
				o.uv1		= TRANSFORM_TEX(v.uv1, _SpriteTex);
				o.color 	= v.color;

				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				half4 mainColor = i.uv1.x * tex2D(_MainTex, i.uv0);
				half4 spriteColor = i.uv1.y * tex2D(_SpriteTex, i.uv0);
				half4 result = i.color * (mainColor + spriteColor);

				return result;
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		
		Lighting Off
		Cull Off
		ZTest Always
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		
		BindChannels
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		Pass
		{
			SetTexture [_MainTex]
			{ 
				combine primary, texture
			}
		}
	}
}
