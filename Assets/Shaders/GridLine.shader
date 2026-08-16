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

            // 平顶(flat-top)六边形常量：外接圆半径1，奇数列下移半行
            static const float SQRT3 = 1.7320508;   // 行高 = √3
            static const float APOTHEM = 0.8660254; // √3/2，上下平边/奇列偏移
            static const float COL_SPACING = 1.5;   // 列间距

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

            // 像素到所在六边形六条边（6条无限延伸直线）的最短距离
            float HexEdgeDist(float2 d)
            {
                // 上下平边
                float flatTop = APOTHEM - d.y;
                float flatBottom = APOTHEM + d.y;
                // 四条斜边：过 (±1,0) 与 (±0.5,±√3/2)，斜率 ±√3
                float upRight = (SQRT3 * d.x + d.y - SQRT3) * 0.5;
                float downRight = (-SQRT3 * d.x + d.y + SQRT3) * 0.5;
                float upLeft = (-SQRT3 * d.x + d.y - SQRT3) * 0.5;
                float downLeft = (SQRT3 * d.x + d.y + SQRT3) * 0.5;
                return min(abs(flatTop), min(abs(flatBottom), min(abs(upRight), min(abs(downRight), min(abs(upLeft), abs(downLeft))))));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 取最近六边形格心（四舍五入到邻格），奇数列沿 +v 偏移半行（与逻辑格 HexUtil 一致）
                float col = floor(i.uv.x / COL_SPACING + 0.5);
                float odd = fmod(abs(col), 2.0); // 奇数列(0/1)
                float row = floor((i.uv.y - odd * APOTHEM) / SQRT3 + 0.5);
                float2 center = float2(col * COL_SPACING, row * SQRT3 + odd * APOTHEM);

                float edgeDist = HexEdgeDist(i.uv - center);
                float lineValue = edgeDist / max(fwidth(i.uv) * _LineThickness, 0.0001);
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
