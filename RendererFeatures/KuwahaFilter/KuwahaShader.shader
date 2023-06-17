Shader "KuwahaShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white"
        _WindowSize ("Window Size", float) = 3
        _NumAreas ("Area Number", int) = 4
    }
        SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off

        HLSLINCLUDE
        #pragma vertex vert
        #pragma fragment frag

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

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
            Name "Kuwaha Filteration"

            HLSLPROGRAM

            float _WindowSize;
            int _NumAreas;

            float dev(float2 _from) // uv
            {
                float4 mean = float4(0, 0, 0, 0);
                for (float i = 0; i < _WindowSize; i+=1)
                {
                    for (float j = 0; j < _WindowSize; j+=1)
                    {
                        float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, _from + float2(i, j) * _MainTex_TexelSize.xy);
                        mean += col;
                    }
                }
                mean = mean / (_WindowSize * _WindowSize);

                float4 deviation = float4(0, 0, 0, 0);
                for (float i = 0; i < _WindowSize; i+=1)
                {
                    for (float j = 0; j < _WindowSize; j+=1)
                    {
                        float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, _from + float2(i, j) * _MainTex_TexelSize.xy);
                        deviation += (col - mean) * (col - mean);
                    }
                }
                deviation = deviation / (_WindowSize * _WindowSize);
                deviation = sqrt(deviation);

                return length(deviation);
            }

            float4 mean(float2 _from) // uv
            {
                float4 mean = float4(0, 0, 0, 0);
                for (float i = 0; i < _WindowSize; i+=1)
                {
                    for (float j = 0; j < _WindowSize; j+=1)
                    {
                        float4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, _from + float2(i, j) * _MainTex_TexelSize.xy);
                        mean += col;
                    }
                }
                mean = mean / (_WindowSize * _WindowSize);

                return mean;
            }


            half4 frag(Varyings IN) : SV_TARGET
            {
				float4 color = dev(IN.uv);

                float top_left = dev(IN.uv + float2(-_WindowSize + 1, 0) * _MainTex_TexelSize.xy);
                float top_right = dev(IN.uv);
                float bottom_left = dev(IN.uv - float2(_WindowSize - 1, _WindowSize - 1) * _MainTex_TexelSize.xy);
                float bottom_right = dev(IN.uv - float2(0, _WindowSize - 1) * _MainTex_TexelSize.xy);

                float4 top_left_m = mean(IN.uv + float2(-_WindowSize + 1, 0) * _MainTex_TexelSize.xy);
                float4 top_right_m = mean(IN.uv);
                float4 bottom_left_m = mean(IN.uv - float2(_WindowSize - 1, _WindowSize - 1) * _MainTex_TexelSize.xy);
                float4 bottom_right_m = mean(IN.uv - float2(0, _WindowSize - 1) * _MainTex_TexelSize.xy);

                float devs[4] = {top_left, top_right, bottom_left, bottom_right};
                float4 means[4] = {top_left_m, top_right_m, bottom_left_m, bottom_right_m};
                int max_idx = 0;
                for (int i = 1; i < 4; i++)
                {
                    max_idx = devs[i] < devs[max_idx] ? i: max_idx;
                } 
                color = means[max_idx];

                return color;
            }
            ENDHLSL
        }
    }
}
