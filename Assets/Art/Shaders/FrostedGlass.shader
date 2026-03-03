Shader "Custom/FrostedGlass_Region_SampleCount"
{
    Properties
    {
        _TintColor("Tint Color", Color) = (1,1,1,0.5)
        _BlurStrength("Blur Strength", Float) = 1.0
        _SampleCount("Sample Count (1~5)", Range(1,5)) = 3
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        LOD 100

        Pass
        {
            Name "FrostedGlass"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _TintColor;
            float _BlurStrength;
            float _SampleCount;
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.screenPos = OUT.positionHCS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                screenUV = screenUV * 0.5 + 0.5;

                float2 texelSize = _ScreenParams.zw * _BlurStrength;

                half4 col = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV);
                float totalWeight = 1.0;

                // Sample the surrounding pixels based on the sample count
                if (_SampleCount >= 2.0)
                {
                    col += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + float2(texelSize.x, 0));
                    col += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV - float2(texelSize.x, 0));
                    totalWeight += 2.0;
                }
                if (_SampleCount >= 3.0)
                {
                    col += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + float2(0, texelSize.y));
                    col += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV - float2(0, texelSize.y));
                    totalWeight += 2.0;
                }
                if (_SampleCount >= 4.0)
                {
                    col += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + texelSize);
                    col += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV - texelSize);
                    totalWeight += 2.0;
                }
                if (_SampleCount >= 5.0)
                {
                    col += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + float2(texelSize.x, -texelSize.y));
                    col += SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV + float2(-texelSize.x, texelSize.y));
                    totalWeight += 2.0;
                }

                col /= totalWeight;

                // blend with tint color
                col.rgb = lerp(col.rgb, _TintColor.rgb, _TintColor.a);
                col.a = 1;

                return SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenUV);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
