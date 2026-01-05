Shader "Custom/InvertedHullOutlineFixed"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineThickness ("Outline Thickness", Range(0.0, 0.5)) = 0.03
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Name "Outline"
            Cull Front  
            ZWrite Off  
            ZTest LEqual 

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
                float4 vertex : SV_POSITION;
            };

            float _OutlineThickness;
            float4 _OutlineColor;

            v2f vert (appdata v)
            {
                v2f o;

                // 1. Convert Vertex to World Space
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

                // 2. Convert Normal to World Space (handles scaling correctly)
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);

                // 3. Apply the offset in World Space
                // Now _OutlineThickness represents actual World Units
                worldPos.xyz += worldNormal * _OutlineThickness;

                // 4. Convert to Clip Space (Screen)
                o.vertex = mul(UNITY_MATRIX_VP, worldPos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}