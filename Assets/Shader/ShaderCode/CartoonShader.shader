// 2025-06-27 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

Shader "Custom/CartoonShaderWithOutline"
{
    Properties
    {
        _Color ("Base Color", Color) = (1,1,1,1)
        _Threshold ("Light Threshold", Range(0,1)) = 0.5
        _MainTex ("Base Texture", 2D) = "white" {} // Base Texture
        _EmissionColor ("Emission Color", Color) = (0,0,0,1) // Emission Color
        _EmissionStrength ("Emission Strength", Range(0,10)) = 1.0 // Emission 강도
        _OutlineColor ("Outline Color", Color) = (0,0,0,1) // Outline 색상
        _OutlineThickness ("Outline Thickness", Range(0.01, 0.1)) = 0.03 // Outline 두께
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" }
            Cull Front // Outline를 위해 앞면을 제거
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0; // UV 좌표
            };

     

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1; // UV 좌표 전달
            };

            float4 _Color;
            float _Threshold;
            sampler2D _MainTex; // Base Texture 샘플러
            float4 _MainTex_ST; // 텍스처 스케일 및 오프셋
            float4 _EmissionColor; // Emission Color
            float _EmissionStrength; // Emission 강도
            float4 _OutlineColor; // Outline 색상
            float _OutlineThickness; // Outline 두께

            v2f vert (appdata v)
            {
                v2f o;
                // Outline을 위해 법선 방향으로 확장
                float3 offset = v.normal * _OutlineThickness;
                v.vertex.xyz += offset;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // UV 좌표 변환
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Outline 색상 반환
                return _OutlineColor;
            }
            ENDCG
        }

        Pass
        {
            Name "MAIN"

            Cull Back // 기본 렌더링
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0; // UV 좌표
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float2 uv : TEXCOORD1; // UV 좌표 전달
            };

            float4 _Color;
            float _Threshold;
            sampler2D _MainTex; // Base Texture 샘플러
            float4 _MainTex_ST; // 텍스처 스케일 및 오프셋
            float4 _EmissionColor; // Emission Color
            float _EmissionStrength; // Emission 강도

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex); // UV 좌표 변환
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 텍스처 색상 샘플링
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // 조명 계산
                float lightIntensity = max(0, dot(i.normal, _WorldSpaceLightPos0.xyz));
                lightIntensity = step(_Threshold, lightIntensity); // 밝기 단계를 나눔

                // Emission 계산
                fixed4 emission = _EmissionColor * _EmissionStrength;

                // 최종 색상 계산 (Base Color + Emission)
                return fixed4(texColor.rgb * lightIntensity, texColor.a) * _Color + emission;
            }
            ENDCG
        }
    }
}