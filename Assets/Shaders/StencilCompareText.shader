Shader "UI/StencilSensitiveText"
{
    Properties
    {
        _MainTex ("Font Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _OverlapColor ("Overlap Color", Color) = (1,0,0,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        // ▼ Pass 1: 겹치는 영역 (Stencil == 1)
        Stencil
        {
            Ref 1
            Comp Equal
            Pass Keep
        }

        Pass
        {
            Name "Overlap"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragOverlap
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OverlapColor;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 fragOverlap(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a < 0.1) discard; // ✨ 투명 픽셀 제거
                return col * _OverlapColor;
            }
            ENDCG
        }

        // ▼ Pass 2: 겹치지 않는 영역 (Stencil != 1)
        Stencil
        {
            Ref 1
            Comp NotEqual
            Pass Keep
        }

        Pass
        {
            Name "Base"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragBase
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BaseColor;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEX0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 fragBase(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a < 0.1) discard; // ✨ 투명 픽셀 제거
                return col * _BaseColor;
            }
            ENDCG
        }
    }
}
