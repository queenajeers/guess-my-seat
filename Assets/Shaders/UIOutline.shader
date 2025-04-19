Shader "UI/OutlineUI_Dilate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineThickness ("Outline Thickness", Float) = 2.0
        _Dilate ("Dilate (Grow/Shrink Alpha)", Float) = 0.0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            Name "UIOUTLINE"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _Dilate;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float SampleAlpha(float2 uv, float2 texelSize, float dilate)
            {
                float result = 0.0;

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        float2 offset = float2(x, y) * texelSize * (1.0 + dilate);
                        result = max(result, tex2D(_MainTex, uv + offset).a);
                    }
                }

                return result;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 resolution = _ScreenParams.xy;
                float2 texelSize = float2(_OutlineThickness / resolution.x, _OutlineThickness / resolution.y);

                float alpha = tex2D(_MainTex, i.uv).a;
                float dilatedAlpha = SampleAlpha(i.uv, texelSize, _Dilate);
                float outlinedAlpha = SampleAlpha(i.uv, texelSize, 1.0);

                float isOutline = step(0.01, outlinedAlpha) * (1.0 - step(0.01, dilatedAlpha));
                float4 outlineCol = _OutlineColor;
                outlineCol.a *= isOutline;

                float4 baseColor = tex2D(_MainTex, i.uv);
                float4 finalColor = lerp(outlineCol, baseColor, dilatedAlpha);
                return finalColor;
            }
            ENDCG
        }
    }
}
