Shader "Custom/CuttingShader" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_CutPos("Cut position", Float) = 0.0
		_CutDirection("Cut direction", Int) = 0
		_Color("Main Color", Color) = (0,0,0,0)
	}
		SubShader{
		Lighting Off
		AlphaTest Greater 0.5

		Tags
	{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		Blend One OneMinusSrcAlpha
		LOD 200

		CGPROGRAM
#pragma surface surf NoLighting alpha
#include "UnityCG.cginc"

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
		fixed4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}

	sampler2D _MainTex;
	float _CutPos;
	int _CutDirection;
	fixed4 _Color;
	struct Input {
		float2 uv_MainTex;
		float3 worldPos;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		if (_CutDirection)
		{
			switch (_CutDirection)
			{
			case(1):
				clip((_CutPos - IN.worldPos.x));
				break;
			case(2):
				clip((_CutPos - IN.worldPos.y));
				break;
			case(3):
				clip((-_CutPos + IN.worldPos.x));
				break;
			case(4):
				clip((-_CutPos + IN.worldPos.y));
				break;
			}
		}
		half4 c = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = c.rgb * _Color.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}