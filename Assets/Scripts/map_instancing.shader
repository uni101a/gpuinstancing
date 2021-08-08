Shader "Unlit/map_instancing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma target 4.5

            #include "UnityCG.cginc"

#if SHADER_TARGET >= 45
            StructuredBuffer<float4> positionBuffer;
            StructuredBuffer<float3> eulerAngleBuffer;
#endif


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
            float4 _MainTex_ST;

            v2f vert (appdata v, uint instanceID : SV_InstanceID)
            {

#if SHADER_TARGET >= 45
                float4 positionData = positionBuffer[instanceID];
                float3 eulerAngleData = eulerAngleBuffer[instanceID];
#else
                float4 positionData = 0;
                float3 eulerAngleData = 0;
#endif

                v2f o;

                float4 scale = float4(v.vertex.xyz * positionData.w, v.vertex.w);
                float4 position = float4(positionData.xyz, 0) + scale;
                o.vertex = UnityObjectToClipPos(position);

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
