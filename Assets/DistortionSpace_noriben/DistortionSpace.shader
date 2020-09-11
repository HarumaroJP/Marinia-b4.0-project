Shader "Noriben/DistortionSpace" {
    Properties {
        _Distortion_Tex ("Distortion_Tex", 2D) = "black" {}
        _Distortion ("Distortion", Range(0, 1)) = 0.6276302
        _Speed ("Speed", Range(0, 10)) = 5.856099
        _Bokeh ("Bokeh", Range(0, 1)) = 1
        _Opacity ("Opacity", Range(0, 1)) = 1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Culling", Int) = 0 //カリングのトグル設定
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
            Cull [_Cull] //カリングの設定
            
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
            uniform float _Bokeh;
            uniform float _Opacity;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float4 node_4262 = _Time;
                float2 node_7243 = lerp(i.uv0,(i.uv0+node_4262.g*float2(0.1,0.1)),_Speed);
                float4 _Distortion_Tex_var = tex2D(_Distortion_Tex,TRANSFORM_TEX(node_7243, _Distortion_Tex));
                float node_3980 = saturate((1.0 - (distance(i.uv0,float2(0.5,0.5))*2.0+0.0)));
                float3 emissive = tex2D( _GrabTexture, (float3(sceneUVs.rg,0.0)+(lerp(float3(0,0,0),_Distortion_Tex_var.rgb,_Distortion)*lerp(float3(1,1,1),float3(node_3980,node_3980,node_3980),_Bokeh)))).rgb;
                float3 finalColor = emissive;
                return fixed4(finalColor,_Opacity);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
