Shader "Unlit/EdgeDetect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Silhouette("OutLine Thickness", Range(-1.0, 1.0)) = 0.05
        _Speed("Noise Speed", Float) = 1.15
        _Octaves("Noise Octaves", Float) = 3.0
        _Colour("Colour", Color) = (1,1,1,1)
        _EdgeColour("Edge Colour", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert 
            #pragma fragment frag alpha
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float  _Silhouette;
            float _Speed;
            float _Octaves;
            half4 _Colour;
            half4 _EdgeColour;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float Noise(float2 n)
            {
            	return frac(sin(dot(n.xy, float2(12.9898,78.233))) * 43758.5453);
            }

            float Perlin (float2 n)
            {
           
            	float noiseVal = 0.0f;

            	for (float i = 0.0f; i < _Octaves; i++)
            	{
            		noiseVal += Noise((n.xy * pow(2, i)) + _Time[1] * _Speed);
            	}

            	noiseVal /= _Octaves; 

            	return noiseVal;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Colour;
               
              
                //ranges the uv from -1 - 1
                half2 Offuv = i.uv * 2.0f - 1.0f;

                half Xfactor = abs(dot(Offuv, float2(1.0f, 0.0f)));
                half Yfactor = abs(dot(Offuv, float2(0.0f, 1.0f)));

                half edgefactor = max(Xfactor, Yfactor);

                if(edgefactor >  _Silhouette)
                {
               		edgefactor = (edgefactor -  _Silhouette) / (1.0f -  _Silhouette);

               		edgefactor = saturate(edgefactor + Perlin(Offuv));

               		col = lerp(col, _EdgeColour, edgefactor);

                }else
                {
                	edgefactor = 0.0f;
                }

               	col.a = 1.0f - edgefactor;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
