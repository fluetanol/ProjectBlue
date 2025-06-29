// 2025-06-27 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

Shader "Custom/DynamicMouthExpression"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}                    // 베이스 텍스처 (눈이 있는 텍스처)
        _MouthTex ("Mouth Expression Texture", 2D) = "white" {}       // 입 표정 텍스처
        _MouthExpressionIndex ("Mouth Expression Index", Int) = 0     // 현재 표정 번호 (0~15)
        _MouthBlendStrength ("Mouth Blend Strength", Range(0,1)) = 1  // 블렌딩 강도
        
        // 입 영역 UV 범위 (이미지 기준으로 조정)
        _MouthUVMin ("Mouth UV Min", Vector) = (0.12, 0.04, 0, 0)     // 입 영역 최소 UV
        _MouthUVMax ("Mouth UV Max", Vector) = (0.18, 0.09, 0, 0)     // 입 영역 최대 UV
        
        // 표정 텍스처 그리드 설정 (4x4 그리드)
        _ExpressionGridSize ("Expression Grid Size", Vector) = (4, 4, 0, 0)
    }
    
    SubShader
    {
        Tags {"Queue"="Geometry" "RenderType"="Opaque"}
        LOD 200
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            sampler2D _MouthTex;
            float4 _MainTex_ST;
            float4 _MouthTex_ST;
            
            float _MouthExpressionIndex; // 정수 대신 float로 처리
            float _MouthBlendStrength;
            float4 _MouthUVMin;
            float4 _MouthUVMax;
            float4 _ExpressionGridSize;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
            
            // UV가 입 영역 안에 있는지 확인
            bool IsInMouthRegion(float2 uv)
            {
                return (uv.x >= _MouthUVMin.x && uv.x <= _MouthUVMax.x &&
                        uv.y >= _MouthUVMin.y && uv.y <= _MouthUVMax.y);
            }
            
            // 입 영역 UV를 0~1 범위로 정규화
            float2 NormalizeMouthUV(float2 uv)
            {
                return (uv - _MouthUVMin.xy) / (_MouthUVMax.xy - _MouthUVMin.xy);
            }
            
            // 표정 인덱스를 기반으로 표정 텍스처의 UV 계산 (정수 연산 제거)
            float2 GetExpressionUV(float2 normalizedMouthUV, float expressionIndex)
            {
                // 그리드에서의 위치 계산 (정수 연산 대신 부동소수점 연산 사용)
                float gridX = frac(expressionIndex / _ExpressionGridSize.x) * _ExpressionGridSize.x;
                float gridY = floor(expressionIndex / _ExpressionGridSize.x);
                
                // 각 표정의 크기
                float2 cellSize = 1.0 / _ExpressionGridSize.xy;
                
                // 표정 텍스처에서의 UV 좌표
                float2 expressionUV;
                expressionUV.x = (gridX + normalizedMouthUV.x) * cellSize.x;
                expressionUV.y = 1.0 - ((gridY + 1.0 - normalizedMouthUV.y) * cellSize.y); // Y축 뒤집기
                
                return expressionUV;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 베이스 텍스처 샘플링
                fixed4 baseColor = tex2D(_MainTex, i.uv);
                
                // 입 영역인지 확인
                if (IsInMouthRegion(i.uv))
                {
                    // 입 영역 UV를 정규화
                    float2 normalizedMouthUV = NormalizeMouthUV(i.uv);
                    
                    // 현재 표정의 UV 계산
                    float2 expressionUV = GetExpressionUV(normalizedMouthUV, _MouthExpressionIndex);
                    
                    // 표정 텍스처 샘플링
                    fixed4 mouthColor = tex2D(_MouthTex, expressionUV);
                    
                    // 알파 기반 블렌딩 (표정 텍스처의 알파 사용)
                    float blendFactor = mouthColor.a * _MouthBlendStrength;
                    
                    // 최종 색상 블렌딩
                    baseColor.rgb = lerp(baseColor.rgb, mouthColor.rgb, blendFactor);
                }
                
                return baseColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}