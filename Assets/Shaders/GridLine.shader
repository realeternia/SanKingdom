Shader "Custom/GridLine"
{
    Properties
    {
        _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _GridSize ("Grid Size", Float) = 1.0
        _LineThickness ("Line Thickness", Range(0.001, 30)) = 2
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

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

            float4 _LineColor;
            float _GridSize;
            float _LineThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _GridSize;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 grid = abs(frac(i.uv - 0.5) - 0.5) / (fwidth(i.uv) * _LineThickness);
                float lineValue = min(grid.x, grid.y);
                float alpha = 1.0 - min(lineValue, 1.0);

                if (alpha < 0.01)
                    discard;

                return float4(_LineColor.rgb, _LineColor.a * alpha);
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
