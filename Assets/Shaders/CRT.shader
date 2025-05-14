Shader "Hidden/CRT" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader {
        Tags { "RenderPipeline"="UniversalRenderPipeline"}
        LOD 200
        Pass {
            HLSLPROGRAM
            #pragma vertex vp
            #pragma fragment fp

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            cbuffer UnityPerMaterial {
                float _Curvature;
                float _VignetteWidth;
                float _LinesPerScan;
            }

            struct VertexData {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vp(VertexData v) {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            

            half4 fp(v2f i) : SV_Target {
                float2 uv = i.uv * 2.0f - 1.0f;
                float2 offset = uv.yx / _Curvature;
                uv = uv + uv * offset * offset;
                uv = uv * 0.5f + 0.5f;

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                if (uv.x <= 0.0f || 1.0f <= uv.x || uv.y <= 0.0f || 1.0f <= uv.y)
                    col = 0;

                uv = uv * 2.0f - 1.0f;
                float2 vignette = _VignetteWidth / _ScreenParams.xy;
                vignette = smoothstep(0.0f, vignette, 1.0f - abs(uv));
                vignette = saturate(vignette);

                col.g *= (sin(i.uv.y * _ScreenParams.y / _LinesPerScan * 2.0f) + 1.0f) * 0.25f + 1.0f;
                col.rb *= (cos(i.uv.y * _ScreenParams.y / _LinesPerScan * 2.0f) + 1.0f) * 0.2f + 1.0f;

                return saturate(col) * vignette.x * vignette.y;
            }

            ENDHLSL
        }
    }

    Fallback "Unlit/Texture"
}