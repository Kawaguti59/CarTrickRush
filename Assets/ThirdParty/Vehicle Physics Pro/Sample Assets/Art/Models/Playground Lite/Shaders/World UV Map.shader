Shader "Custom/World UV Map"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTexWall2 ("Wall Side Texture (RGB)", 2D) = "white" {}
        _MainTexWall ("Wall Front Texture (RGB)", 2D) = "white" {}
        _MainTexFlr2 ("Flr Texture", 2D) = "white" {}
        _Scale ("Texture Scale", Float) = 0.1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/UnityFog.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                half fogFactor : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_MainTexWall);
            SAMPLER(sampler_MainTexWall);
            TEXTURE2D(_MainTexWall2);
            SAMPLER(sampler_MainTexWall2);
            TEXTURE2D(_MainTexFlr2);
            SAMPLER(sampler_MainTexFlr2);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _Scale;
            CBUFFER_END

            Varyings Vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);
                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS = normInputs.normalWS;
                output.fogFactor = ComputeFogFactor(output.positionCS.z);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float3 n = normalize(input.normalWS);
                float2 uv;
                half4 c;

                if (abs(n.x) > 0.5)
                {
                    uv = input.positionWS.yz * _Scale;
                    c = SAMPLE_TEXTURE2D(_MainTexWall2, sampler_MainTexWall2, uv);
                }
                else if (abs(n.z) > 0.5)
                {
                    uv = input.positionWS.xy * _Scale;
                    c = SAMPLE_TEXTURE2D(_MainTexWall, sampler_MainTexWall, uv);
                }
                else
                {
                    uv = input.positionWS.xz * _Scale;
                    c = SAMPLE_TEXTURE2D(_MainTexFlr2, sampler_MainTexFlr2, uv);
                }

                half3 albedo = c.rgb * _Color.rgb;
                half alpha = c.a * _Color.a;
                albedo = MixFog(albedo, input.fogFactor);
                return half4(albedo, alpha);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
