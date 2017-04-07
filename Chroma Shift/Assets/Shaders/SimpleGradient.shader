// Shader created with Shader Forge v1.25 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.25;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:1873,x:33466,y:32687,varname:node_1873,prsc:2|emission-5360-OUT;n:type:ShaderForge.SFN_Color,id:7571,x:32818,y:32618,ptovrint:False,ptlb:Blue,ptin:_Blue,varname:node_7571,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:1,c3:0.6275861,c4:1;n:type:ShaderForge.SFN_Color,id:7913,x:32730,y:32876,ptovrint:False,ptlb:Purple,ptin:_Purple,varname:node_7913,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.6685806,c2:0.04352291,c3:0.8455882,c4:1;n:type:ShaderForge.SFN_Lerp,id:5360,x:33154,y:32803,varname:node_5360,prsc:2|A-7571-RGB,B-7913-RGB,T-3302-OUT;n:type:ShaderForge.SFN_TexCoord,id:4384,x:32443,y:33273,varname:node_4384,prsc:2,uv:0;n:type:ShaderForge.SFN_ComponentMask,id:348,x:32643,y:33332,varname:node_348,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-4384-V;n:type:ShaderForge.SFN_Add,id:1063,x:32943,y:33406,varname:node_1063,prsc:2|A-7916-OUT,B-348-OUT;n:type:ShaderForge.SFN_Slider,id:7916,x:32542,y:33170,ptovrint:False,ptlb:ColorSlider,ptin:_ColorSlider,varname:node_7916,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-1,cur:0,max:1;n:type:ShaderForge.SFN_Clamp01,id:3302,x:33185,y:33371,varname:node_3302,prsc:2|IN-1063-OUT;proporder:7571-7913-7916;pass:END;sub:END;*/

Shader "Shader Forge/Gradient1" {
    Properties {
        _Blue ("Blue", Color) = (0,1,0.6275861,1)
        _Purple ("Purple", Color) = (0.6685806,0.04352291,0.8455882,1)
        _ColorSlider ("ColorSlider", Range(-1, 1)) = 0
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _Blue;
            uniform float4 _Purple;
            uniform float _ColorSlider;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float3 emissive = lerp(_Blue.rgb,_Purple.rgb,saturate((_ColorSlider+i.uv0.g.r)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
