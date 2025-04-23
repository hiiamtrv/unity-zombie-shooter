Shader "Custom/HumanoidShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            sampler2D _MainTex;
            float _Glossiness;
            float _Metallic;

            // Bone matrix support
            #define MAX_BONES 60
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(float4x4, _BoneMatrices[MAX_BONES])
            UNITY_INSTANCING_BUFFER_END(Props)

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 texcoord : TEXCOORD0;

                float4 boneWeights : BLENDWEIGHT;
                int4 boneIndices : BLENDINDICES;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;

                float4x4 boneMatrix =
                    UNITY_ACCESS_INSTANCED_PROP(Props, _BoneMatrices[v.boneIndices.x]) * v.boneWeights.x +
                    UNITY_ACCESS_INSTANCED_PROP(Props, _BoneMatrices[v.boneIndices.y]) * v.boneWeights.y +
                    UNITY_ACCESS_INSTANCED_PROP(Props, _BoneMatrices[v.boneIndices.z]) * v.boneWeights.z +
                    UNITY_ACCESS_INSTANCED_PROP(Props, _BoneMatrices[v.boneIndices.w]) * v.boneWeights.w;

                float4 skinnedVertex = mul(boneMatrix, v.vertex);
                float3 skinnedNormal = normalize(mul((float3x3)boneMatrix, v.normal));

                float4 worldPos = mul(unity_ObjectToWorld, skinnedVertex);
                o.pos = UnityObjectToClipPos(skinnedVertex);
                o.uv = v.texcoord.xy;
                o.worldNormal = UnityObjectToWorldNormal(skinnedNormal);
                o.worldPos = worldPos.xyz;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);

                // Simple Lambert lighting
                fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                fixed NdotL = max(0, dot(i.worldNormal, lightDir));

                fixed3 albedo = tex.rgb * color.rgb;
                fixed3 diffuse = albedo * _LightColor0.rgb * NdotL;

                return fixed4(diffuse, tex.a * color.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}