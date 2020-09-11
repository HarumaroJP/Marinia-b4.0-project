// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33442,y:32381,varname:node_3138,prsc:2|emission-2355-RGB,alpha-7514-OUT;n:type:ShaderForge.SFN_SceneColor,id:2355,x:32910,y:32445,varname:node_2355,prsc:2|UVIN-8300-OUT;n:type:ShaderForge.SFN_ScreenPos,id:2702,x:32309,y:32372,varname:node_2702,prsc:2,sctp:2;n:type:ShaderForge.SFN_Add,id:8300,x:32740,y:32445,varname:node_8300,prsc:2|A-2702-UVOUT,B-6047-OUT;n:type:ShaderForge.SFN_Tex2d,id:4618,x:31980,y:32541,ptovrint:False,ptlb:Distortion_Tex,ptin:_Distortion_Tex,varname:_Normal_Tex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:5ff221ac81d63b3469b8388e4f6fb5ef,ntxv:2,isnm:False|UVIN-7243-OUT;n:type:ShaderForge.SFN_Time,id:4262,x:31395,y:32580,varname:node_4262,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:4535,x:31395,y:32441,varname:node_4535,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:4994,x:31570,y:32481,varname:node_4994,prsc:2,spu:0.1,spv:0.1|UVIN-4535-UVOUT,DIST-4262-T;n:type:ShaderForge.SFN_Lerp,id:8165,x:32309,y:32545,varname:node_8165,prsc:2|A-5056-OUT,B-4618-RGB,T-1663-OUT;n:type:ShaderForge.SFN_Slider,id:1663,x:31809,y:32747,ptovrint:False,ptlb:Distortion,ptin:_Distortion,varname:_NormalPower,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6276302,max:1;n:type:ShaderForge.SFN_Vector3,id:5056,x:31980,y:32440,varname:node_5056,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Lerp,id:7243,x:31748,y:32440,varname:node_7243,prsc:2|A-4535-UVOUT,B-4994-UVOUT,T-5442-OUT;n:type:ShaderForge.SFN_Slider,id:5442,x:31395,y:32749,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:_NormalMoveSpeed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5.856099,max:10;n:type:ShaderForge.SFN_Multiply,id:6047,x:32534,y:32525,varname:node_6047,prsc:2|A-8165-OUT,B-6561-OUT;n:type:ShaderForge.SFN_Slider,id:7514,x:32674,y:32761,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_7514,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:3019,x:31550,y:33028,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_3019,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.223416,max:10;n:type:ShaderForge.SFN_Fresnel,id:689,x:31932,y:32936,varname:node_689,prsc:2|EXP-3019-OUT;n:type:ShaderForge.SFN_Slider,id:122,x:31657,y:33174,ptovrint:False,ptlb:Bokef,ptin:_Bokef,varname:node_122,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:2.197412,max:5;n:type:ShaderForge.SFN_OneMinus,id:9486,x:32117,y:32936,varname:node_9486,prsc:2|IN-689-OUT;n:type:ShaderForge.SFN_Power,id:6561,x:32306,y:32924,varname:node_6561,prsc:2|VAL-9486-OUT,EXP-122-OUT;proporder:4618-1663-5442-7514-3019-122;pass:END;sub:END;*/

Shader "Noriben/DistortionSpaceSphere" {
    Properties {
        _Distortion_Tex ("Distortion_Tex", 2D) = "black" {}
        _Distortion ("Distortion", Range(0, 1)) = 0.6276302
        _Color ("Base Color",Color) = (0,0,0,0)
        _Speed ("Speed", Range(0, 10)) = 5.856099
        _Opacity ("Opacity", Range(0, 1)) = 1
        _Fresnel ("Fresnel", Range(0, 10)) = 1.223416
        _Bokef ("Bokef", Range(0, 5)) = 2.197412
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Distortion_Tex; uniform float4 _Distortion_Tex_ST;
            uniform float _Distortion;
            uniform float _Speed;
            uniform float _Opacity;
            uniform float _Fresnel;
            uniform float _Bokef;
            fixed4 _Color;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 projPos : TEXCOORD3;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float4 node_4262 = _Time;
                float2 node_7243 = lerp(i.uv0,(i.uv0+node_4262.g*float2(0.1,0.1)),_Speed);
                float4 _Distortion_Tex_var = tex2D(_Distortion_Tex,TRANSFORM_TEX(node_7243, _Distortion_Tex));
                float3 emissive = tex2D( _GrabTexture, (float3(sceneUVs.rg,0.0)+(lerp(float3(0,0,0),_Distortion_Tex_var.rgb,_Distortion)*pow((1.0 - pow(1.0-max(0,dot(normalDirection, viewDirection)),_Fresnel)),_Bokef)))).rgb;
                float3 finalColor = emissive * _Color;
                return fixed4(finalColor,_Opacity);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
