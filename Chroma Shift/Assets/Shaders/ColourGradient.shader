Shader "Custom/ColourGradient"
{
	Properties
	{
		_Color0 ("Color.r", Range(0.0, 1.0)) = 0.5
		_Color1 ("Color.g", Range(0.0, 1.0)) = 0.5
		_Color2 ("Color.b", Range(0.0, 1.0)) = 0.5

	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Color0;
			float _Color1;
			float _Color2;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			

		

			fixed4 frag (v2f i) : SV_Target
			{
			float freq = 0.5;
			float amp = 0.5;
			float phase = 0.5;
			float4 col;

				 col.r = (i.uv.y +_Color0) * (freq + amp * sin(_Time[3] / 5 ));
				 col.g = (i.uv.x + _Color1) * (freq + amp * sin(_Time[3] / 5 )); 
				 col.b = (-i.uv.x + _Color2) * (freq + amp * sin(_Time[3] / 5));
				 col.a = 1.0;
				// just invert the colors
				//col = 1 - col;
				return col;
			}
			ENDCG
		}
	}
}
