Shader "UI/Blurred" {
    Properties {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Size("Blur Radius", Range(0, 0.1)) = 0
        [KeywordEnum(Low, Medium, High)] _Samples ("Sample Amount", Float) = 0
        _StandardDeviation("Standard Deviation", Range(0, 0.1)) = 0.02
    }
    SubShader {
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass {
            Tags { "LightMode" = "Always" }
        }
        Pass {
            Tags { "LightMode" = "Always" }
            
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH

#if _SAMPLES_LOW
#define SAMPLES 10
#elif _SAMPLES_MEDIUM
#define SAMPLES 30
#else
#define SAMPLES 100
#endif

#define PI 3.14159265359
#define E 2.71828182846

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : POSITION;
                float4 uvgrab : TEXCOORD0;
            };

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
                float scale = -1.0;
#else
                float scale = 1.0;
#endif
                o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                o.uvgrab.zw = o.vertex.zw;
                return o; 
            }

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _Size;
            float _StandardDeviation;

            half4 frag(v2f i) : COLOR {
                float sum = 0;
                half4 color = half4(0, 0, 0, 0);
                float4 uvgrab = i.uvgrab;
                for (float i = 0; i < SAMPLES; i++) {
                    float offset = (i/(SAMPLES - 1) - 0.5) * _Size;
                    float stdSquared = _StandardDeviation * _StandardDeviation;
                    float guass = (1 / sqrt(2 * PI * stdSquared)) * pow(E, -((offset*offset)/(2 * stdSquared)));
                    sum += guass;
                    color += guass * tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(uvgrab.x + offset, uvgrab.yzw)));
                }
                color = color / sum;

                return color;
            }
            ENDCG
        }

        GrabPass {
            Tags { "LightMode" = "Always" }
        }
        Pass {
            Tags { "LightMode" = "Always" }
            
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH

#if _SAMPLES_LOW
#define SAMPLES 10
#elif _SAMPLES_MEDIUM
#define SAMPLES 30
#else
#define SAMPLES 100
#endif

#define PI 3.14159265359
#define E 2.71828182846

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : POSITION;
                float4 uvgrab : TEXCOORD0;
            };

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
                float scale = -1.0;
#else
                float scale = 1.0;
#endif
                o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                o.uvgrab.zw = o.vertex.zw;
                return o; 
            }

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _Size;
            float _StandardDeviation;

            half4 frag(v2f i) : COLOR {
                float sum = 0;
                half4 color = half4(0, 0, 0, 0);
                float4 uvgrab = i.uvgrab;
                for (float i = 0; i < SAMPLES; i++) {
                    float offset = (i/(SAMPLES - 1) - 0.5) * _Size;
                    float stdSquared = _StandardDeviation * _StandardDeviation;
                    float guass = (1 / sqrt(2 * PI * stdSquared)) * pow(E, -((offset*offset)/(2 * stdSquared)));
                    sum += guass;
                    color += guass * tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(uvgrab.x, uvgrab.y + offset, uvgrab.zw)));
                }
                color = color / sum;

                return color;
            }
            ENDCG
        }

        Pass {
            Name "Default"
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

#include "UnityCG.cginc"
#include "UnityUI.cginc"

#pragma multi_compile __ UNITY_UI_CLIP_RECT
#pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 uvgrab : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert(appdata_t v) {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
#if UNITY_UV_STARTS_AT_TOP
                float scale = -1.0;
#else
                float scale = 1.0;
#endif
                OUT.uvgrab.xy = (float2(OUT.vertex.x, OUT.vertex.y * scale) + OUT.vertex.w) * 0.5;
                OUT.uvgrab.zw = OUT.vertex.zw;

                OUT.color = v.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                half4 blur = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.uvgrab));
                return color;
            }
            ENDCG
        }
    }
}
