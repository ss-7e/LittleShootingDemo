Shader "Custom/UI/HealthBar"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _FillAmount ("Fill Amount", Range(0,1)) = 1.0
        _EdgeSoftness ("Edge Softness", Range(0.001, 0.2)) = 0.02
    }

    SubShader
    {
        Tags { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "RightCutHealthBar"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _FillAmount;
            float _EdgeSoftness;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float uvx = i.uv.x;
                float alphaFactor = 1.0;

                if (uvx > _FillAmount)
                {
                    discard;
                }
                else if (uvx > (_FillAmount - _EdgeSoftness))
                {
                    alphaFactor = smoothstep(_FillAmount, _FillAmount - _EdgeSoftness, uvx);
                }

                half4 col = tex2D(_MainTex, i.uv) * _Color;
                col.a *= alphaFactor;
                return col;
            }
            ENDHLSL
        }
    }
}
