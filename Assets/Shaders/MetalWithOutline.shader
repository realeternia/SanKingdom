Shader "Custom/MetalWithOutline"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 5)) = 1.5
        _Metallic ("Metallic", Range(0, 1)) = 1.0
        _Glossiness ("Glossiness", Range(0, 1)) = 0.8
        _SpecColor ("Specular Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (0,0,0,1)
        _EmissionStrength ("Emission Strength", Range(0, 2)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "OUTLINE"
            Tags { "LightMode" = "Always" }
            Cull Front
            ZWrite On
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float4 _OutlineColor;
            float _OutlineWidth;

            v2f vert (appdata_t v)
            {
                v2f o;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                worldPos.xyz = worldPos.xyz + normalize(worldNormal) * _OutlineWidth * 0.05;
                o.pos = mul(UNITY_MATRIX_VP, worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }

        Pass
        {
            Name "BASE"
            Tags { "LightMode" = "ForwardBase" }
            Cull Back
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

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
                float3 viewDir : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Metallic;
            float _Glossiness;
            float4 _EmissionColor;
            float _EmissionStrength;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = WorldSpaceViewDir(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.texcoord);
                fixed3 baseColor = texColor.rgb * _Color.rgb;

                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                fixed3 viewDir = normalize(i.viewDir);
                fixed3 halfDir = normalize(worldLightDir + viewDir);

                fixed diff = max(0, dot(worldNormal, worldLightDir));
                
                float specPower = exp2(_Glossiness * 10 + 1);
                fixed spec = pow(max(0, dot(worldNormal, halfDir)), specPower);
                
                fixed3 diffuse = diff * _LightColor0.rgb;
                
                fixed3 specColor = lerp(_SpecColor.rgb, baseColor, _Metallic);
                fixed3 specular = spec * specColor * _LightColor0.rgb * _Glossiness;
                
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
                
                fixed3 metalReflect = baseColor * (1 - _Metallic * 0.5);
                
                fixed3 fresnel = pow(1 - max(0, dot(worldNormal, viewDir)), 3);
                fixed3 rim = fresnel * _Glossiness * 0.5;
                
                fixed3 emission = _EmissionColor.rgb * _EmissionStrength;
                
                fixed3 finalColor = metalReflect * (ambient + diffuse) + specular + rim + emission;
                
                return fixed4(finalColor, _Color.a);
            }
            ENDCG
        }
    }

    FallBack "Mobile/Diffuse"
}
