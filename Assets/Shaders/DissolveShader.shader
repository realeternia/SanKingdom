Shader "Custom/DissolveShader"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DissolveTex ("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0
        _DissolveEdgeWidth ("Dissolve Edge Width", Range(0, 0.5)) = 0.1
        _DissolveEdgeColor ("Dissolve Edge Color", Color) = (1, 0.5, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 150
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
            Cull Back
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _DissolveTex;
            float4 _DissolveTex_ST;
            float4 _Color;
            float _DissolveAmount;
            float _DissolveEdgeWidth;
            float4 _DissolveEdgeColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord) * _Color;

                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed diff = max(0, dot(worldNormal, worldLightDir));
                col.rgb *= diff * _LightColor0.rgb + unity_AmbientSky.rgb;

                float dissolveValue = tex2D(_DissolveTex, i.texcoord).r;
                float dissolveThreshold = _DissolveAmount;
                
                if (dissolveValue < dissolveThreshold)
                {
                    discard;
                }

                float edgeFactor = smoothstep(dissolveThreshold, dissolveThreshold + _DissolveEdgeWidth, dissolveValue);
                if (edgeFactor < 1.0)
                {
                    col.rgb = lerp(_DissolveEdgeColor.rgb, col.rgb, edgeFactor);
                    col.a *= edgeFactor;
                }

                return col;
            }
            ENDCG
        }
    }

    FallBack "Mobile/Diffuse"
}
