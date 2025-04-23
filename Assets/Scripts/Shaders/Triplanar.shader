Shader "Custom/TriplanarMapping"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Tint ("Tint Color", Color) = (1,1,1,1)
        _TextureScale ("Texture Scale", Float) = 1.0
        _BlendSharpness ("Blend Sharpness", Range(1, 10)) = 2.0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Tint;
            float _TextureScale;
            float _BlendSharpness;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Get the absolute value of the normal
                float3 blendWeights = abs(i.worldNormal);

                // Raise to a power to sharpen the transition between the projections
                blendWeights = pow(blendWeights, _BlendSharpness);

                // Normalize to ensure the sum equals 1
                blendWeights /= (blendWeights.x + blendWeights.y + blendWeights.z);

                // Scale the texture coordinates
                float2 texX = i.worldPos.yz / _TextureScale;
                float2 texY = i.worldPos.xz / _TextureScale;
                float2 texZ = i.worldPos.xy / _TextureScale;

                // Sample the texture from the three directions
                fixed4 colX = tex2D(_MainTex, texX);
                fixed4 colY = tex2D(_MainTex, texY);
                fixed4 colZ = tex2D(_MainTex, texZ);

                // Blend the results based on the normal's strength in each direction
                fixed4 finalColor = colX * blendWeights.x + colY * blendWeights.y + colZ * blendWeights.z;

                // Apply the tint color
                return finalColor * _Tint;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}