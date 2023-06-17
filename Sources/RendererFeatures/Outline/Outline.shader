Shader "Outline"
{
    Properties
    {
        _Threshold ("DepthCutOff", float) =  0.3
        _Thickness ("Thickness", float) = 0.7
        _OutlineColor ("OutlineColor", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white"
    }
        SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        // The Blit.hlsl file provides the vertex shader (Vert),
        // input structure (Attributes) and output strucutre (Varyings)
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        //#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        struct Attributes
        {
            float4 positionOS : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        TEXTURE2D(_MainTex);
        float4 _MainTex_TexelSize; // Texel info
        float4 _MainTex_ST; // Tiling & Offset info

        SamplerState sampler_point_clamp;
        
        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
            OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
            return OUT;
        }

        ENDHLSL

        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            
            TEXTURE2D_X(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            TEXTURE2D_X(_CameraNormalsTexture);
            SAMPLER(sampler_CameraNormalsTexture);

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float _Threshold_Depth;
            float _Threshold_Normal;
            float _Thickness;
            float4 _OutlineColor;

            float _StepAngleThreshold = 1.0;
            float _StepAngleMultiplier = 1.0;

            float NdotV(float3 normal)
            {
                float3 _Viewdir = mul((float3x3)unity_CameraToWorld, float3(0,0,-1));
                float ret = 1 - dot(_Viewdir, normal);
                return lerp(_StepAngleThreshold, 2.0, ret) * _StepAngleMultiplier + 1.0;
            }

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 res = _ScreenParams;
                float2 _UVOffset = _Thickness * float2(1.0/res[0], 1.0/res[1]);

                // color
                float4 opaque = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, input.uv);

                // depths
                float depth = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv).r;

                float depth_up = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv + float2(0, _UVOffset[1])).r;
                float depth_down = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv + float2(0, -_UVOffset[1])).r;
                float depth_right = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv + float2(_UVOffset[0], 0)).r;
                float depth_left = SAMPLE_TEXTURE2D_X(_CameraDepthTexture, sampler_CameraDepthTexture, input.uv + float2(-_UVOffset[0], 0)).r;

                // noramls
                float3 normal = SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_CameraNormalsTexture, input.uv);

                float3 normal_up = SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_CameraNormalsTexture, input.uv + float2(0, _UVOffset[1]));
                float3 normal_down = SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_CameraNormalsTexture, input.uv + float2(0, -_UVOffset[1]));
                float3 normal_right = SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_CameraNormalsTexture, input.uv + float2(_UVOffset[0], 0));
                float3 normal_left = SAMPLE_TEXTURE2D_X(_CameraNormalsTexture, sampler_CameraNormalsTexture, input.uv + float2(-_UVOffset[0], 0));

                float diff_depth = (depth_up - depth_down)*(depth_up - depth_down) + (depth_right - depth_left)*(depth_right - depth_left);
                diff_depth = pow(diff_depth, 0.5);

                float diff_normal = dot(normal_up - normal_down, normal_up - normal_down) + dot(normal_right - normal_left, normal_right - normal_left);
                diff_normal = pow(diff_normal, 0.5);
                
                float4 color = (diff_depth < _Threshold_Depth * NdotV(normal) && diff_normal < _Threshold_Normal) ? opaque: _OutlineColor;
                //color = multiplier;
                //float4 color = opaque;
                return color;//float4(normal, 1);
            }
            ENDHLSL
        }
    }
}

